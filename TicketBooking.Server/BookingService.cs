using System;
using System.Collections.Generic;
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

                // Kiểm tra nếu ghế đang giữ nhưng đã quá hạn mà luồng quét chưa kịp chạy
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
                    if (seat.LockedBy == userId)
                    {
                        message = $"Bạn đang giữ ghế {seat.SeatId} rồi.";
                    }
                    else
                    {
                        message = $"Ghế {seat.SeatId} đang được người dùng khác giữ.";
                    }
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

        public void ReleaseSeatsByUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;
            List<SeatUpdatedPayload> updates = new List<SeatUpdatedPayload>();

            lock (_seatLock)
            {
                List<SeatModel> heldSeats = _repo.GetAllHeldSeats();
                foreach (SeatModel seat in heldSeats)
                {
                    if (seat.LockedBy == userId)
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