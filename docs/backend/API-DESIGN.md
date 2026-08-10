# GiapTech.BlouseHiding — Thiết kế API

> Cụ thể hóa từ [`../database/ERD-CHI-TIET.md`](../database/ERD-CHI-TIET.md) và
> [`../nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md`](../nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md).
> REST, JSON, versioned qua path `/api/v1`. Auth: JWT Bearer (access token ngắn hạn + refresh token).
> Quy ước code implement các endpoint này: [`KIEN-TRUC-BACKEND.md`](./KIEN-TRUC-BACKEND.md).

---

## 1. Quy ước chung

- **Base URL**: `/api/v1`
- **Auth**: header `Authorization: Bearer <access_token>`. Endpoint không cần đăng nhập ghi rõ `Public`.
- **Phân trang**: query `?page=1&pageSize=20` — xem mục "Phân trang" ngay dưới bảng quy ước này để
  biết định dạng response thật và danh sách endpoint đã áp dụng.
  > ⚠️ Thiết kế gốc dự kiến bọc `{ "data": [...], "meta": {...} }` nhưng **code thật dùng
  > `{ "items": [...], "pageNumber", "pageSize", "totalCount", "totalPages", "hasPreviousPage",
  > "hasNextPage" }`** (phẳng, không có `meta`). Đừng làm theo định dạng cũ.
- **Lỗi**: theo RFC 7807 (Problem Details):
  ```json
  { "type": "validation_error", "title": "...", "status": 400, "errors": { "field": ["message"] } }
  ```
  ⚠️ `ProblemDetailsExceptionHandler` (`src/Web/Infrastructure/`) phải gọi `WriteAsJsonAsync` với biến
  khai kiểu **cụ thể** (`ValidationProblemDetails`, không ép về `ProblemDetails` base) — nếu không,
  `System.Text.Json` chỉ serialize field của base class, mất hoàn toàn field `errors` (bug thật đã gặp,
  không có test nào bắt được vì mọi test khác gọi thẳng `ISender`, không đi qua serialize HTTP thật —
  xem `EndpointRoutingTests.ValidationError_Response_Should_Include_Errors_Field`).
- **Phân quyền (RBAC)**: mỗi endpoint ghi rõ role được phép — `Candidate`, `Employer` (member của org,
  dùng trang **Admin**), `Admin`/`Moderator` (role backend của đội **Vận hành** nội bộ nền tảng — xem
  quy ước tên site ở [`../nghiep-vu/PHAN-TICH-NGHIEP-VU.md`](../nghiep-vu/PHAN-TICH-NGHIEP-VU.md) mục 2),
  hoặc `Owner` (chỉ chủ sở hữu resource, vd. hồ sơ của chính mình).
  > ⚠️ Role backend tên là `Admin` nhưng **site tương ứng gọi là "Vận hành"**, không phải trang "Admin"
  > mà Nhà tuyển dụng dùng — 2 khái niệm khác nhau, tránh nhầm lẫn khi đọc bảng dưới.
- **Enum**: serialize dạng **string** trong JSON (vd `"orgType": "BenhVienTu"`, không phải số thứ tự)
  — cấu hình `JsonStringEnumConverter` toàn cục ở `Web/DependencyInjection.cs`. Ổn định hơn khi thêm
  giá trị enum mới ở giữa, dễ đọc hơn khi debug.
- **Idempotency**: các endpoint tạo giao dịch tiền/credit nhận header `Idempotency-Key`.
- **CORS**: API phục vụ **2 origin frontend riêng biệt** — Client (`web/`, Next.js) và Admin/Vận hành
  (`web-admin/`, shadcn-admin) — xem [ADR-0008](../kien-truc/adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md).
  Whitelist đúng 2 origin domain trong CORS policy; vì auth dùng JWT Bearer (không phải cookie session)
  nên không cần cấu hình `credentials`/cookie domain phức tạp giữa 2 origin.
- **Upload file** (ảnh CCHN, giấy phép doanh nghiệp, CV, avatar, logo): dùng **chung 1 endpoint**
  `POST /uploads/presigned-url` — Owner — body `{ purpose: "license" | "cv" | "avatar" | "org_document" | "org_logo", file_name, content_type }`, trả về URL + field để client upload thẳng lên MinIO (xem
  [`CONG-NGHE-BACKEND.md`](./CONG-NGHE-BACKEND.md)), không upload qua backend. Không cần endpoint riêng
  cho từng loại file ở Giai đoạn 1 — `purpose` đủ để backend áp giới hạn kích thước/định dạng khác nhau.
- **Báo cáo vi phạm** (report): `POST /reports` — Owner (bất kỳ user đăng nhập) — body
  `{ target_type: "job" | "organization" | "profile" | "message", target_id, reason }`. Vận hành xử lý
  qua `/ops/reports/*` (mục 11).
