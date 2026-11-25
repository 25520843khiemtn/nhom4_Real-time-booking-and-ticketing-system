using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TicketBooking.Server
{
    public partial class Form1 : Form
    {
        private TcpServer _server;
        public Form1()
        {
            InitializeComponent();
            _server = new TcpServer();
            _server.OnLog += AppendLog;
            _server.OnClientCountChanged += UpdateClientCount;
            btnStop.Enabled = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            int port;
            if (!int.TryParse(txtPort.Text.Trim(), out port))
            {
                MessageBox.Show("Port không hợp lệ!");
                return;
            }

            try
            {
                string serverId = txtServerId.Text.Trim();
                string peers = txtPeers.Text.Trim();
                string sqlConn = txtSqlConn.Text.Trim();

                _server.Start(serverId, "0.0.0.0", port, peers, sqlConn);
                btnStart.Enabled = false;
                btnStop.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể khởi động Server: " + ex.Message, "Lỗi khởi động", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _server.Stop();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void AppendLog(string text)
        {
            if (rtbLog.InvokeRequired)
            {
                rtbLog.Invoke(new Action<string>(AppendLog), text);
                return;
            }
            rtbLog.AppendText(text + Environment.NewLine);
        }

        private void UpdateClientCount(int count)
        {
            if (lblClients.InvokeRequired)
            {
                lblClients.Invoke(new Action<int>(UpdateClientCount), count);
                return;
            }
            lblClients.Text = $"Client đang kết nối: {count}";
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
