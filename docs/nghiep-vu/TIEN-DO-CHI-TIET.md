# Tiến độ chi tiết theo chức năng

> Bản kiểm kê đầy đủ **mọi chức năng hệ thống** — không chia theo giai đoạn như
> [`DANH-SACH-TINH-NANG.md`](./DANH-SACH-TINH-NANG.md), mà chia theo **module nghiệp vụ**, mỗi dòng gắn
> endpoint/route/màn hình cụ thể + trạng thái + gap còn lại. Dùng để track % hoàn thiện từng module và
> tra cứu nhanh "chức năng X nằm ở đâu, đã nối thật chưa".
>
> **Quan hệ với tài liệu khác:**
> - [`DANH-SACH-TINH-NANG.md`](./DANH-SACH-TINH-NANG.md) — checklist theo giai đoạn (0-3), dùng để biết
>   *thứ tự làm* và *đã xong giai đoạn nào chưa*.
> - [`TIEN-DO-DU-AN.md`](./TIEN-DO-DU-AN.md) — nhật ký từng đợt làm việc (context, quyết định, bug đã
>   vá, verify đã chạy). Dùng để biết *tại sao* một chức năng được làm theo cách hiện tại.
> - File này — bản đồ *chi tiết nhất*, tra theo module, dùng để biết *chính xác cái gì đã nối, cái gì
>   còn mock, cái gì hoàn toàn chưa có*.
>
> **Cập nhật lần cuối:** 2026-08-06, dựa trên khảo sát trực tiếp mã nguồn (không dựa vào docs khác) —
> đọc toàn bộ `src/Web/Endpoints/*.cs`, `web/app/[locale]/**`, `web-admin/src/{routes,features,lib}/**`.
> Khi thêm/sửa chức năng, cập nhật ngay dòng tương ứng trong file này — đừng để lệch khỏi code thật.
>
> Trạng thái: ✅ Nối API thật · 🟨 Có UI nhưng còn mock/thiếu 1 phần · ⬜ Chưa có (backend lẫn UI) ·
> ⛔ Link/route chết

> ⚠️ **KHÔNG dùng file này làm nguồn quyết định làm gì tiếp.** Thứ tự ưu tiên do người dùng chốt nằm ở
> [`CLAUDE.md`](../../CLAUDE.md) mục **1b**: (1) hoàn thiện toàn bộ UI cả hệ thống (dữ liệu giả cũng
> được) → (2) API cho luồng nộp/nhận hồ sơ ứng viên. Thanh toán / email / SMS / OAuth / bộ đề y tế /
> nhắn tin **ngoài phạm vi** — các mục ⬜ thuộc nhóm đó trong file này chỉ để ghi nhận, **không phải
> việc cần làm**.

---

## 1. Auth & Định danh

| Chức năng | Backend | `web/` (Client) | `web-admin/` (Admin/Vận hành) |
|---|---|---|---|
| Đăng ký (chọn role candidate/employer) | ✅ `POST /auth/register` | ✅ `/auth/register` → `app/api/auth/register` proxy | — (không đăng ký ở đây) |
| Xác thực OTP | ✅ `POST /auth/verify-otp` (driver **giả lập nội bộ**, log code thay vì gửi email/SMS thật) | ✅ `/auth/verify-otp` | — |
| Đăng nhập | ✅ `POST /auth/login` | ✅ `/auth/login` → httpOnly cookie qua Route Handler | ✅ `/sign-in` → Zustand + cookie thường + axios interceptor tự refresh |
| Refresh token | ✅ `POST /auth/refresh` | ✅ tự động qua `backendFetch` khi gặp 401 | ✅ tự động qua axios response interceptor (single-flight) |
| Đăng xuất | ✅ `POST /auth/logout` | ✅ | ✅ |
| Quên/đặt lại mật khẩu | ✅ `POST /auth/forgot-password`, `POST /auth/reset-password` | ✅ `/auth/forgot-password`, `/auth/reset-password` | — |
| Đổi mật khẩu khi đã đăng nhập | ✅ `PUT /users/me/password` (yêu cầu đúng mật khẩu hiện tại, khác luồng OTP của forgot/reset) | ✅ `/settings` → `ChangePasswordForm` | ✅ `/settings/account` — nối thật 2026-08-10, lỗi "mật khẩu hiện tại không đúng" từ backend gắn vào đúng field |
| Xem thông tin user hiện tại | ✅ `GET /users/me` | ✅ dùng để gate route + hiện email ở header | ✅ dùng để check role cho phép vào (`ALLOWED_ROLES`) |
| Xóa tài khoản (soft-delete + anonymize) | ✅ `DELETE /users/me` | ✅ `/settings` → `DeleteAccountButton`, 2-click confirm | — (chưa có màn hình cho Vận hành khoá/xoá user khác) |
| OAuth Google | ⬜ | 🟨 nút trên `/auth/login` có loading state + thông báo "sắp ra mắt" (`OAuthButtons`), **vẫn chưa có logic OAuth thật** — chỉ polish UI theo quyết định "UI trước, API sau" | — |
| OAuth Zalo | ⬜ | 🟨 tương tự Google, chỉ UI polish | — |
| Chọn/ghi nhớ ngôn ngữ giao diện (lưu `users.locale`) | ✅ `PUT /users/me/locale` | ✅ `LanguageSwitcher` lưu thật vào `users.locale`, và **từ 2026-08-10** đăng nhập sẽ chuyển sang đúng ngôn ngữ đã lưu (`/api/auth/login` đọc `locale` từ `GET /users/me` rồi `router.push` kèm locale đó). Trước đây lưu nhưng **không chỗ nào đọc lại** nên `web/` định tuyến bằng tiền tố URL sẽ luôn ra tiếng Việt khi vào `/vi/...` | ✅ `LanguageSwitcher` (dropdown Globe ở Header, `react-i18next`) — toàn bộ 7 namespace (`common`/`jobs`/`applications`/`credit`/`candidates`/`members`/`ops`) đã rút chuỗi khỏi component, verify thật bằng Playwright trên cả 9 route chính. `vi`/`en` dịch tay thật, `ja`/`zh`/`ko`/`es` tạm placeholder tiếng Việt chờ dịch thuật (xem ADR-0010). Không lưu vào `users.locale`, chỉ `localStorage` |

