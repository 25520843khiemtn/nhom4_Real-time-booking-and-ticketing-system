using System;
using Newtonsoft.Json;
using TicketBooking.Shared;

namespace TicketBooking.Server
{
    public class RequestHandler
    {
        private readonly Action<string> _log;

        public RequestHandler(Action<string> logCallback)
        {
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
                    _log($"[{session.EndPoint}] PING -> PONG");
                    return ProtocolMessage.CreateResponse(
                        MessageTypes.PING,
                        req.RequestId,
                        ResponseStatus.SUCCESS,
                        new { Message = "PONG", ServerTime = DateTime.Now },
                        req.UserId
                    );

                case MessageTypes.LOGIN:
                    return HandleLoginFrame(req, session);

                case MessageTypes.REGISTER:
                    return HandleRegisterFrame(req, session);

                default:
                    _log($"[{session.EndPoint}] Yêu cầu chưa triển khai ở Giai đoạn 1: {req.MessageType}");
                    return ProtocolMessage.CreateResponse(
                        req.MessageType,
                        req.RequestId,
                        ResponseStatus.ERROR,
                        new { Message = $"Chức năng {req.MessageType} sẽ được mở ở Giai đoạn 2." },
                        req.UserId
                    );
            }
        }

        private ProtocolMessage HandleLoginFrame(ProtocolMessage req, ClientSession session)
        {
            LoginPayload login = req.GetData<LoginPayload>();
            if (login == null || string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
            {
                return ProtocolMessage.CreateResponse(
                    MessageTypes.LOGIN,
                    req.RequestId,
                    ResponseStatus.ERROR,
                    new { Message = "Thiếu thông tin đăng nhập." }
                );
            }

            try
            {
                string decodedPassword = Base64Helper.Decode(login.Password);
                if (string.IsNullOrWhiteSpace(decodedPassword))
                {
                    return ProtocolMessage.CreateResponse(
                        MessageTypes.LOGIN,
                        req.RequestId,
                        ResponseStatus.ERROR,
                        new { Message = "Mật khẩu giải mã rỗng." }
                    );
                }
            }
            catch (FormatException)
            {
                _log($"[{session.EndPoint}] Lỗi giải mã Base64 ở gói tin LOGIN.");
                return ProtocolMessage.CreateResponse(
                    MessageTypes.LOGIN,
                    req.RequestId,
                    ResponseStatus.ERROR,
                    new { Message = "Dữ liệu đăng nhập không đúng định dạng Base64." }
                );
            }

            session.UserId = login.Username;
            _log($"[{session.EndPoint}] Đăng nhập khung thành công: User = {login.Username}");
            return ProtocolMessage.CreateResponse(
                MessageTypes.LOGIN,
                req.RequestId,
                ResponseStatus.SUCCESS,
                new { Message = "Đăng nhập thành công (Chế độ khung Tuần 3)", Username = login.Username },
                login.Username
            );
        }

        private ProtocolMessage HandleRegisterFrame(ProtocolMessage req, ClientSession session)
        {
            RegisterPayload reg = req.GetData<RegisterPayload>();
            if (reg == null || string.IsNullOrWhiteSpace(reg.Username) || string.IsNullOrWhiteSpace(reg.Password))
            {
                return ProtocolMessage.CreateResponse(
                    MessageTypes.REGISTER,
                    req.RequestId,
                    ResponseStatus.ERROR,
                    new { Message = "Thiếu thông tin đăng ký." }
                );
            }

            try
            {
                string decodedPassword = Base64Helper.Decode(reg.Password);
                if (string.IsNullOrWhiteSpace(decodedPassword))
                {
                    return ProtocolMessage.CreateResponse(
                        MessageTypes.REGISTER,
                        req.RequestId,
                        ResponseStatus.ERROR,
                        new { Message = "Mật khẩu đăng ký giải mã rỗng." }
                    );
                }
            }
            catch (FormatException)
            {
                _log($"[{session.EndPoint}] Lỗi giải mã Base64 ở gói tin REGISTER.");
                return ProtocolMessage.CreateResponse(
                    MessageTypes.REGISTER,
                    req.RequestId,
                    ResponseStatus.ERROR,
                    new { Message = "Dữ liệu đăng ký không đúng định dạng Base64." }
                );
            }

            _log($"[{session.EndPoint}] Đăng ký khung hợp lệ: User = {reg.Username}");
            return ProtocolMessage.CreateResponse(
                MessageTypes.REGISTER,
                req.RequestId,
                ResponseStatus.SUCCESS,
                new { Message = "Đăng ký hợp lệ (Chế độ khung Tuần 3)", Username = reg.Username },
                reg.Username
            );
        }
    }
}