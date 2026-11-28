using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Newtonsoft.Json;
using TicketBooking.Server;
using TicketBooking.Shared;
using Xunit;

namespace TransportTests;

public class TlsTransportTests
{
    [Fact]
    public async Task Tcp_server_tls12_handles_multiple_correlated_lines_and_disconnects_cleanly()
    {
        using var certificate = Certificate();
        var path = Path.Combine(AppContext.BaseDirectory, $"test-{Guid.NewGuid():N}.pfx");
        File.WriteAllBytes(path, certificate.Export(X509ContentType.Pfx, "test-only"));
        var server = new TcpServer();
        var counts = new System.Collections.Concurrent.ConcurrentQueue<int>();
        server.OnClientCountChanged += counts.Enqueue;
        try
        {
            server.Start("transport-test", "127.0.0.1", 0, "", null, path, "test-only");
            using (var client = await Loopback.Connect(Loopback.BoundPort(server)))
            using (var tls = new SslStream(client.GetStream(), false, (_, cert, _, _) =>
                cert != null && cert.GetCertHashString() == certificate.GetCertHashString()))
            {
                await tls.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
                {
                    TargetHost = "localhost", EnabledSslProtocols = SslProtocols.Tls12,
                    CertificateRevocationCheckMode = X509RevocationMode.NoCheck
                }).WaitAsync(Loopback.Timeout);
                Assert.Equal(SslProtocols.Tls12, tls.SslProtocol);
                using var writer = new StreamWriter(tls, new UTF8Encoding(false), 1024, true) { AutoFlush = true };
                using var reader = new StreamReader(tls, Encoding.UTF8, false, 1024, true);
                await writer.WriteLineAsync(" ").WaitAsync(Loopback.Timeout);
                foreach (var id in new[] { "tls-1", "tls-2" })
                {
                    await writer.WriteLineAsync(JsonConvert.SerializeObject(
                        ProtocolMessage.CreateResponse(MessageTypes.PING, id, null, null))).WaitAsync(Loopback.Timeout);
                    var response = JsonConvert.DeserializeObject<ProtocolMessage>(await reader.ReadLineAsync().WaitAsync(Loopback.Timeout));
                    Assert.Equal(id, response.RequestId);
                    Assert.Equal(ResponseStatus.SUCCESS, response.Status);
                    Assert.Equal("transport-test", response.ServerId);
                }
                Assert.Contains(1, counts);
            }
            await Loopback.Until(() => counts.LastOrDefault(-1) == 0);
        }
        finally { server.Stop(); File.Delete(path); }
    }

    [Fact]
    public async Task Peer_sync_sends_real_tls_json_with_source_identity_and_payload()
    {
        using var certificate = Certificate();
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var manager = new PeerSyncManager("source-node", new[] { $"127.0.0.1:{Loopback.Port(listener)}" }, _ => { });
        var payload = new SeatUpdatedPayload { ShowtimeId = 101, Reason = "HELD", Seat = new SeatModel { SeatId = "A1", Status = SeatStatus.Held } };
        var sending = manager.SyncSeatToPeersAsync(payload);
        using var accepted = await listener.AcceptTcpClientAsync().WaitAsync(Loopback.Timeout);
        using var tls = new SslStream(accepted.GetStream());
        await tls.AuthenticateAsServerAsync(certificate, false, SslProtocols.Tls12, false).WaitAsync(Loopback.Timeout);
        using var reader = new StreamReader(tls, Encoding.UTF8, false, 1024, true);
        var message = JsonConvert.DeserializeObject<ProtocolMessage>(await reader.ReadLineAsync().WaitAsync(Loopback.Timeout));
        await sending.WaitAsync(Loopback.Timeout);
        Assert.Equal(MessageTypes.SERVER_SYNC_SEAT, message.MessageType);
        Assert.Equal("source-node", message.ServerId);
        Assert.False(string.IsNullOrWhiteSpace(message.RequestId));
        var received = message.GetData<SeatUpdatedPayload>();
        Assert.Equal(101, received.ShowtimeId);
        Assert.Equal("A1", received.Seat.SeatId);
        Assert.Equal(SeatStatus.Held, received.Seat.Status);
        Assert.Equal("HELD", received.Reason);
    }

    private static X509Certificate2 Certificate()
    {
        using var rsa = RSA.Create(2048);
        var request = new CertificateRequest("CN=localhost", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        using var generated = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddHours(1));
        // Reimport gives Windows Schannel a persisted key handle (ephemeral RSA is unsupported).
        return new X509Certificate2(generated.Export(X509ContentType.Pfx, "test-only"), "test-only", X509KeyStorageFlags.Exportable);
    }
}
