using TicketBooking.Server;
using TicketBooking.Shared;
using Xunit;

namespace BookingTests;

public class BookingServiceTests
{
    private readonly InMemoryBookingRepository repo = new();
    private readonly List<string> logs = new();
    private readonly List<SeatUpdatedPayload> local = new();
    private readonly List<SeatUpdatedPayload> remote = new();
    private readonly BookingService service;

    public BookingServiceTests()
    {
        service = new BookingService(repo, logs.Add);
        service.OnLocalSeatChanged += local.Add;
        service.OnRemoteSeatSynced += remote.Add;
    }

    [Fact]
    public void Authentication_and_catalog_are_forwarded_to_repository()
    {
        Assert.True(service.Register("service-user", "secret", "Test", out _));
        Assert.True(service.Login("service-user", "secret", out var id));
        Assert.Equal("service-user", id);
        Assert.False(service.Login("service-user", "bad", out _));
        Assert.False(service.Register("service-user", "other", "Test", out _));
        Assert.Equal(2, service.GetMovies().Count);
        Assert.Equal(2, service.GetShowtimes(1).Count);
        Assert.Equal(15, service.GetSeats(101).Count);
        Assert.Empty(service.GetUserHistory(id));
    }

    [Fact]
    public void Successful_hold_and_cancel_publish_local_events_but_denials_do_not()
    {
        Assert.True(service.HoldSeat("khiem", 101, "a1", out var held, out var message));
        Assert.False(string.IsNullOrWhiteSpace(message));
        var changed = Assert.Single(local);
        Assert.Same(held, changed.Seat);
        Assert.Equal(101, changed.ShowtimeId);
        Assert.Equal("HELD", changed.Reason);
        Assert.False(service.HoldSeat("hanh", 101, "A1", out _, out _));
        Assert.False(service.CancelHold("hanh", 101, "A1", out _, out _));
        Assert.Single(local);
        Assert.True(service.CancelHold("khiem", 101, "A1", out var released, out _));
        Assert.Equal("CANCELLED", local[1].Reason);
        Assert.Same(released, local[1].Seat);
        Assert.Equal(SeatStatus.Available, released.Status);
        Assert.Equal(2, logs.Count);
        Assert.Empty(remote);
    }

    [Fact]
    public void Purchase_normalizes_deduplicates_and_publishes_one_event_per_sold_seat()
    {
        Assert.True(service.HoldSeat("khiem", 101, "A1", out _, out _));
        Assert.True(service.HoldSeat("khiem", 101, "A2", out _, out _));
        local.Clear();
        Assert.True(service.BuyTickets("khiem", 101, new() { " a1 ", "A1", null, " ", "a2" }, out var tx, out var sold, out var message));
        Assert.False(string.IsNullOrWhiteSpace(message));
        Assert.Equal(170000m, tx.TotalPrice);
        Assert.Equal(new[] { "A1", "A2" }, sold.Select(s => s.SeatId));
        Assert.Equal(2, local.Count);
        Assert.All(local, e =>
        {
            Assert.Equal("SOLD", e.Reason);
            Assert.Equal(101, e.ShowtimeId);
            Assert.Equal(SeatStatus.Sold, e.Seat.Status);
            Assert.Equal("khiem", e.Seat.LockedBy);
            Assert.Null(e.Seat.UnlockTime);
        });
        Assert.Same(tx, Assert.Single(service.GetUserHistory("khiem")));
        Assert.Empty(remote);
        Assert.Contains(logs, l => l.Contains("[BUY_SUCCESS]"));
    }

