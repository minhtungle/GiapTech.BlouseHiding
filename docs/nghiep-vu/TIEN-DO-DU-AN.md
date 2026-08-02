# GiapTech.BlouseHiding — Tiến độ dự án & Quy trình chốt giai đoạn

> Tài liệu này khác [`DANH-SACH-TINH-NANG.md`](./DANH-SACH-TINH-NANG.md): file đó là **checklist tính
> năng** (còn gì phải làm), file này là **nhật ký tiến độ** (đã làm gì, đã verify chưa, giai đoạn đã
> đủ điều kiện chốt chưa) — cập nhật khi **kết thúc một giai đoạn**, không phải mỗi commit.

---

## 1. Quy trình chốt 1 giai đoạn

Trước khi chuyển sang giai đoạn tiếp theo, chạy qua đủ checklist này — **không tự ý bỏ qua bước nào**.
Nếu có mục không đạt, ghi rõ lý do + kế hoạch xử lý vào nhật ký (mục 3) thay vì im lặng bỏ qua.

- [ ] **Tất cả tính năng** trong phạm vi giai đoạn ở `DANH-SACH-TINH-NANG.md` đã chuyển ✅ — mục nào
      còn ⬜/🟨 phải có lý do rõ ràng (dời sang giai đoạn sau, không phải quên).
- [ ] **Build sạch** — backend (`dotnet build`, 0 warning/error) + `web/` + `web-admin/`
      (`npm run build`, 0 error).
- [ ] **Test suite chạy pass** — `dotnet test` (unit + functional/integration nếu có), `npm run lint`
      cả 2 frontend.
- [ ] **Verify chạy thật, không chỉ dựa vào build** — dev server thật + gọi API thật (curl/browser),
      không chấp nhận "chắc là chạy được" khi chưa tự tay kiểm tra.
- [ ] **Tài liệu khớp code thật** — ERD/API-DESIGN.md/DANH-SACH-TINH-NANG.md không lệch so với những gì
      vừa implement (CLAUDE.md mục 4 quy tắc 8).
- [ ] **Không còn lỗ hổng nghiệp vụ nghiêm trọng** chưa xử lý hoặc chưa ghi nhận rõ (rà theo kiểu đợt
      audit đã làm trước Giai đoạn 0.2 — xem mục 3).
- [ ] **Đã commit + push** toàn bộ, không còn thay đổi treo lơ lửng.
- [ ] **Ghi log tổng kết vào mục 3 của tài liệu này** trước khi bắt đầu công việc của giai đoạn tiếp
      theo — nhật ký phải nêu rõ: đã làm gì, verify bằng cách nào, còn thiếu gì, quyết định gì đã chốt.

---

## 2. Trạng thái tổng quan (cập nhật mới nhất lên đầu)

| Giai đoạn | Trạng thái | Ghi chú ngắn |
|---|---|---|
| 0.1 — UI Shell | 🟨 Gần xong | Còn thiếu tin nhắn/thông báo thật, export PDF CV |
| 0.2 — Backend & hạ tầng | 🟨 Đang làm | Danh mục (đọc) xong; CI/CD, Command CRUD danh mục, Identity chưa làm |
| 1 — MVP | ⬜ Chưa bắt đầu | |
| 2 — Hoàn thiện | ⬜ Chưa bắt đầu | |
| 3 — Mở rộng | ⬜ Chưa bắt đầu | |

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
- `/ops/catalog` (Vận hành) vẫn dùng mock data — chưa nối API thật (xem Giai đoạn 0.2).

### Giai đoạn 0.2 — Backend & hạ tầng

**Trạng thái: 🟨 Đang làm — chưa đủ điều kiện chốt**

Đã làm:
- Scaffold solution .NET 10 Clean Architecture (Jason Taylor Template, PostgreSQL, API-only).
- Phát hiện + xử lý vấn đề bản quyền: MediatR/AutoMapper (v12+/v13+) đã thương mại hóa — thay bằng
  Mediator (martinothamar, MIT) + Mapster, ghi ở [ADR-0009](../kien-truc/adr/0009-mediator-mapster-thay-mediatr-automapper.md).
  Domain hết phụ thuộc NuGet (đúng Dependency Rule 100%). Gỡ Aspire AppHost (xung đột ADR-0002).
- Docker Compose dev (Postgres/Redis/RabbitMQ/MinIO) — 4 service verify healthy.
- Gỡ bounded context mẫu (TodoItems/TodoLists/WeatherForecasts) của template, seed role đúng
  `candidate/employer/admin/moderator`.
- Bounded context nghiệp vụ đầu tiên: **Danh mục** (specialties/locations/job_packages +
  `*_translations`) — Domain entity → EF Core config → migration → Application Query
  (`ICurrentLocale` resolve theo `Accept-Language`) → endpoint `GET /api/v1/catalog/*`. Seed data khớp
  mock đang dùng ở frontend.

Verify đã chạy: `dotnet build`/`dotnet test` sạch nhiều lần; migration áp thành công vào Postgres thật
qua Docker Compose (`\dt` xác nhận đúng bảng); chạy `dotnet run --project src/Web` thật + curl xác nhận
cả 6 locale trả đúng tên đã dịch, fallback đúng khi header thiếu/không hỗ trợ.

Còn thiếu (chặn việc chốt giai đoạn):
- CI/CD cơ bản (build/test/lint tự động) — chưa làm.
- Command Create/Update cho danh mục (Vận hành quản lý) — mới có Query đọc.
- Identity thật (đăng ký/đăng nhập/OTP/OAuth) — mới có khung Identity mặc định của template, chưa nối
  role/luồng thật của dự án.
- Jobs, Applications/ATS, Credit/Payment — chưa bắt đầu bounded context nào trong số này.
- `web/` và `web-admin/` vẫn dùng mock data cho mọi màn hình trừ danh mục — chưa nối API thật.

### Giai đoạn 1 — MVP

*(Chưa bắt đầu — điền nhật ký khi có tiến độ thật)*

### Giai đoạn 2 — Hoàn thiện

*(Chưa bắt đầu)*

### Giai đoạn 3 — Mở rộng

*(Chưa bắt đầu)*
