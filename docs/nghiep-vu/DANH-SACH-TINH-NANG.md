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
- ⬜ Khởi tạo solution Clean Architecture (từ Jason Taylor Template)
- ⬜ Docker Compose môi trường dev (Postgres, Redis, RabbitMQ, MinIO)
- ⬜ CI/CD cơ bản (build, test, lint) — cho cả `web/`, `web-admin/`, backend
- ⬜ Danh mục chuẩn: `specialties`, `locations`, `employment_types`, `job_packages` + bảng `*_translations`
- ⬜ Middleware `Accept-Language` phía backend (khớp khung i18n đã dựng ở 0.1)
- ⬜ Nối API thật thay mock data, theo thứ tự: danh mục → Identity (login/register) → Jobs →
  Applications/ATS → Credit/Payment — màn hình nào nối xong bỏ mock riêng màn đó, không cần đợi tất cả

## Giai đoạn 1 — MVP

### Tài khoản & định danh
- ⬜ Đăng ký/đăng nhập email + SĐT, OTP
- ⬜ OAuth Google
- ⬜ OAuth Zalo
- ⬜ RBAC (`candidate`/`employer`/`admin`/`moderator` — site Client/Admin/Vận hành)
- ⬜ Đa thành viên HR trong 1 tổ chức
- ⬜ Mời thành viên HR qua email (kể cả email chưa có tài khoản — `organization_invitations`)
- ⬜ Chọn/ghi nhớ ngôn ngữ giao diện (`users.locale`), email/thông báo gửi đúng ngôn ngữ đã chọn
- ⬜ Xóa tài khoản (`DELETE /users/me` — soft-delete + anonymize PII, giữ audit log/application đã ẩn danh)

### Hồ sơ ứng viên
- ⬜ Hồ sơ cơ bản (học vấn, kinh nghiệm, kỹ năng)
- ⬜ Quản lý CCHN (thêm/sửa/upload document)
- ⬜ Gắn chuyên khoa + trình độ
- ⬜ CV Builder (mẫu dựng sẵn) + upload PDF
- ⬜ Hàng đợi duyệt CCHN (Vận hành)

### Cơ sở y tế
- ⬜ Đăng ký hồ sơ tổ chức
- ⬜ Upload giấy phép hoạt động
- ⬜ Hàng đợi duyệt tổ chức (Vận hành)
- ⬜ Trang công khai cơ sở y tế

### Tin tuyển dụng
- ⬜ Tạo/sửa tin (draft)
- ⬜ Gói đăng tin Eco/Pro/Max
- ⬜ Trạng thái `pending_payment` riêng biệt với `pending` (chờ thanh toán ≠ chờ duyệt nội dung)
- ⬜ Quy trình thanh toán thủ công (mã tham chiếu + Vận hành xác nhận/từ chối qua `/ops/payments/{id}`)
- ⬜ Hàng đợi duyệt nội dung tin (Vận hành: Admin/Moderator)
- ⬜ Gia hạn tin = tạo tin mới (clone), không tái sử dụng `job_id`
- ⬜ Rút xác thực tổ chức tự động ẩn (`suspended`) mọi tin `published` của tổ chức đó
- ⬜ Tìm kiếm & lọc (Postgres full-text)

### Ứng tuyển & ATS
- ⬜ Ứng tuyển bằng CV nền tảng/upload
- ⬜ Chụp `cv_snapshot` + tính `score` 1 lần lúc ứng tuyển (không đổi khi hồ sơ gốc thay đổi sau này)
- ⬜ ATS Kanban (6 trạng thái pipeline) — vẫn thao tác được sau khi tin hết hạn/đóng/suspended
- ⬜ Ghi chú nội bộ + match-score theo trường có cấu trúc (chuyên khoa/kinh nghiệm/địa điểm — không NLP/AI, xem ERD mục 4 điểm 8)
- ⬜ Hệ thống Credit + tìm kiếm ứng viên chủ động + Profile Unlock
- ⬜ Hoàn Credit thủ công khi có tranh chấp (`/ops/organizations/{id}/credit-refund`)

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
