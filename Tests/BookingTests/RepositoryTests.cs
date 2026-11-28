using TicketBooking.Server;
using TicketBooking.Shared;
using Xunit;

namespace BookingTests;

public class RepositoryTests
{
    private readonly InMemoryBookingRepository repo = new();

    [Theory]
    [InlineData("khiem", "123456", true)]
    [InlineData("hanh", "123456", true)]
    [InlineData("giang", "123456", true)]
    [InlineData("khiem", "wrong", false)]
    [InlineData("unknown", "123456", false)]
    public void Login_validates_password_and_returns_identity_only_on_success(string user, string password, bool expected)
    {
        Assert.Equal(expected, repo.ValidateLogin(user, password, out var id));
        Assert.Equal(expected ? user : null, id);
    }

    [Fact]
    public void Registration_creates_login_and_duplicate_does_not_replace_password()
    {
        Assert.True(repo.RegisterUser("new-user", "secret", "Tên tiếng Việt", out var error));
        Assert.Null(error);
        Assert.True(repo.ValidateLogin("new-user", "secret", out var id));
        Assert.Equal("new-user", id);
        Assert.False(repo.RegisterUser("new-user", "replacement", "Other", out error));
        Assert.False(string.IsNullOrWhiteSpace(error));
        Assert.False(repo.ValidateLogin("new-user", "replacement", out _));
        Assert.True(repo.ValidateLogin("new-user", "secret", out _));
    }

    [Fact]
    public void Catalog_filters_showtimes_and_seats_by_parent()
    {
        Assert.Equal(2, repo.GetMovies().Count);
        Assert.Equal(new[] { 101, 102 }, repo.GetShowtimes(1).Select(s => s.ShowtimeId));
        Assert.Empty(repo.GetShowtimes(-1));
        Assert.Null(repo.GetShowtimeById(-1));
        Assert.Equal(85000m, repo.GetShowtimeById(101).TicketPrice);
        Assert.Equal(15, repo.GetSeats(101).Count);
        Assert.All(repo.GetSeats(101), s => Assert.Equal(101, s.ShowtimeId));
        Assert.Empty(repo.GetSeats(-1));
    }

    [Fact]
    public void Hold_is_case_insensitive_exclusive_and_cancel_requires_owner()
    {
        var before = DateTime.Now;
        Assert.True(repo.TryHoldSeat("khiem", 101, "a1", 300, out var held, out var error));
        Assert.Null(error);
        Assert.Equal("A1", held.SeatId);
        Assert.Equal(SeatStatus.Held, held.Status);
        Assert.Equal("khiem", held.LockedBy);
        Assert.InRange(held.UnlockTime.Value, before.AddSeconds(300), DateTime.Now.AddSeconds(300));
        Assert.False(repo.TryHoldSeat("khiem", 101, "A1", 300, out _, out _));
        Assert.False(repo.TryHoldSeat("hanh", 101, "A1", 300, out _, out _));
        Assert.False(repo.TryCancelHold("hanh", 101, "A1", out var denied, out error));
        Assert.Null(denied);
        Assert.False(string.IsNullOrWhiteSpace(error));
        Assert.Equal("khiem", repo.GetSeat(101, "A1").LockedBy);
        Assert.True(repo.TryCancelHold("khiem", 101, "a1", out var released, out _));
        AssertAvailable(released);
        Assert.False(repo.TryCancelHold("khiem", 101, "A1", out _, out _));
        Assert.True(repo.TryHoldSeat("hanh", 101, "A1", 300, out _, out _));
    }

    [Fact]
    public void Missing_seat_or_showtime_does_not_create_state()
    {
        Assert.False(repo.TryHoldSeat("khiem", 101, "Z99", 300, out var seat, out var error));
        Assert.Null(seat);
        Assert.False(string.IsNullOrWhiteSpace(error));
        Assert.False(repo.TryHoldSeat("khiem", -1, "A1", 300, out _, out _));
        Assert.False(repo.TryCancelHold("khiem", 101, "Z99", out _, out _));
        Assert.Null(repo.GetSeat(101, "Z99"));
    }

