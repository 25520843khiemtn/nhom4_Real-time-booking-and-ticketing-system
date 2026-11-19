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
            // Ở bản RAM tham chiếu đã cập nhật trực tiếp; khi chuyển sang SQL sẽ chạy lệnh UPDATE tại đây
        }

        public List<SeatModel> GetAllHeldSeats()
        {
            return _seats.Where(s => s.Status == SeatStatus.Held).ToList();
        }
    }
}