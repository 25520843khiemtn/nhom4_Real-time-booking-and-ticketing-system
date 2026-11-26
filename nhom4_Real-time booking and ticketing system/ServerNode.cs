using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nhom4_Real_time_booking_and_ticketing_system
{
    public class ServerNode
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string Endpoint { get { return Host + ":" + Port; } }

        // ComboBox hiển thị kết quả của hàm này; thiếu nó sẽ hiện tên lớp thay vì tên Server
        public override string ToString() { return Name + " (" + Endpoint + ")"; }
    }

    public static class ServerConfig
    {
        public static readonly List<ServerNode> Nodes = new List<ServerNode>
        {
            new ServerNode
            {
                Name = "Load Balancer",
                Host = "127.0.0.1",
                Port = 9000
            },

            new ServerNode
            {
                Name = "Server-1",
                Host = "127.0.0.1",
                Port = 8001
            },

            new ServerNode
            {
                Name = "Server-2",
                Host = "127.0.0.1",
                Port = 8002
            }
        };

        public static bool AutoFailover { get; set; }
    }
}
