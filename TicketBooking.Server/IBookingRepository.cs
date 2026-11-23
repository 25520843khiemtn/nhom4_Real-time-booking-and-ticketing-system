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
        bool TryHoldSeat(string userId, int showtimeId, string seatId, int holdSeconds, out SeatModel resultSeat, out string errorMessage);
        bool TryCancelHold(string userId, int showtimeId, string seatId, out SeatModel resultSeat, out string errorMessage);
        List<SeatModel> ReleaseSeatsByUser(string userId);
        List<SeatModel> ReleaseExpiredSeats();
        bool ExecuteBuyTransaction(string userId, int showtimeId, List<string> seatIds, decimal unitPrice, out TransactionModel transaction, out string errorMessage);
        List<TransactionModel> GetUserTransactions(string userId);
    }
}