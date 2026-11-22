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
        private readonly ConnectionManager _connectionManager;
        private readonly BookingService _bookingService;
        private readonly RequestHandler _requestHandler;

        public event Action<string> OnLog;
        public event Action<int> OnClientCountChanged;

        public TcpServer()
        {
            _connectionManager = new ConnectionManager();

            IBookingRepository repository = new InMemoryBookingRepository();
            _bookingService = new BookingService(repository, WriteLog);
            _bookingService.OnSeatChanged += BroadcastSeatUpdate;

            _requestHandler = new RequestHandler(_bookingService, WriteLog);
        }

        public void Start(string ipAddress, int port, string certPath = "server.pfx", string certPassword = "123456")
        {
            if (!File.Exists(certPath))
            {
                throw new FileNotFoundException($"Không tìm thấy file chứng chỉ TLS tại: {Path.GetFullPath(certPath)}");
            }

            _serverCertificate = new X509Certificate2(certPath, certPassword);

            IPAddress ip = string.IsNullOrWhiteSpace(ipAddress) || ipAddress == "0.0.0.0"
                ? IPAddress.Any
                : IPAddress.Parse(ipAddress);

            _cts = new CancellationTokenSource();
            _listener = new TcpListener(ip, port);
            _listener.Start();

            WriteLog($"Server TLS 1.2 đã khởi động tại {ip}:{port}.");
            Task.Run(() => AcceptLoopAsync(_cts.Token));
            Task.Run(() => _bookingService.StartTimeoutWatcherAsync(_cts.Token));
        }

        public void Stop()
        {
            if (_cts != null) _cts.Cancel();
            if (_listener != null) _listener.Stop();
            _connectionManager.ClearAll();
            OnClientCountChanged?.Invoke(0);
            WriteLog("Server đã dừng hoạt động.");
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
                catch when (token.IsCancellationRequested)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    WriteLog($"Lỗi chấp nhận kết nối: {ex.Message}");
                }
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
                WriteLog($"Client kết nối TLS thành công: {session.EndPoint} (ID: {session.ConnectionId}, Protocol: {sslStream.SslProtocol})");
                OnClientCountChanged?.Invoke(_connectionManager.ActiveCount);

                using (StreamReader reader = new StreamReader(sslStream, new UTF8Encoding(false), false, 1024, leaveOpen: true))
                {
                    while (!token.IsCancellationRequested && tcpClient.Connected)
                    {
                        string line = await reader.ReadLineAsync();
                        if (line == null)
                        {
                            break;
                        }

                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        string responseJson = _requestHandler.HandleRawMessage(line, session);
                        _connectionManager.SendToClient(session, responseJson);
                    }
                }
            }
            catch (AuthenticationException ex)
            {
                WriteLog($"[{remoteEndPoint}] Bắt tay TLS thất bại (Client không hỗ trợ TLS hoặc sai giao thức): {ex.Message}");
            }
            catch (IOException)
            {
                WriteLog($"Client {remoteEndPoint} ngắt kết nối đột ngột.");
            }
            catch (Exception ex)
            {
                WriteLog($"Lỗi xử lý Client {remoteEndPoint}: {ex.Message}");
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
                    WriteLog($"Đã đóng phiên làm việc của {session.EndPoint}.");
                }
                else
                {
                    try { tcpClient.Close(); } catch { }
                }
            }
        }

        private void BroadcastSeatUpdate(SeatUpdatedPayload payload)
        {
            ProtocolMessage broadcastMsg = ProtocolMessage.CreateResponse(
                MessageTypes.SEAT_UPDATED,
                Guid.NewGuid().ToString(),
                ResponseStatus.SUCCESS,
                payload
            );
            string json = JsonConvert.SerializeObject(broadcastMsg);
            _connectionManager.Broadcast(json);
        }

        private void WriteLog(string message)
        {
            OnLog?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
        }
    }
}