# ADR-0009: Dùng Mediator + Mapster thay MediatR + AutoMapper (thương mại hóa)

**Trạng thái:** Đã chấp nhận

## Bối cảnh
Lúc scaffold solution thật (Giai đoạn 0.2) từ Jason Taylor's Clean Architecture Template, build log hiện
cảnh báo bản quyền:

```
warn: LuckyPennySoftware.MediatR.License[0]
      You do not have a valid license key for the Lucky Penny software MediatR.
      This is allowed for development and testing scenarios. If you are running
      in production you are required to have a licensed version.
```

Kiểm chứng lại: **MediatR từ v12 trở đi (bản template ghim là v14.1.0) là sản phẩm thương mại** của
Lucky Penny Software (tác giả Jimmy Bogard), bán qua ComponentSource, giá từ **~$489/năm** (tier
Standard, 1–10 dev) cho dùng production — chỉ miễn phí cho dev/test. **AutoMapper** (cùng tác giả) cũng
thương mại hóa cùng đợt, và template ghim AutoMapper v16.1.1 — cũng thuộc diện phải trả phí.
`docs/backend/CONG-NGHE-BACKEND.md` chọn MediatR (giả định miễn phí, viết trước khi có thay đổi giấy
phép) nhưng lại **đã chọn đúng Mapster** (không phải AutoMapper) từ đầu cho phần mapping — nên việc sửa
AutoMapper→Mapster ở đây là thực thi đúng quyết định cũ, không phải quyết định mới.

## Quyết định
1. **Thay MediatR bằng [Mediator](https://github.com/martinothamar/Mediator)** (tác giả martinothamar,
   MIT license, miễn phí) — thư viện CQRS dựng bằng Roslyn source generator (nhanh hơn MediatR vì
   không dùng reflection runtime). API gần tương tự: `ICommand<TResponse>`/`IQuery<TResponse>` +
   `ICommandHandler`/`IQueryHandler` + `IPipelineBehavior<TMessage,TResponse>` — thực ra **khớp hơn**
   quy ước CQRS đã chốt ở `docs/backend/KIEN-TRUC-BACKEND.md` mục 2 (Command/Query tách biệt rõ ràng,
   không dùng chung 1 `IRequest<T>` như MediatR).
2. **Giữ nguyên Mapster** cho mapping (đã chốt từ `CONG-NGHE-BACKEND.md`) — không đổi gì thêm, chỉ xác
   nhận lại quyết định cũ vẫn đúng.
3. **Domain hết phụ thuộc thư viện mediator** — bản gốc template có `Domain.BaseEvent : INotification`
   (tham chiếu thẳng `MediatR.Contracts` từ Domain), vi phạm Dependency Rule (CLAUDE.md mục 4: Domain
   không được reference gì bên ngoài trừ thứ thuần domain). Nhân dịp sửa, tách `BaseEvent` thành marker
   thuần domain, không implement `INotification` — Infrastructure tự bọc lại thành
   `DomainEventNotification<TDomainEvent> : INotification` lúc dispatch (xem
   `src/Infrastructure/Data/Interceptors/`). `Domain.csproj` giờ **không có PackageReference nào**.
4. **Gỡ Aspire AppHost** (dùng `AddAzureContainerAppEnvironment`/`AddAzurePostgresFlexibleServer`) —
   xung đột với ADR-0002 (self-host VPS, không dùng Azure managed). Dev cục bộ dùng
   `docker-compose.yml` ở repo root (Postgres/Redis/RabbitMQ/MinIO) thay vì Aspire orchestration.
   `ServiceDefaults` (OpenTelemetry + health checks, không có gì Azure-specific) và `TestAppHost` (dùng
   `Aspire.Hosting.PostgreSQL` generic, chỉ để spin container Postgres cho functional test) **vẫn giữ
   nguyên** — không phải Azure-specific, không vi phạm ADR-0002.

## Phương án đã cân nhắc
- **Mua license MediatR chính thức** (~$489/năm): loại bỏ — chưa cần thiết khi có lựa chọn miễn phí
  tương đương về chức năng, dự án còn ở giai đoạn MVP chưa có doanh thu.
- **Ghim MediatR 11.1.0** (bản Apache-2.0 cuối cùng trước thương mại hóa): loại bỏ — không còn được tác
  giả vá lỗi/cập nhật, trong khi Mediator (martinothamar) đang phát triển tích cực và nhanh hơn.
- **Tự viết pattern Command/Handler thủ công, bỏ hẳn thư viện mediator**: loại bỏ — phải tự viết lại
  toàn bộ pipeline behavior (validation, logging, authorization) mà Mediator đã có sẵn, tốn công không
  cần thiết khi đã có lựa chọn nguồn mở phù hợp.

## Hệ quả
- (+) Không rủi ro chi phí/pháp lý license MediatR + AutoMapper — cả 2 thư viện thay thế đều MIT,
  miễn phí vĩnh viễn.
- (+) Domain layer giờ tuân thủ đúng Dependency Rule 100% (0 NuGet package) — chặt hơn cả bản gốc
  template.
- (+) Mediator dùng source generator nên hiệu năng runtime tốt hơn MediatR (không reflection), phù hợp
  usecase API-first cần latency thấp.
- (−) `IPipelineBehavior<TMessage,TResponse>.Handle` trả `ValueTask<TResponse>` (không phải `Task`) và
  nhận `MessageHandlerDelegate` (không phải `RequestHandlerDelegate`) — khác chữ ký MediatR, nhưng
  không ảnh hưởng cách viết Command/Query/Handler nghiệp vụ thực tế theo `KIEN-TRUC-BACKEND.md`.
- (−) Không còn `IRequestPreProcessor<T>` (MediatR) cho logging riêng biệt — `LoggingBehaviour` gộp vào
  `IPipelineBehavior` thường (log trước khi gọi `next()`), không đổi hành vi thực tế.
- (−) Dispatch domain event giờ qua `Activator.CreateInstance` (tạo `DomainEventNotification<T>` đóng
  kín lúc runtime) thay vì gọi thẳng `_mediator.Publish(domainEvent)` — thêm 1 lớp gián tiếp nhỏ, đổi
  lấy việc Domain sạch hoàn toàn.
- Cập nhật liên quan: [`../../backend/CONG-NGHE-BACKEND.md`](../../backend/CONG-NGHE-BACKEND.md) mục
  "Nền tảng", [`../../../CLAUDE.md`](../../../CLAUDE.md) mục 6 (lệnh build/test thật).
