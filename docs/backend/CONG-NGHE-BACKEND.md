# GiapTech.BlouseHiding — Công nghệ Backend

> Xem tổng quan kiến trúc trước ở [`../kien-truc/TONG-QUAN-KIEN-TRUC.md`](../kien-truc/TONG-QUAN-KIEN-TRUC.md).
> Quy ước tổ chức code/layer xem [`KIEN-TRUC-BACKEND.md`](./KIEN-TRUC-BACKEND.md). Hợp đồng API xem
> [`API-DESIGN.md`](./API-DESIGN.md). Tên thư viện cụ thể để scaffold trực tiếp ở Giai đoạn 0; phiên
> bản patch chính xác chốt lúc khởi tạo solution.

**Khung xuất phát điểm:** dùng **Jason Taylor's Clean Architecture Template**
(`dotnet new install Clean.Architecture.Solution.Template` — MIT license) làm khung khởi tạo thay vì
tự dựng từ đầu — đã có sẵn MediatR, FluentValidation, EF Core, Serilog, xUnit đúng như lựa chọn dưới
đây. Xem lý do chọn ở [ADR-0001](../kien-truc/adr/0001-clean-architecture-modular-monolith.md).

---

## 1. Nền tảng

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Runtime | **.NET 10 (LTS)** — đã chốt | Hỗ trợ dài hạn (LTS), phù hợp dự án chạy nhiều năm; VPS self-host tự cài nên không bị giới hạn bởi hosting managed |
| Ngôn ngữ | C# (nullable reference types bật, `ImplicitUsings`) | Chuẩn của hệ sinh thái .NET hiện đại |
| Kiến trúc solution | Clean Architecture: `Domain` / `Application` / `Infrastructure` / `Api` (+ `Web` nếu BFF) | Xem chi tiết layer ở `KIEN-TRUC-BACKEND.md` |
| Pattern nghiệp vụ | CQRS nhẹ với **MediatR** (Command/Query + Handler) | Tách rõ luồng ghi/đọc, dễ test, dễ thêm behavior pipeline (validation, logging) mà không rải code |
| Validation | **FluentValidation**, tích hợp làm MediatR `IPipelineBehavior` | Validate tập trung trước khi vào handler, không rải `if` rải rác |
| Mapping | **Mapster** (thay vì AutoMapper) | Nhanh hơn, cấu hình đơn giản hơn cho project mới |

## 2. Truy cập dữ liệu

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| ORM chính | **EF Core 10** (Npgsql provider) | Migration, tracking, phù hợp CRUD của Identity/Profile/Job/Application |
| Truy vấn đọc nặng | **Dapper** cho các query phức tạp (tìm kiếm, dashboard, báo cáo) | EF Core sinh SQL không tối ưu cho join/aggregate phức tạp — dùng Dapper song song, không thay thế hoàn toàn EF Core |
| Migration | EF Core Migrations, review SQL sinh ra trước khi apply production | Xem quy ước tại [`../database/QUY-UOC-MIGRATION.md`](../database/QUY-UOC-MIGRATION.md) |

## 3. Auth & bảo mật

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Xác thực | **JWT Bearer** (`Microsoft.AspNetCore.Authentication.JwtBearer`) + access token ngắn hạn (~15 phút) + refresh token lưu DB (hash) | Chuẩn cho API-first, dễ dùng chung cho web + mobile sau này |
| Định danh mật khẩu | **ASP.NET Core Identity** (chỉ dùng phần hashing/password policy, không dùng cookie auth mặc định) | Tận dụng hạ tầng hash/lockout đã kiểm chứng thay vì tự viết |
| OAuth ngoài | Google, Zalo — dùng thư viện `AspNet.Security.OAuth.*` hoặc gọi trực tiếp OAuth2 endpoint | Zalo không có SDK .NET chính thức — tự implement theo REST API của Zalo |
| Phân quyền | Policy-based Authorization (`[Authorize(Policy = "EmployerMember")]`) + custom `IAuthorizationHandler` kiểm tra membership tổ chức | RBAC đơn giản (role) không đủ vì cần kiểm tra "có phải member của org này" |
| Rate limiting | `Microsoft.AspNetCore.RateLimiting` (built-in .NET) | Chặn spam OTP, brute-force login |

