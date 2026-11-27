using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nhom4_Real_time_booking_and_ticketing_system
{
    public static class StatusBarHelper
    {
        public static void Attach(Form form, ToolStripStatusLabel lblServer, ToolStripStatusLabel lblEndpoint)
        {
            Action refresh = delegate
            {
                var node = ServerSession.Current;
                lblServer.Text = (ServerSession.IsConnected && node != null) ? "Đã kết nối: " + node.Name : (node != null ? "Mất kết nối - đang thử lại..." : "Chưa kết nối");
                lblEndpoint.Text = node != null ? node.Endpoint : "—";
            };

            EventHandler onChanged = delegate
            {
                if (form.IsHandleCreated && !form.IsDisposed)
                    form.BeginInvoke(refresh);
            };

            ServerSession.Changed += onChanged;
            form.FormClosed += delegate { ServerSession.Changed -= onChanged; };
            refresh();  
        }
    }
}