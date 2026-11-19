using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TicketBooking.Shared;

namespace TicketBooking.Server
{
    public class BookingService
    {
        private readonly IBookingRepository _repo;
        private readonly object _seatLock = new object();
        private readonly Action<string> _log;

        // Quy định giữ ghế tối đa 5 phút (300 giây). Có thể chỉnh thành 15 khi chạy test Timeout.

        private const int HoldDurationSeconds = 300;

        public event Action<SeatUpdatedPayload> OnSeatChanged;

        public BookingService(IBookingRepository repository, Action<string> logCallback)
        {
            _repo = repository;
            _log = logCallback;
        }

        public bool Login(string username, string plainPassword, out string userId)
        {
            return _repo.ValidateLogin(username, plainPassword, out userId);
        }

        public bool Register(string username, string plainPassword, string fullName, out string errorMessage)
        {
            return _repo.RegisterUser(username, plainPassword, fullName, out errorMessage);
        }

        public List<MovieModel> GetMovies()
        {
            return _repo.GetMovies();
        }

        public List<ShowtimeModel> GetShowtimes(int movieId)
        {
            return _repo.GetShowtimes(movieId);
        }

        public List<SeatModel> GetSeats(int showtimeId)
        {
            lock (_seatLock)
            {
                List<SeatModel> rawSeats = _repo.GetSeats(showtimeId);
                List<SeatModel> result = new List<SeatModel>();
                foreach (SeatModel s in rawSeats)
                {
                    result.Add(CloneSeat(s));
                }
                return result;
            }
        }

        public bool HoldSeat(string userId, int showtimeId, string seatId, out SeatModel resultSeat, out string message)
        {
            resultSeat = null;
            SeatUpdatedPayload broadcastData = null;

            lock (_seatLock)
            {
                SeatModel seat = _repo.GetSeat(showtimeId, seatId);
                if (seat == null)
                {
                    message = $"Không tìm thấy ghế {seatId} ở suất chiếu {showtimeId}.";
                    return false;
                }

                if (seat.Status == SeatStatus.Held && seat.UnlockTime.HasValue && seat.UnlockTime.Value <= DateTime.Now)
                {
                    seat.Status = SeatStatus.Available;
                    seat.LockedBy = null;
                    seat.UnlockTime = null;
                }

                if (seat.Status == SeatStatus.Sold)
                {
                    message = $"Ghế {seat.SeatId} đã được bán.";
                    return false;
                }

                if (seat.Status == SeatStatus.Held)
                {
                    message = seat.LockedBy == userId
                        ? $"Bạn đang giữ ghế {seat.SeatId} rồi."
                        : $"Ghế {seat.SeatId} đang được người dùng khác giữ.";
                    return false;
                }

                // Ghế trống -> Khóa cho Client hiện tại
                seat.Status = SeatStatus.Held;
                seat.LockedBy = userId;
                seat.UnlockTime = DateTime.Now.AddSeconds(HoldDurationSeconds);
                _repo.SaveSeatState(seat);

                resultSeat = CloneSeat(seat);
                broadcastData = new SeatUpdatedPayload
                {
                    ShowtimeId = showtimeId,
                    Seat = resultSeat,
                    Reason = "HELD"
                };

                _log($"[HOLD] User '{userId}' giữ ghế {seat.SeatId} (Suất {showtimeId}) đến {seat.UnlockTime:HH:mm:ss}.");
            }

            if (broadcastData != null)
            {
                OnSeatChanged?.Invoke(broadcastData);
            }

            message = $"Giữ ghế {resultSeat.SeatId} thành công trong {HoldDurationSeconds / 60} phút.";
            return true;
        }

