using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Xunit;

namespace TransportTests;

internal static class Loopback
{
    internal static readonly TimeSpan Timeout = TimeSpan.FromSeconds(8);
    internal static int Port(TcpListener listener) => ((IPEndPoint)listener.LocalEndpoint).Port;
    // Production accepts port 0 but exposes no bound endpoint. Only discovery uses reflection;
    // routing, health checks and stream forwarding run unchanged over real sockets.
    internal static int BoundPort(object server) => Port((TcpListener)server.GetType()
        .GetField("_listener", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(server));
    internal static async Task Until(Func<bool> condition)
    {
        using var timeout = new CancellationTokenSource(Timeout);
        while (!condition()) await Task.Delay(10, timeout.Token);
    }
    internal static async Task<TcpClient> Connect(int port)
    {
        var client = new TcpClient();
        try { await client.ConnectAsync(IPAddress.Loopback, port).WaitAsync(Timeout); return client; }
        catch { client.Dispose(); throw; }
    }
}

internal sealed class SocketPair : IDisposable
{
    internal TcpClient Client { get; }
    internal TcpClient Server { get; }
    private SocketPair(TcpClient client, TcpClient server) { Client = client; Server = server; }
    internal static async Task<SocketPair> Create()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var client = await Loopback.Connect(Loopback.Port(listener));
        try { return new(client, await listener.AcceptTcpClientAsync().WaitAsync(Loopback.Timeout)); }
        catch { client.Dispose(); throw; }
    }
    public void Dispose() { Client.Dispose(); Server.Dispose(); }
}
