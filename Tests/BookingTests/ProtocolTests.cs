using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TicketBooking.Shared;
using Xunit;

namespace BookingTests;

public class ProtocolTests
{
    [Theory]
    [InlineData("Hello")]
    [InlineData("Đặt vé: Lật Mặt 🎬\nA1")]
    [InlineData("")]
    public void Base64_roundtrips_utf8(string value) => Assert.Equal(value, Base64Helper.Decode(Base64Helper.Encode(value)));

    [Fact]
    public void Base64_null_is_empty_and_known_encoding_is_standard()
    {
        Assert.Equal("", Base64Helper.Encode(null));
        Assert.Equal("", Base64Helper.Decode(null));
        Assert.Equal("SGVsbG8=", Base64Helper.Encode("Hello"));
        Assert.Equal("Hello", Base64Helper.Decode("SGVsbG8="));
    }

    [Theory]
    [InlineData("not base64!")]
    [InlineData("a")]
    public void Malformed_base64_is_rejected(string value) => Assert.Throws<FormatException>(() => Base64Helper.Decode(value));

    [Fact]
    public void Request_ids_are_valid_unique_guids()
    {
        var ids = Enumerable.Range(0, 100).Select(_ => new ProtocolMessage().RequestId).ToArray();
        Assert.All(ids, id => Assert.True(Guid.TryParse(id, out _)));
        Assert.Equal(ids.Length, ids.Distinct().Count());
    }

    [Fact]
    public void Response_preserves_correlation_routing_status_and_typed_data_across_json()
    {
        var payload = new SeatUpdatedPayload
        {
            ShowtimeId = 101, Reason = "HELD",
            Seat = new() { ShowtimeId = 101, SeatId = "A1", Status = SeatStatus.Held, LockedBy = "khiem", UnlockTime = new DateTime(2026, 11, 28, 19, 30, 0) }
        };
        var response = ProtocolMessage.CreateResponse(MessageTypes.SEAT_UPDATED, "request-123", ResponseStatus.SUCCESS, payload, "khiem", "server-1");
        var decoded = JsonConvert.DeserializeObject<ProtocolMessage>(JsonConvert.SerializeObject(response));
        Assert.Equal("request-123", decoded.RequestId);
        Assert.Equal(MessageTypes.SEAT_UPDATED, decoded.MessageType);
        Assert.Equal(ResponseStatus.SUCCESS, decoded.Status);
        Assert.Equal("khiem", decoded.UserId);
        Assert.Equal("server-1", decoded.ServerId);
        var data = decoded.GetData<SeatUpdatedPayload>();
        Assert.Equal(101, data.ShowtimeId);
        Assert.Equal("HELD", data.Reason);
        Assert.Equal("A1", data.Seat.SeatId);
        Assert.Equal(SeatStatus.Held, data.Seat.Status);
        Assert.Equal("khiem", data.Seat.LockedBy);
        Assert.Equal(payload.Seat.UnlockTime, data.Seat.UnlockTime);
    }

    [Fact]
    public void Null_response_payload_returns_default_typed_data()
    {
        var response = ProtocolMessage.CreateResponse(MessageTypes.LOGIN, "id", ResponseStatus.ERROR, null);
        Assert.Null(response.Data);
        Assert.Null(response.GetData<LoginPayload>());
        Assert.Equal(0, response.GetData<int>());
        Assert.Null(response.UserId);
        Assert.Null(response.ServerId);
    }

    [Fact]
    public void Buy_payload_preserves_legacy_single_seat_and_multi_seat_fields()
    {
        var message = new ProtocolMessage { MessageType = MessageTypes.BUY_TICKET, Data = JToken.FromObject(new BuyTicketPayload { ShowtimeId = 101, SeatId = "A1", SeatIds = new() { "A1", "B2" } }) };
        var payload = message.GetData<BuyTicketPayload>();
        Assert.Equal(101, payload.ShowtimeId);
        Assert.Equal("A1", payload.SeatId);
        Assert.Equal(new[] { "A1", "B2" }, payload.SeatIds);
    }

    [Fact]
    public void Transaction_details_are_initialized_and_survive_wire_roundtrip()
    {
        var tx = new TransactionModel { TransactionId = 42, UserId = "khiem", ShowtimeId = 101, TotalPrice = 85000m };
        Assert.NotNull(tx.Details);
        Assert.Empty(tx.Details);
        tx.Details.Add(new() { DetailId = 9, TransactionId = 42, SeatId = "A1", UnitPrice = 85000m });
        var response = ProtocolMessage.CreateResponse(MessageTypes.GET_HISTORY, "history-id", ResponseStatus.SUCCESS, new[] { tx });
        var restored = JsonConvert.DeserializeObject<ProtocolMessage>(JsonConvert.SerializeObject(response)).GetData<List<TransactionModel>>();
        var item = Assert.Single(restored);
        Assert.Equal(85000m, item.TotalPrice);
        Assert.Equal("A1", Assert.Single(item.Details).SeatId);
    }

    [Fact]
    public void Seat_status_numeric_values_are_wire_compatible()
    {
        Assert.Equal(0, (int)SeatStatus.Available);
        Assert.Equal(1, (int)SeatStatus.Held);
        Assert.Equal(2, (int)SeatStatus.Sold);
        Assert.Equal("{\"Status\":2}", JsonConvert.SerializeObject(new { Status = SeatStatus.Sold }));
    }

    [Fact]
    public void Message_type_names_are_stable_unique_wire_tokens()
    {
        var expected = new[] { "PING", "LOGIN", "REGISTER", "GET_MOVIES", "GET_SHOWTIMES", "GET_SEATS", "HOLD_SEAT", "CANCEL_HOLD", "BUY_TICKET", "GET_HISTORY", "SEAT_UPDATED", "SERVER_SYNC_SEAT", "SERVER_HEARTBEAT" };
        var actual = typeof(MessageTypes).GetFields().Select(f => (string)f.GetRawConstantValue()).ToArray();
        Assert.Equal(expected.OrderBy(x => x), actual.OrderBy(x => x));
        Assert.Equal(actual.Length, actual.Distinct().Count());
        Assert.Equal("SUCCESS", ResponseStatus.SUCCESS);
        Assert.Equal("ERROR", ResponseStatus.ERROR);
    }
}
