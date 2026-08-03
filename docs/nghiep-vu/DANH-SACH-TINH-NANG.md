# GiapTech.BlouseHiding — Danh sách tính năng

> Checklist tính năng theo giai đoạn, dùng để theo dõi tiến độ thực tế — cập nhật trạng thái khi triển
> khai, không để tài liệu này lệch khỏi code thật. Chi tiết nghiệp vụ từng mục xem
> [`PHAN-TICH-NGHIEP-VU.md`](./PHAN-TICH-NGHIEP-VU.md) mục 3; luồng & màn hình xem
> [`LUONG-NGHIEP-VU-MAN-HINH.md`](./LUONG-NGHIEP-VU-MAN-HINH.md).
>
> Trạng thái: ⬜ Chưa làm · 🟨 Đang làm · ✅ Hoàn thành

---

## Giai đoạn 0 — Khởi tạo

> **Thứ tự ưu tiên: dựng giao diện (0.1) trước, nối backend thật (0.2) sau** — mục tiêu trước mắt là
> thấy được hình hài sản phẩm nhanh, dùng mock data thay API thật, chưa cần toàn diện ngay. Danh sách
> chi tiết theo màn hình xem [`LUONG-NGHIEP-VU-MAN-HINH.md`](./LUONG-NGHIEP-VU-MAN-HINH.md) mục 2.

### 0.1 UI Shell (ưu tiên làm trước, không cần chờ backend)
- ⬜ Khởi tạo `web/` (Next.js + shadcn/ui) — copy component cần dùng, áp token "Tin cậy lâm sàng"
  ([`../frontend/THIET-KE-GIAO-DIEN.md`](../frontend/THIET-KE-GIAO-DIEN.md) mục 5), tự host Be Vietnam Pro
- ⬜ Khởi tạo `web-admin/` (shadcn-admin, Vite + TanStack Router) — áp token trung tính (ADR-0007), theo
  cấu trúc đã chốt ở [ADR-0008](../kien-truc/adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md)
- ⬜ Khung đa ngôn ngữ ở `web/`: routing `next-intl` 6 locale (`localePrefix: always`), cấu trúc thư
  mục `messages/`, bộ chọn ngôn ngữ (header + footer) — dựng khung trước dù bản dịch tạm để tiếng Việt
- ⬜ Mock data layer (fixture JSON tĩnh hoặc MSW) thay API thật — đủ thể hiện trạng thái loading/rỗng/lỗi/có dữ liệu cho mọi màn hình bên dưới
- ⬜ Dựng màn hình `web/` theo mục 2.1/2.2 LUONG-NGHIEP-VU-MAN-HINH.md: trang chủ, tìm việc, chi tiết
  tin, đăng ký/đăng nhập, dashboard ứng viên, hồ sơ + CCHN, CV Builder, cài đặt tài khoản (áp dụng
  skill `taste-skill` cho các trang này — xem CLAUDE.md mục 7)
- ⬜ Dựng màn hình `web-admin/` phần Admin theo mục 2.3: dashboard NTD, hồ sơ tổ chức, đăng/sửa tin,
  danh sách tin, mua gói, ví Credit, ATS Kanban, tìm ứng viên chủ động
- ⬜ Dựng màn hình `web-admin/` phần Vận hành theo mục 2.4: dashboard tổng quan, 3 hàng đợi duyệt
  (CCHN/doanh nghiệp/tin), đối soát thanh toán, quản lý danh mục/gói, quản lý người dùng, xử lý report
- ⬜ Rà lại responsive + dark mode + contrast WCAG AA trên toàn bộ màn hình vừa dựng

### 0.2 Backend & hạ tầng (sau khi UI shell đã thấy hình hài)
- ✅ Khởi tạo solution Clean Architecture (từ Jason Taylor Template — Mediator/Mapster thay
  MediatR/AutoMapper, xem [ADR-0009](../kien-truc/adr/0009-mediator-mapster-thay-mediatr-automapper.md))
- ✅ Docker Compose môi trường dev (Postgres, Redis, RabbitMQ, MinIO)
- ✅ CI/CD cơ bản (GitHub Actions, path-filter riêng): `backend.yml` (`dotnet build` + `dotnet test`),
  `web.yml` (`npm run lint` + `npm run build`), `web-admin.yml` (`npm run lint` + `format:check` +
  `npm run build`)
- ✅ Danh mục chuẩn: `specialties`, `locations`, `job_packages` + bảng `*_translations` (Domain +
  Infrastructure + Query + endpoint `GET /api/v1/catalog/*`, đã seed data khớp mock cũ ở frontend).
  `employment_types` là enum thuần (không bảng riêng), endpoint `GET /catalog/employment-types` chỉ
  echo tên enum — nhãn dịch nằm ở file dịch frontend, không lưu DB
- ✅ Command Create/Update cho Specialty/Location/JobPackage (`POST`/`PUT` `/api/v1/ops/catalog/*`,
  `/api/v1/ops/job-packages/*`), `[Authorize(Roles = "admin,moderator")]`, validator kiểm tra field bắt
  buộc + đủ 5 locale không phải `vi` (`CatalogLocales.IsSupported`)
