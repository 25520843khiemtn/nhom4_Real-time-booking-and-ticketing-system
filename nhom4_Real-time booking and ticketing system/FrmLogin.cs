using Newtonsoft.Json.Linq;
using nhom4_Real_time_booking_and_ticketing_system.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicketBooking.Shared;

namespace nhom4_Real_time_booking_and_ticketing_system
{
    public partial class FrmLogin : Form
    {
        private const string SERVER_THUMBPRINT = "DD0F92848C18C81BFFDF225F3D35B253ED88EB8E";

        public FrmLogin()
        {
            InitializeComponent();

            StatusBarHelper.Attach(this, tsslServer, tsslEndpoint);
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            lblMessage.Text = "";

            tsslServer.Text = "Chưa kết nối";
            tsslEndpoint.Text = txtServerIP.Text + ":" + txtServerPort.Text;
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string ip = txtServerIP.Text.Trim();
            string portText = txtServerPort.Text.Trim();

            // 1. Kiểm tra dữ liệu nhập vào.
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                lblMessage.Text = "Vui lòng nhập tên đăng nhập và mật khẩu.";
                return;
            }

            int port;

            if (!int.TryParse(portText, out port) || port < 1 || port > 65535)
            {
                lblMessage.Text = "Port không hợp lệ.";
                return;
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                lblMessage.Text = "Vui lòng nhập IP Load Balancer.";
                return;
            }

            // 2. Khóa nút để tránh gửi nhiều yêu cầu liên tục.
            btnLogin.Enabled = false;
            lblMessage.Text = "Đang kết nối đến Server...";
            tsslEndpoint.Text = ip + ":" + port;

            ClientConnection newConnection = null;

            try
            {
                // 3. Tạo kết nối nếu chưa có kết nối đang hoạt động.
                if (AppSession.Connection == null ||
                    !AppSession.Connection.IsConnected)
                {
                    // Nếu kết nối cũ không còn hoạt động -> giải phóng
                    AppSession.Clear();

                    newConnection = new ClientConnection();

                    await newConnection.ConnectAsync(
                        ip,
                        port,
                        SERVER_THUMBPRINT
                    );

                    AppSession.Connection = newConnection;
                    newConnection = null;

                    tsslServer.Text = "Đã kết nối";
                }

                // 4. Kiểm tra Server bằng PING.
                lblMessage.Text = "Đang kiểm tra Server...";

                ProtocolMessage pingRequest = new ProtocolMessage
                {
                    MessageType = MessageTypes.PING
                };

                ProtocolMessage pingResponse =
                    await AppSession.Connection.SendRequestAsync(
                        pingRequest
                    );

                if (pingResponse.Status != ResponseStatus.SUCCESS)
                {
                    throw new Exception("Server không phản hồi PING thành công.");
                }

                string pingMessage = null;

                if (pingResponse.Data != null)
                {
                    pingMessage =
                        pingResponse.Data["Message"]?.ToString();
                }

                if (pingMessage != "PONG")
                {
                    throw new Exception("Phản hồi PING không hợp lệ.");
                }

                AppSession.ServerId = pingResponse.ServerId;

                // 5. Mã hóa mật khẩu theo giao thức của nhóm.
                string encodedPassword =
                    Base64Helper.Encode(password);

                LoginPayload loginPayload = new LoginPayload
                {
                    Username = username,
                    Password = encodedPassword
                };

                // 6. Tạo yêu cầu đăng nhập.
                ProtocolMessage loginRequest = new ProtocolMessage
                {
                    MessageType = MessageTypes.LOGIN,
                    Data = JObject.FromObject(loginPayload)
                };

                lblMessage.Text = "Đang xác thực tài khoản...";

                // 7. Gửi yêu cầu LOGIN đến Server.
                ProtocolMessage loginResponse =
                    await AppSession.Connection.SendRequestAsync(
                        loginRequest
                    );

                // 8. Kiểm tra kết quả đăng nhập.
                if (loginResponse.Status != ResponseStatus.SUCCESS)
                {
                    string errorMessage = "Đăng nhập thất bại.";

                    if (loginResponse.Data != null)
                    {
                        errorMessage =
                            loginResponse.Data["Message"]?.ToString()
                            ?? errorMessage;
                    }

                    lblMessage.Text = errorMessage;
                    return;
                }

                // 9. Lưu thông tin phiên đăng nhập.
                AppSession.UserId = loginResponse.UserId;
                AppSession.ServerId = loginResponse.ServerId;

                if (string.IsNullOrWhiteSpace(AppSession.UserId))
                {
                    throw new Exception(
                        "Server không trả về UserId hợp lệ."
                    );
                }

                // 10. Mở màn hình danh sách phim.
                lblMessage.Text = "Đăng nhập thành công!";

                FrmMovieList next = new FrmMovieList();

                // Khi đóng màn hình tiếp theo, giải phóng phiên.
                next.FormClosed += (s, args) =>
                {
                    AppSession.Clear();
                    Close();
                };

                next.Show();
                Hide();
            }
            catch (Exception ex)
            {
                // Nếu kết nối mới chưa được đưa vào AppSession, giải phóng tài nguyên của nó.
                if (newConnection != null)
                {
                    newConnection.Dispose();
                }

                // Nếu kết nối đang lưu bị lỗi, đóng phiên.
                AppSession.Clear();

                tsslServer.Text = "Mất kết nối";
                lblMessage.Text = "Lỗi: " + ex.Message;
            }
            finally
            {
                // Chỉ bật lại nút nếu Form đăng nhập vẫn đang hiển thị.
                if (!IsDisposed && Visible)
                {
                    btnLogin.Enabled = true;
                }
            }
        }
    }
}
