using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nhom4_Real_time_booking_and_ticketing_system
{
    public static class AppConfig
    {
        public static string ServerThumbprint
        {
            get { return (ConfigurationManager.AppSettings["ServerThumbprint"] ?? "").Replace(" ", ""); }
        }
    }
}
