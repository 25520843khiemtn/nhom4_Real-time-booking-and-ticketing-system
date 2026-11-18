using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TicketBooking.Server
{
    public class TcpServer
    {
        private TcpListener _listener;
        private CancellationTokenSource _cts;
        private readonly ConnectionManager _connectionManager;
        private readonly RequestHandler _requestHandler;

        public event Action<string> OnLog;
        public event Action<int> OnClientCountChanged;

        public TcpServer()
        {
            _connectionManager = new ConnectionManager();
            _requestHandler = new RequestHandler(WriteLog);
        }

        public void Start(string ipAddress, int port)
        {
            IPAddress ip = string.IsNullOrWhiteSpace(ipAddress) || ipAddress == "0.0.0.0"
                ? IPAddress.Any
                : IPAddress.Parse(ipAddress);

            _cts = new CancellationTokenSource();
            _listener = new TcpListener(ip, port);
            _listener.Start();

            WriteLog($"Server đã khởi động tại {ip}:{port}. Đang chờ Client...");
            Task.Run(() => AcceptLoopAsync(_cts.Token));
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
                    ClientSession session = _connectionManager.AddClient(tcpClient);

                    WriteLog($"Client kết nối: {session.EndPoint} (ID: {session.ConnectionId})");
                    OnClientCountChanged?.Invoke(_connectionManager.ActiveCount);

                    _ = Task.Run(() => HandleClientAsync(session, token));
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

        private async Task HandleClientAsync(ClientSession session, CancellationToken token)
        {
            try
            {
                NetworkStream stream = session.Client.GetStream();
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    while (!token.IsCancellationRequested && session.Client.Connected)
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
            catch (IOException)
            {
                WriteLog($"Client {session.EndPoint} ngắt kết nối đột ngột.");
            }
            catch (Exception ex)
            {
                WriteLog($"Lỗi xử lý Client {session.EndPoint}: {ex.Message}");
            }
            finally
            {
                _connectionManager.RemoveClient(session.ConnectionId);
                OnClientCountChanged?.Invoke(_connectionManager.ActiveCount);
                WriteLog($"Đã đóng phiên làm việc của {session.EndPoint}.");
            }
        }

        private void WriteLog(string message)
        {
            OnLog?.Invoke($"[{DateTime.Now:HH:mm:ss}] {message}");
        }
    }
}