using System.Collections.Generic;
using TicketBooking.Shared;

namespace TicketBooking.Server
{
    public interface IBookingRepository
    {
        bool ValidateLogin(string username, string plainPassword, out string userId);
        bool RegisterUser(string username, string plainPassword, string fullName, out string errorMessage);
        List<MovieModel> GetMovies();
        List<ShowtimeModel> GetShowtimes(int movieId);
        List<SeatModel> GetSeats(int showtimeId);
        SeatModel GetSeat(int showtimeId, string seatId);
        void SaveSeatState(SeatModel seat);
        List<SeatModel> GetAllHeldSeats();
    }
}