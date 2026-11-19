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
        ShowtimeModel GetShowtimeById(int showtimeId);
        List<SeatModel> GetSeats(int showtimeId);
        SeatModel GetSeat(int showtimeId, string seatId);
        void SaveSeatState(SeatModel seat);
        List<SeatModel> GetAllHeldSeats();
        bool ExecuteBuyTransaction(string userId, int showtimeId, List<string> seatIds, decimal unitPrice, out TransactionModel transaction, out string errorMessage);
        List<TransactionModel> GetUserTransactions(string userId);
    }
}