USE master;
GO

-- ============================================================
-- PHẦN 1: KHỞI TẠO DATABASE (AN TOÀN KHI CHẠY LẠI NHIỀU LẦN)
-- ============================================================
IF DB_ID(N'TicketBookingDB') IS NOT NULL
BEGIN
    ALTER DATABASE TicketBookingDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TicketBookingDB;
END
GO

CREATE DATABASE TicketBookingDB;
GO

USE TicketBookingDB;
GO

-- ============================================================
-- PHẦN 2: TẠO 8 BẢNG CHÍNH & RÀNG BUỘC TOÀN VẸN (PK, FK, CHECK)
-- ============================================================

-- 1. Bảng Users: Cột PasswordHash VARCHAR(100) chứa chuỗi băm 60 ký tự của bcrypt
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash VARCHAR(100) NOT NULL,
    FullName NVARCHAR(100) NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
);
GO

-- 2. Bảng Movies: Danh sách phim
CREATE TABLE Movies (
    MovieId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    DurationMinutes INT NOT NULL CHECK (DurationMinutes > 0),
    Genre NVARCHAR(100) NOT NULL
);
GO

-- 3. Bảng Rooms: Danh sách phòng chiếu (mặc định 48 ghế = 6 hàng x 8 cột)
CREATE TABLE Rooms (
    RoomId INT IDENTITY(1,1) PRIMARY KEY,
    RoomName NVARCHAR(50) NOT NULL UNIQUE,
    TotalSeats INT NOT NULL DEFAULT 48 CHECK (TotalSeats > 0)
);
GO

-- 4. Bảng Seats: Cấu trúc ghế cố định của từng phòng (A1 -> F8)
CREATE TABLE Seats (
    RoomId INT NOT NULL,
    SeatId VARCHAR(10) NOT NULL,
    RowLabel CHAR(1) NOT NULL,
    ColNumber INT NOT NULL CHECK (ColNumber > 0),
    CONSTRAINT PK_Seats PRIMARY KEY (RoomId, SeatId),
    CONSTRAINT FK_Seats_Rooms FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId)
);
GO

-- 5. Bảng Showtimes: Suất chiếu theo phim, phòng và khung giờ
CREATE TABLE Showtimes (
    ShowtimeId INT IDENTITY(1,1) PRIMARY KEY,
    MovieId INT NOT NULL,
    RoomId INT NOT NULL,
    StartTime DATETIME NOT NULL,
    TicketPrice DECIMAL(18, 0) NOT NULL CHECK (TicketPrice >= 0),
    CONSTRAINT FK_Showtimes_Movies FOREIGN KEY (MovieId) REFERENCES Movies(MovieId),
    CONSTRAINT FK_Showtimes_Rooms FOREIGN KEY (RoomId) REFERENCES Rooms(RoomId)
);
GO

-- 6. Bảng SeatReservations: Quản lý trạng thái ghế thời gian thực cho từng suất chiếu
-- Quy ước Status: 0 = Available (Trống), 1 = Held (Đang giữ), 2 = Sold (Đã bán)
CREATE TABLE SeatReservations (
    ShowtimeId INT NOT NULL,
    SeatId VARCHAR(10) NOT NULL,
    Status INT NOT NULL DEFAULT 0 CHECK (Status IN (0, 1, 2)),
    LockedBy INT NULL,
    UnlockTime DATETIME NULL,
    CONSTRAINT PK_SeatReservations PRIMARY KEY (ShowtimeId, SeatId),
    CONSTRAINT FK_SeatReservations_Showtimes FOREIGN KEY (ShowtimeId) REFERENCES Showtimes(ShowtimeId),
    CONSTRAINT FK_SeatReservations_Users FOREIGN KEY (LockedBy) REFERENCES Users(UserId)
);
GO

-- 7. Bảng Transactions: Hóa đơn tổng khi khách thanh toán thành công
CREATE TABLE Transactions (
    TransactionId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    ShowtimeId INT NOT NULL,
    BuyTime DATETIME NOT NULL DEFAULT GETDATE(),
    TotalPrice DECIMAL(18, 0) NOT NULL CHECK (TotalPrice >= 0),
    CONSTRAINT FK_Transactions_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT FK_Transactions_Showtimes FOREIGN KEY (ShowtimeId) REFERENCES Showtimes(ShowtimeId)
);
GO

-- 8. Bảng TransactionDetails: Chi tiết từng ghế trong hóa đơn
CREATE TABLE TransactionDetails (
    DetailId INT IDENTITY(1,1) PRIMARY KEY,
    TransactionId INT NOT NULL,
    SeatId VARCHAR(10) NOT NULL,
    UnitPrice DECIMAL(18, 0) NOT NULL CHECK (UnitPrice >= 0),
    CONSTRAINT FK_TransactionDetails_Transactions FOREIGN KEY (TransactionId) REFERENCES Transactions(TransactionId)
);
GO