---

## 2. Thành viên tổ chức (Employer members)

| Chức năng | Backend | `web-admin/` |
|---|---|---|
| Tạo tổ chức lần đầu (→ tạo owner member) | ✅ `POST /organizations` | ✅ (màn hình tạo tổ chức, luồng onboarding NTD) |
| Xem tổ chức của tôi | ✅ `GET /organizations/mine` | ✅ `useMyOrganization()` hook dùng khắp mọi trang NTD |
| Cập nhật thông tin tổ chức (description/logo/cover/address) | ✅ `PUT /organizations/{id}` (không cho sửa name/orgType/licenseNo) | ✅ `/organization` — `OrganizationInfoForm` (giới thiệu + địa chỉ) + `LogoSection` (đổi logo), 2 form riêng cùng gọi 1 endpoint, mỗi form giữ nguyên field của form khác khi lưu. Chưa có UI cho `coverUrl` (thứ yếu, ngoài phạm vi) |
| Danh sách thành viên + lời mời đang chờ | ✅ `GET /organizations/{id}/members` | ✅ `/users` ("Thành viên") |
| Mời thành viên (hr_manager/hr_member, kể cả email chưa có tài khoản) | ✅ `POST /organizations/{id}/members/invite` (driver gửi email **giả lập nội bộ**, log token) | ✅ dialog mời trong `/users` |
| Chấp nhận lời mời | ✅ `POST /invitations/{token}/accept` | ✅ `/accept-invitation` — màn hình nhập mã lời mời thủ công (chưa có email thật kèm link, xem log Giai đoạn 1) |
| Xoá thành viên (không xoá được owner) | ✅ `DELETE /organizations/{id}/members/{memberId}` | ✅ dialog xác nhận trong `/users` |
| Xem trước lời mời trước khi đăng nhập | ⬜ (đã ghi chú trong API-DESIGN.md là chưa làm, MVP chấp nhận UX kém hơn) | ⬜ |

---

## 3. Hồ sơ ứng viên

