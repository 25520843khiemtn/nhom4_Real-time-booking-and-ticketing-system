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
        // Kết nối được dùng chung trong toàn bộ ứng dụng.
        public static ClientConnection Connection { get; set; }
        public static string UserId { get; set; }
        public static string ServerId { get; set; }
        public static void Clear()
        {
            if (Connection != null)
            {
                Connection.Dispose();
                Connection = null;
            }

            UserId = null;
            ServerId = null;
        }
    }
}