- **Ngôn ngữ**: request gửi header `Accept-Language` với 1 trong 6 giá trị `vi`/`en`/`ja`/`zh`/`ko`/`es`
  — dùng để trả lỗi validate đúng ngôn ngữ và (xem [ADR-0006](../kien-truc/adr/0006-da-ngon-ngu.md))
  để endpoint danh mục (mục 10) resolve sẵn `name` theo đúng locale đó (JOIN bảng `*_translations`,
  lùi về tiếng Việt nếu thiếu bản dịch) — response chỉ có 1 trường `name` đã dịch, không trả cả 6 ngôn
  ngữ cùng lúc.

---

### Phân trang (từ 2026-08-10)

Các endpoint danh sách **có thể phình to** trả về `PaginatedList<T>` thay vì mảng thuần:

```json
{ "items": [...], "pageNumber": 1, "pageSize": 20, "totalCount": 137,
  "totalPages": 7, "hasPreviousPage": false, "hasNextPage": true }
```

Query param: `?page=N&pageSize=M`. `pageSize` mặc định 20, **chặn cứng ở 100** — truyền lớn hơn
sẽ bị kẹp lại, không thể lách để tải toàn bộ bảng. `page < 1` tự về 1.

Đã áp dụng: `GET /jobs`, `GET /candidates/search`, `GET /notifications`,
`GET /jobs/{id}/applications`, `GET /organizations`, `GET /candidates/me/applications`,
`GET /candidates/me/saved-jobs`, `GET /organizations/{id}/jobs`.

**Cố ý KHÔNG phân trang** (danh sách nhỏ theo bản chất, thêm phân trang chỉ làm phức tạp):
`/catalog/*` (danh mục cố định), `/candidates/me/cvs`, `/organizations/mine`,
`/organizations/{id}/documents`, `/applications/{id}/notes`, `/applications/{id}/history`,
mọi hàng đợi `/ops/*` (xử lý xong là hết).

> ⚠️ Mọi query phân trang đều phải có **khóa sắp xếp phụ** (`ThenBy(Id)`). Sắp theo
> `PublishedAt`/`AppliedAt` đơn thuần không đủ: nhiều bản ghi cùng thời điểm (vd Vận hành duyệt
> hàng loạt) cho thứ tự bất định, khiến 1 bản ghi xuất hiện ở 2 trang hoặc biến mất.

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
| GET | `/users/me` | Owner | Thông tin tài khoản hiện tại (gồm `locale` hiện tại) |
| PATCH | `/users/me` | Owner | Đổi email/SĐT (yêu cầu xác thực lại) |
| PUT | `/users/me/locale` | Owner | Đổi ngôn ngữ giao diện đã lưu (`users.locale`) — đồng bộ lựa chọn giữa `web/` và `web-admin/` khi cùng 1 tài khoản, body `{ locale: "vi"\|"en"\|"ja"\|"zh"\|"ko"\|"es" }` |
| PUT | `/users/me/password` | Owner | Đổi mật khẩu khi đã đăng nhập — body `{ currentPassword, newPassword }`, yêu cầu `currentPassword` đúng. Khác `POST /auth/forgot-password`→`POST /auth/reset-password` (dùng OTP, không cần đăng nhập, cho trường hợp quên mật khẩu) |
| DELETE | `/users/me` | Owner | Yêu cầu xóa tài khoản (NĐ 13/2023 — soft delete + anonymize) |

---