| Chức năng | Backend | `web/` |
|---|---|---|
| Xem hồ sơ của tôi (+ % hoàn thiện) | ✅ `GET /candidates/me` | ✅ `/dashboard`, `/profile` |
| Cập nhật hồ sơ cơ bản (họ tên, headline, summary, dob, gender, địa chỉ) | ✅ `PUT /candidates/me`, `GetMyProfileQuery` trả đủ `dob`/`gender`/`address` (trước đây chưa map ra DTO) | ✅ `/profile` → `ProfileForm` đủ input cho cả 6 field, qua `next-intl` đầy đủ — **bug đã sửa**: trước đây form chỉ có 2 input (fullName/headline), submit gửi cứng `dob/gender/address: null` khiến mỗi lần lưu xóa mất dữ liệu đã có nếu tồn tại từ trước |
| Thêm CCHN | ✅ `POST /candidates/me/licenses` | ✅ `/profile` → `LicenseSection` |
| Sửa CCHN | ✅ `PUT /candidates/me/licenses/{id}` (chỉ khi `canEdit`, DTO trả kèm field này) | ✅ `/profile` → `LicenseSection` — nút Sửa (bút chì, ẩn khi đã Verified) mở form pre-fill |
| Xoá CCHN | ✅ `DELETE /candidates/me/licenses/{id}` | ✅ `/profile` → `LicenseSection` — nút Xóa (có `window.confirm`), ẩn khi đã Verified |
| Thêm chuyên khoa + trình độ | ✅ `POST /candidates/me/specialties` | ✅ `/profile` → `SpecialtySection` mới (dropdown chuyên khoa qua `getSpecialties`, ẩn chuyên khoa đã có) |
| Upload ảnh CCHN thật (MinIO) | ✅ `POST /uploads/presigned-url` | ✅ `LicenseSection` dùng `lib/upload.ts` — input file thật, PUT thẳng MinIO |
| Đổi ảnh đại diện | ✅ `PUT /candidates/me/avatar` (command riêng, tách khỏi `PUT /candidates/me`) | ✅ `AvatarSection` trong `/profile` — input file ẩn qua nút "Đổi ảnh đại diện" |
| Học vấn / kinh nghiệm (`Experiences`/`Educations`) | ✅ `PUT /candidates/me/history` (replace-all) — thêm 2026-08-10, kèm enum `FacilityTier` (tuyến cơ sở) | ✅ `/profile` → khối "Kinh nghiệm làm việc" + "Học vấn", nhập nhiều dòng rồi lưu 1 lần |
| CV Builder | ✅ bảng `Cvs` + `PUT /candidates/me/cvs/builder` (upsert) + `GET /candidates/me/cvs` — thêm 2026-08-10 | ✅ `/profile/cv` lưu thật, tải lại trang dữ liệu vẫn còn, preview dùng tên/headline thật từ hồ sơ (trước đây `MOCK_CANDIDATE`). Trang đã gate đăng nhập |
| Xuất CV ra PDF | — (không cần endpoint) | ✅ `/profile/cv` → nút "Xuất PDF" gọi `window.print()`, CSS `@media print` ở `globals.css` chỉ để lại khối `#cv-print`. Chọn cách này thay `jspdf`/`html2canvas` vì thư viện đó biến chữ thành ảnh (mờ khi in, không copy được text, tiếng Việt có dấu dễ lỗi) và nặng ~1MB bundle. Bản in bổ sung email liên hệ/địa chỉ/tiểu sử/chuyên khoa/CCHN — trước đó khối xem trước chỉ có tên + học vấn/kinh nghiệm/kỹ năng. **Chỉ in CCHN đã Verified** (CCHN chờ duyệt in ra sẽ gây nhầm lẫn vì trên giấy không thấy trạng thái) |
| Chọn CV đã lưu khi ứng tuyển | ✅ cột `applications.cv_id` + kiểm tra CV thuộc về chính ứng viên — thêm 2026-08-10. **Sửa lỗ hổng 2026-08-10**: chọn CV dạng file đã lưu thì `cv_file_url` vẫn null nên NTD mở đơn KHÔNG thấy link CV nào (NTD đọc `ApplicationDto.CvFileUrl`, không đọc bảng `cvs`) — giờ sao chép `FileUrl` sang đơn lúc nộp | ✅ dialog ứng tuyển có ô "Dùng CV đã lưu" bên cạnh upload file riêng. Trước đây phải upload lại file MỖI LẦN ứng tuyển vì không có CV nào lưu thật |
| CV upload file (lưu vào bảng `Cvs`) | ✅ `POST /candidates/me/cvs/file` + `POST .../{cvId}/primary` + `DELETE .../{cvId}` — thêm 2026-08-10 | ✅ `/profile/cv` → khối "CV dạng file": upload nhiều CV lên MinIO, đặt CV chính, xóa. Trước đây CV file chỉ tồn tại trong 1 đơn (`applications.cv_file_url`) nên mỗi lần ứng tuyển phải upload lại đúng file đó |
| Xác thực SĐT | ⬜ (`phone_verified_at` field tồn tại nhưng không có luồng verify) | ⬜ |
| Hàng đợi duyệt CCHN (Vận hành) | ✅ `GET /ops/licenses`, `POST /ops/licenses/{id}/verify` | (xem mục Vận hành) |

---

## 4. Cơ sở y tế (Organization)

