# Báo cáo kiểm tra framework và kiểm thử tự động

## 1. Tổng quan

- Nhánh thực hiện: `feat/server_testqa`.
- Framework production: WinForms / .NET Framework 4.7.2, gồm client, server, shared protocol và TCP load balancer.
- Đã bổ sung bộ test tại `Tests/`, tham khảo cách tổ chức xUnit của `/mnt/q/DoAnNT106/Tests`.
- Không thay đổi mã production. Các lỗi phát hiện được giữ dưới dạng regression test đang hoạt động, không skip hoặc đảo assertion để tạo kết quả xanh.

## 2. Thiết kế bộ test

Hai project xUnit chạy trên .NET 8, liên kết trực tiếp source production thay vì sao chép logic:

| Project | Phạm vi | Tổng | Pass | Fail | Skip |
| --- | --- | ---: | ---: | ---: | ---: |
| BookingTests | Repository RAM, BookingService, shared protocol | 49 | 45 | 4 | 0 |
| TransportTests | Request handler, connection manager, TCP/load balancer, TLS và peer transport | 29 | 21 | 8 | 0 |
| Tổng | | 78 | 66 | 12 | 0 |

Các hành vi đã chạy kiểm tra: đăng nhập/đăng ký, quyền giữ/hủy ghế, hết hạn, thanh toán nhiều ghế, kiểm tra rollback RAM, chuẩn hóa mã ghế, lịch sử, tranh chấp đồng thời, sự kiện, JSON/Base64, gửi/broadcast qua socket thật, TCP forwarding hai chiều 40 KB, failover, TLS 1.2 PING và truyền payload peer bằng chứng chỉ tự tạo cho test.

Đây là kiểm thử source trên .NET 8, không phải chứng nhận binary production .NET Framework 4.7.2 hoặc giao diện WinForms.

## 3. Kết quả thực thi

Lượt chạy Release đã thực hiện ngày 2026-11-28 bằng Windows SDK 10.0.401 và runtime .NET 8.0.31:

```bash
dotnet test Tests/TicketBooking.Tests.sln -c Release
```

Kết quả: 78 test, 66 pass, 12 fail, 0 skip; exit code 1. Toàn bộ 12 case fail mang trait `Category=KnownDefect`, tái hiện hành vi sai hiện có của production.

Chạy lại riêng baseline không bao gồm KnownDefect: 66 pass, 0 fail, 0 skip; exit code 0. Đây là kết quả đã lọc, không phải toàn bộ suite pass.

Source SQL phát cảnh báo CS0618 khi compile với package System.Data.SqlClient của project test. Không truy cập SQL Server trong các lượt chạy này.

## 4. Các lỗi được test tái hiện

| Nhóm | Số case fail | Vị trí production | Hành vi quan sát được |
| --- | ---: | --- | --- |
| Giả danh UserId | 4 | `TicketBooking.Server/RequestHandler.cs:104,174,193,212` | Session chưa đăng nhập vẫn được phép HOLD_SEAT, CANCEL_HOLD, BUY_TICKET và GET_HISTORY bằng UserId trong request. |
| Data sai kiểu | 3 | `TicketBooking.Server/RequestHandler.cs:42`; `TicketBooking.Shared/ProtocolModels.cs:74` | LOGIN, HOLD_SEAT, GET_SEATS ném JsonSerializationException thay vì trả ERROR có RequestId tương ứng. |
| Giả mạo peer sync | 1 | `TicketBooking.Server/RequestHandler.cs:61` | Client chưa xác thực gửi SERVER_SYNC_SEAT vẫn làm phát sự kiện remote sync. |
| Giao dịch rỗng qua service | 1 | `TicketBooking.Server/BookingService.cs:81-101` | Danh sách chỉ có null/khoảng trắng trở thành rỗng sau normalize nhưng vẫn tạo giao dịch thành công. |
| Giao dịch rỗng qua repository | 1 | `TicketBooking.Server/InMemoryBookingRepository.cs:187-239` | Repository chấp nhận danh sách ghế rỗng và ghi lịch sử. |
| Tính tiền trùng ghế | 1 | `TicketBooking.Server/InMemoryBookingRepository.cs:196-234` | Gọi repository trực tiếp với A1 và a1 tạo hai chi tiết, tính tiền hai lần cho cùng ghế. Service bình thường có bước deduplicate. |
| Giữ trùng giữa node RAM | 1 | `TicketBooking.Server/BookingService.cs:134-138` | Remote sync chỉ phát sự kiện; node RAM thứ hai vẫn giữ được ghế đã giữ ở node thứ nhất. |

Ưu tiên sửa: xác thực session và peer trước, sau đó validation payload/giao dịch và chính sách multi-node. Không nên coi event replication RAM là bảo đảm atomic chống bán trùng; cần shared SQL và test concurrency thực tế.

## 5. Phát hiện qua review chưa có runtime regression riêng

`TicketBooking.Server/TcpServer.cs:148-150` nhả ghế theo user khi một kết nối đóng. Nếu user có nhiều session, đóng một session có thể nhả cả ghế đang dùng bởi session còn lại. Đây là nhận xét từ review source, chưa được tính vào 12 case fail nói trên.

## 6. Cách chạy lại

Từ thư mục gốc repository:

```bash
bash Tests/run-tests.sh
```

Bash runner ưu tiên dotnet native; nếu SDK WSL bị thiếu host/fxr như môi trường kiểm tra hiện tại, runner dùng Windows dotnet tại vị trí mặc định và chuyển đường dẫn bằng wslpath.

Windows PowerShell:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Tests/run-tests.ps1
```

Runner mặc định chạy cả KnownDefect, vì vậy hiện trả exit code khác 0. Chỉ kiểm tra baseline khi cần:

```bash
bash Tests/run-tests.sh --filter 'Category!=KnownDefect'
```

Sau khi sửa production, giữ regression test và bỏ trait KnownDefect tương ứng.

## 7. Bằng chứng và tài liệu

- `Tests/README.md`: hướng dẫn, phạm vi và giới hạn.
- `Tests/VERIFICATION.md`: lệnh chạy và kết quả cụ thể.
- `Tests/TestResults/tests_net8.0_20261128232501.trx`: 29 transport tests.
- `Tests/TestResults/tests_net8.0_20261128232502.trx`: 49 booking tests.
- Hai TRX baseline có hậu tố `20261128232547` và `20261128232553`.

TRX, bin và obj là artifact local được gitignore, không nằm trong commit. Có thể tạo lại bằng runner; report này lưu số liệu tổng hợp.

## 8. Giới hạn và bước tiếp theo

1. Chưa build/smoke test binary WinForms/.NET Framework 4.7.2.
2. Chưa kiểm thử SQL Server thật: schema, row locking, rollback/deadlock, cạnh tranh giữa hai node và expiry. Cần database dùng một lần, không dùng dữ liệu production.
3. TLS test sử dụng chứng chỉ tự tạo; chưa xác minh cấu hình chứng chỉ triển khai, xác thực peer hay phục hồi handshake bị treo.
4. Chưa kiểm tra reconnect/UI lifecycle và nhiều session cùng user đầy đủ.
5. Chưa chạy soak/load benchmark; số lượng test không phải throughput/latency production.

Kết luận: đã có bộ test chạy được và tái hiện các lỗi quan trọng, nhưng framework hiện chưa đạt trạng thái toàn bộ test pass hoặc đủ điều kiện kết luận an toàn cho triển khai multi-server.
