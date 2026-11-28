using System.Text;
using TicketBooking.Server;
using Xunit;

namespace TransportTests;

public class ConnectionManagerTests
{
    [Fact]
    public async Task Direct_send_broadcast_exclusion_and_removal_use_real_connections()
    {
        using var a = await SocketPair.Create();
        using var b = await SocketPair.Create();
        var manager = new ConnectionManager();
        try
        {
            var first = manager.AddClient(a.Server, a.Server.GetStream());
            var second = manager.AddClient(b.Server, b.Server.GetStream());
            Assert.Equal(2, manager.ActiveCount);
            Assert.NotEqual(first.ConnectionId, second.ConnectionId);
            Assert.Equal(a.Server.Client.RemoteEndPoint.ToString(), first.EndPoint);
            using var ar = new StreamReader(a.Client.GetStream(), Encoding.UTF8, false, 1024, true);
            using var br = new StreamReader(b.Client.GetStream(), Encoding.UTF8, false, 1024, true);
            manager.SendToClient(first, "direct-đúng");
            Assert.Equal("direct-đúng", await ar.ReadLineAsync().WaitAsync(Loopback.Timeout));
            manager.Broadcast("broadcast", first.ConnectionId);
            manager.SendToClient(first, "sentinel");
            Assert.Equal("broadcast", await br.ReadLineAsync().WaitAsync(Loopback.Timeout));
            Assert.Equal("sentinel", await ar.ReadLineAsync().WaitAsync(Loopback.Timeout));
            manager.RemoveClient(first.ConnectionId);
            manager.RemoveClient(first.ConnectionId);
            Assert.Equal(1, manager.ActiveCount);
            Assert.Null(await ar.ReadLineAsync().WaitAsync(Loopback.Timeout));
            manager.ClearAll();
            Assert.Equal(0, manager.ActiveCount);
            Assert.Null(await br.ReadLineAsync().WaitAsync(Loopback.Timeout));
        }
        finally { manager.ClearAll(); }
    }

    [Fact]
    public async Task Broadcast_removes_sessions_whose_real_stream_is_closed()
    {
        using var pair = await SocketPair.Create();
        var manager = new ConnectionManager();
        try
        {
            var session = manager.AddClient(pair.Server, pair.Server.GetStream());
            session.SecureStream.Close();
            manager.Broadcast("closed-stream");
            Assert.Equal(0, manager.ActiveCount);
        }
        finally { manager.ClearAll(); }
    }
}