- ✅ Middleware `Accept-Language` phía backend (`ICurrentLocale`/`CurrentLocale`, verify qua curl cả
  6 locale + fallback đúng khi thiếu header hoặc locale không hỗ trợ)
- 🟨 Nối API thật thay mock data, theo thứ tự: **danh mục — phần đọc (xong)** → Identity (login/register)
  → Jobs → Applications/ATS → Credit/Payment — màn hình nào nối xong bỏ mock riêng màn đó, không cần
  đợi tất cả. `web/` (trang tìm việc) và `web-admin/` (`jobs/new`, `ops/catalog`) đã nối phần đọc danh
  mục tới API thật (fallback mock khi API lỗi/rỗng); form thêm/sửa ở `/ops/catalog` chưa gọi Command
  thật (nút còn tĩnh)

## Giai đoạn 1 — MVP

### Tài khoản & định danh
- ✅ Đăng ký/đăng nhập email, OTP — OTP hiện là **driver giả lập nội bộ** (log thay vì gửi email/SMS
  thật, xem docs/nghiep-vu/TIEN-DO-DU-AN.md), chưa chốt nhà cung cấp SMTP/SMS thật. Xác thực SĐT
  (`phone_verified_at`) chưa làm — mới có email
- ⬜ OAuth Google
- ⬜ OAuth Zalo
- ✅ RBAC (`candidate`/`employer`/`admin`/`moderator` — site Client/Admin/Vận hành) — JWT access token
  (15 phút) + refresh token xoay vòng (hash lưu DB), claim role dùng cho `[Authorize(Roles=...)]`
- 🟨 Đa thành viên HR trong 1 tổ chức — `POST /organizations` (tạo tổ chức lần đầu → owner member) đã
  làm; `POST /organizations/{id}/members/invite` chưa làm
- ⬜ Mời thành viên HR qua email (kể cả email chưa có tài khoản — `organization_invitations`)
- ⬜ Chọn/ghi nhớ ngôn ngữ giao diện (`users.locale`), email/thông báo gửi đúng ngôn ngữ đã chọn
- ✅ Xóa tài khoản (`DELETE /users/me` — soft-delete + anonymize PII, giữ audit log/application đã ẩn danh)

### Hồ sơ ứng viên
- ✅ Hồ sơ cơ bản (`candidate_profiles` — tên, mô tả ngắn, tiểu sử; tự tạo lúc `PUT /candidates/me` lần
  đầu). Học vấn/kinh nghiệm/kỹ năng (`experiences`/`educations`) chưa làm — dời sang đợt sau
- ✅ Quản lý CCHN — thêm/sửa (chỉ khi `pending`/`rejected`)/xóa (chỉ khi chưa `verified`); upload document
  giả định URL có sẵn (chưa nối `POST /uploads/presigned-url`/MinIO thật)
- ✅ Gắn chuyên khoa + trình độ (`profile_specialties`, không trùng chuyên khoa)
- ⬜ CV Builder (mẫu dựng sẵn) + upload PDF
- ✅ Hàng đợi duyệt CCHN (Vận hành) — `GET /ops/licenses`, `POST /ops/licenses/{id}/verify` (duyệt/từ
  chối kèm lý do bắt buộc khi từ chối)

### Cơ sở y tế
- ✅ Đăng ký hồ sơ tổ chức (`POST /organizations` — tạo tổ chức lần đầu → owner member, xem mục
  "Tài khoản & định danh")
- ⬜ Upload giấy phép hoạt động
- ✅ Hàng đợi duyệt tổ chức (Vận hành) — `GET /ops/organizations`, `POST /ops/organizations/{id}/verify`
  (action `Verify`/`Reject`/`Suspend`). Rút xác thực (`Suspend`) **tự động** chuyển mọi tin `published`
  của tổ chức sang `suspended` trong cùng transaction (ERD mục 4.6) — không thao tác riêng từng tin
- ⬜ Trang công khai cơ sở y tế

### Tin tuyển dụng
- ✅ Tạo/sửa tin (draft/rejected — `CanEdit` invariant), đóng tin sớm (`close`), gia hạn (`renew` — tạo
  `jobs` row mới, tin gốc chuyển `closed`, không tái sử dụng `job_id`)
- 🟨 Nộp duyệt (`submit`) — **chỉ hỗ trợ gói Free ở MVP** (thẳng `pending`, không qua thanh toán). Gói
  Eco/Pro/Max + `pending_payment` + quy trình thanh toán thủ công là bounded context Payments riêng,
  quyết định tách khỏi vòng Jobs này (đã xác nhận với người dùng) — chưa làm
- ⬜ Quy trình thanh toán thủ công (mã tham chiếu + Vận hành xác nhận/từ chối qua `/ops/payments/{id}`)
- ✅ Hàng đợi duyệt nội dung tin (Vận hành: Admin/Moderator) — `GET /ops/jobs`,
  `POST /ops/jobs/{id}/moderate`, chặn publish khi tổ chức chưa `verified` (ERD mục 4.1)
