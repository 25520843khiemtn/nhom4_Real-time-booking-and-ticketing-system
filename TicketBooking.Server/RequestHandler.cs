using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using TicketBooking.Shared;

namespace TicketBooking.Server
{
    public class RequestHandler
    {
        private readonly BookingService _bookingService;
        private readonly Action<string> _log;

        public RequestHandler(BookingService bookingService, Action<string> logCallback)
        {
            _bookingService = bookingService;
            _log = logCallback;
        }

        public string HandleRawMessage(string rawJson, ClientSession session)
        {
            ProtocolMessage request;
            try
            {
                request = JsonConvert.DeserializeObject<ProtocolMessage>(rawJson);
                if (request == null || string.IsNullOrWhiteSpace(request.MessageType))
                {
                    ProtocolMessage invalidResp = ProtocolMessage.CreateResponse(
                        "UNKNOWN",
                        Guid.NewGuid().ToString(),
                        ResponseStatus.ERROR,
                        new { Message = "Gói tin thiếu trường MessageType." }
                    );
                    return JsonConvert.SerializeObject(invalidResp);
                }
            }
            catch (JsonException ex)
            {
                _log($"[{session.EndPoint}] Gửi JSON không hợp lệ: {ex.Message}");
                ProtocolMessage parseErr = ProtocolMessage.CreateResponse(
                    "PARSE_ERROR",
                    Guid.NewGuid().ToString(),
                    ResponseStatus.ERROR,
                    new { Message = "Định dạng JSON không hợp lệ." }
                );
                return JsonConvert.SerializeObject(parseErr);
            }

            ProtocolMessage response = RouteRequest(request, session);
            return JsonConvert.SerializeObject(response);
        }

        private ProtocolMessage RouteRequest(ProtocolMessage req, ClientSession session)
        {
            switch (req.MessageType)
            {
                case MessageTypes.PING:
                    return ProtocolMessage.CreateResponse(
                        MessageTypes.PING,
                        req.RequestId,
                        ResponseStatus.SUCCESS,
                        new { Message = "PONG", ServerTime = DateTime.Now },
                        req.UserId
                    );

                case MessageTypes.LOGIN:
                    return HandleLogin(req, session);

                case MessageTypes.REGISTER:
                    return HandleRegister(req, session);

                case MessageTypes.GET_MOVIES:
                    return ProtocolMessage.CreateResponse(
                        MessageTypes.GET_MOVIES,
                        req.RequestId,
                        ResponseStatus.SUCCESS,
                        _bookingService.GetMovies(),
                        session.UserId ?? req.UserId
                    );

                case MessageTypes.GET_SHOWTIMES:
                    GetShowtimesPayload showReq = req.GetData<GetShowtimesPayload>();
                    if (showReq == null)
                    {
                        return ProtocolMessage.CreateResponse(MessageTypes.GET_SHOWTIMES, req.RequestId, ResponseStatus.ERROR, new { Message = "Thiếu MovieId." });
                    }
                    return ProtocolMessage.CreateResponse(
                        MessageTypes.GET_SHOWTIMES,
                        req.RequestId,
                        ResponseStatus.SUCCESS,
                        _bookingService.GetShowtimes(showReq.MovieId),
                        session.UserId ?? req.UserId
                    );

                case MessageTypes.GET_SEATS:
                    GetSeatsPayload seatReq = req.GetData<GetSeatsPayload>();
                    if (seatReq == null)
                    {
                        return ProtocolMessage.CreateResponse(MessageTypes.GET_SEATS, req.RequestId, ResponseStatus.ERROR, new { Message = "Thiếu ShowtimeId." });
                    }
                    return ProtocolMessage.CreateResponse(
                        MessageTypes.GET_SEATS,
                        req.RequestId,
                        ResponseStatus.SUCCESS,
                        _bookingService.GetSeats(seatReq.ShowtimeId),
                        session.UserId ?? req.UserId
                    );

                case MessageTypes.HOLD_SEAT:
                    return HandleHoldSeat(req, session);

                case MessageTypes.CANCEL_HOLD:
                    return HandleCancelHold(req, session);

                case MessageTypes.BUY_TICKET:
                    return HandleBuyTicket(req, session);

                case MessageTypes.GET_HISTORY:
                    string historyUser = !string.IsNullOrWhiteSpace(session.UserId) ? session.UserId : req.UserId;
                    if (string.IsNullOrWhiteSpace(historyUser))
                    {
                        return ProtocolMessage.CreateResponse(MessageTypes.GET_HISTORY, req.RequestId, ResponseStatus.ERROR, new { Message = "Vui lòng đăng nhập để xem lịch sử." });
                    }
                    return ProtocolMessage.CreateResponse(
                        MessageTypes.GET_HISTORY,
                        req.RequestId,
                        ResponseStatus.SUCCESS,
                        _bookingService.GetUserHistory(historyUser),
                        historyUser
                    );

                default:
                    return ProtocolMessage.CreateResponse(
                        req.MessageType,
                        req.RequestId,
                        ResponseStatus.ERROR,
                        new { Message = $"Lệnh không được hỗ trợ: {req.MessageType}" },
                        session.UserId ?? req.UserId
                    );
            }
        }

