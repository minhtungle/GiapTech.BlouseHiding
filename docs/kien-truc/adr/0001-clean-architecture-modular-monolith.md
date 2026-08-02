# ADR-0001: Clean Architecture + Modular Monolith (không microservice ngay)

**Trạng thái:** Đã chấp nhận

## Bối cảnh
Hệ thống có nhiều bounded context (Identity, Profile, Job, Application, Messaging, Ops...) và cần
dễ bảo trì/bàn giao khi có người mới tham gia. Microservice giải quyết được vấn đề scale độc lập từng
domain nhưng trả giá bằng độ phức tạp vận hành (network, distributed transaction, observability) —
không phù hợp giai đoạn đội ngũ nhỏ, MVP chưa xác định tải thực tế.

## Quyết định
- Dùng **Clean Architecture** (Domain/Application/Infrastructure/Web) để tách rõ business logic khỏi
  framework/hạ tầng, giữ khả năng test và thay đổi hạ tầng mà không đụng business logic.
- Dùng **Modular Monolith**: mỗi bounded context là 1 nhóm thư mục `Application/{Context}/` độc lập,
  giao tiếp nội bộ qua Mediator (thư viện CQRS — xem [ADR-0009](./0009-mediator-mapster-thay-mediatr-automapper.md)
  cho tên thư viện cụ thể đã đổi so với ghi chú ban đầu) — KHÔNG tách microservice ở giai đoạn này.

## Phương án đã cân nhắc
- **Microservice ngay từ đầu**: loại bỏ vì đội ngũ nhỏ, tải chưa rõ, tốn công vận hành không cần thiết.
- **Layered Architecture truyền thống (3 lớp UI-BLL-DAL không rõ boundary)**: loại bỏ vì dễ rối khi
  logic nghiệp vụ leak vào Controller, khó test độc lập.

## Hệ quả
- (+) Business logic độc lập với framework, dễ test, dễ bàn giao vì có ranh giới rõ.
- (+) Có thể tách 1 bounded context thành microservice riêng sau này nếu thực sự cần scale, vì đã có
  ranh giới module sẵn — không phải tái cấu trúc từ đầu.
- (−) Không tận dụng được scale độc lập theo domain ngay từ đầu — chấp nhận được vì MVP chưa cần.
- Chi tiết layer/quy ước code: [`../../backend/KIEN-TRUC-BACKEND.md`](../../backend/KIEN-TRUC-BACKEND.md).
