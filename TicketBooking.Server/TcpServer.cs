using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TicketBooking.Shared;

namespace TicketBooking.Server
{
    public class TcpServer
    {
        private TcpListener _listener;
        private CancellationTokenSource _cts;
        private X509Certificate2 _serverCertificate;
        private ConnectionManager _connectionManager;
        private BookingService _bookingService;
        private RequestHandler _requestHandler;
        private PeerSyncManager _peerSyncManager;
        private string _serverId = "Server-1";
        private readonly object _fileLock = new object();

        public event Action<string> OnLog;
        public event Action<int> OnClientCountChanged;

        public void Start(string serverId, string ipAddress, int port, string peerEndpointsCsv, string sqlConnectionString = null, string certPath = "server.pfx", string certPassword = "123456")
        {
            if (!File.Exists(certPath))
            {
                throw new FileNotFoundException($"Không tìm thấy file chứng chỉ TLS tại: {Path.GetFullPath(certPath)}");
            }

            _serverId = string.IsNullOrWhiteSpace(serverId) ? "Server-1" : serverId.Trim();
            _serverCertificate = new X509Certificate2(certPath, certPassword);
            _connectionManager = new ConnectionManager();

            // Nếu có chuỗi kết nối SQL Server của Hạnh -> dùng SqlBookingRepository (hỗ trợ Multi-Server khóa mức dòng)
            // Nếu để trống -> chạy tạm trên RAM để test nhanh
            IBookingRepository repository = !string.IsNullOrWhiteSpace(sqlConnectionString)
                ? (IBookingRepository)new SqlBookingRepository(sqlConnectionString.Trim())
                : new InMemoryBookingRepository();

            string[] peers = string.IsNullOrWhiteSpace(peerEndpointsCsv)
                ? new string[0]
                : peerEndpointsCsv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            _peerSyncManager = new PeerSyncManager(_serverId, peers, WriteLog);

            _bookingService = new BookingService(repository, WriteLog);
            _bookingService.OnLocalSeatChanged += HandleLocalSeatChanged;
            _bookingService.OnRemoteSeatSynced += BroadcastSeatUpdateToClients;

            _requestHandler = new RequestHandler(_bookingService, _serverId, WriteLog);

            IPAddress ip = string.IsNullOrWhiteSpace(ipAddress) || ipAddress == "0.0.0.0"
                ? IPAddress.Any
                : IPAddress.Parse(ipAddress);

            _cts = new CancellationTokenSource();
            _listener = new TcpListener(ip, port);
            _listener.Start();

            string dbMode = !string.IsNullOrWhiteSpace(sqlConnectionString) ? "SQL Server (Shared DB)" : "In-Memory RAM";
            WriteLog($"Node khởi động thành công tại {ip}:{port} | Chế độ DB: {dbMode} | Peers: {(peers.Length > 0 ? string.Join(", ", peers) : "None")}");

            Task.Run(() => AcceptLoopAsync(_cts.Token));
            Task.Run(() => _bookingService.StartTimeoutWatcherAsync(_cts.Token));
            Task.Run(() => _peerSyncManager.StartHeartbeatLoopAsync(_cts.Token));
        }

        public void Stop()
        {
            if (_cts != null) _cts.Cancel();
            if (_listener != null) _listener.Stop();
            if (_connectionManager != null) _connectionManager.ClearAll();
            OnClientCountChanged?.Invoke(0);
            WriteLog("Server Node đã dừng hoạt động.");
        }

        private async Task AcceptLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClientAsync(tcpClient, token));
                }
                catch when (token.IsCancellationRequested) { break; }
                catch (ObjectDisposedException) { break; }
                catch (Exception ex) { WriteLog($"Lỗi chấp nhận kết nối: {ex.Message}"); }
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient, CancellationToken token)
        {
            ClientSession session = null;
            string remoteEndPoint = tcpClient.Client.RemoteEndPoint?.ToString() ?? "Unknown";

            try
            {
                SslStream sslStream = new SslStream(tcpClient.GetStream(), false);
                await sslStream.AuthenticateAsServerAsync(
                    _serverCertificate,
                    clientCertificateRequired: false,
                    enabledSslProtocols: SslProtocols.Tls12,
                    checkCertificateRevocation: false
                );

                session = _connectionManager.AddClient(tcpClient, sslStream);
                WriteLog($"Kết nối TLS mới: {session.EndPoint} (ID: {session.ConnectionId})");
                OnClientCountChanged?.Invoke(_connectionManager.ActiveCount);

                using (StreamReader reader = new StreamReader(sslStream, new UTF8Encoding(false), false, 1024, leaveOpen: true))
                {
                    while (!token.IsCancellationRequested && tcpClient.Connected)
                    {
                        string line = await reader.ReadLineAsync();
                        if (line == null) break;
                        if (string.IsNullOrWhiteSpace(line)) continue;

                        string responseJson = _requestHandler.HandleRawMessage(line, session);
                        _connectionManager.SendToClient(session, responseJson);
                    }
                }
            }
            catch (AuthenticationException ex)
            {
                WriteLog($"[{remoteEndPoint}] Bắt tay TLS thất bại: {ex.Message}");
            }
            catch (IOException)
            {
                WriteLog($"Kết nối {remoteEndPoint} đã đóng.");
            }
            catch (Exception ex)
            {
                WriteLog($"Lỗi xử lý {remoteEndPoint}: {ex.Message}");
            }
            finally
            {
                if (session != null)
                {
                    if (!string.IsNullOrWhiteSpace(session.UserId))
                    {
                        _bookingService.ReleaseSeatsByUser(session.UserId);
                    }
                    _connectionManager.RemoveClient(session.ConnectionId);
                    OnClientCountChanged?.Invoke(_connectionManager.ActiveCount);
                }
                else
                {
                    try { tcpClient.Close(); } catch { }
                }
            }
        }

        private void HandleLocalSeatChanged(SeatUpdatedPayload payload)
        {
            BroadcastSeatUpdateToClients(payload);
            _ = _peerSyncManager.SyncSeatToPeersAsync(payload);
        }

        private void BroadcastSeatUpdateToClients(SeatUpdatedPayload payload)
        {
            ProtocolMessage broadcastMsg = ProtocolMessage.CreateResponse(
                MessageTypes.SEAT_UPDATED,
                Guid.NewGuid().ToString(),
                ResponseStatus.SUCCESS,
                payload,
                null,
                _serverId
            );
            string json = JsonConvert.SerializeObject(broadcastMsg);
            _connectionManager.Broadcast(json);
        }

        private void WriteLog(string message)
        {
            string logLine = $"[{DateTime.Now:HH:mm:ss}] [{_serverId}] {message}";
            OnLog?.Invoke(logLine);

            try
            {
                lock (_fileLock)
                {
                    string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"log_{_serverId}.txt");
                    File.AppendAllText(logPath, logLine + Environment.NewLine, Encoding.UTF8);
                }
            }
            catch
            {
            }
        }
    }
}