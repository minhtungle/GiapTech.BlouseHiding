# GiapTech.BlouseHiding — Thiết kế API

> Cụ thể hóa từ [`../database/ERD-CHI-TIET.md`](../database/ERD-CHI-TIET.md) và
> [`../nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md`](../nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md).
> REST, JSON, versioned qua path `/api/v1`. Auth: JWT Bearer (access token ngắn hạn + refresh token).
> Quy ước code implement các endpoint này: [`KIEN-TRUC-BACKEND.md`](./KIEN-TRUC-BACKEND.md).

---

## 1. Quy ước chung

- **Base URL**: `/api/v1`
- **Auth**: header `Authorization: Bearer <access_token>`. Endpoint không cần đăng nhập ghi rõ `Public`.
- **Phân trang**: query `?page=1&pageSize=20`, response bọc trong:
  ```json
  { "data": [...], "meta": { "page": 1, "pageSize": 20, "total": 134 } }
  ```
- **Lỗi**: theo RFC 7807 (Problem Details):
  ```json
  { "type": "validation_error", "title": "...", "status": 400, "errors": { "field": ["message"] } }
  ```
- **Phân quyền (RBAC)**: mỗi endpoint ghi rõ role được phép — `Candidate`, `Employer` (member của org,
  dùng trang **Admin**), `Admin`/`Moderator` (role backend của đội **Vận hành** nội bộ nền tảng — xem
  quy ước tên site ở [`../nghiep-vu/PHAN-TICH-NGHIEP-VU.md`](../nghiep-vu/PHAN-TICH-NGHIEP-VU.md) mục 2),
  hoặc `Owner` (chỉ chủ sở hữu resource, vd. hồ sơ của chính mình).
  > ⚠️ Role backend tên là `Admin` nhưng **site tương ứng gọi là "Vận hành"**, không phải trang "Admin"
  > mà Nhà tuyển dụng dùng — 2 khái niệm khác nhau, tránh nhầm lẫn khi đọc bảng dưới.
- **Idempotency**: các endpoint tạo giao dịch tiền/credit nhận header `Idempotency-Key`.

---

## 2. Identity — `/auth`, `/users`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| POST | `/auth/register` | Public | Đăng ký (role: candidate/employer), gửi OTP |
| POST | `/auth/verify-otp` | Public | Xác thực OTP email/SĐT |
| POST | `/auth/login` | Public | Đăng nhập, trả access + refresh token |
| POST | `/auth/refresh` | Public (refresh token) | Cấp access token mới |
| POST | `/auth/logout` | Owner | Thu hồi refresh token |
| POST | `/auth/forgot-password` | Public | Gửi email reset |
| POST | `/auth/reset-password` | Public (reset token) | Đặt lại mật khẩu |
| GET | `/users/me` | Owner | Thông tin tài khoản hiện tại |
| PATCH | `/users/me` | Owner | Đổi email/SĐT (yêu cầu xác thực lại) |
| DELETE | `/users/me` | Owner | Yêu cầu xóa tài khoản (NĐ 13/2023 — soft delete + anonymize) |

---