| Chức năng | Backend | `web/` | `web-admin/` |
|---|---|---|---|
| Trang công khai 1 tổ chức | ✅ `GET /organizations/{id}` | ✅ `/organizations/[id]` | — |
| Danh sách tổ chức công khai (list) | ✅ `GET /organizations?q=` (chỉ `Verified`, filter tên optional) — thêm 2026-08-06 | ✅ `/organizations` nối API thật, không còn suy ra từ `getJobs()` — hiện được cả tổ chức chưa có tin nào | — |
| Trang chủ hiện tổ chức nổi bật | — | ✅ vẫn suy ra từ `getJobs()` (dedupe theo `organizationId`, sắp theo số tin đang có) — khác trang `/organizations` đã dùng API list riêng, trang chủ giữ cách cũ vì mục đích khác (nổi bật theo hoạt động, không phải danh sách đầy đủ) | — |
| Ops tìm tổ chức theo tên (mọi trạng thái) | ✅ `GET /ops/organizations/search?q=` — thêm 2026-08-06, khác endpoint public (trả cả `pending`/`rejected`/`suspended`, kèm `creditBalance`) | — | ✅ `/ops/credit` dùng để tìm tổ chức trước khi cộng Credit |
| Upload giấy phép hoạt động | ✅ `GET/POST /organizations/{id}/documents` | — | ✅ `/organization` → khối "Giấy phép hoạt động" — upload thật lên MinIO qua `uploadFile(file, 'org_document')`, hiện danh sách file đã tải kèm link xem. Dòng ghi "⬜ chưa nối MinIO" trước đây **đã lỗi thời**, xác nhận lại qua code 2026-08-10. **Hoàn thiện 2026-08-10**: cho chọn 1 trong 4 loại giấy tờ (`GET /catalog/organization-document-types`), backend chốt danh sách bằng validator; toàn bộ màn hình đã qua i18n 6 ngôn ngữ (25 chuỗi hardcode, kể cả dropdown Loại hình/Quy mô trước đây hiện **enum thô** `BenhVienCong`/`Under50` cho người dùng thật). Cũng **bịt lỗ hổng**: `GET /organizations/{id}/documents` trước đây bất kỳ user đăng nhập nào biết `organizationId` đều tải được giấy phép doanh nghiệp của tổ chức khác |
| Duyệt tổ chức (Verify/Reject/Suspend) | ✅ `POST /ops/organizations/{id}/verify` — Suspend tự động ẩn mọi tin `published` cùng transaction | — | ✅ `/ops/verification` (tab Tổ chức) |
| Cộng Credit thủ công (khuyến mãi/hỗ trợ) | ✅ `POST /ops/organizations/{id}/credit-bonus` | — | ✅ `/ops/credit` (`features/ops-credit/`) — nối API thật (tìm tổ chức + cộng Credit), không còn `MOCK_ORGANIZATIONS` |
| Hoàn Credit thủ công khi tranh chấp | ✅ `POST /ops/organizations/{id}/credit-refund` + `GET .../refundable-unlocks` — thêm 2026-08-10 | — | ✅ `/ops/credit` → khối "Hoàn Credit khi tranh chấp": chọn lần mở hồ sơ, nhập lý do (bắt buộc), lần đã hoàn hiện "Đã hoàn" và ẩn nút |
| Đánh giá cơ sở y tế (review, có kiểm duyệt) | ✅ bảng `OrganizationReviews` + `GET/POST /organizations/{id}/reviews` (candidate) + `GET /ops/organization-reviews` + `POST .../moderate` (Vận hành) — thêm 2026-08-06 | ✅ `/organizations/[id]` — xem đánh giá đã duyệt (ẩn danh người viết) + form gửi đánh giá mới, nối API thật thay `MOCK_APPROVED_REVIEWS`/`localStorage` | ✅ `/ops/reviews` (`features/ops-reviews/`) — hàng đợi duyệt/từ chối, **UI mới hoàn toàn**, chưa từng có kể cả dạng mock |
| Nhật ký kiểm toán (Audit log) | ✅ bảng `AuditLogEntries` + `GET /ops/audit-logs?action=` — ghi tường minh 4/5 hành động nhạy cảm (duyệt CCHN/tổ chức, xử lý báo cáo, khóa/mở khóa user; xóa tài khoản để lại sau) — thêm 2026-08-06 | — | ✅ `/ops/audit` (`features/ops-audit/`) — nối API thật thay mock, filter theo loại thao tác |

---

## 5. Tin tuyển dụng (Jobs)

| Chức năng | Backend | `web/` | `web-admin/` |
|---|---|---|---|
| Tìm kiếm/lọc tin công khai | ✅ `GET /jobs` (LINQ/EF Core, chưa chuyển Postgres full-text) | ✅ `/jobs` — filter qua URL query, giữ SSR + form tìm theo từ khóa (`keyword`, đã có sẵn ở backend/`getJobs`, trước đây chỉ thiếu UI); form tìm nhanh ở trang chủ (`HeroSearchForm`) điều hướng thẳng sang `/jobs?keyword=...` | — |
| Xem chi tiết tin | ✅ `GET /jobs/{id}` | ✅ `/jobs/[id]` | ✅ (trong danh sách tin của tổ chức) |
| Tạo tin (draft) | ✅ `POST /jobs` | — | ✅ `/jobs/new` |
| Sửa tin (chỉ draft/rejected) | ✅ `PUT /jobs/{id}` | — | ✅ `/jobs/$jobId/edit` — nút bút chì chỉ hiện khi `Draft`/`Rejected` |
| Nộp duyệt (gói Free/Eco/Pro/Max) | ✅ `POST /jobs/{id}/submit` | — | ✅ `/jobs/new` (Free thẳng pending; trả phí → tạo Payment) |
| Đóng tin sớm | ✅ `POST /jobs/{id}/close` | — | ✅ nút trong danh sách tin (`/jobs`, có xác nhận trước khi gọi) |
| Gia hạn tin (tạo `jobs` row mới) | ✅ `POST /jobs/{id}/renew` | — | ✅ nút trong danh sách tin (`/jobs`, hiện khi `Closed`/`Expired`/`Suspended`) |
| Danh sách tin của tổ chức (mọi trạng thái) | ✅ `GET /organizations/{id}/jobs` | — | ✅ `/jobs` |
| Hàng đợi duyệt nội dung (Vận hành) | ✅ `GET /ops/jobs`, `POST /ops/jobs/{id}/moderate` | — | ✅ `/ops/verification` (tab Tin tuyển dụng) |
| Trang chủ hiện tin nổi bật | — | ✅ 6 tin đầu từ `getJobs()` | — |

---

## 6. Ứng tuyển & ATS

