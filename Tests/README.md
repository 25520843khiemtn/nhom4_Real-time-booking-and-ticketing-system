# Automated tests / Kiểm thử tự động

## Phạm vi

Bộ test tham khảo cách tổ chức xUnit trong `/mnt/q/DoAnNT106/Tests`, nhưng kiểm tra mã TicketBooking của repository này. Không sao chép logic nghiệp vụ vào test.

- `BookingTests`: repository RAM, BookingService và JSON protocol.
- `TransportTests`: request handler, quản lý kết nối và TCP/load balancer (xem test cụ thể trong project).
- `TicketBooking.Tests.sln`: solution test riêng; không thay đổi solution WinForms hiện tại.

Production là WinForms/.NET Framework 4.7.2. Test liên kết trực tiếp source không phụ thuộc UI và chạy trên .NET 8; vì vậy kết quả KHÔNG chứng minh binary .NET Framework/WinForms đã chạy đúng. Không dùng tài khoản, chứng chỉ hoặc database production.

## Chạy

Yêu cầu .NET SDK hỗ trợ net8.0, runtime .NET 8 và truy cập NuGet khi restore lần đầu.

Linux/WSL:

    bash Tests/run-tests.sh

Windows PowerShell, từ thư mục repo:

    powershell -NoProfile -ExecutionPolicy Bypass -File Tests/run-tests.ps1

Hoặc trực tiếp (đa nền tảng):

    dotnet test Tests/TicketBooking.Tests.sln -c Release

Runner ghi TRX vào `Tests/TestResults` (được gitignore). Chạy tất cả theo mặc định, không tự giấu test lỗi. Exit code khác 0 nghĩa là có lỗi build/restore/test và cần đọc output/TRX.

Test tái hiện lỗi production mang trait `Category=KnownDefect`, vẫn được chạy mặc định. Chỉ để kiểm tra phần baseline riêng:

    dotnet test Tests/TicketBooking.Tests.sln -c Release --filter 'Category!=KnownDefect'

Lệnh lọc không phải xác nhận toàn framework an toàn; khi sửa lỗi, bỏ trait KnownDefect nhưng giữ regression test.

## Các rủi ro phát hiện qua review source

1. `RequestHandler.cs:104,174,193,212`: khi session chưa đăng nhập, thao tác bảo vệ vẫn tin `req.UserId`, cho phép giả danh.
2. `RequestHandler.cs:61`: client có thể gửi `SERVER_SYNC_SEAT` mà không có xác thực peer; sự kiện giả được broadcast.
3. `RequestHandler.cs:42` và `ProtocolModels.cs:75`: chuyển kiểu Data nằm ngoài catch parse JSON, có thể ném exception và làm đóng kết nối.
4. `BookingService.cs:81-101`: danh sách chỉ có null/khoảng trắng vượt kiểm tra ban đầu, sau normalize rỗng nhưng vẫn tạo giao dịch.
5. `BookingService.cs:134-138`: remote sync chỉ phát sự kiện, không cập nhật RAM repository. RAM mỗi node không đảm bảo chống bán trùng giữa các node; cần shared SQL và kiểm thử concurrency thực tế.
6. `TcpServer.cs:148-150`: đóng một session nhả tất cả ghế của user, kể cả khi user còn session khác.

Đây là hạng mục kiểm tra, không phải tuyên bố tất cả đã được chứng minh bằng runtime. Test thất bại được liệt kê trong báo cáo chạy kèm.

## Chưa được bảo đảm bởi bộ test lõi

- Build và smoke test binary WinForms/.NET Framework 4.7.2 trên Windows.
- SQL Server thật: schema, transaction rollback, deadlock/row locking giữa hai node, quyền truy cập và expiry.
- TLS production/cấu hình chứng chỉ triển khai và xác thực peer; reconnect/UI lifecycle. Test đã có handshake TLS 1.2, PING nhiều dòng và truyền peer payload thật bằng chứng chỉ tự tạo chỉ dùng trong test.
- Soak/load benchmark: số lượng test hoặc contention unit test không phải số đo throughput/latency production.

Không trỏ test tới database thật để bổ sung các gate này; dùng SQL Server dùng một lần, dữ liệu riêng và cleanup rõ ràng.