    [Fact]
    public void Returned_seat_snapshots_cannot_mutate_repository()
    {
        repo.TryHoldSeat("khiem", 101, "A1", 300, out var held, out _);
        held.Status = SeatStatus.Sold;
        repo.GetSeat(101, "A1").LockedBy = "intruder";
        repo.GetSeats(101).Single(s => s.SeatId == "A1").UnlockTime = null;
        var actual = repo.GetSeat(101, "A1");
        Assert.Equal(SeatStatus.Held, actual.Status);
        Assert.Equal("khiem", actual.LockedBy);
        Assert.NotNull(actual.UnlockTime);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Expiry_releases_only_expired_holds_without_sleep(int seconds)
    {
        Hold("khiem", "A1", seconds);
        Hold("khiem", "A2");
        var released = Assert.Single(repo.ReleaseExpiredSeats());
        Assert.Equal("A1", released.SeatId);
        AssertAvailable(released);
        AssertAvailable(repo.GetSeat(101, "A1"));
        Assert.Equal(SeatStatus.Held, repo.GetSeat(101, "A2").Status);
        Assert.Empty(repo.ReleaseExpiredSeats());
    }

    [Fact]
    public void Expired_hold_can_be_reclaimed_by_another_user()
    {
        Hold("khiem", "A1", -1);
        Assert.True(repo.TryHoldSeat("hanh", 101, "A1", 300, out var seat, out _));
        Assert.Equal("hanh", seat.LockedBy);
    }

    [Theory]
    [InlineData("A2")]
    [InlineData("B1")]
    [InlineData("Z99")]
    public void Purchase_validation_is_atomic_for_foreign_unheld_or_missing_seat(string invalidSeat)
    {
        Hold("khiem", "A1");
        Hold("hanh", "A2");
        Assert.False(repo.ExecuteBuyTransaction("khiem", 101, new() { "A1", invalidSeat }, 85000m, out var tx, out var error));
        Assert.Null(tx);
        Assert.False(string.IsNullOrWhiteSpace(error));
        Assert.Equal(SeatStatus.Held, repo.GetSeat(101, "A1").Status);
        Assert.Equal("khiem", repo.GetSeat(101, "A1").LockedBy);
        Assert.Equal("hanh", repo.GetSeat(101, "A2").LockedBy);
        Assert.Empty(repo.GetUserTransactions("khiem"));
    }

    [Fact]
    public void Expired_purchase_is_rejected_without_selling_other_valid_hold()
    {
        Hold("khiem", "A1");
        Hold("khiem", "A2", -1);
        Assert.False(repo.ExecuteBuyTransaction("khiem", 101, new() { "A1", "A2" }, 85000m, out var tx, out _));
        Assert.Null(tx);
        Assert.Equal(SeatStatus.Held, repo.GetSeat(101, "A1").Status);
        AssertAvailable(repo.GetSeat(101, "A2"));
        Assert.Empty(repo.GetUserTransactions("khiem"));
    }

    [Fact]
    public void Purchase_commits_details_total_and_user_scoped_newest_first_history()
    {
        Hold("khiem", "A1");
        Hold("khiem", "A2");
        Assert.True(repo.ExecuteBuyTransaction("khiem", 101, new() { "a1", "A2" }, 85000m, out var first, out var error));
        Assert.Null(error);
        Assert.Equal(170000m, first.TotalPrice);
        Assert.Equal("khiem", first.UserId);
        Assert.Equal(101, first.ShowtimeId);
        Assert.Equal(new[] { "A1", "A2" }, first.Details.Select(d => d.SeatId));
        Assert.All(first.Details, d => { Assert.Equal(first.TransactionId, d.TransactionId); Assert.Equal(85000m, d.UnitPrice); });
        Assert.Equal(2, first.Details.Select(d => d.DetailId).Distinct().Count());
        foreach (var id in new[] { "A1", "A2" })
        {
            var sold = repo.GetSeat(101, id);
            Assert.Equal(SeatStatus.Sold, sold.Status);
            Assert.Null(sold.UnlockTime);
            Assert.False(repo.TryHoldSeat("hanh", 101, id, 300, out _, out _));
            Assert.False(repo.TryCancelHold("khiem", 101, id, out _, out _));
        }
        Hold("khiem", "B1");
        Assert.True(repo.ExecuteBuyTransaction("khiem", 101, new() { "B1" }, 85000m, out var second, out _));
        Assert.True(second.TransactionId > first.TransactionId);
        Assert.Equal(new[] { second.TransactionId, first.TransactionId }, repo.GetUserTransactions("khiem").Select(t => t.TransactionId));
        Assert.Empty(repo.GetUserTransactions("hanh"));
        Assert.False(repo.ExecuteBuyTransaction("khiem", 101, new() { "A1" }, 85000m, out _, out _));
        Assert.Equal(2, repo.GetUserTransactions("khiem").Count);
    }

    [Fact]
    public void Disconnect_releases_only_matching_users_held_seats_across_showtimes()
    {
        Hold("khiem", "A1");
        Hold("khiem", "A2");
        Hold("hanh", "B1");
        Assert.True(repo.TryHoldSeat("khiem", 102, "C1", 300, out _, out _));
        Assert.True(repo.ExecuteBuyTransaction("khiem", 101, new() { "A1" }, 85000m, out _, out _));
        var released = repo.ReleaseSeatsByUser("khiem");
        Assert.Equal(2, released.Count);
        Assert.All(released, AssertAvailable);
        Assert.Equal(SeatStatus.Sold, repo.GetSeat(101, "A1").Status);
        Assert.Equal(SeatStatus.Held, repo.GetSeat(101, "B1").Status);
        Assert.Empty(repo.ReleaseSeatsByUser("khiem"));
    }

    [Fact]
    public async Task Contending_holds_have_exactly_one_winner()
    {
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var tasks = Enumerable.Range(0, 32).Select(async i =>
        {
            await gate.Task;
            var user = "contender-" + i;
            return (user, won: repo.TryHoldSeat(user, 101, "A1", 300, out _, out _));
        }).ToArray();
        gate.SetResult();
        var results = await Task.WhenAll(tasks).WaitAsync(TimeSpan.FromSeconds(10));
        var winner = Assert.Single(results, r => r.won);
        Assert.Equal(winner.user, repo.GetSeat(101, "A1").LockedBy);
    }

    [Fact]
    public async Task Concurrent_purchase_of_same_hold_commits_exactly_once()
    {
        Hold("khiem", "A1");
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var tasks = Enumerable.Range(0, 16).Select(async _ =>
        {
            await gate.Task;
            return repo.ExecuteBuyTransaction("khiem", 101, new() { "A1" }, 85000m, out var transaction, out var error);
        }).ToArray();
        gate.SetResult();
        var results = await Task.WhenAll(tasks).WaitAsync(TimeSpan.FromSeconds(10));
        Assert.Single(results, won => won);
        Assert.Single(repo.GetUserTransactions("khiem"));
        Assert.Equal(SeatStatus.Sold, repo.GetSeat(101, "A1").Status);
    }

    private void Hold(string user, string id, int seconds = 300) =>
        Assert.True(repo.TryHoldSeat(user, 101, id, seconds, out _, out _));

    private static void AssertAvailable(SeatModel seat)
    {
        Assert.Equal(SeatStatus.Available, seat.Status);
        Assert.Null(seat.LockedBy);
        Assert.Null(seat.UnlockTime);
    }
}
