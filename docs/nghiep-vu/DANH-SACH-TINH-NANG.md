# GiapTech.BlouseHiding — Danh sách tính năng

> Checklist tính năng theo giai đoạn, dùng để theo dõi tiến độ thực tế — cập nhật trạng thái khi triển
> khai, không để tài liệu này lệch khỏi code thật. Chi tiết nghiệp vụ từng mục xem
> [`PHAN-TICH-NGHIEP-VU.md`](./PHAN-TICH-NGHIEP-VU.md) mục 3; luồng & màn hình xem
> [`LUONG-NGHIEP-VU-MAN-HINH.md`](./LUONG-NGHIEP-VU-MAN-HINH.md).
>
> Trạng thái: ⬜ Chưa làm · 🟨 Đang làm · ✅ Hoàn thành

---

## Giai đoạn 0 — Khởi tạo

- ⬜ Khởi tạo solution Clean Architecture (từ Jason Taylor Template)
- ⬜ Khởi tạo Next.js + shadcn/ui
- ⬜ Docker Compose môi trường dev (Postgres, Redis, RabbitMQ, MinIO)
- ⬜ CI/CD cơ bản (build, test, lint)
- ⬜ Danh mục chuẩn: `specialties`, `locations`, `employment_types` (kèm cột `name_en`)
- ⬜ Khung đa ngôn ngữ: routing `next-intl` (vi/en), file dịch theo route group, middleware `Accept-Language` phía backend

## Giai đoạn 1 — MVP

### Tài khoản & định danh
- ⬜ Đăng ký/đăng nhập email + SĐT, OTP
- ⬜ OAuth Google
- ⬜ OAuth Zalo
- ⬜ RBAC (`candidate`/`employer`/`admin`/`moderator` — site Client/Admin/Vận hành)
- ⬜ Đa thành viên HR trong 1 tổ chức
- ⬜ Mời thành viên HR qua email (kể cả email chưa có tài khoản — `organization_invitations`)
- ⬜ Chọn/ghi nhớ ngôn ngữ giao diện (`users.locale`), email/thông báo gửi đúng ngôn ngữ đã chọn

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
- ⬜ Ghi chú nội bộ + chấm điểm hồ sơ (CV Scoring)
- ⬜ Hệ thống Credit + tìm kiếm ứng viên chủ động + Profile Unlock
- ⬜ Hoàn Credit thủ công khi có tranh chấp (`/ops/organizations/{id}/credit-refund`)

### Thông báo
- ⬜ Thông báo trong ứng dụng
- ⬜ Thông báo email

### Vận hành
- ⬜ Dashboard số liệu cơ bản
- ⬜ Quản lý người dùng (khóa/mở khóa)
- ⬜ Xử lý báo cáo vi phạm

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
