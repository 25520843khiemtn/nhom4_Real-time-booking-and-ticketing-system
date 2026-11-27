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
        public FrmLogin()
        {
            InitializeComponent();
            StatusBarHelper.Attach(this, tsslServer, tsslEndpoint);
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            lblMessage.Text = "";
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string ip = txtServerIP.Text.Trim();
            int port;

            if (username == "" || password == "")
            {
                lblMessage.Text = "Vui lòng nhập tên đăng nhập và mật khẩu.";
                return;
            }
            if (ip == "")
            {
                lblMessage.Text = "Vui lòng nhập IP Load Balancer.";
                return;
            }
            if (!int.TryParse(txtServerPort.Text.Trim(), out port) || port < 1 || port > 65535)
            {
                lblMessage.Text = "Port không hợp lệ.";
                return;
            }

            btnLogin.Enabled = false;

            try
            {
                lblMessage.Text = "Đang kết nối " + ip + ":" + port + "...";
                await AppSession.EnsureConnectedAsync(ip, port);

                lblMessage.Text = "Đang kiểm tra Server...";
                ProtocolMessage ping = await AppSession.Connection.SendRequestAsync(
                    new ProtocolMessage { MessageType = MessageTypes.PING });

                if (ping.Status != ResponseStatus.SUCCESS)
                    throw new Exception("Server không phản hồi PING thành công.");

                string pong = ping.Data != null ? ping.Data["Message"]?.ToString() : null;
                if (pong != "PONG")
                    throw new Exception("Phản hồi PING không hợp lệ.");

                AppSession.ServerId = ping.ServerId;

                ServerSession.Set(new ServerNode
                {
                    Name = string.IsNullOrEmpty(ping.ServerId) ? "Server" : ping.ServerId,
                    Host = ip,
                    Port = port
                }, true);

                string encoded = Base64Helper.Encode(password);
                lblMessage.Text = "Đang xác thực tài khoản...";

                ProtocolMessage resp = await AppSession.Connection.SendRequestAsync(new ProtocolMessage
                {
                    MessageType = MessageTypes.LOGIN,
                    Data = JObject.FromObject(new LoginPayload { Username = username, Password = encoded })
                });

                if (resp.Status != ResponseStatus.SUCCESS)
                {
                    string err = resp.Data != null ? resp.Data["Message"]?.ToString() : null;
                    lblMessage.Text = err ?? "Đăng nhập thất bại.";
                    return;
                }

                AppSession.UserId = resp.UserId;
                AppSession.ServerId = resp.ServerId;
                if (string.IsNullOrWhiteSpace(AppSession.UserId))
                    throw new Exception("Server không trả về UserId hợp lệ.");

                AppSession.RememberLogin(username, encoded);

                lblMessage.Text = "Đăng nhập thành công!";
                FrmMovieList next = new FrmMovieList();

                next.FormClosed += (s, args) =>
                {
                    AppSession.Clear();
                    ServerSession.Set(null, false);

                    if (next.LoggingOut)
                    {
                        txtPassword.Clear();
                        lblMessage.Text = "";
                        btnLogin.Enabled = true;
                        Show();
                    }
                    else    
                    {
                        Close();
                    }
                };

                next.Show();
                Hide();
            }
            catch (Exception ex)
            {
                AppSession.Clear();
                ServerSession.Set(null, false);
                lblMessage.Text = "Lỗi: " + ex.Message;
            }
            finally
            {
                if (!IsDisposed && Visible)
                    btnLogin.Enabled = true;
            }
        }

        private void lnkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            int port;
            int.TryParse(txtServerPort.Text.Trim(), out port);

            using (FrmRegister reg = new FrmRegister(txtServerIP.Text.Trim(), port))
            {
                reg.ShowDialog(this);

                if (reg.RegisteredUsername != null)
                {
                    txtUsername.Text = reg.RegisteredUsername;
                    txtPassword.Clear();
                    txtPassword.Focus();
                    lblMessage.Text = "Đăng ký thành công. Hãy đăng nhập.";
                }
            }
        }
    }
}