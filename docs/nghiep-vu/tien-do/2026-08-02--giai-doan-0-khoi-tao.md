# 2026-08-02 — Giai đoạn 0: Khởi tạo

> Nhật ký giai đoạn dựng khung (UI Shell + Backend & hạ tầng). Ngày ghi là ngày commit tài liệu
> tiến độ đầu tiên; công việc thực tế trải dài trước đó.
> Mục lục mọi ngày: [`../TIEN-DO-DU-AN.md`](../TIEN-DO-DU-AN.md)

---

## 3. Nhật ký tiến độ theo giai đoạn

### Giai đoạn 0.1 — UI Shell

**Trạng thái: 🟨 Gần xong — chưa đủ điều kiện chốt**

Đã làm:
- Scaffold `web/` (Next.js 16 + shadcn/ui) và `web-admin/` (vendor `satnaing/shadcn-admin`, gỡ Clerk).
- Token thiết kế "Tin cậy lâm sàng" (Client) và trung tính (Admin/Vận hành, ADR-0007) áp cho cả 2 app.
- next-intl 6 ngôn ngữ, routing tiền tố URL, bản dịch thật (không placeholder) cho các namespace đã có.
- 10 màn hình Client: trang chủ, tìm việc, chi tiết tin, đăng ký/đăng nhập, dashboard ứng viên, hồ sơ +
  CCHN, CV Builder (chưa export PDF), cài đặt tài khoản, trang tổ chức công khai.
- 9 route Admin/Vận hành: dashboard NTD, danh sách tin + đăng tin, ATS Kanban (kéo-thả `@dnd-kit`), ví
  Credit, duyệt CCHN/tổ chức, đối soát thanh toán, xử lý report, danh mục & gói tin.

Verify đã chạy: build + lint sạch cả 2 app (nhiều lần, sau mỗi đợt thay đổi); dev server thật + curl xác
nhận route trả đúng mã trạng thái (kể cả 404 đúng khi job không tồn tại).

Còn thiếu (chặn việc chốt giai đoạn):
- Tin nhắn/thông báo thật — đang dùng route demo có sẵn của template `web-admin/`, chưa thay dữ liệu
  cho đúng ngữ cảnh dự án.
- CV Builder chưa xuất PDF thật (nút bị disable, có ghi chú "sắp ra mắt").
- `/ops/catalog` (Vận hành) đã nối API thật cho phần đọc (xem Giai đoạn 0.2) — form thêm/sửa vẫn
  chưa nối (còn dùng nút tĩnh).

### Giai đoạn 0.2 — Backend & hạ tầng

**Trạng thái: 🟨 Gần xong — chưa đủ điều kiện chốt**

Đã làm:
- Scaffold solution .NET 10 Clean Architecture (Jason Taylor Template, PostgreSQL, API-only).
- Phát hiện + xử lý vấn đề bản quyền: MediatR/AutoMapper (v12+/v13+) đã thương mại hóa — thay bằng
  Mediator (martinothamar, MIT) + Mapster, ghi ở [ADR-0009](../../kien-truc/adr/0009-mediator-mapster-thay-mediatr-automapper.md).
  Domain hết phụ thuộc NuGet (đúng Dependency Rule 100%). Gỡ Aspire AppHost (xung đột ADR-0002).
- Docker Compose dev (Postgres/Redis/RabbitMQ/MinIO) — 4 service verify healthy.
- Gỡ bounded context mẫu (TodoItems/TodoLists/WeatherForecasts) của template, seed role đúng
  `candidate/employer/admin/moderator`.
- Bounded context nghiệp vụ đầu tiên: **Danh mục** (specialties/locations/job_packages +
  `*_translations`) — Domain entity → EF Core config → migration → Application Query
  (`ICurrentLocale` resolve theo `Accept-Language`) → endpoint `GET /api/v1/catalog/*`. Seed data khớp
  mock đang dùng ở frontend.
- Command Create/Update cho Specialty/Location/JobPackage (`POST`/`PUT` `/api/v1/ops/catalog/*`,
  `/api/v1/ops/job-packages/*`), mỗi command có FluentValidation validator (kiểm tra field bắt buộc +
  `CatalogLocales.IsSupported` cho bản dịch) và `[Authorize(Roles = "admin,moderator")]` theo đúng vai
  trò Vận hành ở `THUAT-NGU.md`.
- Nối `web/` (trang tìm việc) và `web-admin/` (`jobs/new`, `ops/catalog`) tới API danh mục thật qua
  `lib/api.ts` ở mỗi app, fallback về mock data khi API lỗi/rỗng (demo offline vẫn chạy được).
- CI/CD cơ bản: 3 GitHub Actions workflow độc lập theo path filter — `backend.yml` (`dotnet build` +
  `dotnet test`), `web.yml` (`npm run lint` + `npm run build`), `web-admin.yml` (`npm run lint` +
  `npm run format:check` + `npm run build`).

Verify đã chạy: `dotnet build`/`dotnet test` sạch (Release config, khớp CI); `dotnet ef` migration áp
thành công vào Postgres thật qua Docker Compose (`\dt` xác nhận đúng bảng); chạy `dotnet run --project
src/Web` thật + curl xác nhận cả 6 locale trả đúng tên đã dịch, fallback đúng khi header thiếu/không hỗ
trợ; `POST /api/v1/ops/catalog/specialties` không token trả 401, `GET /api/v1/catalog/*` vẫn public;
`curl http://localhost:3000/vi/jobs` trả tên chuyên khoa/địa điểm thật từ Postgres (không phải mock);
`npm run build` + `npm run lint` sạch ở cả `web/` và `web-admin/` sau khi nối API.

Còn thiếu (chặn việc chốt giai đoạn):
- Jobs, Applications/ATS, Credit/Payment — chưa bắt đầu bounded context nào trong số này (Identity đã
  làm, xem log Giai đoạn 1 bên dưới).
- Form thêm/sửa danh mục ở `web-admin/` (`/ops/catalog`, `jobs/new`) chưa gọi Command thật — mới nối
  phần đọc (dropdown/danh sách), nút submit còn tĩnh.
- `web/` và `web-admin/` vẫn dùng mock data cho mọi màn hình khác ngoài danh mục (Jobs/ATS/Credit/...).
