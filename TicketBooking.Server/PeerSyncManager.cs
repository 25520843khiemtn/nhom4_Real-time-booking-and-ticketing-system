using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TicketBooking.Shared;

namespace TicketBooking.Server
{
    public class PeerNode
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public override string ToString() { return $"{Host}:{Port}"; }
    }

    public class PeerSyncManager
    {
        private readonly string _serverId;
        private readonly List<PeerNode> _peers = new List<PeerNode>();
        private readonly Action<string> _log;

        public PeerSyncManager(string serverId, IEnumerable<string> peerEndpoints, Action<string> logCallback)
        {
            _serverId = serverId;
            _log = logCallback;

            if (peerEndpoints != null)
            {
                foreach (string raw in peerEndpoints)
                {
                    string item = raw.Trim();
                    if (string.IsNullOrEmpty(item)) continue;

                    string[] parts = item.Split(':');
                    int port;
                    if (parts.Length == 2 && int.TryParse(parts[1], out port))
                    {
                        _peers.Add(new PeerNode { Host = parts[0].Trim(), Port = port });
                    }
                    else if (parts.Length == 1 && int.TryParse(parts[0], out port))
                    {
                        _peers.Add(new PeerNode { Host = "127.0.0.1", Port = port });
                    }
                }
            }
        }

        public async Task SyncSeatToPeersAsync(SeatUpdatedPayload payload)
        {
            if (_peers.Count == 0) return;

            ProtocolMessage syncMsg = ProtocolMessage.CreateResponse(
                MessageTypes.SERVER_SYNC_SEAT,
                Guid.NewGuid().ToString(),
                ResponseStatus.SUCCESS,
                payload,
                null,
                _serverId
            );
            string json = JsonConvert.SerializeObject(syncMsg);

            foreach (PeerNode peer in _peers)
            {
                await SendToPeerAsync(peer, json, logError: true);
            }
        }

        public async Task StartHeartbeatLoopAsync(CancellationToken token)
        {
            if (_peers.Count == 0) return;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(15000, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                ProtocolMessage hb = ProtocolMessage.CreateResponse(
                    MessageTypes.SERVER_HEARTBEAT,
                    Guid.NewGuid().ToString(),
                    ResponseStatus.SUCCESS,
                    new { Timestamp = DateTime.Now },
                    null,
                    _serverId
                );
                string json = JsonConvert.SerializeObject(hb);

                foreach (PeerNode peer in _peers)
                {
                    await SendToPeerAsync(peer, json, logError: false);
                }
            }
        }

        private async Task SendToPeerAsync(PeerNode peer, string jsonLine, bool logError)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    Task connectTask = client.ConnectAsync(peer.Host, peer.Port);
                    if (await Task.WhenAny(connectTask, Task.Delay(1500)) != connectTask)
                    {
                        if (logError) _log($"[PEER_SYNC] Timeout khi đồng bộ sang Node {peer}.");
                        return;
                    }
                    await connectTask;

                    using (SslStream sslStream = new SslStream(
                        client.GetStream(),
                        false,
                        (sender, cert, chain, errors) => true))
                    {
                        await sslStream.AuthenticateAsClientAsync("localhost", null, SslProtocols.Tls12, false);
                        using (StreamWriter writer = new StreamWriter(sslStream, new UTF8Encoding(false)) { AutoFlush = true })
                        {
                            await writer.WriteLineAsync(jsonLine);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (logError)
                {
                    _log($"[PEER_SYNC] Không thể gửi đồng bộ tới Node {peer} (Node có thể đang tắt): {ex.Message}");
                }
            }
        }
    }
}