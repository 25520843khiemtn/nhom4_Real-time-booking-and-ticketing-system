using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TicketBooking.Server;
using TicketBooking.Shared;
using Xunit;

namespace TransportTests;

public class RequestHandlerTests
{
    private readonly InMemoryBookingRepository repository = new();
    private RequestHandler Handler => new(new BookingService(repository, _ => { }), "test-node", _ => { });
    private static ProtocolMessage Send(RequestHandler handler, ClientSession session, string type, object data = null, string user = null) =>
        JsonConvert.DeserializeObject<ProtocolMessage>(handler.HandleRawMessage(JsonConvert.SerializeObject(
            ProtocolMessage.CreateResponse(type, "correlation-42", null, data, user)), session));
    private static void Correlated(ProtocolMessage response, string type, string status)
    {
        Assert.Equal("correlation-42", response.RequestId);
        Assert.Equal(type, response.MessageType);
        Assert.Equal(status, response.Status);
        Assert.Equal("test-node", response.ServerId);
    }

    [Theory]
    [InlineData(MessageTypes.PING)]
    [InlineData(MessageTypes.SERVER_HEARTBEAT)]
    [InlineData("UNSUPPORTED")]
    public void Routes_preserve_request_correlation(string type)
    {
        var response = Send(Handler, new(), type);
        Correlated(response, type, type == "UNSUPPORTED" ? ResponseStatus.ERROR : ResponseStatus.SUCCESS);
        if (type == MessageTypes.PING) Assert.Equal("PONG", (string)response.Data["Message"]);
    }

    [Theory]
    [InlineData("{", "PARSE_ERROR")]
    [InlineData("null", "UNKNOWN")]
    [InlineData("{}", "UNKNOWN")]
    public void Invalid_envelopes_return_errors(string raw, string expectedType)
    {
        var result = JsonConvert.DeserializeObject<ProtocolMessage>(Handler.HandleRawMessage(raw, new()));
        Assert.Equal(ResponseStatus.ERROR, result.Status);
        Assert.Equal(expectedType, result.MessageType);
        Assert.False(string.IsNullOrWhiteSpace(result.RequestId));
    }

    [Theory]
    [InlineData(MessageTypes.HOLD_SEAT)]
    [InlineData(MessageTypes.CANCEL_HOLD)]
    [InlineData(MessageTypes.BUY_TICKET)]
    [InlineData(MessageTypes.GET_HISTORY)]
    public void Anonymous_sensitive_requests_are_rejected(string type) =>
        Correlated(Send(Handler, new(), type), type, ResponseStatus.ERROR);

    [Theory]
    [InlineData(MessageTypes.LOGIN)]
    [InlineData(MessageTypes.REGISTER)]
    public void Invalid_base64_does_not_authenticate(string type)
    {
        var session = new ClientSession();
        var result = Send(Handler, session, type, new { Username = "transport-user", Password = "not%base64" });
        Correlated(result, type, ResponseStatus.ERROR);
        Assert.Null(session.UserId);
    }

    [Fact]
    public void Register_then_login_binds_session_and_session_identity_overrides_wire_identity()
    {
        var handler = Handler;
        var session = new ClientSession();
        var credentials = new { Username = "transport-user", Password = Base64Helper.Encode("test-only-password") };
        Correlated(Send(handler, session, MessageTypes.REGISTER, credentials), MessageTypes.REGISTER, ResponseStatus.SUCCESS);
        Assert.Null(session.UserId);
        Correlated(Send(handler, session, MessageTypes.LOGIN, credentials), MessageTypes.LOGIN, ResponseStatus.SUCCESS);
        Assert.Equal("transport-user", session.UserId);
        var hold = Send(handler, session, MessageTypes.HOLD_SEAT, new SeatActionPayload { ShowtimeId = 101, SeatId = "A1" }, "forged-user");
        Correlated(hold, MessageTypes.HOLD_SEAT, ResponseStatus.SUCCESS);
        Assert.Equal("transport-user", repository.GetSeat(101, "A1").LockedBy);
    }

    [Theory]
    [InlineData(MessageTypes.HOLD_SEAT)]
    [InlineData(MessageTypes.CANCEL_HOLD)]
    [InlineData(MessageTypes.BUY_TICKET)]
    [InlineData(MessageTypes.GET_HISTORY)]
    [Trait("Category", "KnownDefect")]
    public void Unauthenticated_wire_user_id_must_not_authorize_sensitive_requests(string type)
    {
        if (type != MessageTypes.HOLD_SEAT)
            repository.TryHoldSeat("forged-user", 101, "A1", 300, out _, out _);
        var result = Send(Handler, new(), type,
            new { ShowtimeId = 101, SeatId = "A1" }, "forged-user");
        Correlated(result, type, ResponseStatus.ERROR);
        Assert.Equal(type == MessageTypes.HOLD_SEAT ? SeatStatus.Available : SeatStatus.Held,
            repository.GetSeat(101, "A1").Status);
    }

    [Theory]
    [InlineData(MessageTypes.LOGIN)]
    [InlineData(MessageTypes.GET_SEATS)]
    [InlineData(MessageTypes.HOLD_SEAT)]
    [Trait("Category", "KnownDefect")]
    public void Malformed_typed_data_returns_correlated_error_instead_of_throwing(string type)
    {
        var result = Send(Handler, new ClientSession { UserId = "test-user" }, type, "not-an-object");
        Correlated(result, type, ResponseStatus.ERROR);
    }

    [Fact]
    [Trait("Category", "KnownDefect")]
    public void Anonymous_peer_sync_must_not_emit_remote_events()
    {
        var service = new BookingService(repository, _ => { });
        var events = 0;
        service.OnRemoteSeatSynced += _ => events++;
        var result = Send(new RequestHandler(service, "test-node", _ => { }), new(), MessageTypes.SERVER_SYNC_SEAT,
            new SeatUpdatedPayload { ShowtimeId = 101, Seat = new SeatModel { SeatId = "A1", Status = SeatStatus.Sold } });
        Assert.Equal(0, events);
        Correlated(result, MessageTypes.SERVER_SYNC_SEAT, ResponseStatus.ERROR);
    }
}
