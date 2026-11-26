using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TicketBooking.Shared;

namespace nhom4_Real_time_booking_and_ticketing_system.Network
{
    public class ClientConnection : IDisposable
    {
        private TcpClient _tcpClient;
        private SslStream _sslStream;

        private StreamReader _reader;
        private StreamWriter _writer;

        private CancellationTokenSource _cts;
        private Task _readTask;

        // Ngăn nhiều yêu cầu ghi dữ liệu cùng lúc vào một StreamWriter.
        private readonly SemaphoreSlim _writeLock =
            new SemaphoreSlim(1, 1);

        // Lưu các yêu cầu đang chờ Server phản hồi.
        // Key là RequestId.
        private readonly ConcurrentDictionary<
            string,
            TaskCompletionSource<ProtocolMessage>
        > _pendingRequests =
            new ConcurrentDictionary<
                string,
                TaskCompletionSource<ProtocolMessage>
            >();

        public bool IsConnected
        {
            get
            {
                return _tcpClient != null
                    && _sslStream != null
                    && _sslStream.IsAuthenticated;
            }
        }

        public string ServerId { get; private set; }

        // Nhận thông báo từ Server về trạng thái ghế.
        public event Action<ProtocolMessage>
            MessageReceived;

        // Thông báo khi kết nối bị đóng hoặc gặp lỗi.
        public event Action<Exception>
            ConnectionClosed;

        // Thông báo khi nhận được ServerId.
        public event Action<string>
            ServerIdChanged;

        // Kết nối đến Load Balancer và thiết lập TLS 1.2.

        public async Task ConnectAsync(
            string host,
            int port,
            string expectedThumbprint)
        {
            if (IsConnected)
                throw new InvalidOperationException(
                    "Client đã kết nối."
                );

            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("IP không hợp lệ.");

            if (port <= 0 || port > 65535)
                throw new ArgumentException("Port không hợp lệ.");

            if (string.IsNullOrWhiteSpace(expectedThumbprint))
                throw new ArgumentException(
                    "Chưa có Thumbprint của chứng chỉ Server."
                );

            try
            {
                // 1. Tạo TCP Client và kết nối Load Balancer.
                _tcpClient = new TcpClient();

                await _tcpClient.ConnectAsync(host, port);

                // 2. Thiết lập SslStream.
                _sslStream = new SslStream(
                    _tcpClient.GetStream(),
                    false,
                    (sender, certificate, chain, errors) =>
                    {
                        if (certificate == null)
                            return false;

                        string actualThumbprint =
                            certificate.GetCertHashString();

                        return string.Equals(
                            actualThumbprint,
                            NormalizeThumbprint(expectedThumbprint),
                            StringComparison.OrdinalIgnoreCase
                        );
                    }
                );

                // 3. TLS Handshake.
                await _sslStream.AuthenticateAsClientAsync(
                    "localhost",
                    null,
                    SslProtocols.Tls12,
                    false
                );

                // 4. Tạo Reader và Writer.
                _reader = new StreamReader(
                    _sslStream,
                    new UTF8Encoding(false),
                    false,
                    1024,
                    true
                );

                _writer = new StreamWriter(
                    _sslStream,
                    new UTF8Encoding(false),
                    1024,
                    true
                )
                {
                    AutoFlush = true
                };

                // 5. Bắt đầu luồng đọc dữ liệu.
                _cts = new CancellationTokenSource();

                _readTask = ReadLoopAsync(_cts.Token);
            }
            catch
            {
                // Nếu TCP hoặc TLS thất bại, giải phóng tài nguyên.
                Dispose();

                throw;
            }
        }

        // Chuẩn hóa Thumbprint, loại bỏ dấu cách.
        private string NormalizeThumbprint(string thumbprint)
        {
            return thumbprint.Replace(" ", "").Trim();
        }