| Chức năng | Backend | `web/` | `web-admin/` |
|---|---|---|---|
| Ứng tuyển (CV nền tảng, chặn trùng + tin chưa published) | ✅ `POST /jobs/{id}/applications` | ✅ `ApplyButton` trên `/jobs/[id]` | — |
| Ứng tuyển bằng CV upload riêng | ✅ `applications.cv_file_url` — field `cvFileUrl` optional trong `POST .../applications`, không đụng bảng `cvs`/CV Builder (backlog riêng) | ✅ dialog ứng tuyển (`ApplyButton`) có input file tùy chọn | ✅ hiện link CV trong trang chi tiết ATS + icon báo trên Kanban card |
| Lưu tin / bỏ lưu (bookmark) | ✅ bảng `SavedJobs` + `POST /candidates/me/saved-jobs/{jobId}/toggle` + `GET /candidates/me/saved-jobs` — thêm 2026-08-10, **không có trong ERD gốc** | ✅ nút "Lưu việc này" ở trang tin (optimistic update) + trang `/dashboard/saved-jobs`. Trước đây lưu ở `localStorage` nên mất khi đổi máy/xóa cache; danh sách phải gọi `getJobById` từng ID, giờ backend trả sẵn đủ thông tin | — |
| Xem đơn ứng tuyển của tôi (ứng viên) | ✅ `GET /candidates/me/applications` | ✅ `/dashboard/applications` — trang riêng, tách nhóm "Đang xử lý" / "Đã kết thúc" kèm số đếm, hiện ngày nộp + lý do từ chối (`rejectedReason` có sẵn trong DTO nhưng trước đây chưa dùng ở đâu). Thêm 2026-08-10; `/dashboard` giữ 5 đơn gần nhất + link "Xem tất cả" | — |
| Danh sách ứng viên theo tin (ATS) | ✅ `GET /jobs/{id}/applications` | — | ✅ `/applications/$jobId` (Kanban kéo-thả) |
| Chuyển giai đoạn ATS + lưu lịch sử | ✅ `PATCH /applications/{id}/stage` | — | ✅ kéo-thả trong Kanban |
| Xem chi tiết 1 đơn | ✅ `GET /applications/{id}` | — | ✅ `/applications/$jobId/$applicationId` (link từ Kanban card) |
| Ghi chú nội bộ trên đơn | ✅ `POST /applications/{id}/notes`, `GET /applications/{id}/notes` (mới, danh sách kèm email tác giả) | — | ✅ trang chi tiết — thêm ghi chú + xem danh sách |
| Chấm điểm ghi đè thủ công | ✅ `PATCH /applications/{id}/score` | — | ✅ trang chi tiết — form nhập điểm 0-100 |
| Xem lịch sử chuyển giai đoạn | ✅ `GET /applications/{id}/history` | — | ✅ trang chi tiết — danh sách chuyển giai đoạn kèm thời gian |

---

## 7. Credit & Tìm ứng viên chủ động

| Chức năng | Backend | `web-admin/` |
|---|---|---|
| Xem ví Credit + lịch sử giao dịch | ✅ `GET /organizations/{id}/credit-wallet`, `.../credit-transactions` | ✅ `/credit` |
| Tìm ứng viên (ẩn liên hệ tới khi mở) | ✅ `GET /candidates/search` | ✅ `/candidates` |
| Mở hồ sơ ứng viên (trừ Credit) | ✅ `POST /candidates/{id}/unlock` | ✅ nút "Mở hồ sơ" trong `/candidates` |
| Xem chi tiết hồ sơ sau khi mở | ✅ `GET /candidates/{id}` (query `organizationId`, 403 nếu chưa unlock, trả `EmployerCandidateProfileDto` — **từ 2026-08-10 có kèm học vấn/kinh nghiệm**) | ✅ `/candidates/$candidateId` — tên/headline/summary/avatar/email/chuyên khoa/CCHN + **học vấn & kinh nghiệm làm việc** (trước đây thiếu 2 khối này dù NTD đã trả Credit) |
| Gợi ý ứng viên cho 1 tin (chuyên khoa trùng) | ✅ dùng lại `GET /candidates/search` (không có endpoint mới) — thêm 2026-08-07 | ✅ nút "Ứng viên gợi ý" ở trang ATS 1 tin (`/applications/$jobId`), mở Sheet, tự tra `specialtyId` qua danh mục vì `ApiJob` không có field này, loại ứng viên đã ứng tuyển, unlock ngay trong Sheet |

---

## 8. Thanh toán (Payments)

| Chức năng | Backend | `web-admin/` |
|---|---|---|
| Tạo giao dịch mua gói tin (job-package) | ✅ `POST /payments/job-package` — chặn trùng nếu tin đã có giao dịch pending | ✅ `/jobs/new` — dialog hiện mã tham chiếu sau khi chọn gói trả phí |
| Tạo giao dịch nạp Credit | ✅ `POST /payments/credit-topup` (quy đổi tạm 1 Credit = 1.000đ) — chặn trùng theo tổ chức | ✅ `/credit` — dialog "Nạp thêm Credit" |
| Xem trạng thái 1 giao dịch | ✅ `GET /payments/{id}` | ✅ `/job-packages` — ô tra cứu theo mã giao dịch, hiện loại/số tiền/mã tham chiếu/trạng thái đối soát (thêm 2026-08-10, trước đó endpoint có sẵn nhưng không UI nào gọi) |
| So sánh gói đăng tin (Eco/Pro/Max) | ✅ `GET /catalog/job-packages` (trả kèm `perks`, `maxActiveJobs`) | ✅ `/job-packages` — bảng so sánh đủ giá/thời hạn/số tin tối đa/từng quyền lợi dạng ✓/— (thêm 2026-08-10). Trước đó gói chỉ hiện dạng radio list gọn trong form tạo tin, không hiện `perks` lẫn `maxActiveJobs` nên NTD không so sánh được. **Không có nút "Mua" rời** — `POST /jobs/{id}/submit` cần `jobId` nên gói chỉ chọn được trong luồng đăng tin |
| Hàng đợi đối soát (Vận hành) | ✅ `GET /ops/payments` | ✅ `/ops/payments` |
| Xác nhận/từ chối giao dịch (Vận hành) | ✅ `POST /ops/payments/{id}/confirm`, `.../reject` | ✅ nút Xác nhận/Không khớp trong `/ops/payments` |
| Cổng thanh toán tự động (VNPay/Momo/ZaloPay) | ⬜ hoãn theo ADR-0003, chỉ có `manual_transfer` | ⬜ |

