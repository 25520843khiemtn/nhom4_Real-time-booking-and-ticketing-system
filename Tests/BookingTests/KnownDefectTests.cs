using TicketBooking.Server;
using TicketBooking.Shared;
using Xunit;

namespace BookingTests;

// Active intended-behavior regressions: never skip or invert these to make the suite green.
[Trait("Category", "KnownDefect")]
public class KnownDefectTests
{
    [Fact]
    public void Blank_only_purchase_must_not_create_zero_ticket_transaction()
    {
        var repo = new InMemoryBookingRepository();
        var service = new BookingService(repo, _ => { });
        var events = new List<SeatUpdatedPayload>();
        service.OnLocalSeatChanged += events.Add;
        var success = service.BuyTickets("khiem", 101, new() { null, "", " \t " }, out var tx, out var sold, out _);
        Assert.False(success);
        Assert.Null(tx);
        Assert.Empty(sold);
        Assert.Empty(repo.GetUserTransactions("khiem"));
        Assert.Empty(events);
    }

    [Fact]
    public void Repository_must_reject_empty_purchase_without_history()
    {
        var repo = new InMemoryBookingRepository();
        Assert.False(repo.ExecuteBuyTransaction("khiem", 101, new(), 85000m, out var tx, out _));
        Assert.Null(tx);
        Assert.Empty(repo.GetUserTransactions("khiem"));
    }

    [Fact]
    public void Repository_must_not_charge_twice_for_duplicate_seat_ids()
    {
        var repo = new InMemoryBookingRepository();
        Assert.True(repo.TryHoldSeat("khiem", 101, "A1", 300, out _, out _));
        var success = repo.ExecuteBuyTransaction("khiem", 101, new() { "A1", "a1" }, 85000m, out var tx, out _);
        // Either reject invalid duplicate input or normalize to exactly one ticket.
        if (!success)
        {
            Assert.Null(tx);
            Assert.Empty(repo.GetUserTransactions("khiem"));
            Assert.Equal(SeatStatus.Held, repo.GetSeat(101, "A1").Status);
            return;
        }
        Assert.Single(tx.Details);
        Assert.Equal(85000m, tx.TotalPrice);
    }

    [Fact]
    public void Remote_hold_sync_must_prevent_second_server_from_granting_same_seat()
    {
        var first = new BookingService(new InMemoryBookingRepository(), _ => { });
        var second = new BookingService(new InMemoryBookingRepository(), _ => { });
        first.OnLocalSeatChanged += payload => second.ApplyRemoteSeatSync("server-1", payload);
        Assert.True(first.HoldSeat("khiem", 101, "A1", out _, out _));
        Assert.False(second.HoldSeat("hanh", 101, "A1", out _, out _));
    }
}
