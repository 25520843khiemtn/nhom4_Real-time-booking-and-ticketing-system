using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace TicketBooking.Server
{
    public class ClientSession
    {
        public string ConnectionId { get; set; }
        public string EndPoint { get; set; }
        public string UserId { get; set; }
        public TcpClient Client { get; set; }
        public Stream SecureStream { get; set; }
        public StreamWriter Writer { get; set; }
    }

    public class ConnectionManager
    {
        private readonly Dictionary<string, ClientSession> _sessions = new Dictionary<string, ClientSession>();
        private readonly object _lock = new object();

        public int ActiveCount
        {
            get
            {
                lock (_lock)
                {
                    return _sessions.Count;
                }
            }
        }

        public ClientSession AddClient(TcpClient tcpClient, Stream stream)
        {
            ClientSession session = new ClientSession
            {
                ConnectionId = Guid.NewGuid().ToString("N").Substring(0, 8),
                EndPoint = tcpClient.Client.RemoteEndPoint.ToString(),
                Client = tcpClient,
                SecureStream = stream,
                Writer = new StreamWriter(stream, new UTF8Encoding(false), 1024, leaveOpen: true) { AutoFlush = true }
            };

            lock (_lock)
            {
                _sessions[session.ConnectionId] = session;
            }
            return session;
        }

        public void RemoveClient(string connectionId)
        {
            ClientSession removed = null;
            lock (_lock)
            {
                if (_sessions.TryGetValue(connectionId, out removed))
                {
                    _sessions.Remove(connectionId);
                }
            }

            if (removed != null)
            {
                try { removed.Writer.Dispose(); } catch { }
                try { removed.SecureStream.Close(); } catch { }
                try { removed.Client.Close(); } catch { }
            }
        }

        public void SendToClient(ClientSession session, string jsonLine)
        {
            if (session == null || session.Writer == null) return;
            lock (session.Writer)
            {
                session.Writer.WriteLine(jsonLine);
            }
        }

        public void Broadcast(string jsonLine, string excludeConnectionId = null)
        {
            List<ClientSession> snapshot;
            lock (_lock)
            {
                snapshot = _sessions.Values.ToList();
            }

            foreach (ClientSession session in snapshot)
            {
                if (excludeConnectionId != null && session.ConnectionId == excludeConnectionId)
                {
                    continue;
                }

                try
                {
                    SendToClient(session, jsonLine);
                }
                catch
                {
                    RemoveClient(session.ConnectionId);
                }
            }
        }

        public void ClearAll()
        {
            List<string> ids;
            lock (_lock)
            {
                ids = _sessions.Keys.ToList();
            }

            foreach (string id in ids)
            {
                RemoveClient(id);
            }
        }
    }
}