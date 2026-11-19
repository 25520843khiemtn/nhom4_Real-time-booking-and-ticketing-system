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
        private int _nextTransactionId = 1000;
        private int _nextDetailId = 5000;

        public InMemoryBookingRepository()
        {
            SeedSampleData();
        }

        private void SeedSampleData()
        {
            _users["khiem"] = "123456";
            _users["hanh"] = "123456";
            _users["giang"] = "123456";

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
            string storedPwd;
            if (_users.TryGetValue(username, out storedPwd) && storedPwd == plainPassword)
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
            _users[username] = plainPassword;
            return true;
        }

        public List<MovieModel> GetMovies()
        {
            return _movies.ToList();
        }

        public List<ShowtimeModel> GetShowtimes(int movieId)
        {
            return _showtimes.Where(s => s.MovieId == movieId).ToList();
        }

        public ShowtimeModel GetShowtimeById(int showtimeId)
        {
            return _showtimes.FirstOrDefault(s => s.ShowtimeId == showtimeId);
        }

        public List<SeatModel> GetSeats(int showtimeId)
        {
            return _seats.Where(s => s.ShowtimeId == showtimeId).ToList();
        }

        public SeatModel GetSeat(int showtimeId, string seatId)
        {
            return _seats.FirstOrDefault(s => s.ShowtimeId == showtimeId && s.SeatId.Equals(seatId, StringComparison.OrdinalIgnoreCase));
        }

        public void SaveSeatState(SeatModel seat)
        {
        }

        public List<SeatModel> GetAllHeldSeats()
        {
            return _seats.Where(s => s.Status == SeatStatus.Held).ToList();
        }

        public bool ExecuteBuyTransaction(string userId, int showtimeId, List<string> seatIds, decimal unitPrice, out TransactionModel transaction, out string errorMessage)
        {
            transaction = null;
            errorMessage = null;

            List<SeatModel> targetSeats = new List<SeatModel>();
            foreach (string id in seatIds)
            {
                SeatModel seat = GetSeat(showtimeId, id);
                if (seat == null)
                {
                    errorMessage = $"Ghế {id} không tồn tại, giao dịch đã được hoàn tác (Rollback).";
                    return false;
                }
                targetSeats.Add(seat);
            }

            try
            {
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
            catch (Exception ex)
            {
                errorMessage = $"Lỗi giao dịch: {ex.Message}";
                return false;
            }
        }

        public List<TransactionModel> GetUserTransactions(string userId)
        {
            return _transactions.Where(t => t.UserId == userId).OrderByDescending(t => t.BuyTime).ToList();
        }
    }
}