-- ============================================================
-- PHẦN 3: INDEX TỐI ƯU ĐA LUỒNG & MULTI-SERVER
-- ============================================================

-- 1. Tối ưu cho StartTimeoutWatcherAsync quét ghế quá hạn 5 phút mà không khóa cả bảng
CREATE INDEX IX_SeatReservations_Status_UnlockTime
ON SeatReservations (Status, UnlockTime)
INCLUDE (ShowtimeId, SeatId);
GO

-- 2. Tối ưu cho ReleaseSeatsByUser khi 1 Client ngắt kết nối đột ngột
CREATE INDEX IX_SeatReservations_LockedBy
ON SeatReservations (LockedBy)
WHERE Status = 1;
GO

-- 3. Tối ưu cho GET_SHOWTIMES lọc theo phim và ngày chiếu
CREATE INDEX IX_Showtimes_MovieId_StartTime
ON Showtimes (MovieId, StartTime);
GO

-- 4. Tối ưu cho GET_HISTORY lấy lịch sử mua vé mới nhất của người dùng
CREATE INDEX IX_Transactions_UserId_BuyTime
ON Transactions (UserId, BuyTime DESC);
GO

-- ============================================================
-- PHẦN 4: TRIGGER TỰ ĐỘNG KHỞI TẠO 48 GHẾ KHI THÊM SUẤT CHIẾU MỚI
-- ============================================================
CREATE TRIGGER TRG_Showtimes_AfterInsert_InitSeats
ON Showtimes
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO SeatReservations (ShowtimeId, SeatId, Status, LockedBy, UnlockTime)
    SELECT i.ShowtimeId, s.SeatId, 0, NULL, NULL
    FROM inserted i
    INNER JOIN Seats s ON i.RoomId = s.RoomId;
END;
GO

-- ============================================================
-- PHẦN 5: DỮ LIỆU MẪU (15 PHIM MỚI 2025 - 2026 & 25 SUẤT CHIẾU)
-- ============================================================

-- 1. Thêm 15 Phim mẫu (Phim Việt + Bom tấn quốc tế 2025 - 2026)
INSERT INTO Movies (Title, DurationMinutes, Genre) VALUES
(N'Lật Mặt 8: Vòng Tay Nắng', 135, N'Hành động / Gia đình'),
(N'Địa Đạo: Mặt Trời Trong Bóng Tối', 128, N'Lịch sử / Chiến tranh'),
(N'Thám Tử Kiên: Kỳ Án Không Đầu', 122, N'Trinh thám / Kinh dị'),
(N'Bộ Tứ Báo Thủ', 132, N'Hài / Gia đình'),
(N'Nhà Gia Tiên', 117, N'Gia đình / Tâm linh / Hài'),
(N'Nụ Hôn Bạc Tỷ', 100, N'Tình cảm / Hài'),
(N'Avatar: Lửa và Tro Tàn', 190, N'Khoa học viễn tưởng / Phiêu lưu'),
(N'Phi Vụ Động Trời 2 (Zootopia 2)', 108, N'Hoạt hình / Hài'),
(N'Avengers: Doomsday', 155, N'Hành động / Siêu anh hùng'),
(N'Spider-Man: Brand New Day', 140, N'Hành động / Siêu anh hùng'),
(N'The Odyssey', 160, N'Sử thi / Phiêu lưu'),
(N'Câu Chuyện Đồ Chơi 5 (Toy Story 5)', 105, N'Hoạt hình / Gia đình'),
(N'Nhiệm Vụ Bất Khả Thi: Nghiệp Báo Cuối Cùng', 165, N'Hành động / Điệp viên'),
(N'Bộ Tứ Siêu Đẳng: Bước Đi Đầu Tiên', 125, N'Hành động / Khoa học viễn tưởng'),
(N'Doraemon: Nobita và Cuộc Phiêu Lưu Vào Thế Giới Trong Tranh', 105, N'Hoạt hình / Phiêu lưu');
GO

-- 2. Thêm 3 Phòng chiếu
INSERT INTO Rooms (RoomName, TotalSeats) VALUES
(N'Phòng 01', 48),
(N'Phòng 02', 48),
(N'Phòng IMAX', 48);
GO

