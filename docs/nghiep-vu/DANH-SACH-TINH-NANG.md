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
- ⬜ Danh mục chuẩn: `specialties`, `locations`, `employment_types`

## Giai đoạn 1 — MVP

### Tài khoản & định danh
- ⬜ Đăng ký/đăng nhập email + SĐT, OTP
- ⬜ OAuth Google
- ⬜ OAuth Zalo
- ⬜ RBAC (Candidate/Employer/Admin/Moderator)
- ⬜ Đa thành viên HR trong 1 tổ chức

### Hồ sơ ứng viên
- ⬜ Hồ sơ cơ bản (học vấn, kinh nghiệm, kỹ năng)
- ⬜ Quản lý CCHN (thêm/sửa/upload document)
- ⬜ Gắn chuyên khoa + trình độ
- ⬜ CV Builder (mẫu dựng sẵn) + upload PDF
- ⬜ Hàng đợi duyệt CCHN (Admin)

### Cơ sở y tế
- ⬜ Đăng ký hồ sơ tổ chức
- ⬜ Upload giấy phép hoạt động
- ⬜ Hàng đợi duyệt tổ chức (Admin)
- ⬜ Trang công khai cơ sở y tế

### Tin tuyển dụng
- ⬜ Tạo/sửa tin (draft)
- ⬜ Gói đăng tin Eco/Pro/Max
- ⬜ Quy trình thanh toán thủ công (chuyển khoản + Admin đối soát)
- ⬜ Hàng đợi duyệt nội dung tin (Admin/Moderator)
- ⬜ Tìm kiếm & lọc (Postgres full-text)

### Ứng tuyển & ATS
- ⬜ Ứng tuyển bằng CV nền tảng/upload
- ⬜ ATS Kanban (6 trạng thái pipeline)
- ⬜ Ghi chú nội bộ + chấm điểm hồ sơ (CV Scoring)
- ⬜ Hệ thống Credit + tìm kiếm ứng viên chủ động + Profile Unlock

### Thông báo
- ⬜ Thông báo trong ứng dụng
- ⬜ Thông báo email

### Quản trị
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
