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
    public partial class FrmRegister : Form
    {
        private readonly string _host;
        private readonly int _port;

        public string RegisteredUsername { get; private set; }

        public FrmRegister() : this("127.0.0.1", 9000) { }

        public FrmRegister(string host, int port)
        {
            InitializeComponent();
            _host = string.IsNullOrWhiteSpace(host) ? "127.0.0.1" : host;
            _port = (port >= 1 && port <= 65535) ? port : 9000;
            StatusBarHelper.Attach(this, tsslServer, tsslEndpoint);
        }

        private async void btnRegister_Click(object sender, EventArgs e)
        {
            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (fullName == "" || username == "" || password == "")
            {
                lblMessage.Text = "Vui lòng nhập đầy đủ thông tin.";
                return;
            }
            if (password.Length < 6)
            {
                lblMessage.Text = "Mật khẩu phải có ít nhất 6 ký tự.";
                return;
            }
            if (password != txtConfirmPassword.Text)
            {
                lblMessage.Text = "Mật khẩu nhập lại không khớp.";
                return;
            }

            btnRegister.Enabled = false;
            lblMessage.Text = "Đang gửi yêu cầu đăng ký...";

            try
            {
                ClientConnection conn = await AppSession.EnsureConnectedAsync(_host, _port);

                RegisterPayload payload = new RegisterPayload
                {
                    Username = username,
                    Password = Base64Helper.Encode(password),
                    FullName = fullName
                };

                ProtocolMessage resp = await conn.SendRequestAsync(new ProtocolMessage
                {
                    MessageType = MessageTypes.REGISTER,
                    Data = JObject.FromObject(payload)
                });

                ServerSession.Set(new ServerNode
                {
                    Name = string.IsNullOrEmpty(resp.ServerId) ? "Load Balancer" : resp.ServerId,
                    Host = _host,
                    Port = _port
                }, true);

                if (resp.Status != ResponseStatus.SUCCESS)
                {
                    string err = resp.Data != null ? resp.Data["Message"]?.ToString() : null;
                    lblMessage.Text = err ?? "Đăng ký thất bại.";
                    return;
                }

                RegisteredUsername = username;
                MessageBox.Show("Đăng ký thành công! Hãy quay lại đăng nhập.", "Thành công",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                AppSession.Clear();
                ServerSession.Set(null, false);
                lblMessage.Text = "Không kết nối được Server: " + ex.Message;
            }
            finally
            {
                if (!IsDisposed) btnRegister.Enabled = true;
            }
        }

        private void lnkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Close();
        }
    }
}