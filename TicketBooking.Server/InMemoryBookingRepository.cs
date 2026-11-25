using System;
using System.Collections.Generic;
using System.Linq;
using TicketBooking.Shared;

namespace TicketBooking.Server
{
    public class InMemoryBookingRepository : IBookingRepository
    {
        private readonly Dictionary<string, string> _users = new Dictionary<string, string>();
        private readonly List<MovieModel> _movies = new List<MovieModel>();
        private readonly List<ShowtimeModel> _showtimes = new List<ShowtimeModel>();
        private readonly List<SeatModel> _seats = new List<SeatModel>();
        private readonly List<TransactionModel> _transactions = new List<TransactionModel>();
        private readonly object _ramLock = new object();
        private int _nextTransactionId = 1000;
        private int _nextDetailId = 5000;

        public InMemoryBookingRepository()
        {
            SeedSampleData();
        }

        private void SeedSampleData()
        {
            _users["khiem"] = BCrypt.Net.BCrypt.HashPassword("123456");
            _users["hanh"] = BCrypt.Net.BCrypt.HashPassword("123456");
            _users["giang"] = BCrypt.Net.BCrypt.HashPassword("123456");

            _movies.Add(new MovieModel { MovieId = 1, Title = "Lật Mặt 8: Vòng Tay Nắng", DurationMinutes = 135, Genre = "Hành động / Gia đình" });
            _movies.Add(new MovieModel { MovieId = 2, Title = "Địa Đạo: Mặt Trời Trong Bóng Tối", DurationMinutes = 128, Genre = "Lịch sử / Chiến tranh" });

            _showtimes.Add(new ShowtimeModel { ShowtimeId = 101, MovieId = 1, RoomName = "Phòng 01", StartTime = DateTime.Today.AddHours(19), TicketPrice = 85000 });
            _showtimes.Add(new ShowtimeModel { ShowtimeId = 102, MovieId = 1, RoomName = "Phòng 02", StartTime = DateTime.Today.AddHours(21), TicketPrice = 95000 });
            _showtimes.Add(new ShowtimeModel { ShowtimeId = 201, MovieId = 2, RoomName = "Phòng IMAX", StartTime = DateTime.Today.AddHours(20), TicketPrice = 120000 });

            int[] showIds = { 101, 102, 201 };
            string[] rows = { "A", "B", "C" };

            foreach (int showId in showIds)
            {
                foreach (string row in rows)
                {
                    for (int col = 1; col <= 5; col++)
                    {
                        _seats.Add(new SeatModel
                        {
                            ShowtimeId = showId,
                            SeatId = row + col,
                            Status = SeatStatus.Available,
                            LockedBy = null,
                            UnlockTime = null
                        });
                    }
                }
            }
        }

        public bool ValidateLogin(string username, string plainPassword, out string userId)
        {
            userId = null;
            string storedHash;
            if (_users.TryGetValue(username, out storedHash) && BCrypt.Net.BCrypt.Verify(plainPassword, storedHash))
            {
                userId = username;
                return true;
            }
            return false;
        }

        public bool RegisterUser(string username, string plainPassword, string fullName, out string errorMessage)
        {
            errorMessage = null;
            if (_users.ContainsKey(username))
            {
                errorMessage = "Tên đăng nhập đã tồn tại.";
                return false;
            }
            _users[username] = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            return true;
        }

        public List<MovieModel> GetMovies() { return _movies.ToList(); }
        public List<ShowtimeModel> GetShowtimes(int movieId) { return _showtimes.Where(s => s.MovieId == movieId).ToList(); }
        public ShowtimeModel GetShowtimeById(int showtimeId) { return _showtimes.FirstOrDefault(s => s.ShowtimeId == showtimeId); }

        public List<SeatModel> GetSeats(int showtimeId)
        {
            lock (_ramLock)
            {
                return _seats.Where(s => s.ShowtimeId == showtimeId).Select(CloneSeat).ToList();
            }
        }

        public SeatModel GetSeat(int showtimeId, string seatId)
        {
            lock (_ramLock)
            {
                SeatModel s = _seats.FirstOrDefault(x => x.ShowtimeId == showtimeId && x.SeatId.Equals(seatId, StringComparison.OrdinalIgnoreCase));
                return s != null ? CloneSeat(s) : null;
            }
        }

        public bool TryHoldSeat(string userId, int showtimeId, string seatId, int holdSeconds, out SeatModel resultSeat, out string errorMessage)
        {
            resultSeat = null;
            errorMessage = null;
            lock (_ramLock)
            {
                SeatModel seat = _seats.FirstOrDefault(s => s.ShowtimeId == showtimeId && s.SeatId.Equals(seatId, StringComparison.OrdinalIgnoreCase));
                if (seat == null) { errorMessage = $"Không tìm thấy ghế {seatId}."; return false; }

                if (seat.Status == SeatStatus.Held && seat.UnlockTime.HasValue && seat.UnlockTime.Value <= DateTime.Now)
                {
                    seat.Status = SeatStatus.Available;
                    seat.LockedBy = null;
                    seat.UnlockTime = null;
                }

                if (seat.Status == SeatStatus.Sold) { errorMessage = $"Ghế {seat.SeatId} đã được bán."; return false; }
                if (seat.Status == SeatStatus.Held)
                {
                    errorMessage = seat.LockedBy == userId ? $"Bạn đang giữ ghế {seat.SeatId} rồi." : $"Ghế {seat.SeatId} đang được người khác giữ.";
                    return false;
                }

                seat.Status = SeatStatus.Held;
                seat.LockedBy = userId;
                seat.UnlockTime = DateTime.Now.AddSeconds(holdSeconds);
                resultSeat = CloneSeat(seat);
                return true;
            }
        }