## 3. Hồ sơ ứng viên — `/candidates`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/candidates/me` | Candidate (Owner) | Xem hồ sơ của chính mình |
| PUT | `/candidates/me` | Candidate (Owner) | Cập nhật thông tin chung |
| GET | `/candidates/{id}` | Employer (đã unlock) / Vận hành | Xem hồ sơ ứng viên khác — 403 nếu chưa unlock (mục 7) |
| GET | `/candidates/{id}/public-summary` | Public | Bản rút gọn ẩn danh (vd. sau khi NTD xem trong kết quả tìm kiếm chưa mở) |
| POST | `/candidates/me/licenses` | Candidate (Owner) | Thêm CCHN + upload document |
| PUT | `/candidates/me/licenses/{licenseId}` | Candidate (Owner) | Sửa CCHN (chỉ khi `pending`/`rejected`) |
| DELETE | `/candidates/me/licenses/{licenseId}` | Candidate (Owner) | Xóa CCHN chưa duyệt |
| POST | `/candidates/me/specialties` | Candidate (Owner) | Gắn chuyên khoa + trình độ |
| POST | `/candidates/me/experiences` | Candidate (Owner) | Thêm kinh nghiệm làm việc |
| PUT/DELETE | `/candidates/me/experiences/{id}` | Candidate (Owner) | Sửa/xóa |
| POST | `/candidates/me/educations` | Candidate (Owner) | Thêm học vấn |
| POST | `/candidates/me/certificates` | Candidate (Owner) | Thêm chứng chỉ CME |
| GET | `/candidates/me/cvs` | Candidate (Owner) | Danh sách CV (builder + upload) |
| POST | `/candidates/me/cvs` | Candidate (Owner) | Tạo CV mới (từ template hoặc upload) |
| PUT | `/candidates/me/cvs/{cvId}` | Candidate (Owner) | Sửa nội dung CV Builder |
| POST | `/candidates/me/cvs/{cvId}/export` | Candidate (Owner) | Xuất PDF |

**Ví dụ response `GET /candidates/me`:**
```json
{
  "id": "uuid",
  "fullName": "Nguyễn Văn A",
  "headline": "Điều dưỡng ICU 5 năm kinh nghiệm",
  "completionPct": 80,
  "specialties": [{ "code": "GAY_ME_HOI_SUC", "name": "Gây mê hồi sức", "level": "senior" }],
  "licenses": [
    { "id": "uuid", "licenseNo": "CCHN-001234", "verifyStatus": "verified", "expiredAt": "2027-01-01" }
  ]
}
```

---

## 4. Cơ sở y tế (Organization) — `/organizations`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| POST | `/organizations` | Employer (đã đăng ký user) | Tạo hồ sơ tổ chức lần đầu → tạo `owner` member |
| GET | `/organizations/{id}` | Public | Trang công khai của cơ sở y tế |
| PUT | `/organizations/{id}` | Employer (owner/hr_manager) | Cập nhật thông tin |
| POST | `/organizations/{id}/documents` | Employer (owner) | Upload giấy phép hoạt động |
| GET | `/organizations/{id}/members` | Employer (thành viên) | Danh sách HR trong tổ chức |
| POST | `/organizations/{id}/members/invite` | Employer (owner/hr_manager) | Mời thành viên qua email |
| DELETE | `/organizations/{id}/members/{memberId}` | Employer (owner) | Xóa thành viên |
| GET | `/organizations/{id}/reviews` | Public | Đánh giá đã duyệt (Giai đoạn 2) |
| POST | `/organizations/{id}/reviews` | Candidate | Gửi đánh giá (vào hàng đợi kiểm duyệt) |

---

## 5. Tin tuyển dụng — `/jobs`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/jobs` | Public | Tìm kiếm/lọc: `?specialty=&location=&employmentType=&salaryMin=&keyword=` |
| GET | `/jobs/{id}` | Public | Chi tiết tin (chỉ tin `published`, trừ khi là chủ sở hữu) |
| POST | `/jobs` | Employer (member) | Tạo tin (status = `draft`) |
| PUT | `/jobs/{id}` | Employer (member cùng org) | Sửa tin (chỉ khi `draft`/`rejected`) |
| POST | `/jobs/{id}/submit` | Employer (member) | Nộp duyệt → `pending` (kèm `packageId` đã chọn) |
| POST | `/jobs/{id}/close` | Employer (member) | Đóng tin sớm |
| GET | `/jobs/{id}/applications` | Employer (member cùng org) | Danh sách ứng viên đã nộp (ATS) |
| GET | `/organizations/{id}/jobs` | Employer (member) | Danh sách tin của tổ chức (mọi trạng thái) |

**Query tìm kiếm ví dụ:**
`GET /jobs?specialty=DIEU_DUONG&location=ha-noi&employmentType=truc_ca&salaryMin=8000000&page=1`

---