    [Fact]
    public void Rejected_purchase_leaves_valid_hold_and_publishes_no_sale_events()
    {
        service.HoldSeat("khiem", 101, "A1", out _, out _);
        local.Clear();
        Assert.False(service.BuyTickets("khiem", 101, new() { "A1", "Z99" }, out var tx, out var sold, out var message));
        Assert.Null(tx);
        Assert.Empty(sold);
        Assert.Empty(local);
        Assert.Equal(SeatStatus.Held, repo.GetSeat(101, "A1").Status);
        Assert.False(string.IsNullOrWhiteSpace(message));
        Assert.Contains(logs, l => l.Contains("[BUY_ROLLBACK]"));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Null_or_empty_purchase_is_rejected(bool useNull)
    {
        Assert.False(service.BuyTickets("khiem", 101, useNull ? null : new(), out var tx, out var sold, out var message));
        Assert.Null(tx);
        Assert.Empty(sold);
        Assert.Empty(local);
        Assert.False(string.IsNullOrWhiteSpace(message));
    }

    [Fact]
    public void Missing_showtime_purchase_is_rejected_without_side_effects()
    {
        Assert.False(service.BuyTickets("khiem", -1, new() { "A1" }, out var tx, out var sold, out _));
        Assert.Null(tx);
        Assert.Empty(sold);
        Assert.Empty(local);
        Assert.Empty(service.GetUserHistory("khiem"));
    }

    [Fact]
    public void Disconnect_publishes_only_released_seats_and_ignores_blank_user()
    {
        service.HoldSeat("khiem", 101, "A1", out _, out _);
        service.HoldSeat("hanh", 101, "A2", out _, out _);
        local.Clear();
        service.ReleaseSeatsByUser(null);
        service.ReleaseSeatsByUser(" ");
        Assert.Empty(local);
        service.ReleaseSeatsByUser("khiem");
        var e = Assert.Single(local);
        Assert.Equal("DISCONNECT_RELEASE", e.Reason);
        Assert.Equal("A1", e.Seat.SeatId);
        Assert.Equal(SeatStatus.Available, e.Seat.Status);
        Assert.Equal(SeatStatus.Held, repo.GetSeat(101, "A2").Status);
    }

    [Fact]
    public void Remote_sync_notifies_remote_only_without_rebroadcast_or_repository_mutation()
    {
        service.ApplyRemoteSeatSync("server-2", null);
        service.ApplyRemoteSeatSync("server-2", new SeatUpdatedPayload());
        Assert.Empty(remote);
        var payload = new SeatUpdatedPayload { ShowtimeId = 101, Reason = "SOLD", Seat = new() { ShowtimeId = 101, SeatId = "A1", Status = SeatStatus.Sold } };
        service.ApplyRemoteSeatSync("server-2", payload);
        Assert.Same(payload, Assert.Single(remote));
        Assert.Empty(local);
        Assert.Equal(SeatStatus.Available, repo.GetSeat(101, "A1").Status);
        Assert.Contains(logs, l => l.Contains("server-2"));
    }

    [Fact]
    public async Task Timeout_watcher_releases_expired_hold_and_publishes_event_then_cancels()
    {
        Assert.True(repo.TryHoldSeat("khiem", 101, "A1", -1, out _, out _));
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var observed = new TaskCompletionSource<SeatUpdatedPayload>(TaskCreationOptions.RunContinuationsAsynchronously);
        service.OnLocalSeatChanged += e => { if (e.Reason == "TIMEOUT") { observed.TrySetResult(e); cancellation.Cancel(); } };
        var watcher = service.StartTimeoutWatcherAsync(cancellation.Token);
        var payload = await observed.Task.WaitAsync(TimeSpan.FromSeconds(5));
        await watcher.WaitAsync(TimeSpan.FromSeconds(5));
        Assert.Equal("A1", payload.Seat.SeatId);
        Assert.Equal(SeatStatus.Available, payload.Seat.Status);
        Assert.Null(payload.Seat.LockedBy);
        Assert.Single(local);
    }

    [Fact]
    public async Task Already_cancelled_watcher_exits_without_events()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await service.StartTimeoutWatcherAsync(cancellation.Token).WaitAsync(TimeSpan.FromSeconds(2));
        Assert.Empty(local);
    }
}