---

## 9. Danh mục dùng chung (Catalog)

| Chức năng | Backend | `web/` | `web-admin/` |
|---|---|---|---|
| Đọc chuyên khoa/địa điểm/gói tin/loại hình việc làm | ✅ `GET /catalog/*` (Public) | ✅ dùng cho filter `/jobs`, form ứng tuyển | ✅ dùng cho filter/form khắp nơi |
| Tạo/sửa chuyên khoa | ✅ `POST/PUT /ops/catalog/specialties` | — | ✅ `/ops/catalog` (dialog, cây cha-con) |
| Tạo/sửa địa điểm | ✅ `POST/PUT /ops/catalog/locations` | — | ✅ `/ops/catalog` |
| Tạo/sửa gói tin | ✅ `POST/PUT /ops/job-packages` | — | ✅ `/ops/catalog` (tier chỉ chọn lúc tạo) |
| Xóa/vô hiệu hóa danh mục | ⬜ backend chưa có Delete/Deactivate cho cả 3 entity | — | ⬜ (cố ý không có nút xóa giả) |

---

## 10. Vận hành nền tảng (Ops) — tổng hợp riêng

| Chức năng | Backend | `web-admin/` |
|---|---|---|
| Duyệt CCHN | ✅ `GET /ops/licenses`, `POST /ops/licenses/{id}/verify` | ✅ `/ops/verification` (tab CCHN) |
| Duyệt tổ chức | ✅ `GET /ops/organizations`, `POST /ops/organizations/{id}/verify` | ✅ `/ops/verification` (tab Tổ chức) |
| Duyệt nội dung tin | ✅ `GET /ops/jobs`, `POST /ops/jobs/{id}/moderate` | ✅ `/ops/verification` (tab Tin tuyển dụng) |
| Đối soát thanh toán | ✅ `GET/POST /ops/payments/*` | ✅ `/ops/payments` |
| Quản lý danh mục/gói tin | ✅ `POST/PUT /ops/catalog/*`, `/ops/job-packages/*` | ✅ `/ops/catalog` |
| Cộng Credit thủ công | ✅ `POST /ops/organizations/{id}/credit-bonus` (role `admin`, không phải `moderator`) | ✅ `/ops/credit` — **tìm tổ chức dùng mock tạm** (chưa có API tìm/liệt kê tất cả tổ chức cho Vận hành), form cộng Credit gọi API thật |
| Hoàn Credit khi tranh chấp | ✅ hoàn **đúng số đã trừ** (không nhập tay), chỉ nhận giao dịch `unlock_profile`, mỗi giao dịch hoàn 1 lần, ghi audit log `credit_refund` kèm lý do — thêm 2026-08-10 | ✅ `/ops/credit` |
| Xử lý báo cáo vi phạm | ✅ `GET /ops/reports`, `POST /ops/reports/{id}/resolve` | ✅ `/ops/reports` — nối API thật, không còn mock |
| Quản lý người dùng hệ thống (khoá/mở khoá) | ✅ `GET /ops/users`, `POST /ops/users/{id}/{suspend,unsuspend}` | ✅ `/ops/users` — tìm theo email + nút Khóa/Mở khóa |
| Dashboard tổng quan Vận hành | ✅ `GET /ops/dashboard/stats` (đếm đơn giản, không có xu hướng theo thời gian) | ✅ `/ops/dashboard` |
| Sidebar tách theo role (Admin vs Vận hành) | — | ✅ `getSidebarData(role)` — employer chỉ thấy `employerGroup`, admin/moderator chỉ thấy `opsGroup`. Nhóm `Pages` (Auth/Errors demo của template) đã gỡ hẳn 2026-08-10; nhóm `Other` (Cài đặt, Trợ giúp) giữ lại cho mọi role và đã chuyển sang khóa i18n |

---

## 11. Thông báo, tin nhắn & hạ tầng UI khác