## 4. Xử lý nền & realtime

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Background job | **Hangfire** (Postgres storage) | Dashboard giám sát sẵn có, phù hợp job như hết hạn CCHN, hết hạn tin, gửi email hàng loạt |
| Message queue | **RabbitMQ** qua **MassTransit** | MassTransit trừu tượng hóa AMQP, dễ đổi broker sau này, tích hợp outbox pattern sẵn |
| Realtime | **SignalR** (Redis backplane khi scale nhiều instance) | Chat, thông báo |
| Resilience | **Polly** (retry, circuit breaker) cho gọi cổng thanh toán/SMS ngoài | Các dịch vụ bên thứ 3 (SMS OTP...) không ổn định 100% |

## 5. Hạ tầng phụ trợ trong code

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Logging | **Serilog** → sink Console (dev) + Seq (dev/staging) + Grafana Loki (prod, self-host) | Structured logging, dễ truy vết theo `correlationId` |
| Observability | **OpenTelemetry** (traces + metrics), export sang Prometheus/Grafana self-host | Chuẩn mở, không khóa vào 1 vendor |
| API docs | **Swashbuckle** (Swagger/OpenAPI) | Tự sinh tài liệu từ code, dùng luôn để test thủ công |
| File lưu trữ | `AWSSDK.S3` trỏ endpoint vào **MinIO tự host** (API tương thích S3) | Upload ảnh CCHN, giấy phép, logo qua presigned URL — không cần đổi SDK dù đổi hạ tầng lưu trữ sau này |
| Email | **MailKit** gửi qua SMTP (Amazon SES hoặc SMTP relay như Mailgun/Brevo) | Email xác thực, thông báo |
| Testing | **xUnit** + **FluentAssertions** + **Testcontainers** (Postgres/Redis thật trong test) + **Bogus** (sinh dữ liệu giả) | Test tích hợp sát thực tế thay vì mock DB |
| Contract test bên ngoài | **WireMock.Net** | Giả lập SMS OTP/cổng thanh toán khi test, không gọi thật |

## 6. Dịch vụ bên thứ ba đặc thù Việt Nam (tích hợp qua backend)

| Nhu cầu | Đề xuất | Ghi chú |
|---|---|---|
| Thanh toán (gói tin, credit) | **Chưa quyết — để sau** | Xem [ADR-0003](../kien-truc/adr/0003-hoan-cong-thanh-toan-tu-dong.md). MVP xử lý tạm bằng quy trình thủ công: NTD chuyển khoản kèm mã tham chiếu, Admin đối soát qua endpoint `POST /admin/...`. `payments`/`job_purchases`/`credit_transactions` (ERD) giữ nguyên schema để cắm cổng tự động sau. |
| SMS OTP | **eSMS.vn** hoặc **SpeedSMS** | Giá tốt hơn Twilio cho số điện thoại Việt Nam, độ trễ thấp trong nước |
| OAuth đăng nhập | Google (chuẩn), **Zalo** (phổ biến tại VN, tự implement REST vì không có SDK .NET chính thức) | |
| Bản đồ/địa chỉ | **Goong Maps** (Việt hóa, giá tốt) hoặc Google Maps nếu cần độ phủ toàn cầu | Không cấp thiết ở MVP |

---

## Xem thêm
- Kiến trúc/quy ước code backend: [`KIEN-TRUC-BACKEND.md`](./KIEN-TRUC-BACKEND.md)
- Hợp đồng API: [`API-DESIGN.md`](./API-DESIGN.md)
- Schema dữ liệu: [`../database/ERD-CHI-TIET.md`](../database/ERD-CHI-TIET.md)