-- 3. Tự động sinh 48 ghế (Hàng A -> F, Cột 1 -> 8) cho cả 3 phòng
DECLARE @r INT = 1;
WHILE @r <= 3
BEGIN
    DECLARE @rowIdx INT = 0;
    WHILE @rowIdx < 6
    BEGIN
        DECLARE @rowChar CHAR(1) = CHAR(65 + @rowIdx);
        DECLARE @col INT = 1;
        WHILE @col <= 8
        BEGIN
            INSERT INTO Seats (RoomId, SeatId, RowLabel, ColNumber)
            VALUES (@r, @rowChar + CAST(@col AS VARCHAR(2)), @rowChar, @col);
            SET @col = @col + 1;
        END
        SET @rowIdx = @rowIdx + 1;
    END
    SET @r = @r + 1;
END
GO

-- 4. Thêm 25 Suất chiếu cho HÔM NAY, NGÀY MAI và NGÀY KIA
-- (Trigger sẽ tự động sinh 48 ghế cho từng suất -> tổng 1.200 ghế)
DECLARE @today DATE = CAST(GETDATE() AS DATE);
DECLARE @tomorrow DATE = DATEADD(DAY, 1, @today);
DECLARE @dayAfter DATE = DATEADD(DAY, 2, @today);

INSERT INTO Showtimes (MovieId, RoomId, StartTime, TicketPrice) VALUES
-- HÔM NAY
(1, 1, DATEADD(HOUR, 18, CAST(@today AS DATETIME)), 85000),
(1, 2, DATEADD(HOUR, 20, CAST(@today AS DATETIME)), 95000),
(2, 3, DATEADD(HOUR, 19, CAST(@today AS DATETIME)), 120000),
(3, 1, DATEADD(HOUR, 21, CAST(@today AS DATETIME)), 90000),
(4, 2, DATEADD(HOUR, 17, CAST(@today AS DATETIME)), 80000),
(5, 1, DATEADD(HOUR, 15, CAST(@today AS DATETIME)), 85000),
(6, 2, DATEADD(HOUR, 14, CAST(@today AS DATETIME)), 80000),
(7, 3, DATEADD(HOUR, 21, CAST(@today AS DATETIME)), 135000),
(8, 1, DATEADD(HOUR, 10, CAST(@today AS DATETIME)), 75000),
(9, 3, DATEADD(HOUR, 16, CAST(@today AS DATETIME)), 130000),
(10, 2, DATEADD(HOUR, 22, CAST(@today AS DATETIME)), 110000),

-- NGÀY MAI
(1, 1, DATEADD(HOUR, 19, CAST(@tomorrow AS DATETIME)), 85000),
(2, 3, DATEADD(HOUR, 20, CAST(@tomorrow AS DATETIME)), 120000),
(4, 2, DATEADD(HOUR, 18, CAST(@tomorrow AS DATETIME)), 85000),
(7, 3, DATEADD(HOUR, 17, CAST(@tomorrow AS DATETIME)), 135000),
(9, 3, DATEADD(HOUR, 21, CAST(@tomorrow AS DATETIME)), 130000),
(11, 3, DATEADD(HOUR, 14, CAST(@tomorrow AS DATETIME)), 125000),
(12, 1, DATEADD(HOUR, 10, CAST(@tomorrow AS DATETIME)), 75000),
(13, 2, DATEADD(HOUR, 20, CAST(@tomorrow AS DATETIME)), 105000),
(14, 1, DATEADD(HOUR, 16, CAST(@tomorrow AS DATETIME)), 100000),
(15, 2, DATEADD(HOUR, 11, CAST(@tomorrow AS DATETIME)), 75000),

-- NGÀY KIA
(5, 1, DATEADD(HOUR, 19, CAST(@dayAfter AS DATETIME)), 85000),
(10, 3, DATEADD(HOUR, 20, CAST(@dayAfter AS DATETIME)), 125000),
(11, 3, DATEADD(HOUR, 17, CAST(@dayAfter AS DATETIME)), 125000),
(13, 2, DATEADD(HOUR, 21, CAST(@dayAfter AS DATETIME)), 105000);
GO

-- ============================================================
-- PHẦN 6: KIỂM TRA NHANH DỮ LIỆU VỪA TẠO
-- ============================================================
SELECT * FROM Movies;
SELECT * FROM Rooms;
SELECT st.ShowtimeId, m.Title, r.RoomName, st.StartTime, st.TicketPrice
FROM Showtimes st
JOIN Movies m ON st.MovieId = m.MovieId
JOIN Rooms r ON st.RoomId = r.RoomId
ORDER BY st.StartTime;

SELECT ShowtimeId, COUNT(*) AS TongSoGheDaKhoiTao
FROM SeatReservations
GROUP BY ShowtimeId
ORDER BY ShowtimeId;
GO