| Chức năng | Backend | `web/` | `web-admin/` |
|---|---|---|---|
| Thông báo trong ứng dụng (`Channel = InApp`) | ✅ `GET /notifications` (`?unreadOnly`), `PATCH .../read`, `PATCH .../read-all` | ✅ `NotificationBell` (dropdown, poll 30s) + trang `/notifications` (lịch sử đầy đủ + đánh dấu tất cả đã đọc) | ✅ tương tự — `NotificationBell` + route `/notifications` |
| Thông báo email | ⬜ | ⬜ | ⬜ |
| Nhắn tin NTD ↔ ứng viên | ⬜ | ⬜ | ⬜ chưa làm — trang `/chats` template (đọc `data/convo.json` tĩnh) đã gỡ 2026-08-10 cùng mục "Tin nhắn" trong sidebar (mục đó có badge "3" tin nhắn giả, dễ nhầm là tính năng thật) |
| Trang `/tasks`, `/apps` (template mẫu Jira/App Store) | — | — | ✅ đã gỡ hẳn 2026-08-10 cùng dependency `@faker-js/faker` |
| Trang Settings cá nhân | — | (xem mục 1) | ✅ **làm thật 2026-08-10**. Còn 3 màn: **Hồ sơ** (email/vai trò/trạng thái/xác thực email từ `GET /users/me` + tên tổ chức — chỉ đọc vì `AuthUserDto` không có username/bio/avatar để sửa), **Tài khoản** (đổi mật khẩu thật), **Giao diện** (theme/font — thật, lưu localStorage). **Đã xóa 2 màn**: `notifications` (chưa có provider email/SMS nào nên chỉ là công tắc không nối gì) và `display` (demo chọn thư mục macOS Finder) |
| Link `/about` (giới thiệu nền tảng) | — | ✅ trang tĩnh `/about` — 3 giá trị cốt lõi (tin cậy lâm sàng/đúng chuyên khoa/cộng đồng y tế), không gọi API | — |
| Help Center (FAQ + hướng dẫn liên hệ hỗ trợ) | — | — | ✅ `/help-center` — nội dung tĩnh (4 câu hỏi thường gặp: xác thực tổ chức, mời thành viên, Credit, sửa tin), thay `<ComingSoon />` cũ. Không gọi API |

---

## Tổng hợp nhanh: nợ kỹ thuật lộ diện qua đợt khảo sát này

Những mục này **không nằm trong checklist cũ**, phát hiện khi rà soát code thật — cần dọn hoặc quyết định rõ ràng (làm tiếp hay bỏ):

> **Lưu ý:** mục này dễ lỗi thời nhất trong toàn file — mỗi lần rà lại (2026-08-06 xác nhận qua Explore
> agent đọc trực tiếp code, không dựa vào bản ghi cũ) đã phát hiện phần lớn các dòng dưới đây **đã được
> sửa từ các đợt làm việc trước nhưng chưa gạch bỏ ở đây**. Trước khi bắt tay làm 1 mục trong danh sách
> này, luôn verify lại bằng code thật — đừng tin danh sách này.

