using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nhom4_Real_time_booking_and_ticketing_system
{
    public static class ServerSession
    {
        public static ServerNode Current { get; private set; }
        public static bool IsConnected { get; private set; }
        public static event EventHandler Changed;

        public static void Set(ServerNode node, bool connected)
        {
            Current = node;
            IsConnected = connected;
            var handler = Changed;
            if (handler != null) handler(null, EventArgs.Empty);
        }
    }
}