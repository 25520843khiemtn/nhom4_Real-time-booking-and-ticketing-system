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
        private readonly Action<string> _log;
        private const int HoldDurationSeconds = 300;

        public event Action<SeatUpdatedPayload> OnLocalSeatChanged;
        public event Action<SeatUpdatedPayload> OnRemoteSeatSynced;

        public BookingService(IBookingRepository repository, Action<string> logCallback)
        {
            _repo = repository;
            _log = logCallback;
        }

        public bool Login(string username, string plainPassword, out string userId) { return _repo.ValidateLogin(username, plainPassword, out userId); }
        public bool Register(string username, string plainPassword, string fullName, out string errorMessage) { return _repo.RegisterUser(username, plainPassword, fullName, out errorMessage); }
        public List<MovieModel> GetMovies() { return _repo.GetMovies(); }
        public List<ShowtimeModel> GetShowtimes(int movieId) { return _repo.GetShowtimes(movieId); }
        public List<SeatModel> GetSeats(int showtimeId) { return _repo.GetSeats(showtimeId); }
        public List<TransactionModel> GetUserHistory(string userId) { return _repo.GetUserTransactions(userId); }

        public bool HoldSeat(string userId, int showtimeId, string seatId, out SeatModel resultSeat, out string message)
        {
            string err;
            bool ok = _repo.TryHoldSeat(userId, showtimeId, seatId, HoldDurationSeconds, out resultSeat, out err);
            if (!ok)
            {
                message = err;
                return false;
            }

            _log($"[HOLD] User '{userId}' giữ ghế {resultSeat.SeatId} (Suất {showtimeId}) đến {resultSeat.UnlockTime:HH:mm:ss}.");
            OnLocalSeatChanged?.Invoke(new SeatUpdatedPayload
            {
                ShowtimeId = showtimeId,
                Seat = resultSeat,
                Reason = "HELD"
            });

            message = $"Giữ ghế {resultSeat.SeatId} thành công trong {HoldDurationSeconds / 60} phút.";
            return true;
        }

        public bool CancelHold(string userId, int showtimeId, string seatId, out SeatModel resultSeat, out string message)
        {
            string err;
            bool ok = _repo.TryCancelHold(userId, showtimeId, seatId, out resultSeat, out err);
            if (!ok)
            {
                message = err;
                return false;
            }

            _log($"[CANCEL] User '{userId}' hủy giữ ghế {resultSeat.SeatId} (Suất {showtimeId}).");
            OnLocalSeatChanged?.Invoke(new SeatUpdatedPayload
            {
                ShowtimeId = showtimeId,
                Seat = resultSeat,
                Reason = "CANCELLED"
            });

            message = $"Đã hủy giữ ghế {resultSeat.SeatId}.";
            return true;
        }

        public bool BuyTickets(string userId, int showtimeId, List<string> rawSeatIds, out TransactionModel transaction, out List<SeatModel> soldSeats, out string message)
        {
            transaction = null;
            soldSeats = new List<SeatModel>();

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

            ShowtimeModel showtime = _repo.GetShowtimeById(showtimeId);
            if (showtime == null)
            {
                message = $"Suất chiếu {showtimeId} không tồn tại.";
                return false;
            }

            string txError;
            bool committed = _repo.ExecuteBuyTransaction(userId, showtimeId, distinctSeatIds, showtime.TicketPrice, out transaction, out txError);
            if (!committed)
            {
                message = txError ?? "Giao dịch mua vé thất bại, dữ liệu đã được hoàn tác.";
                _log($"[BUY_ROLLBACK] User '{userId}' mua vé thất bại: {message}");
                return false;
            }

            foreach (string seatId in distinctSeatIds)
            {
                SeatModel soldSeat = new SeatModel
                {
                    ShowtimeId = showtimeId,
                    SeatId = seatId,
                    Status = SeatStatus.Sold,
                    LockedBy = userId,
                    UnlockTime = null
                };
                soldSeats.Add(soldSeat);

                OnLocalSeatChanged?.Invoke(new SeatUpdatedPayload
                {
                    ShowtimeId = showtimeId,
                    Seat = soldSeat,
                    Reason = "SOLD"
                });
            }

            _log($"[BUY_SUCCESS] User '{userId}' mua thành công {soldSeats.Count} vé ({string.Join(", ", distinctSeatIds)}) - Mã GD #{transaction.TransactionId}.");
            message = $"Thanh toán thành công {soldSeats.Count} vé (Mã GD: #{transaction.TransactionId}).";
            return true;
        }

        public void ApplyRemoteSeatSync(string fromServerId, SeatUpdatedPayload payload)
        {
            if (payload == null || payload.Seat == null) return;
            _log($"[CLUSTER_SYNC] Nhận đồng bộ từ [{fromServerId}]: Ghế {payload.Seat.SeatId} (Suất {payload.ShowtimeId}) -> {payload.Reason}.");
            OnRemoteSeatSynced?.Invoke(payload);
        }

        public void ReleaseSeatsByUser(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return;
            List<SeatModel> released = _repo.ReleaseSeatsByUser(userId);
            foreach (SeatModel seat in released)
            {
                _log($"[DISCONNECT] Tự động nhả ghế {seat.SeatId} (Suất {seat.ShowtimeId}) do User '{userId}' mất kết nối.");
                OnLocalSeatChanged?.Invoke(new SeatUpdatedPayload
                {
                    ShowtimeId = seat.ShowtimeId,
                    Seat = seat,
                    Reason = "DISCONNECT_RELEASE"
                });
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

                List<SeatModel> expired = _repo.ReleaseExpiredSeats();
                foreach (SeatModel seat in expired)
                {
                    _log($"[TIMEOUT] Hết hạn 5 phút -> Đã mở lại ghế {seat.SeatId} (Suất {seat.ShowtimeId}).");
                    OnLocalSeatChanged?.Invoke(new SeatUpdatedPayload
                    {
                        ShowtimeId = seat.ShowtimeId,
                        Seat = seat,
                        Reason = "TIMEOUT"
                    });
                }
            }
        }
    }
}