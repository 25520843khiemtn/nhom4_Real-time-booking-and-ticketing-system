using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TicketBooking.Shared;

namespace TicketBooking.Server
{
    public class SqlBookingRepository : IBookingRepository
    {
        private readonly string _connectionString;

        public SqlBookingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool ValidateLogin(string username, string plainPassword, out string userId)
        {
            userId = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT UserId, PasswordHash FROM Users WHERE Username = @Username";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string dbUserId = reader["UserId"].ToString();
                            string hash = reader["PasswordHash"].ToString();
                            if (BCrypt.Net.BCrypt.Verify(plainPassword, hash))
                            {
                                userId = dbUserId;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public bool RegisterUser(string username, string plainPassword, string fullName, out string errorMessage)
        {
            errorMessage = null;
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string checkSql = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
                using (SqlCommand checkCmd = new SqlCommand(checkSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Username", username);
                    if ((int)checkCmd.ExecuteScalar() > 0)
                    {
                        errorMessage = "Tên đăng nhập đã tồn tại.";
                        return false;
                    }
                }

                string insertSql = "INSERT INTO Users (Username, PasswordHash, FullName) VALUES (@Username, @PasswordHash, @FullName)";
                using (SqlCommand cmd = new SqlCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    cmd.Parameters.AddWithValue("@FullName", (object)fullName ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        public List<MovieModel> GetMovies()
        {
            List<MovieModel> list = new List<MovieModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT MovieId, Title, DurationMinutes, Genre FROM Movies", conn))
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new MovieModel
                        {
                            MovieId = Convert.ToInt32(r["MovieId"]),
                            Title = r["Title"].ToString(),
                            DurationMinutes = Convert.ToInt32(r["DurationMinutes"]),
                            Genre = r["Genre"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<ShowtimeModel> GetShowtimes(int movieId)
        {
            List<ShowtimeModel> list = new List<ShowtimeModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"SELECT s.ShowtimeId, s.MovieId, r.RoomName, s.StartTime, s.TicketPrice 
                               FROM Showtimes s JOIN Rooms r ON s.RoomId = r.RoomId 
                               WHERE s.MovieId = @MovieId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MovieId", movieId);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new ShowtimeModel
                            {
                                ShowtimeId = Convert.ToInt32(r["ShowtimeId"]),
                                MovieId = Convert.ToInt32(r["MovieId"]),
                                RoomName = r["RoomName"].ToString(),
                                StartTime = Convert.ToDateTime(r["StartTime"]),
                                TicketPrice = Convert.ToDecimal(r["TicketPrice"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        public ShowtimeModel GetShowtimeById(int showtimeId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"SELECT s.ShowtimeId, s.MovieId, r.RoomName, s.StartTime, s.TicketPrice 
                               FROM Showtimes s JOIN Rooms r ON s.RoomId = r.RoomId 
                               WHERE s.ShowtimeId = @ShowtimeId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ShowtimeId", showtimeId);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            return new ShowtimeModel
                            {
                                ShowtimeId = Convert.ToInt32(r["ShowtimeId"]),
                                MovieId = Convert.ToInt32(r["MovieId"]),
                                RoomName = r["RoomName"].ToString(),
                                StartTime = Convert.ToDateTime(r["StartTime"]),
                                TicketPrice = Convert.ToDecimal(r["TicketPrice"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<SeatModel> GetSeats(int showtimeId)
        {
            List<SeatModel> list = new List<SeatModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT ShowtimeId, SeatId, Status, LockedBy, UnlockTime FROM SeatReservations WHERE ShowtimeId = @ShowtimeId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ShowtimeId", showtimeId);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new SeatModel
                            {
                                ShowtimeId = Convert.ToInt32(r["ShowtimeId"]),
                                SeatId = r["SeatId"].ToString(),
                                Status = (SeatStatus)Convert.ToInt32(r["Status"]),
                                LockedBy = r["LockedBy"] == DBNull.Value ? null : r["LockedBy"].ToString(),
                                UnlockTime = r["UnlockTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["UnlockTime"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        public SeatModel GetSeat(int showtimeId, string seatId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT ShowtimeId, SeatId, Status, LockedBy, UnlockTime FROM SeatReservations WHERE ShowtimeId = @ShowtimeId AND SeatId = @SeatId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ShowtimeId", showtimeId);
                    cmd.Parameters.AddWithValue("@SeatId", seatId);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            return new SeatModel
                            {
                                ShowtimeId = Convert.ToInt32(r["ShowtimeId"]),
                                SeatId = r["SeatId"].ToString(),
                                Status = (SeatStatus)Convert.ToInt32(r["Status"]),
                                LockedBy = r["LockedBy"] == DBNull.Value ? null : r["LockedBy"].ToString(),
                                UnlockTime = r["UnlockTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["UnlockTime"])
                            };
                        }
                    }
                }
            }
            return null;
        }

        public bool TryHoldSeat(string userId, int showtimeId, string seatId, int holdSeconds, out SeatModel resultSeat, out string errorMessage)
        {
            resultSeat = null;
            errorMessage = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        // Khóa mức dòng UPDLOCK + ROWLOCK để Server 1 và Server 2 không bao giờ giữ trùng ghế
                        string selectSql = @"SELECT Status, LockedBy, UnlockTime 
                                             FROM SeatReservations WITH (UPDLOCK, ROWLOCK) 
                                             WHERE ShowtimeId = @ShowtimeId AND SeatId = @SeatId";

                        int status;
                        string lockedBy;
                        DateTime? unlockTime;

                        using (SqlCommand cmd = new SqlCommand(selectSql, conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@ShowtimeId", showtimeId);
                            cmd.Parameters.AddWithValue("@SeatId", seatId);
                            using (SqlDataReader r = cmd.ExecuteReader())
                            {
                                if (!r.Read())
                                {
                                    tx.Rollback();
                                    errorMessage = $"Không tìm thấy ghế {seatId}.";
                                    return false;
                                }
                                status = Convert.ToInt32(r["Status"]);
                                lockedBy = r["LockedBy"] == DBNull.Value ? null : r["LockedBy"].ToString();
                                unlockTime = r["UnlockTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["UnlockTime"]);
                            }
                        }

                        if (status == (int)SeatStatus.Sold)
                        {
                            tx.Rollback();
                            errorMessage = $"Ghế {seatId} đã được bán.";
                            return false;
                        }

                        if (status == (int)SeatStatus.Held && unlockTime.HasValue && unlockTime.Value > DateTime.Now)
                        {
                            tx.Rollback();
                            errorMessage = lockedBy == userId ? $"Bạn đang giữ ghế {seatId} rồi." : $"Ghế {seatId} đang được người khác giữ.";
                            return false;
                        }

                        DateTime newUnlock = DateTime.Now.AddSeconds(holdSeconds);
                        string updateSql = @"UPDATE SeatReservations 
                                             SET Status = @Status, LockedBy = @LockedBy, UnlockTime = @UnlockTime 
                                             WHERE ShowtimeId = @ShowtimeId AND SeatId = @SeatId";
                        using (SqlCommand upCmd = new SqlCommand(updateSql, conn, tx))
                        {
                            upCmd.Parameters.AddWithValue("@Status", (int)SeatStatus.Held);
                            upCmd.Parameters.AddWithValue("@LockedBy", userId);
                            upCmd.Parameters.AddWithValue("@UnlockTime", newUnlock);
                            upCmd.Parameters.AddWithValue("@ShowtimeId", showtimeId);
                            upCmd.Parameters.AddWithValue("@SeatId", seatId);
                            upCmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                        resultSeat = new SeatModel
                        {
                            ShowtimeId = showtimeId,
                            SeatId = seatId.ToUpper(),
                            Status = SeatStatus.Held,
                            LockedBy = userId,
                            UnlockTime = newUnlock
                        };
                        return true;
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        errorMessage = "Lỗi SQL khi giữ ghế: " + ex.Message;
                        return false;
                    }
                }
            }
        }

        public bool TryCancelHold(string userId, int showtimeId, string seatId, out SeatModel resultSeat, out string errorMessage)
        {
            resultSeat = null;
            errorMessage = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"UPDATE SeatReservations 
                               SET Status = 0, LockedBy = NULL, UnlockTime = NULL 
                               WHERE ShowtimeId = @ShowtimeId AND SeatId = @SeatId AND Status = 1 AND LockedBy = @UserId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ShowtimeId", showtimeId);
                    cmd.Parameters.AddWithValue("@SeatId", seatId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        errorMessage = $"Không thể hủy giữ ghế {seatId} (ghế không ở trạng thái giữ bởi bạn).";
                        return false;
                    }
                }
            }

            resultSeat = new SeatModel
            {
                ShowtimeId = showtimeId,
                SeatId = seatId.ToUpper(),
                Status = SeatStatus.Available,
                LockedBy = null,
                UnlockTime = null
            };
            return true;
        }

        public List<SeatModel> ReleaseSeatsByUser(string userId)
        {
            List<SeatModel> released = new List<SeatModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"UPDATE SeatReservations 
                               SET Status = 0, LockedBy = NULL, UnlockTime = NULL 
                               OUTPUT DELETED.ShowtimeId, DELETED.SeatId 
                               WHERE Status = 1 AND LockedBy = @UserId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            released.Add(new SeatModel
                            {
                                ShowtimeId = Convert.ToInt32(r["ShowtimeId"]),
                                SeatId = r["SeatId"].ToString(),
                                Status = SeatStatus.Available
                            });
                        }
                    }
                }
            }
            return released;
        }

        public List<SeatModel> ReleaseExpiredSeats()
        {
            List<SeatModel> released = new List<SeatModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                // Câu lệnh UPDATE nguyên tử kèm OUTPUT: chỉ 1 Server Node lấy được dòng hết hạn, chống quét đè
                string sql = @"UPDATE SeatReservations 
                               SET Status = 0, LockedBy = NULL, UnlockTime = NULL 
                               OUTPUT DELETED.ShowtimeId, DELETED.SeatId 
                               WHERE Status = 1 AND UnlockTime <= GETDATE()";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        released.Add(new SeatModel
                        {
                            ShowtimeId = Convert.ToInt32(r["ShowtimeId"]),
                            SeatId = r["SeatId"].ToString(),
                            Status = SeatStatus.Available
                        });
                    }
                }
            }
            return released;
        }

        public bool ExecuteBuyTransaction(string userId, int showtimeId, List<string> seatIds, decimal unitPrice, out TransactionModel transaction, out string errorMessage)
        {
            transaction = null;
            errorMessage = null;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (SqlTransaction tx = conn.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    try
                    {
                        foreach (string seatId in seatIds)
                        {
                            string lockSql = @"SELECT Status, LockedBy, UnlockTime 
                                               FROM SeatReservations WITH (UPDLOCK, ROWLOCK) 
                                               WHERE ShowtimeId = @ShowtimeId AND SeatId = @SeatId";
                            using (SqlCommand cmd = new SqlCommand(lockSql, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@ShowtimeId", showtimeId);
                                cmd.Parameters.AddWithValue("@SeatId", seatId);
                                using (SqlDataReader r = cmd.ExecuteReader())
                                {
                                    if (!r.Read())
                                    {
                                        tx.Rollback();
                                        errorMessage = $"Ghế {seatId} không tồn tại.";
                                        return false;
                                    }
                                    int status = Convert.ToInt32(r["Status"]);
                                    string lockedBy = r["LockedBy"] == DBNull.Value ? null : r["LockedBy"].ToString();
                                    DateTime? unlockTime = r["UnlockTime"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["UnlockTime"]);

                                    if (status != (int)SeatStatus.Held || lockedBy != userId || !unlockTime.HasValue || unlockTime.Value <= DateTime.Now)
                                    {
                                        tx.Rollback();
                                        errorMessage = $"Ghế {seatId} không hợp lệ hoặc đã hết hạn giữ 5 phút.";
                                        return false;
                                    }
                                }
                            }
                        }

                        decimal totalPrice = unitPrice * seatIds.Count;
                        DateTime buyTime = DateTime.Now;
                        string insertTxSql = @"INSERT INTO Transactions (UserId, ShowtimeId, BuyTime, TotalPrice) 
                                               OUTPUT INSERTED.TransactionId 
                                               VALUES (@UserId, @ShowtimeId, @BuyTime, @TotalPrice)";
                        int txId;
                        using (SqlCommand txCmd = new SqlCommand(insertTxSql, conn, tx))
                        {
                            txCmd.Parameters.AddWithValue("@UserId", userId);
                            txCmd.Parameters.AddWithValue("@ShowtimeId", showtimeId);
                            txCmd.Parameters.AddWithValue("@BuyTime", buyTime);
                            txCmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
                            txId = Convert.ToInt32(txCmd.ExecuteScalar());
                        }

                        TransactionModel createdTx = new TransactionModel
                        {
                            TransactionId = txId,
                            UserId = userId,
                            ShowtimeId = showtimeId,
                            BuyTime = buyTime,
                            TotalPrice = totalPrice
                        };

                        foreach (string seatId in seatIds)
                        {
                            string updateSeatSql = @"UPDATE SeatReservations 
                                                     SET Status = 2, LockedBy = @UserId, UnlockTime = NULL 
                                                     WHERE ShowtimeId = @ShowtimeId AND SeatId = @SeatId";
                            using (SqlCommand upCmd = new SqlCommand(updateSeatSql, conn, tx))
                            {
                                upCmd.Parameters.AddWithValue("@UserId", userId);
                                upCmd.Parameters.AddWithValue("@ShowtimeId", showtimeId);
                                upCmd.Parameters.AddWithValue("@SeatId", seatId);
                                upCmd.ExecuteNonQuery();
                            }

                            string insertDetailSql = @"INSERT INTO TransactionDetails (TransactionId, SeatId, UnitPrice) 
                                                       OUTPUT INSERTED.DetailId 
                                                       VALUES (@TransactionId, @SeatId, @UnitPrice)";
                            using (SqlCommand detCmd = new SqlCommand(insertDetailSql, conn, tx))
                            {
                                detCmd.Parameters.AddWithValue("@TransactionId", txId);
                                detCmd.Parameters.AddWithValue("@SeatId", seatId);
                                detCmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
                                int detailId = Convert.ToInt32(detCmd.ExecuteScalar());
                                createdTx.Details.Add(new TransactionDetailModel
                                {
                                    DetailId = detailId,
                                    TransactionId = txId,
                                    SeatId = seatId.ToUpper(),
                                    UnitPrice = unitPrice
                                });
                            }
                        }

                        tx.Commit();
                        transaction = createdTx;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        errorMessage = "Giao dịch thất bại (đã Rollback): " + ex.Message;
                        return false;
                    }
                }
            }
        }

        public List<TransactionModel> GetUserTransactions(string userId)
        {
            List<TransactionModel> list = new List<TransactionModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT TransactionId, UserId, ShowtimeId, BuyTime, TotalPrice FROM Transactions WHERE UserId = @UserId ORDER BY BuyTime DESC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (SqlDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new TransactionModel
                            {
                                TransactionId = Convert.ToInt32(r["TransactionId"]),
                                UserId = r["UserId"].ToString(),
                                ShowtimeId = Convert.ToInt32(r["ShowtimeId"]),
                                BuyTime = Convert.ToDateTime(r["BuyTime"]),
                                TotalPrice = Convert.ToDecimal(r["TotalPrice"])
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}