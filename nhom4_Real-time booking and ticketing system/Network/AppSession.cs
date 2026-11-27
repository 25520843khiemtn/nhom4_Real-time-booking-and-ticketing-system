using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using TicketBooking.Shared;

namespace nhom4_Real_time_booking_and_ticketing_system.Network
{
    public static class AppSession
    {
        public static ClientConnection Connection { get; set; }
        public static string UserId { get; set; }
        public static string ServerId { get; set; }

        // Thông tin dùng để tự đăng nhập lại khi mất kết nối
        public static string Host { get; private set; }
        public static int Port { get; private set; }
        private static string _username;
        private static string _encodedPassword;

        // Mỗi lần Clear() tăng lên 1, để dừng vòng lặp kết nối lại cũ
        private static int _generation;

        // Các sự kiện này được phát từ LUỒNG NỀN: nơi nhận phải dùng BeginInvoke
        public static event Action Reconnected;
        public static event Action ReconnectFailed;

        public static async Task<ClientConnection> EnsureConnectedAsync(string host, int port)
        {
            if (Connection != null && Connection.IsConnected && Host == host && Port == port)
                return Connection;

            Clear();

            ClientConnection c = new ClientConnection();
            await c.ConnectAsync(host, port, AppConfig.ServerThumbprint);

            c.ConnectionClosed += OnConnectionClosed;
            Connection = c;
            Host = host;
            Port = port;
            return c;
        }

        // Gọi sau khi đăng nhập thành công để có thể tự đăng nhập lại
        public static void RememberLogin(string username, string encodedPassword)
        {
            _username = username;
            _encodedPassword = encodedPassword;
        }

        public static void Clear()
        {
            _generation++;
            _username = null;
            _encodedPassword = null;

            if (Connection != null)
            {
                Connection.Dispose();
                Connection = null;
            }

            UserId = null;
            ServerId = null;
        }

        private static async void OnConnectionClosed(Exception ex)
        {
            if (_username == null) return;   // chưa đăng nhập hoặc đã đăng xuất

            int gen = _generation;
            string user = _username;
            string pass = _encodedPassword;
            string host = Host;
            int port = Port;

            // Giữ nguyên Current, chỉ đổi thành "mất kết nối"
            ServerSession.Set(ServerSession.Current, false);

            for (int attempt = 1; attempt <= 15; attempt++)
            {
                await Task.Delay(2000);
                if (gen != _generation) return;   // người dùng đã đăng xuất

                ClientConnection c = null;
                try
                {
                    c = new ClientConnection();
                    await c.ConnectAsync(host, port, AppConfig.ServerThumbprint);

                    ProtocolMessage login = await c.SendRequestAsync(new ProtocolMessage
                    {
                        MessageType = MessageTypes.LOGIN,
                        Data = JObject.FromObject(new LoginPayload { Username = user, Password = pass })
                    });

                    if (login.Status != ResponseStatus.SUCCESS)
                    {
                        c.Dispose();
                        break;   // tài khoản không hợp lệ nữa thì dừng
                    }

                    if (gen != _generation) { c.Dispose(); return; }

                    c.ConnectionClosed += OnConnectionClosed;
                    ClientConnection old = Connection;
                    Connection = c;
                    if (old != null) old.Dispose();

                    UserId = login.UserId;
                    ServerId = login.ServerId;

                    ServerSession.Set(new ServerNode
                    {
                        Name = string.IsNullOrEmpty(login.ServerId) ? "Load Balancer" : login.ServerId,
                        Host = host,
                        Port = port
                    }, true);

                    Action ok = Reconnected;
                    if (ok != null) ok();
                    return;
                }
                catch
                {
                    if (c != null) c.Dispose();   // thất bại -> thử lại vòng sau
                }
            }

            if (gen == _generation)
            {
                ServerSession.Set(null, false);
                Action fail = ReconnectFailed;
                if (fail != null) fail();
            }
        }
    }
}