        public bool TryCancelHold(string userId, int showtimeId, string seatId, out SeatModel resultSeat, out string errorMessage)
        {
            resultSeat = null;
            errorMessage = null;
            lock (_ramLock)
            {
                SeatModel seat = _seats.FirstOrDefault(s => s.ShowtimeId == showtimeId && s.SeatId.Equals(seatId, StringComparison.OrdinalIgnoreCase));
                if (seat == null) { errorMessage = $"Không tìm thấy ghế {seatId}."; return false; }
                if (seat.Status != SeatStatus.Held) { errorMessage = $"Ghế {seat.SeatId} hiện không ở trạng thái đang giữ."; return false; }
                if (seat.LockedBy != userId) { errorMessage = $"Bạn không có quyền hủy giữ ghế {seat.SeatId} của người khác."; return false; }

                seat.Status = SeatStatus.Available;
                seat.LockedBy = null;
                seat.UnlockTime = null;
                resultSeat = CloneSeat(seat);
                return true;
            }
        }

        public List<SeatModel> ReleaseSeatsByUser(string userId)
        {
            List<SeatModel> released = new List<SeatModel>();
            lock (_ramLock)
            {
                foreach (SeatModel seat in _seats.Where(s => s.Status == SeatStatus.Held && s.LockedBy == userId))
                {
                    seat.Status = SeatStatus.Available;
                    seat.LockedBy = null;
                    seat.UnlockTime = null;
                    released.Add(CloneSeat(seat));
                }
            }
            return released;
        }

        public List<SeatModel> ReleaseExpiredSeats()
        {
            List<SeatModel> released = new List<SeatModel>();
            DateTime now = DateTime.Now;
            lock (_ramLock)
            {
                foreach (SeatModel seat in _seats.Where(s => s.Status == SeatStatus.Held && s.UnlockTime.HasValue && s.UnlockTime.Value <= now))
                {
                    seat.Status = SeatStatus.Available;
                    seat.LockedBy = null;
                    seat.UnlockTime = null;
                    released.Add(CloneSeat(seat));
                }
            }
            return released;
        }

        public bool ExecuteBuyTransaction(string userId, int showtimeId, List<string> seatIds, decimal unitPrice, out TransactionModel transaction, out string errorMessage)
        {
            transaction = null;
            errorMessage = null;
            lock (_ramLock)
            {
                DateTime now = DateTime.Now;
                List<SeatModel> targetSeats = new List<SeatModel>();

                foreach (string id in seatIds)
                {
                    SeatModel seat = _seats.FirstOrDefault(s => s.ShowtimeId == showtimeId && s.SeatId.Equals(id, StringComparison.OrdinalIgnoreCase));
                    if (seat == null) { errorMessage = $"Ghế {id} không tồn tại."; return false; }
                    if (seat.Status == SeatStatus.Sold) { errorMessage = $"Ghế {seat.SeatId} đã có người mua trước đó."; return false; }
                    if (seat.Status != SeatStatus.Held || seat.LockedBy != userId) { errorMessage = $"Ghế {seat.SeatId} chưa được bạn giữ hoặc đang bị người khác giữ."; return false; }
                    if (!seat.UnlockTime.HasValue || seat.UnlockTime.Value <= now)
                    {
                        seat.Status = SeatStatus.Available;
                        seat.LockedBy = null;
                        seat.UnlockTime = null;
                        errorMessage = $"Ghế {seat.SeatId} đã hết thời gian giữ 5 phút.";
                        return false;
                    }
                    targetSeats.Add(seat);
                }

                TransactionModel tx = new TransactionModel
                {
                    TransactionId = ++_nextTransactionId,
                    UserId = userId,
                    ShowtimeId = showtimeId,
                    BuyTime = DateTime.Now,
                    TotalPrice = unitPrice * targetSeats.Count
                };

                foreach (SeatModel seat in targetSeats)
                {
                    seat.Status = SeatStatus.Sold;
                    seat.LockedBy = userId;
                    seat.UnlockTime = null;

                    tx.Details.Add(new TransactionDetailModel
                    {
                        DetailId = ++_nextDetailId,
                        TransactionId = tx.TransactionId,
                        SeatId = seat.SeatId,
                        UnitPrice = unitPrice
                    });
                }

                _transactions.Add(tx);
                transaction = tx;
                return true;
            }
        }

        public List<TransactionModel> GetUserTransactions(string userId)
        {
            lock (_ramLock)
            {
                return _transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.BuyTime).ToList();
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