## 3. Hồ sơ ứng viên — `/candidates`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/candidates/me` | Candidate (Owner) | Xem hồ sơ của chính mình |
| PUT | `/candidates/me` | Candidate (Owner) | Cập nhật thông tin chung |
| PUT | `/candidates/me/avatar` | Candidate (Owner) | Cập nhật ảnh đại diện — tách riêng khỏi `PUT /candidates/me` để đổi ảnh không phải gửi kèm mọi field hồ sơ khác. Body `{ avatarUrl }` (đã upload qua `POST /uploads/presigned-url`, `purpose=avatar`) |
| GET | `/candidates/{id}` | Employer (member, đã unlock) | Xem hồ sơ ứng viên khác — query param `organizationId` bắt buộc (dùng để check unlock đúng tổ chức, giống `/candidates/search`), 403 nếu chưa unlock (mục 7). Trả `EmployerCandidateProfileDto` (fullName/headline/summary/avatarUrl/contactEmail/specialties/licenses + **experiences/educations** — 2 khối sau thêm 2026-08-10, trước đó NTD trả 15 Credit mở hồ sơ mà không thấy học vấn lẫn kinh nghiệm) — DTO riêng, không dùng chung với `GET /candidates/me` vì khác nhu cầu field theo role (CLAUDE.md mục 4 quy tắc #9). **Chưa hỗ trợ Vận hành xem qua endpoint này** — khác thiết kế gốc, chưa làm |
| GET | `/candidates/{id}/public-summary` | Public | ⬜ **chưa implement** — bản rút gọn ẩn danh (vd. sau khi NTD xem trong kết quả tìm kiếm chưa mở) |
| POST | `/candidates/me/licenses` | Candidate (Owner) | Thêm CCHN + upload document |
| PUT | `/candidates/me/licenses/{licenseId}` | Candidate (Owner) | Sửa CCHN (chỉ khi `pending`/`rejected`) |
| DELETE | `/candidates/me/licenses/{licenseId}` | Candidate (Owner) | Xóa CCHN chưa duyệt |
| POST | `/candidates/me/specialties` | Candidate (Owner) | Gắn chuyên khoa + trình độ |
| PUT | `/candidates/me/history` | Candidate (Owner) | ✅ Lưu **toàn bộ** kinh nghiệm làm việc + học vấn trong 1 request. Body `{ experiences: [{ organizationName, position, tier?, fromDate, toDate?, description? }], educations: [{ schoolName, degree?, major?, fromYear, toYear? }] }`. **Ngữ nghĩa replace-all**: xóa hết bản ghi cũ rồi ghi lại danh sách gửi lên — thay cho 3 endpoint POST/PUT/DELETE từng dòng ở thiết kế gốc, vì UI là 1 form nhiều dòng bấm Lưu 1 lần; tách 3 loại request thì client phải tự theo dõi dòng nào mới/sửa/xóa và dễ lệch trạng thái nếu 1 request lỗi giữa chừng. `tier` là enum `FacilityTier` (`TrungUong`/`Tinh`/`Huyen`/`TuNhan`), `toDate`/`toYear` = `null` nghĩa là **đang làm/đang học**. Học vấn + kinh nghiệm cũng trả về trong `GET /candidates/me` và `GET /candidates/{id}` (sau unlock) |
| POST | `/candidates/me/certificates` | Candidate (Owner) | ⬜ **chưa implement** — thêm chứng chỉ CME |
| GET | `/candidates/me/cvs` | Candidate (Owner) | Danh sách CV (builder + upload) |
| POST | `/candidates/me/cvs` | Candidate (Owner) | Tạo CV mới (từ template hoặc upload) |
| PUT | `/candidates/me/cvs/{cvId}` | Candidate (Owner) | Sửa nội dung CV Builder |
| POST | `/candidates/me/cvs/file` | Candidate (Owner) | ✅ Lưu CV dạng **file** vào bảng `Cvs` để tái dùng nhiều lần ứng tuyển — thêm 2026-08-10. Body `{ title, fileUrl }` (file đã upload sẵn qua `POST /uploads/presigned-url`, `purpose=cv`). Là **INSERT**, khác `PUT .../cvs/builder` (upsert đúng 1 CV builder): ứng viên được có nhiều CV file (bản tiếng Việt/tiếng Anh, bản theo chuyên khoa). CV đầu tiên tự thành CV chính |
| POST | `/candidates/me/cvs/{cvId}/primary` | Candidate (Owner) | ✅ Đặt CV này làm CV chính (mặc định khi ứng tuyển) — thêm 2026-08-10. Bỏ cờ ở CV cũ + bật ở CV mới trong **cùng 1** transaction, tránh trạng thái 0 hoặc 2 CV chính |
| DELETE | `/candidates/me/cvs/{cvId}` | Candidate (Owner) | ✅ Xóa 1 CV — thêm 2026-08-10. Xóa **cứng** (CV là dữ liệu ứng viên tự tạo, không thuộc nhóm phải giữ vĩnh viễn như CCHN/audit log). Xóa CV chính thì tự chuyển cờ sang CV còn lại mới nhất. Đơn đã nộp **không** bị ảnh hưởng vì `applications.cv_file_url` giữ bản sao URL riêng |
| ~~POST~~ | ~~`/candidates/me/cvs/{cvId}/export`~~ | — | ⬜ **không làm** — xuất PDF chuyển sang render phía client bằng hộp thoại in của trình duyệt (`window.print()` + `@media print` ở `web/app/globals.css`), không cần endpoint. Lý do: backend render (QuestPDF) phải dựng lại layout CV lần 2 + nhúng font tiếng Việt, còn `jspdf`/`html2canvas` biến chữ thành ảnh (mờ khi in, không copy được text, tiếng Việt có dấu dễ lỗi) |

**Ví dụ response `GET /candidates/me`:**
```json
{
  "id": "uuid",
  "fullName": "Nguyễn Văn A",
  "headline": "Điều dưỡng ICU 5 năm kinh nghiệm",
  "avatarUrl": null,
  "dob": null,
  "gender": null,
  "address": null,
  "completionPct": 80,
  "specialties": [{ "code": "GAY_ME_HOI_SUC", "name": "Gây mê hồi sức", "level": "senior" }],
  "licenses": [
    {
      "id": "uuid",
      "licenseNo": "CCHN-001234",
      "issuedBy": "Sở Y tế TP.HCM",
      "scope": null,
      "issuedAt": "2020-01-01",
      "verifyStatus": "verified",
      "expiredAt": "2027-01-01",
      "rejectReason": null,
      "documentUrl": "https://...",
      "canEdit": false
    }
  ]
}
```
`canEdit` phản chiếu invariant `License.CanEdit` (Domain) — chỉ `true` khi `verifyStatus` là
`pending`/`rejected`. Frontend dùng field này để ẩn nút Sửa/Xóa, không tự suy luận lại logic.

---

## 4. Cơ sở y tế (Organization) — `/organizations`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/organizations?q=` | Public | Danh sách tổ chức **CÔNG KHAI** — chỉ `verifyStatus=Verified` (không lộ tổ chức `pending`/`rejected`/`suspended`). `q` filter theo tên (contains, không phân biệt hoa/thường), optional |
| POST | `/organizations` | Employer (đã đăng ký user) | Tạo hồ sơ tổ chức lần đầu → tạo `owner` member |
| GET | `/organizations/{id}` | Public | Trang công khai của cơ sở y tế. Response gồm cả `rejectReason` (nullable, chỉ có giá trị khi `verifyStatus = Rejected`) để NTD biết lý do bị từ chối xác thực — trước đây field này tồn tại ở entity nhưng chưa map ra DTO |
| PUT | `/organizations/{id}` | Employer (thành viên, không giới hạn chỉ owner — khớp quyền `POST .../documents` mục dưới) | Cập nhật `description`/`logoUrl`/`coverUrl`/`address`/`locationId`. **Không** cho sửa `name`/`orgType`/`licenseNo` qua endpoint này — thông tin định danh đã dùng để Vận hành xác thực tổ chức, sửa tự do sau khi `verified` rủi ro cho tính toàn vẹn xác thực |
| GET | `/organizations/{id}/documents` | Employer (**chỉ thành viên tổ chức**) | Danh sách giấy tờ pháp lý đã upload. **Sửa lỗ hổng 2026-08-10**: dòng này trước đây ghi `Public` và code cũng chỉ `RequireAuthorization()` (query không kiểm gì) — nên **bất kỳ user đăng nhập nào** biết `organizationId` đều tải được giấy phép doanh nghiệp của tổ chức khác, vi phạm CLAUDE.md mục 4 quy tắc #9. Vận hành **không** dùng endpoint này (họ xem qua `GET /ops/organizations`, đã lồng sẵn `documents` vào response) |
| POST | `/organizations/{id}/documents` | Employer (thành viên, không giới hạn chỉ owner) | Ghi nhận 1 document sau khi đã `PUT` file lên MinIO qua `POST /uploads/presigned-url` (`purpose=org_document`) — endpoint này chỉ lưu `fileUrl` trả về, không nhận file trực tiếp. Body `{ docType, fileUrl }`; **từ 2026-08-10 `docType` phải nằm trong danh sách** `GET /catalog/organization-document-types` (trước đó nhận chuỗi tự do nên 1 lỗi chính tả ở client tạo ra loại giấy tờ mới mà Vận hành không lọc/dịch được) |
| GET | `/organizations/{id}/members` | Employer (thành viên) | Danh sách thành viên + lời mời đang chờ (`PendingInvitations`) trong tổ chức |
| POST | `/organizations/{id}/members/invite` | Employer (thành viên) | Tạo `organization_invitations` (role `hr_manager`/`hr_member`, không mời thêm `owner`), gửi email chứa link token — hoạt động **kể cả khi email chưa có tài khoản** (driver hiện là giả lập nội bộ log token, giống OTP — xem `docs/nghiep-vu/TIEN-DO-DU-AN.md`) |
| POST | `/invitations/{token}/accept` | Đã đăng nhập, email khớp lời mời | Chấp nhận → tạo `employer_members` thật, set `accepted_at`. **Chưa làm** endpoint xem trước lời mời trước khi đăng nhập (`GET .../invitations/{token}`) — MVP yêu cầu đăng nhập/đăng ký trước rồi mới biết được lời mời hợp lệ hay không |
| DELETE | `/organizations/{id}/members/{memberId}` | Employer (thành viên, không tự xoá owner) | Xóa thành viên |
| GET | `/organizations/{id}/reviews` | Public | Đánh giá đã duyệt (`status=Approved`) — response **không** lộ `candidateUserId` (ẩn danh người viết) |
| POST | `/organizations/{id}/reviews` | Candidate | Gửi đánh giá `{ rating: 1-5, comment }` → tạo với `status=Pending`, chưa hiển thị công khai cho tới khi Vận hành duyệt (mục 11). Chặn gửi lần 2 cho cùng 1 tổ chức (unique index `organizationId+candidateUserId`, kiểm tra thêm ở Application layer để trả lỗi rõ ràng thay vì lỗi DB thô) |

---

## 5. Tin tuyển dụng — `/jobs`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/jobs` | Public | Tìm kiếm/lọc: `?specialty=&location=&organizationId=&employmentType=&salaryMin=&keyword=`. `organizationId` dùng để lấy danh sách tin **công khai** (chỉ `published`) của 1 tổ chức — khác `GET /organizations/{id}/jobs` (yêu cầu member, trả mọi trạng thái) |
| GET | `/jobs/{id}` | Public | Chi tiết tin (chỉ tin `published`, trừ khi là chủ sở hữu) |
| POST | `/jobs` | Employer (member) | Tạo tin (status = `draft`) |
| PUT | `/jobs/{id}` | Employer (member cùng org) | Sửa tin (chỉ khi `draft`/`rejected`) |
| POST | `/jobs/{id}/submit` | Employer (member) | Nộp duyệt kèm `packageId`. Gói trả phí → `status = pending_payment` (chờ mục 6); gói free → thẳng `pending` (chờ duyệt nội dung) |
| POST | `/jobs/{id}/close` | Employer (member) | Đóng tin sớm (`status → closed`) — **không khóa ATS**, `applications` của tin vẫn thao tác được bình thường (xem ERD mục 4.9) |
| POST | `/jobs/{id}/renew` | Employer (member cùng org) | **Gia hạn = tạo `jobs` row MỚI** sao chép nội dung từ tin này, `expires_at` mới; tin gốc chuyển `closed`. Không tái sử dụng `job_id` cũ (xem ERD mục 4.7) |
| GET | `/jobs/{id}/applications` | Employer (member cùng org) | Danh sách ứng viên đã nộp (ATS) — khả dụng bất kể `jobs.status` |
| GET | `/organizations/{id}/jobs` | Employer (member) | Danh sách tin của tổ chức (mọi trạng thái, gồm `pending_payment`/`suspended`) |

**Query tìm kiếm ví dụ:**
`GET /jobs?specialty=DIEU_DUONG&location=ha-noi&employmentType=truc_ca&salaryMin=8000000&page=1`

---

## 6. Gói tin & thanh toán — `/job-packages`, `/payments`

> ⚠️ **MVP dùng quy trình thủ công** (xem [ADR-0003](../kien-truc/adr/0003-hoan-cong-thanh-toan-tu-dong.md))
> — chưa có cổng thanh toán tự động. Endpoint dưới đây đã implement đúng luồng thủ công; các dòng
> đánh dấu 🔒 là **dự phòng cho tương lai**, chưa hoạt động ở MVP.

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/job-packages` | Public | Danh sách gói Eco/Pro/Max + giá |
| POST | `/payments/job-package` | Employer (member cùng org) | Gọi ngay sau `POST /jobs/{id}/submit` khi job đã ở `pending_payment` (mục 5) — tạo `payments` (`provider=manual_transfer`, `status=pending`) + `job_purchases` (liên kết `job_id`/`package_id`/`payment_id` ngay từ lúc này, không đợi tới confirm) + sinh `reference_code`. Response trả **thông tin chuyển khoản** (số tiền + `reference_code`) — **không** trả URL cổng thanh toán ở MVP. Chặn tạo giao dịch mới nếu tin đã có `payments.status=pending` khác |
| GET | `/payments/{id}` | Employer (member cùng org) | Tra cứu trạng thái giao dịch (`pending`/`success`/`failed`) |
| 🔒 POST | `/payments/webhook/{provider}` | Public (xác thực chữ ký) | Webhook callback cổng tự động — **chưa dùng ở MVP**, giữ chỗ endpoint/schema cho khi chọn cổng (xem `payments.provider` ở ERD) |

**Xác nhận thanh toán thủ công** (đội Vận hành, xem mục 11): `POST /ops/payments/{id}/confirm` — đối
soát sao kê ngân hàng theo `reference_code`, set `payments.status = success` + `confirmed_by`, chuyển
`jobs.status: pending_payment → pending` (vào hàng đợi duyệt nội dung) qua `Job.ConfirmPayment()`.
Nếu từ chối (sai số tiền/không nhận được) → `POST /ops/payments/{id}/reject` → `jobs.status → draft`
để NTD sửa lại và nộp lại qua `Job.RejectPayment()`.

---

## 7. Credit & mở hồ sơ ứng viên — `/credit`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/organizations/{id}/credit-wallet` | Employer (member) | Xem số dư |
| GET | `/organizations/{id}/credit-transactions` | Employer (member) | Lịch sử giao dịch |
| POST | `/payments/credit-topup` | Employer (member) | Tạo `payments` (`type=credit_topup`, `provider=manual_transfer`) như mục 6 — trả thông tin chuyển khoản + `reference_code`, **chưa cộng credit** cho tới khi Vận hành xác nhận. Quy đổi tạm thời MVP: 1 Credit = 1.000đ (chưa có bảng giá gói Credit riêng như `job_packages`). Chặn tạo giao dịch mới nếu tổ chức đã có `payments.type=credit_topup, status=pending` khác |
| GET | `/candidates/search` | Employer (member) | Tìm ứng viên theo chuyên khoa/khu vực — **kết quả ẩn liên hệ**. Query param: `organizationId` (Guid, **bắt buộc** — dùng để tính `isUnlocked` theo đúng tổ chức), `specialty` (Guid, optional), `location` (Guid, optional) — chú ý tên param **không** có hậu tố `Id`. Dùng lại cho "Ứng viên gợi ý" ở trang ATS 1 tin (`web-admin/features/applications/`, mục Giai đoạn 2) — frontend tự tra `specialtyId` từ `job.specialtyName` qua danh mục `catalog/specialties` (response `ApiJob` không có `specialtyId`) rồi gọi endpoint này, lọc client-side bỏ ứng viên đã có `applications` cho tin đó |
| POST | `/candidates/{id}/unlock` | Employer (member) | Trừ Credit, tạo `profile_unlocks`, trả hồ sơ đầy đủ |

**Xác nhận nạp Credit thủ công** (Vận hành): dùng chung `POST /ops/payments/{id}/confirm` (mục 6) —
khi `payments.type = credit_topup` thành công, cộng `credit_amount` (số Credit quy đổi từ `amount` lúc
tạo) vào `credit_wallets.balance` + ghi `credit_transactions` (`reason = purchase`, khác `reason =
bonus` của `POST /ops/organizations/{id}/credit-bonus` ở mục 11 — bonus là khuyến mãi/hỗ trợ thủ công,
không qua Payment) trong cùng transaction.

**Ví dụ gọi:** `GET /candidates/search?organizationId=<uuid>&specialty=<uuid>`

**Response khi chưa unlock (`GET /candidates/search`):**
```json
[
  { "id": "uuid", "headline": "Điều dưỡng ICU...", "isUnlocked": false, "unlockCost": 15,
    "contactEmail": null }
]
```
**Sau khi unlock** (candidate đã có `profile_unlocks` cho đúng `organizationId` đang search):
`isUnlocked: true`, `contactEmail` trả giá trị thật thay vì `null`. Trả về `List<>`, không phải object
đơn — mỗi phần tử là 1 kết quả tìm kiếm.

---

## 8. Ứng tuyển & ATS — `/applications`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| POST | `/jobs/{jobId}/applications` | Candidate | Ứng tuyển — body `{ coverLetter, cvFileUrl }`, cả 2 optional. `cvFileUrl` là CV riêng đã upload qua `POST /uploads/presigned-url` (`purpose=cv`) — nếu bỏ trống, NTD chỉ xem `cvSnapshot` (chụp từ hồ sơ nền tảng). Backend tự chụp `cv_snapshot` từ `candidate_profiles` tại thời điểm này và tính `score` ban đầu — cả hai **không đổi** dù ứng viên sửa hồ sơ sau đó (xem ERD mục 4.8). Chưa hỗ trợ chọn CV Builder (`cvId`/bảng `cvs`) — backlog riêng chưa chốt thiết kế |
| GET | `/candidates/me/applications` | Candidate (Owner) | Danh sách đơn đã nộp + trạng thái |
| GET | `/applications/{id}` | Candidate (Owner) / Employer (org liên quan) | Chi tiết 1 đơn |
| PATCH | `/applications/{id}/stage` | Employer (member) | Chuyển trạng thái pipeline (`stage`, `silent: bool`) |
| POST | `/applications/{id}/notes` | Employer (member) | Thêm ghi chú nội bộ |
| GET | `/applications/{id}/notes` | Employer (member) | Danh sách ghi chú nội bộ (kèm email tác giả), mới nhất trước — **không** cho Candidate xem (ghi chú nội bộ NTD) |
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
| PATCH | `/notifications/read-all` | Owner | Đánh dấu tất cả thông báo chưa đọc của user hiện tại thành đã đọc |

---

## 10. Danh mục dùng chung — `/catalog`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/catalog/specialties` | Public | Cây chuyên khoa — mỗi node trả `{ "name": "..." }` đã resolve theo `Accept-Language` (JOIN `specialty_translations`, lùi về tiếng Việt nếu thiếu) |
| GET | `/catalog/locations` | Public | Cây tỉnh/thành → quận/huyện — cùng cơ chế resolve theo locale |
| GET | `/catalog/employment-types` | Public | Danh sách loại hình làm việc — nhãn hiển thị lấy từ file dịch phía frontend (đây là enum cố định, không lưu bản dịch trong DB) |
| GET | `/catalog/organization-document-types` | Public | ✅ Danh sách loại giấy tờ pháp lý tổ chức (`operating_license`/`pharmacy_license`/`business_registration`/`other`) — thêm 2026-08-10. Trả tên mã, nhãn dịch nằm ở file dịch frontend (cùng cách với `employment-types`). Có endpoint riêng để UI không hardcode lại rồi lệch với validator của `POST /organizations/{id}/documents` |

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
| POST | `/ops/organizations/{id}/verify` | Admin/Moderator | Duyệt/từ chối/rút xác thực (`verified→rejected` hoặc `verified→suspended`). Chuyển khỏi `verified` sẽ **tự động** chuyển mọi `jobs.status=published` của tổ chức sang `suspended` trong cùng transaction (xem ERD mục 4.6) — không cần thao tác riêng cho từng tin |
| GET | `/ops/jobs?status=pending` | Admin/Moderator | Hàng đợi duyệt tin (không lẫn tin đang `pending_payment` — cột lọc riêng) |
| POST | `/ops/jobs/{id}/moderate` | Admin/Moderator | Duyệt/từ chối kèm lý do |
| GET | `/ops/payments` | Admin/Moderator | Hàng đợi giao dịch chờ đối soát (`status=pending`, cả `job_package` lẫn `credit_topup`) |
| POST | `/ops/payments/{id}/confirm` | Admin/Moderator | **Xác nhận đã nhận chuyển khoản** theo `reference_code` — set `payments.status=success` + `confirmed_by`; nếu `type=job_package` → `jobs.status: pending_payment→pending` qua `Job.ConfirmPayment()` (`job_purchases` đã tạo sẵn từ lúc `POST /payments/job-package`, mục 6); nếu `type=credit_topup` → cộng `credit_wallets.balance` (mục 6, 7) |
| POST | `/ops/payments/{id}/reject` | Admin/Moderator | Không nhận được/sai số tiền → `payments.status=failed`, `jobs.status: pending_payment→draft` để NTD sửa & nộp lại |
| GET | `/ops/organizations/{id}/refundable-unlocks` | Admin/Moderator | ✅ Danh sách lần mở hồ sơ của tổ chức để chọn khi hoàn Credit — thêm 2026-08-10. Trả `{ transactionId, creditCost, unlockedAt, isRefunded }`. Query **riêng** cho Vận hành vì `GET /organizations/{id}/credit-transactions` (mục 7) chặn người không phải thành viên tổ chức |
| POST | `/ops/organizations/{id}/credit-refund` | Admin | ✅ Hoàn Credit thủ công khi có tranh chấp — implement 2026-08-10. Body `{ transactionId, reason }`, ghi `credit_transactions` (`reason=refund`, `created_by`, `reference_id` = giao dịch gốc) + audit log `credit_refund`. Khác `credit-bonus`: **hoàn đúng số đã trừ** (không nhập tay), chỉ nhận giao dịch `unlock_profile`, và **mỗi giao dịch hoàn được 1 lần** (kiểm qua `reference_id`) |
| POST | `/reports` | Owner (bất kỳ user đăng nhập) | Tạo báo cáo vi phạm — `{ targetType, targetId, reason }` |
| GET | `/ops/reports` | Admin/Moderator | Danh sách báo cáo đang `pending` (không nhận query filter — MVP chỉ có 1 hàng đợi) |
| POST | `/ops/reports/{id}/resolve` | Admin/Moderator | Body: `{ action: "dismissed" \| "warned" \| "content_removed" \| "account_suspended", note }` — `dismissed` thêm ngoài 3 giá trị ERD gốc, dùng cho "Bỏ qua" (không có `resolution_action`, tách khỏi trạng thái `resolved`). `content_removed` khi `targetType=Job` gọi `Job.Close()` cùng transaction; các `targetType` khác (`Organization`/`Profile`/`Message`) hiện chỉ lưu action, chưa có state change tương ứng — giới hạn MVP đã xác nhận, xem `docs/nghiep-vu/TIEN-DO-DU-AN.md` |
| GET | `/ops/users?email=` | Admin | Tìm kiếm người dùng theo email (tối đa 50 kết quả) |
| POST | `/ops/users/{id}/suspend` | Admin | Khóa tài khoản (`status: active→suspended`) |
| POST | `/ops/users/{id}/unsuspend` | Admin | Mở khóa tài khoản (`status: suspended→active`) — bổ sung ngoài thiết kế gốc, cần thiết để đảo ngược `suspend` |
| CRUD | `/ops/catalog/specialties`, `/ops/catalog/locations` | Admin | Quản lý danh mục |
| CRUD | `/ops/job-packages` | Admin | Cấu hình gói tin/giá |
| GET | `/ops/audit-logs?action=` | Admin/Moderator | Tra cứu nhật ký kiểm toán — ghi tường minh trong Command Handler (không dùng EF interceptor tự động, không phân biệt được "thao tác nhạy cảm" với sửa field thường) cho 4/5 hành động nhạy cảm: duyệt/từ chối/rút xác thực tổ chức, duyệt/từ chối CCHN, xử lý báo cáo, khóa/mở khóa người dùng. **Chưa ghi** xóa tài khoản (đi qua `IIdentityService` trực tiếp, không qua Mediator Command — cần xử lý riêng, để lại Giai đoạn sau). `action` filter optional, khớp đúng chuỗi backend ghi (vd `organization_verify_verify`, `license_verify_approve`) |
| GET | `/ops/organizations/search?q=` | Admin/Moderator | Tìm tổ chức theo tên — **mọi trạng thái xác thực** (khác `GET /organizations` mục 4, chỉ trả `Verified`), kèm `creditBalance`, dùng cho màn "Cộng Credit thủ công". `q` rỗng → trả `[]`, không trả toàn bộ danh sách. Giới hạn 20 kết quả |
| GET | `/ops/organization-reviews` | Admin/Moderator | Hàng đợi đánh giá cơ sở y tế chờ duyệt (`status=Pending`), kèm tên tổ chức |
| POST | `/ops/organization-reviews/{id}/moderate` | Admin/Moderator | Body: `{ approved: bool }` — duyệt (`status→Approved`, hiển thị công khai) hoặc từ chối (`status→Rejected`, không bao giờ public). Ghi `audit_log_entries` (`review_moderate_approve`/`review_moderate_reject`) |
| GET | `/ops/dashboard/stats` | Admin/Moderator | Số liệu đếm tổng quan — tổng ứng viên/NTD theo role, tin theo `status`, tổ chức theo `verify_status`, số báo cáo/thanh toán đang chờ xử lý |

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
| 1.3 Đăng tin + mua gói | `POST /jobs` → `PUT /jobs/{id}` → `POST /jobs/{id}/submit` (kèm packageId, → `pending_payment` nếu trả phí) → `POST /payments/job-package` (chuyển khoản thủ công) → (Vận hành) `POST /ops/payments/{id}/confirm` (→ `pending`) → (Vận hành) `POST /ops/jobs/{id}/moderate` (→ `published`) |
| 1.4 Tìm & ứng tuyển | `GET /jobs` → `GET /jobs/{id}` → `POST /jobs/{jobId}/applications` |
| 1.5 ATS pipeline | `GET /jobs/{id}/applications` → `PATCH /applications/{id}/stage` → `POST /applications/{id}/notes` |
| 1.6 Credit mở hồ sơ | `GET /candidates/search` → `POST /payments/credit-topup` (nếu thiếu) → (Vận hành) `POST /ops/payments/{id}/confirm` → `POST /candidates/{id}/unlock` |
| 1.7 Kiểm duyệt (trang Vận hành) | `GET /ops/licenses|organizations|jobs|reports?status=pending` → `POST .../verify|moderate|resolve` |

---

## 14. Bước tiếp theo

→ Xem [`../frontend/wireframes/man-hinh-cot-loi.html`](../frontend/wireframes/man-hinh-cot-loi.html) —
wireframe trực quan cho 3 màn hình cốt lõi (Chi tiết tin, Hồ sơ CCHN, ATS Kanban), chú thích ánh xạ
trực tiếp tới các endpoint ở trên và các bảng trong [`../database/ERD-CHI-TIET.md`](../database/ERD-CHI-TIET.md).
Xem thêm [`../frontend/wireframes/trang-chu-3-vai-tro.html`](../frontend/wireframes/trang-chu-3-vai-tro.html)
— trang chủ/dashboard của cả 3 vai trò cạnh nhau để đối chiếu.