        public bool CancelHold(string userId, int showtimeId, string seatId, out SeatModel resultSeat, out string message)
        {
            resultSeat = null;
            SeatUpdatedPayload broadcastData = null;

            lock (_seatLock)
            {
                SeatModel seat = _repo.GetSeat(showtimeId, seatId);
                if (seat == null)
                {
                    message = $"Không tìm thấy ghế {seatId}.";
                    return false;
                }

                if (seat.Status != SeatStatus.Held)
                {
                    message = $"Ghế {seat.SeatId} hiện không ở trạng thái đang giữ.";
                    return false;
                }

                if (seat.LockedBy != userId)
                {
                    message = $"Bạn không có quyền hủy giữ ghế {seat.SeatId} của người khác.";
                    return false;
                }

                seat.Status = SeatStatus.Available;
                seat.LockedBy = null;
                seat.UnlockTime = null;
                _repo.SaveSeatState(seat);

                resultSeat = CloneSeat(seat);
                broadcastData = new SeatUpdatedPayload
                {
                    ShowtimeId = showtimeId,
                    Seat = resultSeat,
                    Reason = "CANCELLED"
                };

                _log($"[CANCEL] User '{userId}' đã hủy giữ ghế {seat.SeatId} (Suất {showtimeId}).");
            }

            if (broadcastData != null)
            {
                OnSeatChanged?.Invoke(broadcastData);
            }

            message = $"Đã hủy giữ ghế {resultSeat.SeatId}.";
            return true;
        }

        public bool BuyTickets(string userId, int showtimeId, List<string> rawSeatIds, out TransactionModel transaction, out List<SeatModel> soldSeats, out string message)
        {
            transaction = null;
            soldSeats = new List<SeatModel>();
            List<SeatUpdatedPayload> broadcasts = new List<SeatUpdatedPayload>();

            if (rawSeatIds == null || rawSeatIds.Count == 0)
            {
                message = "Danh sách ghế thanh toán đang trống.";
                return false;
            }

            List<string> distinctSeatIds = rawSeatIds
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToUpper())
                .Distinct()
                .ToList();

            lock (_seatLock)
            {
                ShowtimeModel showtime = _repo.GetShowtimeById(showtimeId);
                if (showtime == null)
                {
                    message = $"Suất chiếu {showtimeId} không tồn tại.";
                    _log($"[BUY_ERROR] User '{userId}' mua vé thất bại: Suất chiếu {showtimeId} không hợp lệ.");
                    return false;
                }

                DateTime now = DateTime.Now;
                List<SeatUpdatedPayload> expiredBroadcasts = new List<SeatUpdatedPayload>();

                foreach (string seatId in distinctSeatIds)
                {
                    SeatModel seat = _repo.GetSeat(showtimeId, seatId);
                    if (seat == null)
                    {
                        message = $"Ghế {seatId} không tồn tại.";
                        _log($"[BUY_ERROR] User '{userId}' mua ghế không tồn tại ({seatId}).");
                        return false;
                    }

                    if (seat.Status == SeatStatus.Sold)
                    {
                        message = $"Ghế {seat.SeatId} đã có người mua trước đó.";
                        _log($"[BUY_ERROR] User '{userId}' cố mua ghế đã bán ({seat.SeatId}).");
                        return false;
                    }

                    if (seat.Status != SeatStatus.Held || seat.LockedBy != userId)
                    {
                        message = $"Ghế {seat.SeatId} chưa được bạn giữ hoặc đang bị người khác giữ.";
                        _log($"[BUY_ERROR] User '{userId}' không phải người giữ ghế {seat.SeatId}.");
                        return false;
                    }

                    if (!seat.UnlockTime.HasValue || seat.UnlockTime.Value <= now)
                    {
                        seat.Status = SeatStatus.Available;
                        seat.LockedBy = null;
                        seat.UnlockTime = null;
                        _repo.SaveSeatState(seat);

                        expiredBroadcasts.Add(new SeatUpdatedPayload
                        {
                            ShowtimeId = showtimeId,
                            Seat = CloneSeat(seat),
                            Reason = "TIMEOUT"
                        });

                        message = $"Ghế {seat.SeatId} đã hết thời gian giữ 5 phút. Vui lòng chọn lại.";
                        _log($"[BUY_ERROR] User '{userId}' thanh toán trễ hạn ghế {seat.SeatId}.");

                        foreach (var exp in expiredBroadcasts) OnSeatChanged?.Invoke(exp);
                        return false;
                    }
                }

                string txError;
                bool committed = _repo.ExecuteBuyTransaction(userId, showtimeId, distinctSeatIds, showtime.TicketPrice, out transaction, out txError);
                if (!committed)
                {
                    message = txError ?? "Giao dịch mua vé thất bại, dữ liệu đã được hoàn tác.";
                    _log($"[BUY_ROLLBACK] Giao dịch của '{userId}' thất bại: {message}");
                    return false;
                }

                foreach (string seatId in distinctSeatIds)
                {
                    SeatModel updated = CloneSeat(_repo.GetSeat(showtimeId, seatId));
                    soldSeats.Add(updated);
                    broadcasts.Add(new SeatUpdatedPayload
                    {
                        ShowtimeId = showtimeId,
                        Seat = updated,
                        Reason = "SOLD"
                    });
                }

                _log($"[BUY_SUCCESS] User '{userId}' mua thành công {soldSeats.Count} vé ({string.Join(", ", distinctSeatIds)}) - Mã GD #{transaction.TransactionId} - Tổng: {transaction.TotalPrice:N0}đ.");
            }