        // Gửi một yêu cầu và chờ phản hồi tương ứng.
        public async Task<ProtocolMessage> SendRequestAsync(
            ProtocolMessage request,
            int timeoutSeconds = 15)
        {
            if (!IsConnected)
                throw new InvalidOperationException(
                    "Client chưa kết nối Server."
                );

            if (request == null)
                throw new ArgumentNullException("request");

            if (string.IsNullOrWhiteSpace(request.RequestId))
                request.RequestId = Guid.NewGuid().ToString();

            var completion =
                new TaskCompletionSource<ProtocolMessage>(
                    TaskCreationOptions.RunContinuationsAsynchronously
                );

            if (!_pendingRequests.TryAdd(
                request.RequestId,
                completion))
            {
                throw new InvalidOperationException(
                    "RequestId đã tồn tại."
                );
            }

            try
            {
                string json = JsonConvert.SerializeObject(request);

                // Chỉ cho phép một thao tác ghi tại một thời điểm.
                await _writeLock.WaitAsync();

                try
                {
                    await _writer.WriteLineAsync(json);
                }
                finally
                {
                    _writeLock.Release();
                }

                // Chờ Server phản hồi hoặc hết thời gian.
                Task completedTask = await Task.WhenAny(
                    completion.Task,
                    Task.Delay(TimeSpan.FromSeconds(timeoutSeconds))
                );

                if (completedTask != completion.Task)
                {
                    throw new TimeoutException(
                        "Server không phản hồi trong thời gian quy định."
                    );
                }

                return await completion.Task;
            }
            finally
            {
                TaskCompletionSource<ProtocolMessage> removed;

                _pendingRequests.TryRemove(
                    request.RequestId,
                    out removed
                );
            }
        }

        // Chỉ có một luồng duy nhất đọc dữ liệu từ Server.
        private async Task ReadLoopAsync(
            CancellationToken cancellationToken)
        {
            Exception connectionError = null;

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    string line = await _reader.ReadLineAsync();
                    if (line == null)
                        break;

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    ProtocolMessage message;

                    try
                    {
                        message =
                            JsonConvert.DeserializeObject<ProtocolMessage>(
                                line
                            );
                    }
                    catch (JsonException)
                    {
                        continue;
                    }

                    if (message == null)
                        continue;

                    // Cập nhật ServerId nếu Server có gửi.
                    if (!string.IsNullOrWhiteSpace(message.ServerId))
                    {
                        ServerId = message.ServerId;

                        ServerIdChanged?.Invoke(ServerId);
                    }

                    // Tìm yêu cầu đang chờ theo RequestId.
                    TaskCompletionSource<ProtocolMessage> completion;

                    if (!string.IsNullOrWhiteSpace(message.RequestId)
                        && _pendingRequests.TryGetValue(
                            message.RequestId,
                            out completion))
                    {
                        completion.TrySetResult(message);
                    }
                    else
                    {
                        // Thông điệp không phải phản hồi đang chờ.
                        MessageReceived?.Invoke(message);
                    }
                }
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                    connectionError = ex;
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    var error = connectionError
                        ?? new IOException("Server đã đóng kết nối.");

                    foreach (var item in _pendingRequests)
                    {
                        item.Value.TrySetException(error);
                    }

                    ConnectionClosed?.Invoke(error);
                }
            }
        }

        public void Dispose()
        {
            if (_cts != null)
                _cts.Cancel();

            try
            {
                if (_reader != null)
                    _reader.Dispose();
            }
            catch { }

            try
            {
                if (_writer != null)
                    _writer.Dispose();
            }
            catch { }

            try
            {
                if (_sslStream != null)
                    _sslStream.Dispose();
            }
            catch { }

            try
            {
                if (_tcpClient != null)
                    _tcpClient.Close();
            }
            catch { }

            foreach (var item in _pendingRequests)
            {
                item.Value.TrySetException(
                    new IOException("Client đã đóng kết nối.")
                );
            }

            if (_cts != null)
                _cts.Dispose();

            _writeLock.Dispose();

            _tcpClient = null;
            _sslStream = null;
            _reader = null;
            _writer = null;
        }
    }
}