- ✅ Rút xác thực tổ chức tự động ẩn (`suspended`) mọi tin `published` của tổ chức đó — xem mục
  "Cơ sở y tế" ở trên
- 🟨 Tìm kiếm & lọc — `GET /jobs` lọc theo specialty/location/employmentType/salaryMin/keyword bằng
  LINQ (EF Core), chưa chuyển sang Postgres full-text (`pg_trgm`/`tsvector`) như thiết kế ban đầu ghi
  ở ERD mục 0 — đủ dùng cho lượng dữ liệu MVP, tối ưu sau khi có traffic thật

### Ứng tuyển & ATS
- 🟨 Ứng tuyển — `POST /jobs/{id}/applications` (chỉ hỗ trợ CV nền tảng qua `CandidateProfile` hiện có;
  upload CV riêng/CV Builder chưa làm, xem mục "Hồ sơ ứng viên"). Chặn ứng tuyển trùng
  (`UNIQUE(job_id, candidate_id)`) và tin chưa `published`
- ✅ Chụp `cv_snapshot` (jsonb từ `CandidateProfile`+`Licenses`+`ProfileSpecialties`) + tính `score` 1
  lần lúc ứng tuyển — không đổi khi hồ sơ gốc thay đổi sau này
- ✅ ATS Kanban (7 trạng thái pipeline `new→reviewing→shortlisted→interview→offer→hired`, `rejected`
  tách nhánh) — `GET /jobs/{id}/applications`, khả dụng bất kể `jobs.status` (đúng ERD mục 4.9)
- ✅ Chuyển trạng thái ghi `application_stage_history` cùng transaction (`PATCH /applications/{id}/stage`,
  tham số `silent`), ghi chú nội bộ (`POST /applications/{id}/notes`), chấm điểm ghi đè thủ công
  (`PATCH /applications/{id}/score`) — match-score theo trường có cấu trúc (chuyên khoa/CCHN đã duyệt/
  địa điểm — không NLP/AI, xem ERD mục 4 điểm 8)
- ✅ Ví Credit (`credit_wallets` tự tạo lúc `POST /organizations`, `balance CHECK >= 0`) + tìm kiếm ứng
  viên chủ động (`GET /candidates/search` — ẩn liên hệ) + Profile Unlock (`POST /candidates/{id}/unlock`
  — trừ Credit qua `ExecuteUpdateAsync` có điều kiện `WHERE Balance >= cost`, atomic tại DB, tránh
  race condition không cần row lock thủ công; idempotent theo `UNIQUE(org_id, candidate_id)`, mở lại
  không mất thêm Credit)
- 🟨 Nạp Credit — **chưa nối `POST /payments/credit-topup` thật** (phụ thuộc bounded context Payments
  chưa làm). Thay bằng `POST /ops/organizations/{id}/credit-bonus` (Vận hành cộng thủ công,
  `reason=bonus`) để test unlock end-to-end — quyết định tạm thời đã xác nhận với người dùng, sẽ đổi
  khi Payments hoàn thiện
- ⬜ Hoàn Credit thủ công khi có tranh chấp (`/ops/organizations/{id}/credit-refund`) — chưa làm, khác
  `credit-bonus` (dùng khi tranh chấp/lỗi hệ thống, không phải nạp thường)

### Thông báo
- ⬜ Thông báo trong ứng dụng
- ⬜ Thông báo email

### Vận hành
- ⬜ Dashboard số liệu cơ bản
- ⬜ Quản lý người dùng (khóa/mở khóa)
- ⬜ Xử lý báo cáo vi phạm (`POST /reports` tạo báo cáo, `/ops/reports/{id}/resolve` với 3 hành động: `warned`/`content_removed`/`account_suspended`)

## Giai đoạn 2 — Hoàn thiện

- ⬜ Chuyển tìm kiếm sang OpenSearch
- ⬜ Gợi ý việc làm / matching ứng viên
- ⬜ Chat realtime (SignalR)
- ⬜ Mobile app (Flutter/React Native)
- ⬜ Test đánh giá năng lực chuyên môn theo vị trí
- ⬜ Công cụ tính phụ cấp trực/độc hại/thuế TNCN
- ⬜ Đánh giá cơ sở y tế (review, có kiểm duyệt)

## Giai đoạn 3 — Mở rộng

- ⬜ Sự kiện/hội thảo/CME
- ⬜ Chuyên mục nội dung (tin tức/kỹ năng nghề y)
- ⬜ Cộng đồng sinh viên y khoa (mô hình CTV)
- ⬜ Phân tích/BI, employer branding
- ⬜ Matching bằng ML
- ⬜ Tích hợp/đối soát xác thực CCHN theo dữ liệu ngành

## Chưa có mốc thời gian cụ thể (chờ quyết định)

- ⬜ Tích hợp cổng thanh toán tự động (xem [ADR-0003](../kien-truc/adr/0003-hoan-cong-thanh-toan-tu-dong.md))