            foreach (SeatUpdatedPayload item in broadcasts)
            {
                OnSeatChanged?.Invoke(item);
            }

            message = $"Thanh toán thành công {soldSeats.Count} vé (Mã GD: #{transaction.TransactionId}).";
            return true;
        }

        public List<TransactionModel> GetUserHistory(string userId)
        {
            return _repo.GetUserTransactions(userId);
        }

        public void ReleaseSeatsByUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;
            List<SeatUpdatedPayload> updates = new List<SeatUpdatedPayload>();

            lock (_seatLock)
            {
                List<SeatModel> heldSeats = _repo.GetAllHeldSeats();
                foreach (SeatModel seat in heldSeats)
                {
                    if (seat.Status == SeatStatus.Held && seat.LockedBy == userId)
                    {
                        seat.Status = SeatStatus.Available;
                        seat.LockedBy = null;
                        seat.UnlockTime = null;
                        _repo.SaveSeatState(seat);

                        updates.Add(new SeatUpdatedPayload
                        {
                            ShowtimeId = seat.ShowtimeId,
                            Seat = CloneSeat(seat),
                            Reason = "DISCONNECT_RELEASE"
                        });

                        _log($"[DISCONNECT] Tự động nhả ghế {seat.SeatId} (Suất {seat.ShowtimeId}) do User '{userId}' mất kết nối.");
                    }
                }
            }

            foreach (SeatUpdatedPayload item in updates)
            {
                OnSeatChanged?.Invoke(item);
            }
        }

        public async Task StartTimeoutWatcherAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(1000, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                List<SeatUpdatedPayload> expiredUpdates = new List<SeatUpdatedPayload>();

                lock (_seatLock)
                {
                    DateTime now = DateTime.Now;
                    List<SeatModel> heldSeats = _repo.GetAllHeldSeats();

                    foreach (SeatModel seat in heldSeats)
                    {
                        if (seat.Status == SeatStatus.Held && seat.UnlockTime.HasValue && seat.UnlockTime.Value <= now)
                        {
                            string oldHolder = seat.LockedBy;
                            seat.Status = SeatStatus.Available;
                            seat.LockedBy = null;
                            seat.UnlockTime = null;
                            _repo.SaveSeatState(seat);

                            expiredUpdates.Add(new SeatUpdatedPayload
                            {
                                ShowtimeId = seat.ShowtimeId,
                                Seat = CloneSeat(seat),
                                Reason = "TIMEOUT"
                            });

                            _log($"[TIMEOUT] Hết thời gian giữ ghế {seat.SeatId} (Suất {seat.ShowtimeId}) của '{oldHolder}'. Đã mở lại ghế.");
                        }
                    }
                }

                foreach (SeatUpdatedPayload item in expiredUpdates)
                {
                    OnSeatChanged?.Invoke(item);
                }
            }
        }

        private SeatModel CloneSeat(SeatModel source)
        {
            return new SeatModel
            {
                ShowtimeId = source.ShowtimeId,
                SeatId = source.SeatId,
                Status = source.Status,
                LockedBy = source.LockedBy,
                UnlockTime = source.UnlockTime
            };
        }
    }
}