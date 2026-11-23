using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TicketBooking.LoadBalancer
{
    public class TcpLoadBalancer
    {
        private readonly int _listenPort;
        private readonly List<BackendNode> _nodes;
        private readonly object _selectLock = new object();
        private int _roundRobinIndex = 0;
        private TcpListener _listener;
        private CancellationTokenSource _cts;

        public TcpLoadBalancer(int listenPort, List<BackendNode> backendNodes)
        {
            _listenPort = listenPort;
            _nodes = backendNodes;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, _listenPort);
            _listener.Start();

            Log($"Load Balancer đã khởi động tại cổng {_listenPort}.");
            Log("Danh sách Backend Server: " + string.Join(", ", _nodes.Select(n => n.ToString())));

            Task.Run(() => HealthCheckLoopAsync(_cts.Token));
            Task.Run(() => AcceptLoopAsync(_cts.Token));
        }

        public void Stop()
        {
            if (_cts != null) _cts.Cancel();
            if (_listener != null) _listener.Stop();
            Log("Load Balancer đã dừng.");
        }

        private async Task AcceptLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClientProxyAsync(client, token));
                }
                catch when (token.IsCancellationRequested) { break; }
                catch (Exception ex) { Log($"Lỗi nhận kết nối: {ex.Message}"); }
            }
        }

        private BackendNode SelectNextNode()
        {
            lock (_selectLock)
            {
                List<BackendNode> aliveNodes = _nodes.Where(n => n.IsAlive).ToList();
                if (aliveNodes.Count == 0) return null;

                // Chọn Server đang ít kết nối nhất (Least Connections), nếu bằng nhau thì xoay vòng Round-Robin
                int minConn = aliveNodes.Min(n => n.ActiveConnections);
                List<BackendNode> candidates = aliveNodes.Where(n => n.ActiveConnections == minConn).ToList();

                BackendNode chosen = candidates[_roundRobinIndex % candidates.Count];
                _roundRobinIndex = (_roundRobinIndex + 1) % int.MaxValue;
                return chosen;
            }
        }

        private async Task HandleClientProxyAsync(TcpClient client, CancellationToken token)
        {
            string clientEp = client.Client.RemoteEndPoint?.ToString() ?? "Unknown";
            BackendNode selectedNode = null;
            TcpClient backendClient = null;

            try
            {
                // Thử kết nối tới Server sống, nếu lỗi thì tự động Failover sang Server kế tiếp
                int attempts = _nodes.Count;
                while (attempts > 0)
                {
                    selectedNode = SelectNextNode();
                    if (selectedNode == null)
                    {
                        Log($"[FAIL] Client {clientEp} không thể kết nối: Không có Backend Server nào hoạt động.");
                        return;
                    }

                    try
                    {
                        backendClient = new TcpClient();
                        Task connectTask = backendClient.ConnectAsync(selectedNode.Host, selectedNode.Port);
                        if (await Task.WhenAny(connectTask, Task.Delay(2000, token)) != connectTask)
                        {
                            throw new TimeoutException("Timeout kết nối Backend.");
                        }
                        await connectTask;
                        break;
                    }
                    catch
                    {
                        selectedNode.IsAlive = false;
                        if (backendClient != null) { backendClient.Close(); backendClient = null; }
                        Log($"[FAILOVER] {selectedNode} không phản hồi -> Đánh dấu OFFLINE và thử Server khác.");
                        attempts--;
                    }
                }

                if (backendClient == null) return;

                selectedNode.IncrementConnections();
                Log($"[DISPATCH] Client {clientEp} ---> {selectedNode} | Đang tải: {selectedNode.ActiveConnections} Client");

                NetworkStream clientStream = client.GetStream();
                NetworkStream backendStream = backendClient.GetStream();

                // Chuyển tiếp luồng byte 2 chiều (giữ nguyên toàn vẹn phiên mã hóa TLS 1.2)
                Task c2s = PipeStreamAsync(clientStream, backendStream, token);
                Task s2c = PipeStreamAsync(backendStream, clientStream, token);
                await Task.WhenAny(c2s, s2c);
            }
            catch (Exception ex)
            {
                Log($"[PROXY_ERR] {clientEp}: {ex.Message}");
            }
            finally
            {
                if (selectedNode != null && backendClient != null)
                {
                    selectedNode.DecrementConnections();
                    Log($"[CLOSED] Client {clientEp} ngắt khỏi {selectedNode} | Còn lại: {selectedNode.ActiveConnections} Client");
                }
                try { if (backendClient != null) backendClient.Close(); } catch { }
                try { client.Close(); } catch { }
            }
        }

        private async Task PipeStreamAsync(Stream source, Stream destination, CancellationToken token)
        {
            byte[] buffer = new byte[8192];
            try
            {
                int bytesRead;
                while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, token)) > 0)
                {
                    await destination.WriteAsync(buffer, 0, bytesRead, token);
                    await destination.FlushAsync(token);
                }
            }
            catch { }
        }

        private async Task HealthCheckLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                foreach (BackendNode node in _nodes)
                {
                    bool currentlyAlive = await CheckNodeAliveAsync(node);
                    if (currentlyAlive != node.IsAlive)
                    {
                        node.IsAlive = currentlyAlive;
                        Log($"[HEALTH] {node} chuyển trạng thái -> {(currentlyAlive ? "ONLINE" : "OFFLINE")}");
                    }
                }

                try { await Task.Delay(10000, token); }
                catch (TaskCanceledException) { break; }
            }
        }

        private async Task<bool> CheckNodeAliveAsync(BackendNode node)
        {
            try
            {
                using (TcpClient probe = new TcpClient())
                {
                    Task connectTask = probe.ConnectAsync(node.Host, node.Port);
                    if (await Task.WhenAny(connectTask, Task.Delay(1000)) != connectTask)
                        return false;
                    await connectTask;
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [LB] {message}");
        }
    }
}