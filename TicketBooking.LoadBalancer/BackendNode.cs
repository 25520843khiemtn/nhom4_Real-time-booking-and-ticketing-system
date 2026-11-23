using System.Threading;

namespace TicketBooking.LoadBalancer
{
    public class BackendNode
    {
        public string Name { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool IsAlive { get; set; }

        private int _activeConnections;
        public int ActiveConnections
        {
            get { return Volatile.Read(ref _activeConnections); }
        }

        public BackendNode(string name, string host, int port)
        {
            Name = name;
            Host = host;
            Port = port;
            IsAlive = true;
        }

        public void IncrementConnections() { Interlocked.Increment(ref _activeConnections); }
        public void DecrementConnections() { Interlocked.Decrement(ref _activeConnections); }

        public override string ToString()
        {
            return $"{Name} ({Host}:{Port})";
        }
    }
}