1. ~~**Link chết trên `web/`**: `/organizations` và `/about` 404~~ — **đã sửa** (2026-08-06): cả 2 route đã có nội dung thật, không còn 404.
2. ~~**`web/lib/mock-data.ts` có 8/11 export chết**~~ — **đã sửa** (2026-08-06): xóa `MOCK_JOBS`, `getJobById`, `getOrganizationById`, `getJobsByOrganization`, `MOCK_SPECIALTIES`, `MOCK_LOCATIONS`, `MOCK_EMPLOYMENT_TYPES`, `MOCK_APPLICATIONS`. Chỉ còn `MOCK_CANDIDATE`/`MOCK_CV` (đang dùng thật ở CV Builder).
3. ~~**Route Handler tồn tại nhưng chưa có UI gọi ở `web/`: sửa/xoá CCHN, thêm chuyên khoa**~~ — **lỗi thời, đã có UI từ trước** (xác nhận 2026-08-06): `license-section.tsx` có đủ nút Sửa/Xóa gọi đúng `PUT`/`DELETE`; `specialty-section.tsx` có form thêm chuyên khoa gọi đúng `POST`. Xem mục 3 "Hồ sơ ứng viên" ở trên — đã ghi ✅ từ trước, dòng này trong "Tổng hợp nhanh" chỉ là bản ghi cũ chưa xóa.
4. ~~**`applicationsApi` có hàm chưa được UI gọi ở `web-admin/`: `getById`/`addNote`/`score`/`getHistory`**~~ — **lỗi thời, đã có UI từ trước** (xác nhận 2026-08-06): `features/applications/detail.tsx` gọi đủ cả 4 hàm, có input điểm số + nút thêm ghi chú thật. Xem mục 6 "Ứng tuyển & ATS" ở trên — đã ghi ✅ từ trước.
5. ~~**Chuỗi tiếng Việt hardcode ở `web/`**~~ — **lỗi thời, đã dọn từ trước** (xác nhận 2026-08-06, quét cả 8 file được nêu bằng regex Unicode tiếng Việt): không còn chuỗi hardcode hiển thị UI nào trong `profile-form.tsx`/`profile/page.tsx`/`license-section.tsx`/`settings/page.tsx`/`delete-account-button.tsx`/`dashboard/page.tsx`/home `page.tsx`/`jobs/[id]/page.tsx`. `formatSalary` đã dùng `t("salaryNegotiable")`/`t("million")` qua tham số.
6. **`web-admin/` sidebar tách theo role** — đã làm ở đợt trước (`getSidebarData(role)`, xem mục 10 Vận hành và log tương ứng trong `TIEN-DO-DU-AN.md`) — dòng này trong danh sách cũ đã lỗi thời, cần xóa hẳn ở lần dọn tiếp theo.
7. **Vận hành — cộng Credit thủ công**: đã có UI (`/ops/credit`, dùng mock tìm tổ chức — xem mục 4 "Cơ sở y tế" ở trên). Quản lý người dùng hệ thống + dashboard tổng quan: cần verify lại trực tiếp, danh sách cũ có thể đã lỗi thời như các mục trên.
8. ~~**`/tasks`, `/apps`, `/chats`, trang Settings cá nhân** là tàn dư template, không nằm trong sidebar chức năng thật nên người dùng không vô tình vào được — giữ nguyên, không gỡ.~~ **Đánh giá này SAI và đã xử lý (2026-08-10).** Kiểm tra lại code cho thấy: `/chats` **có** nằm trong sidebar nghiệp vụ (`employerGroup`, mục `nav.chats` kèm badge "3" tin nhắn giả), và nhóm `Pages` (Auth demo + Errors demo) hiện với **mọi** role vì `getSidebarData` chỉ lọc `employerGroup`/`opsGroup`. Tức là NTD/Vận hành thật đăng nhập vào đều thấy menu "Tasks", "Apps", "Tin nhắn", "Sign In (2 Col)", "401/403/404" — rất lộ là template chưa dọn. Đã gỡ hẳn `/tasks`, `/apps`, `/chats`, các route auth trùng lặp (`sign-in-2`/`sign-up`/`forgot-password`/`otp` — đăng ký/quên mật khẩu thật đã có ở `web/`), route demo trang lỗi, `coming-soon.tsx` và dependency `@faker-js/faker`. **Giữ lại** `features/errors/*` (dùng thật làm `errorComponent` ở `__root.tsx`) và trang Settings cá nhân (là màn hình có thật trong tài liệu). Trang Settings **đã làm thật cùng ngày** — xem điểm 10.
9. **Trạng thái đang tải / lỗi ở `web-admin/`** (2026-08-10): trước đây phần lớn màn chỉ xử lý trạng thái RỖNG, không có loading/error — API chậm hoặc lỗi thì người dùng thấy màn hình trắng trơn, hoặc thấy y hệt "không có dữ liệu" nên không phân biệt được "chưa có gì" với "gọi API hỏng". Đã bổ sung cho 6 màn còn thiếu (Dashboard NTD, Ops Dashboard, Đối soát thanh toán, Duyệt đánh giá, Quản lý người dùng, Xử lý báo cáo) qua component dùng chung `components/query-state.tsx`; 2 dashboard dùng skeleton dạng thẻ để giữ layout lưới. Các màn đã có sẵn từ trước: `jobs`, `candidates`, `credit`, `users` (Thành viên), `ops-audit`.
10. **Trang Cài đặt cá nhân `web-admin/` làm thật** (2026-08-10): 5 màn Settings đều là template
    shadcn-admin — tiếng Anh, submit chỉ gọi `showSubmittedData` (hiện toast JSON, không lưu gì),
    email giả `m@example.com`, mục "friend requests/follows", chọn 9 ngôn ngữ không khớp 6 ngôn ngữ
    của dự án. **Xóa 2 màn** (`notifications` — chưa có provider email/SMS nào nên là hứa hẹn tính
    năng không tồn tại; `display` — demo chọn thư mục macOS Finder), **viết lại 3 màn** bằng dữ liệu
    thật.

    Rà kèm phát hiện **5 lỗi ảnh hưởng MỌI trang**, đều đã sửa cùng lượt:
    - `auth.user` chỉ set lúc đăng nhập, **không bao giờ restore** → sau F5 chỉ còn token, `user`
      là `null`, `app-sidebar` mất `role` nên hiện **cả nhóm Vận hành cho Nhà tuyển dụng**
      (`getSidebarData(undefined)` trả cả 2 nhóm). Thêm hook `useCurrentUser`.
    - `lib/http.ts` cố định `Accept-Language: 'vi'` (comment dẫn ADR-0008 mục 5 — **đã lỗi thời**,
      ADR-0010 đảo lại thành 6 ngôn ngữ) → mọi thông báo lỗi từ backend về tiếng Việt dù UI đang
      tiếng Anh.
    - Chân sidebar hiện `Người dùng demo / demo@blousehiding.vn`, `ProfileDropdown` hiện
      `satnaing / satnaingdev@gmail.com` kèm mục `Billing`/`New Team` không tồn tại.
    - `TeamSwitcher` có dropdown đổi team + "Add team" — hệ thống **không có** khái niệm nhiều team
      (mỗi NTD thuộc đúng 1 tổ chức). Thay bằng `AppBrand` hiện tên tổ chức thật.
    - Nút tìm kiếm + command palette (⌘K) còn tiếng Anh: `Search`, `Type a command or search...`,
      `No results found.`, `Theme/Light/Dark/System`.

    Test 84 → 94, thêm `src/i18n.test.ts` kiểm **mọi** namespace có đủ khóa ở cả 6 ngôn ngữ (quy tắc
    #10 CLAUDE.md) — thiếu khóa thì i18next lặng lẽ lùi về `fallbackLng`, không ai thấy.

---

## Cách dùng file này khi thêm tính năng mới

Theo đúng CLAUDE.md mục 5 — khi hoàn thành 1 chức năng, cập nhật ngay dòng tương ứng ở đây (đổi ⬜/🟨
→ ✅, ghi rõ endpoint + route), cùng lúc với `DANH-SACH-TINH-NANG.md` (đổi trạng thái giai đoạn) và
`TIEN-DO-DU-AN.md` (thêm log chi tiết đợt làm việc). Ba file phục vụ 3 mục đích khác nhau — đừng gộp
nội dung, chỉ tham chiếu chéo bằng link.