        private ProtocolMessage HandleLogin(ProtocolMessage req, ClientSession session)
        {
            LoginPayload login = req.GetData<LoginPayload>();
            if (login == null || string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
            {
                return ProtocolMessage.CreateResponse(MessageTypes.LOGIN, req.RequestId, ResponseStatus.ERROR, new { Message = "Thiếu thông tin đăng nhập." });
            }

            string decodedPassword;
            try
            {
                decodedPassword = Base64Helper.Decode(login.Password);
            }
            catch (FormatException)
            {
                return ProtocolMessage.CreateResponse(MessageTypes.LOGIN, req.RequestId, ResponseStatus.ERROR, new { Message = "Mật khẩu không đúng định dạng Base64." });
            }

            string validUserId;
            if (!_bookingService.Login(login.Username, decodedPassword, out validUserId))
            {
                _log($"[{session.EndPoint}] Đăng nhập thất bại cho tài khoản '{login.Username}'.");
                return ProtocolMessage.CreateResponse(MessageTypes.LOGIN, req.RequestId, ResponseStatus.ERROR, new { Message = "Sai tên đăng nhập hoặc mật khẩu." });
            }

            session.UserId = validUserId;
            _log($"[{session.EndPoint}] Đăng nhập thành công: User = {validUserId}");
            return ProtocolMessage.CreateResponse(
                MessageTypes.LOGIN,
                req.RequestId,
                ResponseStatus.SUCCESS,
                new { Message = "Đăng nhập thành công.", UserId = validUserId },
                validUserId
            );
        }

        private ProtocolMessage HandleRegister(ProtocolMessage req, ClientSession session)
        {
            RegisterPayload reg = req.GetData<RegisterPayload>();
            if (reg == null || string.IsNullOrWhiteSpace(reg.Username) || string.IsNullOrWhiteSpace(reg.Password))
            {
                return ProtocolMessage.CreateResponse(MessageTypes.REGISTER, req.RequestId, ResponseStatus.ERROR, new { Message = "Thiếu thông tin đăng ký." });
            }

            string decodedPassword;
            try
            {
                decodedPassword = Base64Helper.Decode(reg.Password);
            }
            catch (FormatException)
            {
                return ProtocolMessage.CreateResponse(MessageTypes.REGISTER, req.RequestId, ResponseStatus.ERROR, new { Message = "Mật khẩu đăng ký không đúng định dạng Base64." });
            }

            string errorMsg;
            if (!_bookingService.Register(reg.Username, decodedPassword, reg.FullName, out errorMsg))
            {
                return ProtocolMessage.CreateResponse(MessageTypes.REGISTER, req.RequestId, ResponseStatus.ERROR, new { Message = errorMsg });
            }

            _log($"[{session.EndPoint}] Đăng ký tài khoản mới thành công: User = {reg.Username}");
            return ProtocolMessage.CreateResponse(
                MessageTypes.REGISTER,
                req.RequestId,
                ResponseStatus.SUCCESS,
                new { Message = "Đăng ký tài khoản thành công.", Username = reg.Username },
                reg.Username
            );
        }

        private ProtocolMessage HandleHoldSeat(ProtocolMessage req, ClientSession session)
        {
            string activeUser = !string.IsNullOrWhiteSpace(session.UserId) ? session.UserId : req.UserId;
            if (string.IsNullOrWhiteSpace(activeUser))
            {
                return ProtocolMessage.CreateResponse(MessageTypes.HOLD_SEAT, req.RequestId, ResponseStatus.ERROR, new { Message = "Vui lòng đăng nhập trước khi giữ ghế." });
            }

            SeatActionPayload payload = req.GetData<SeatActionPayload>();
            if (payload == null || string.IsNullOrWhiteSpace(payload.SeatId))
            {
                return ProtocolMessage.CreateResponse(MessageTypes.HOLD_SEAT, req.RequestId, ResponseStatus.ERROR, new { Message = "Thiếu thông tin suất chiếu hoặc mã ghế." });
            }

            SeatModel updatedSeat;
            string message;
            bool ok = _bookingService.HoldSeat(activeUser, payload.ShowtimeId, payload.SeatId, out updatedSeat, out message);

            return ProtocolMessage.CreateResponse(
                MessageTypes.HOLD_SEAT,
                req.RequestId,
                ok ? ResponseStatus.SUCCESS : ResponseStatus.ERROR,
                new { Message = message, Seat = updatedSeat },
                activeUser
            );
        }

        private ProtocolMessage HandleCancelHold(ProtocolMessage req, ClientSession session)
        {
            string activeUser = !string.IsNullOrWhiteSpace(session.UserId) ? session.UserId : req.UserId;
            if (string.IsNullOrWhiteSpace(activeUser))
            {
                return ProtocolMessage.CreateResponse(MessageTypes.CANCEL_HOLD, req.RequestId, ResponseStatus.ERROR, new { Message = "Vui lòng đăng nhập." });
            }

            SeatActionPayload payload = req.GetData<SeatActionPayload>();
            if (payload == null || string.IsNullOrWhiteSpace(payload.SeatId))
            {
                return ProtocolMessage.CreateResponse(MessageTypes.CANCEL_HOLD, req.RequestId, ResponseStatus.ERROR, new { Message = "Thiếu thông tin suất chiếu hoặc mã ghế." });
            }

            SeatModel updatedSeat;
            string message;
            bool ok = _bookingService.CancelHold(activeUser, payload.ShowtimeId, payload.SeatId, out updatedSeat, out message);

            return ProtocolMessage.CreateResponse(
                MessageTypes.CANCEL_HOLD,
                req.RequestId,
                ok ? ResponseStatus.SUCCESS : ResponseStatus.ERROR,
                new { Message = message, Seat = updatedSeat },
                activeUser
            );
        }

        private ProtocolMessage HandleBuyTicket(ProtocolMessage req, ClientSession session)
        {
            string activeUser = !string.IsNullOrWhiteSpace(session.UserId) ? session.UserId : req.UserId;
            if (string.IsNullOrWhiteSpace(activeUser))
            {
                return ProtocolMessage.CreateResponse(MessageTypes.BUY_TICKET, req.RequestId, ResponseStatus.ERROR, new { Message = "Vui lòng đăng nhập hợp lệ trước khi thanh toán." });
            }

            BuyTicketPayload payload = req.GetData<BuyTicketPayload>();
            if (payload == null)
            {
                return ProtocolMessage.CreateResponse(MessageTypes.BUY_TICKET, req.RequestId, ResponseStatus.ERROR, new { Message = "Dữ liệu thanh toán không hợp lệ." });
            }

            List<string> targetSeats = new List<string>();
            if (payload.SeatIds != null && payload.SeatIds.Count > 0)
            {
                targetSeats.AddRange(payload.SeatIds);
            }
            else if (!string.IsNullOrWhiteSpace(payload.SeatId))
            {
                targetSeats.Add(payload.SeatId);
            }

            TransactionModel transaction;
            List<SeatModel> soldSeats;
            string message;
            bool ok = _bookingService.BuyTickets(activeUser, payload.ShowtimeId, targetSeats, out transaction, out soldSeats, out message);

            return ProtocolMessage.CreateResponse(
                MessageTypes.BUY_TICKET,
                req.RequestId,
                ok ? ResponseStatus.SUCCESS : ResponseStatus.ERROR,
                new { Message = message, Transaction = transaction, Seats = soldSeats },
                activeUser
            );
        }
    }
}