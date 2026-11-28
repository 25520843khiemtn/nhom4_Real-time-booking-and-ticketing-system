using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using TicketBooking.LoadBalancer;
using Xunit;

namespace TransportTests;

public class LoadBalancerTests
{
    [Fact]
    public void Backend_connection_counter_is_atomic_under_parallel_updates()
    {
        var node = new BackendNode("test", "127.0.0.1", 1);
        Parallel.For(0, 10000, _ => node.IncrementConnections());
        Assert.Equal(10000, node.ActiveConnections);
        Parallel.For(0, 10000, _ => node.DecrementConnections());
        Assert.Equal(0, node.ActiveConnections);
    }

    [Fact]
    public async Task Proxy_preserves_binary_bytes_bidirectionally_and_releases_connection_count()
    {
        await using var backend = new EchoBackend();
        var node = backend.Node;
        var balancer = new TcpLoadBalancer(0, new() { node });
        try
        {
            balancer.Start();
            using (var client = await Loopback.Connect(Loopback.BoundPort(balancer)))
            {
                var payload = Enumerable.Range(0, 40000).Select(i => (byte)(i % 256)).ToArray();
                var stream = client.GetStream();
                await stream.WriteAsync(payload).AsTask().WaitAsync(Loopback.Timeout);
                var received = new byte[payload.Length];
                await stream.ReadExactlyAsync(received).AsTask().WaitAsync(Loopback.Timeout);
                Assert.Equal(payload, received);
                Assert.Equal(1, node.ActiveConnections);
            }
            await Loopback.Until(() => node.ActiveConnections == 0);
        }
        finally { balancer.Stop(); }
    }

    [Fact]
    public async Task Failed_selected_backend_is_marked_offline_and_same_client_fails_over()
    {
        await using var backend = new EchoBackend();
        // Bound but not listening: reserves an ephemeral port that refuses connections.
        using var refused = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        refused.Bind(new IPEndPoint(IPAddress.Loopback, 0));
        var dead = new BackendNode("refused", "127.0.0.1", ((IPEndPoint)refused.LocalEndPoint).Port);
        var balancer = new TcpLoadBalancer(0, new() { dead, backend.Node });
        try
        {
            balancer.Start();
            await Loopback.Until(() => !dead.IsAlive); // initial health check has rejected it
            dead.IsAlive = true; // stale healthy state forces the real connect/failover path
            using (var client = await Loopback.Connect(Loopback.BoundPort(balancer)))
            {
                await client.GetStream().WriteAsync(new byte[] { 17, 0, 255 }).AsTask().WaitAsync(Loopback.Timeout);
                var reply = new byte[3];
                await client.GetStream().ReadExactlyAsync(reply).AsTask().WaitAsync(Loopback.Timeout);
                Assert.Equal(new byte[] { 17, 0, 255 }, reply);
                Assert.False(dead.IsAlive);
                Assert.Equal(0, dead.ActiveConnections);
                Assert.Equal(1, backend.Node.ActiveConnections);
            }
            await Loopback.Until(() => backend.Node.ActiveConnections == 0);
        }
        finally { balancer.Stop(); }
    }

    [Fact]
    public async Task Empty_backend_pool_closes_client_without_hanging()
    {
        var balancer = new TcpLoadBalancer(0, new());
        try
        {
            balancer.Start();
            using var client = await Loopback.Connect(Loopback.BoundPort(balancer));
            Assert.Equal(0, await client.GetStream().ReadAsync(new byte[1]).AsTask().WaitAsync(Loopback.Timeout));
        }
        finally { balancer.Stop(); }
    }

    private sealed class EchoBackend : IAsyncDisposable
    {
        private readonly TcpListener listener = new(IPAddress.Loopback, 0);
        private readonly CancellationTokenSource stop = new();
        private readonly ConcurrentBag<Task> sessions = new();
        private readonly Task accepting;
        internal BackendNode Node { get; }
        internal EchoBackend()
        {
            listener.Start();
            Node = new("echo", "127.0.0.1", Loopback.Port(listener));
            accepting = Accept();
        }
        private async Task Accept()
        {
            try
            {
                while (!stop.IsCancellationRequested)
                {
                    var client = await listener.AcceptTcpClientAsync(stop.Token);
                    sessions.Add(Echo(client));
                }
            }
            catch (OperationCanceledException) when (stop.IsCancellationRequested) { }
        }
        private async Task Echo(TcpClient client)
        {
            using (client)
            {
                try
                {
                    var buffer = new byte[4096];
                    int count;
                    while ((count = await client.GetStream().ReadAsync(buffer, stop.Token)) > 0)
                        await client.GetStream().WriteAsync(buffer.AsMemory(0, count), stop.Token);
                }
                catch (OperationCanceledException) when (stop.IsCancellationRequested) { }
                catch (System.IO.IOException) { }
            }
        }
        public async ValueTask DisposeAsync()
        {
            stop.Cancel();
            await accepting.WaitAsync(Loopback.Timeout);
            listener.Stop();
            await Task.WhenAll(sessions).WaitAsync(Loopback.Timeout);
            stop.Dispose();
        }
    }
}
