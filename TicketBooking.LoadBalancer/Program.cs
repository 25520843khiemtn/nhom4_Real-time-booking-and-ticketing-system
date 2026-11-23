using System;
using System.Collections.Generic;
using System.Text;

namespace TicketBooking.LoadBalancer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "TicketBooking - TCP Load Balancer (Port 9000)";

            List<BackendNode> nodes = new List<BackendNode>
            {
                new BackendNode("Server-1", "127.0.0.1", 8001),
                new BackendNode("Server-2", "127.0.0.1", 8002)
            };

            TcpLoadBalancer lb = new TcpLoadBalancer(9000, nodes);
            lb.Start();

            Console.WriteLine("Nhấn phím Q để dừng Load Balancer...");
            while (Console.ReadKey(true).Key != ConsoleKey.Q) { }
            lb.Stop();
        }
    }
}