## 6. Gói tin & thanh toán — `/job-packages`, `/payments`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/job-packages` | Public | Danh sách gói Eco/Pro/Max + giá |
| POST | `/payments/job-package` | Employer | Khởi tạo thanh toán mua gói cho 1 job → trả URL cổng thanh toán |
| POST | `/payments/webhook/{provider}` | Public (xác thực chữ ký) | Webhook callback từ VNPay/Momo/ZaloPay |
| GET | `/payments/{id}` | Employer (owner) | Tra cứu trạng thái giao dịch |

> Sau khi webhook xác nhận `success`, backend tạo `job_purchases` + set `jobs.status = pending` (vào hàng đợi duyệt nội dung).

---

## 7. Credit & mở hồ sơ ứng viên — `/credit`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/organizations/{id}/credit-wallet` | Employer (member) | Xem số dư |
| GET | `/organizations/{id}/credit-transactions` | Employer (member) | Lịch sử giao dịch |
| POST | `/payments/credit-topup` | Employer | Nạp Credit (qua cổng thanh toán) |
| GET | `/candidates/search` | Employer (member) | Tìm ứng viên theo chuyên khoa/kinh nghiệm/khu vực — **kết quả ẩn liên hệ** |
| POST | `/candidates/{id}/unlock` | Employer (member) | Trừ Credit, tạo `profile_unlocks`, trả hồ sơ đầy đủ |

**Response khi chưa unlock (`GET /candidates/search`):**
```json
{ "id": "uuid", "headline": "Điều dưỡng ICU...", "specialty": "...", "yearsOfExperience": 5,
  "isUnlocked": false, "unlockCost": 15 }
```
**Sau `POST /candidates/{id}/unlock`:** trả full profile như mục 3, cộng `contactEmail`, `contactPhone`.

---

## 8. Ứng tuyển & ATS — `/applications`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| POST | `/jobs/{jobId}/applications` | Candidate | Ứng tuyển (chọn `cvId`, `coverLetter`) |
| GET | `/candidates/me/applications` | Candidate (Owner) | Danh sách đơn đã nộp + trạng thái |
| GET | `/applications/{id}` | Candidate (Owner) / Employer (org liên quan) | Chi tiết 1 đơn |
| PATCH | `/applications/{id}/stage` | Employer (member) | Chuyển trạng thái pipeline (`stage`, `silent: bool`) |
| POST | `/applications/{id}/notes` | Employer (member) | Thêm ghi chú nội bộ |
| PATCH | `/applications/{id}/score` | Employer (member) | Chấm điểm hồ sơ |
| GET | `/applications/{id}/history` | Employer (member) / Candidate (Owner) | Lịch sử chuyển trạng thái |

---

## 9. Nhắn tin & thông báo — `/conversations`, `/notifications`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/conversations` | Owner (candidate hoặc employer member) | Danh sách hội thoại |
| POST | `/conversations` | Candidate / Employer | Tạo hội thoại mới (gắn `jobId` tùy chọn) |
| GET | `/conversations/{id}/messages` | Owner của hội thoại | Lịch sử tin nhắn (phân trang) |
| POST | `/conversations/{id}/messages` | Owner của hội thoại | Gửi tin nhắn |
| WS | `/ws/conversations/{id}` | Owner | SignalR hub — realtime message + typing indicator |
| GET | `/notifications` | Owner | Danh sách thông báo (`?unreadOnly=true`) |
| PATCH | `/notifications/{id}/read` | Owner | Đánh dấu đã đọc |

---

## 10. Danh mục dùng chung — `/catalog`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/catalog/specialties` | Public | Cây chuyên khoa |
| GET | `/catalog/locations` | Public | Cây tỉnh/thành → quận/huyện |
| GET | `/catalog/employment-types` | Public | Danh sách loại hình làm việc |

---

## 11. Vận hành nền tảng (Ops) — `/ops`

