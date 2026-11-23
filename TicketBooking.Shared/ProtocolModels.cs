using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TicketBooking.Shared
{
    public static class MessageTypes
    {
        public const string PING = "PING";
        public const string LOGIN = "LOGIN";
        public const string REGISTER = "REGISTER";
        public const string GET_MOVIES = "GET_MOVIES";
        public const string GET_SHOWTIMES = "GET_SHOWTIMES";
        public const string GET_SEATS = "GET_SEATS";
        public const string HOLD_SEAT = "HOLD_SEAT";
        public const string CANCEL_HOLD = "CANCEL_HOLD";
        public const string BUY_TICKET = "BUY_TICKET";
        public const string GET_HISTORY = "GET_HISTORY";
        public const string SEAT_UPDATED = "SEAT_UPDATED";

        // Protocol v2: Đồng bộ liên Server (Multi-Server)
        public const string SERVER_SYNC_SEAT = "SERVER_SYNC_SEAT";
        public const string SERVER_HEARTBEAT = "SERVER_HEARTBEAT";
    }

    public static class ResponseStatus
    {
        public const string SUCCESS = "SUCCESS";
        public const string ERROR = "ERROR";
    }

    public enum SeatStatus
    {
        Available = 0,
        Held = 1,
        Sold = 2
    }

    public static class Base64Helper
    {
        public static string Encode(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }

        public static string Decode(string base64Text)
        {
            if (string.IsNullOrEmpty(base64Text)) return string.Empty;
            byte[] bytes = Convert.FromBase64String(base64Text);
            return Encoding.UTF8.GetString(bytes);
        }
    }

    public class ProtocolMessage
    {
        public string MessageType { get; set; }
        public string RequestId { get; set; }
        public string UserId { get; set; }
        public string ServerId { get; set; }
        public JToken Data { get; set; }
        public string Status { get; set; }

        public ProtocolMessage()
        {
            RequestId = Guid.NewGuid().ToString();
        }

        public T GetData<T>()
        {
            if (Data == null) return default(T);
            return Data.ToObject<T>();
        }

        public static ProtocolMessage CreateResponse(string messageType, string requestId, string status, object dataObj, string userId = null, string serverId = null)
        {
            return new ProtocolMessage
            {
                MessageType = messageType,
                RequestId = requestId,
                UserId = userId,
                ServerId = serverId,
                Status = status,
                Data = dataObj != null ? JToken.FromObject(dataObj) : null
            };
        }
    }

    public class LoginPayload
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterPayload
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
    }

    public class MovieModel
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public int DurationMinutes { get; set; }
        public string Genre { get; set; }
    }

    public class ShowtimeModel
    {
        public int ShowtimeId { get; set; }
        public int MovieId { get; set; }
        public string RoomName { get; set; }
        public DateTime StartTime { get; set; }
        public decimal TicketPrice { get; set; }
    }

    public class SeatModel
    {
        public int ShowtimeId { get; set; }
        public string SeatId { get; set; }
        public SeatStatus Status { get; set; }
        public string LockedBy { get; set; }
        public DateTime? UnlockTime { get; set; }
    }

    public class GetShowtimesPayload
    {
        public int MovieId { get; set; }
    }

    public class GetSeatsPayload
    {
        public int ShowtimeId { get; set; }
    }

    public class SeatActionPayload
    {
        public int ShowtimeId { get; set; }
        public string SeatId { get; set; }
    }

    public class BuyTicketPayload
    {
        public int ShowtimeId { get; set; }
        public string SeatId { get; set; }
        public List<string> SeatIds { get; set; }
    }

    public class TransactionDetailModel
    {
        public int DetailId { get; set; }
        public int TransactionId { get; set; }
        public string SeatId { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class TransactionModel
    {
        public int TransactionId { get; set; }
        public string UserId { get; set; }
        public int ShowtimeId { get; set; }
        public DateTime BuyTime { get; set; }
        public decimal TotalPrice { get; set; }
        public List<TransactionDetailModel> Details { get; set; }

        public TransactionModel()
        {
            Details = new List<TransactionDetailModel>();
        }
    }

    public class SeatUpdatedPayload
    {
        public int ShowtimeId { get; set; }
        public SeatModel Seat { get; set; }
        public string Reason { get; set; }
    }
}