> Prefix `/ops` (không phải `/admin`) — role backend vẫn tên `Admin`/`Moderator`, nhưng site tương ứng
> gọi là **Vận hành**, phân biệt với trang **Admin** dành cho Nhà tuyển dụng (mục 4 trở lên). Xem quy
> ước tên site ở [`../nghiep-vu/PHAN-TICH-NGHIEP-VU.md`](../nghiep-vu/PHAN-TICH-NGHIEP-VU.md) mục 2.

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/ops/licenses?status=pending` | Admin/Moderator | Hàng đợi duyệt CCHN |
| POST | `/ops/licenses/{id}/verify` | Admin/Moderator | Duyệt/từ chối kèm lý do |
| GET | `/ops/organizations?status=pending` | Admin/Moderator | Hàng đợi duyệt tổ chức |
| POST | `/ops/organizations/{id}/verify` | Admin/Moderator | Duyệt/từ chối |
| GET | `/ops/jobs?status=pending` | Admin/Moderator | Hàng đợi duyệt tin |
| POST | `/ops/jobs/{id}/moderate` | Admin/Moderator | Duyệt/từ chối kèm lý do |
| GET | `/ops/reports?status=pending` | Admin/Moderator | Danh sách báo cáo vi phạm |
| POST | `/ops/reports/{id}/resolve` | Admin/Moderator | Xử lý report |
| GET | `/ops/users` | Admin | Tìm kiếm/quản lý người dùng |
| POST | `/ops/users/{id}/suspend` | Admin | Khóa tài khoản |
| CRUD | `/ops/catalog/specialties`, `/ops/catalog/locations` | Admin | Quản lý danh mục |
| CRUD | `/ops/job-packages` | Admin | Cấu hình gói tin/giá |
| GET | `/ops/audit-logs` | Admin | Tra cứu nhật ký kiểm toán |
| GET | `/ops/dashboard/stats` | Admin | Số liệu tổng quan |

---

## 12. Sự kiện & CME — `/events` (Giai đoạn 3)

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/events` | Public | Danh sách sự kiện/CME/học bổng |
| GET | `/events/{id}` | Public | Chi tiết |
| POST | `/events/{id}/register` | Candidate/Employer | Đăng ký tham dự |
| POST | `/events` | Employer / Vận hành | Tạo sự kiện (kiểm duyệt trước publish) |

---

## 13. Ánh xạ luồng nghiệp vụ → endpoint (tham chiếu nhanh)

| Luồng (xem `LUONG-NGHIEP-VU-MAN-HINH.md`) | Endpoint chính theo thứ tự gọi |
|---|---|
| 1.1 Đăng ký & xác thực CCHN | `POST /auth/register` → `POST /auth/verify-otp` → `POST /candidates/me/licenses` → (Vận hành) `POST /ops/licenses/{id}/verify` |
| 1.2 Xác minh cơ sở y tế | `POST /organizations` → `POST /organizations/{id}/documents` → (Vận hành) `POST /ops/organizations/{id}/verify` |
| 1.3 Đăng tin + mua gói | `POST /jobs` → `POST /payments/job-package` → webhook → `POST /jobs/{id}/submit` → (Vận hành) `POST /ops/jobs/{id}/moderate` |
| 1.4 Tìm & ứng tuyển | `GET /jobs` → `GET /jobs/{id}` → `POST /jobs/{jobId}/applications` |
| 1.5 ATS pipeline | `GET /jobs/{id}/applications` → `PATCH /applications/{id}/stage` → `POST /applications/{id}/notes` |
| 1.6 Credit mở hồ sơ | `GET /candidates/search` → `POST /payments/credit-topup` (nếu thiếu) → `POST /candidates/{id}/unlock` |
| 1.7 Kiểm duyệt (trang Vận hành) | `GET /ops/licenses|organizations|jobs|reports?status=pending` → `POST .../verify|moderate|resolve` |

---

## 14. Bước tiếp theo

→ Xem [`../frontend/wireframes/man-hinh-cot-loi.html`](../frontend/wireframes/man-hinh-cot-loi.html) —
wireframe trực quan cho 3 màn hình cốt lõi (Chi tiết tin, Hồ sơ CCHN, ATS Kanban), chú thích ánh xạ
trực tiếp tới các endpoint ở trên và các bảng trong [`../database/ERD-CHI-TIET.md`](../database/ERD-CHI-TIET.md).
