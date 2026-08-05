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
> **Cập nhật lần cuối:** 2026-08-03, dựa trên khảo sát trực tiếp mã nguồn (không dựa vào docs khác) —
> đọc toàn bộ `src/Web/Endpoints/*.cs`, `web/app/[locale]/**`, `web-admin/src/{routes,features,lib}/**`.
> Khi thêm/sửa chức năng, cập nhật ngay dòng tương ứng trong file này — đừng để lệch khỏi code thật.
>
> Trạng thái: ✅ Nối API thật · 🟨 Có UI nhưng còn mock/thiếu 1 phần · ⬜ Chưa có (backend lẫn UI) ·
> ⛔ Link/route chết

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
| Đổi mật khẩu khi đã đăng nhập | ✅ `PUT /users/me/password` (yêu cầu đúng mật khẩu hiện tại, khác luồng OTP của forgot/reset) | ✅ `/settings` → `ChangePasswordForm` | — (cố ý bỏ qua, `settings/` cá nhân là tàn dư template chưa đầu tư, xem điểm 8) |
| Xem thông tin user hiện tại | ✅ `GET /users/me` | ✅ dùng để gate route + hiện email ở header | ✅ dùng để check role cho phép vào (`ALLOWED_ROLES`) |
| Xóa tài khoản (soft-delete + anonymize) | ✅ `DELETE /users/me` | ✅ `/settings` → `DeleteAccountButton`, 2-click confirm | — (chưa có màn hình cho Vận hành khoá/xoá user khác) |
| OAuth Google | ⬜ | 🟨 nút có trên `/auth/login` nhưng **không có logic**, chỉ UI tĩnh | — |
| OAuth Zalo | ⬜ | 🟨 tương tự Google, chỉ UI tĩnh | — |
| Chọn/ghi nhớ ngôn ngữ giao diện (lưu `users.locale`) | ⬜ | 🟨 `LanguageSwitcher` đổi route theo `next-intl` (hoạt động), nhưng **không lưu** lựa chọn vào user profile — mất khi đăng nhập máy khác | ✅ `LanguageSwitcher` (dropdown Globe ở Header, `react-i18next`) — toàn bộ 7 namespace (`common`/`jobs`/`applications`/`credit`/`candidates`/`members`/`ops`) đã rút chuỗi khỏi component, verify thật bằng Playwright trên cả 9 route chính. `vi`/`en` dịch tay thật, `ja`/`zh`/`ko`/`es` tạm placeholder tiếng Việt chờ dịch thuật (xem ADR-0010). Không lưu vào `users.locale`, chỉ `localStorage` |

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
| Học vấn / kinh nghiệm (`experiences`/`educations`) | ⬜ | ⬜ |
| CV Builder + xuất PDF | ⬜ | 🟨 `/profile/cv` **100% mock** (`MOCK_CANDIDATE`, `MOCK_CV`), nút Lưu/Xuất PDF không có handler, nút Export bị `disabled` |
| Xác thực SĐT | ⬜ (`phone_verified_at` field tồn tại nhưng không có luồng verify) | ⬜ |
| Hàng đợi duyệt CCHN (Vận hành) | ✅ `GET /ops/licenses`, `POST /ops/licenses/{id}/verify` | (xem mục Vận hành) |

---

## 4. Cơ sở y tế (Organization)

| Chức năng | Backend | `web/` | `web-admin/` |
|---|---|---|---|
| Trang công khai 1 tổ chức | ✅ `GET /organizations/{id}` | ✅ `/organizations/[id]` | — |
| Danh sách tổ chức công khai (list) | ⬜ chưa có endpoint | ⛔ **`/organizations` (index) không tồn tại** — link "Cơ sở y tế" trên header trỏ vào route 404 | — |
| Trang chủ hiện tổ chức nổi bật | — | 🟨 dùng `MOCK_ORGANIZATIONS` (mock-data.ts) vì chưa có API list | — |
| Upload giấy phép hoạt động | ⬜ | — | ⬜ chưa nối MinIO |
| Duyệt tổ chức (Verify/Reject/Suspend) | ✅ `POST /ops/organizations/{id}/verify` — Suspend tự động ẩn mọi tin `published` cùng transaction | — | ✅ `/ops/verification` (tab Tổ chức) |
| Cộng Credit thủ công (khuyến mãi/hỗ trợ) | ✅ `POST /ops/organizations/{id}/credit-bonus` | — | ⬜ **chưa có UI** — chỉ dùng qua API trực tiếp lúc test, không có màn hình cho Vận hành |
| Hoàn Credit thủ công khi tranh chấp | ⬜ `/ops/organizations/{id}/credit-refund` chưa làm | — | ⬜ |

---

## 5. Tin tuyển dụng (Jobs)

| Chức năng | Backend | `web/` | `web-admin/` |
|---|---|---|---|
| Tìm kiếm/lọc tin công khai | ✅ `GET /jobs` (LINQ/EF Core, chưa chuyển Postgres full-text) | ✅ `/jobs` — filter qua URL query, giữ SSR | — |
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
| Xem đơn ứng tuyển của tôi (ứng viên) | ✅ `GET /candidates/me/applications` | ✅ `/dashboard` | — |
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
| Xem chi tiết hồ sơ sau khi mở | ✅ `GET /candidates/{id}` (query `organizationId`, 403 nếu chưa unlock, trả `EmployerCandidateProfileDto`) | ✅ `/candidates/$candidateId` — tên/headline/summary/avatar/email/chuyên khoa/CCHN, link từ card đã mở trong `/candidates` |

---

## 8. Thanh toán (Payments)

| Chức năng | Backend | `web-admin/` |
|---|---|---|
| Tạo giao dịch mua gói tin (job-package) | ✅ `POST /payments/job-package` — chặn trùng nếu tin đã có giao dịch pending | ✅ `/jobs/new` — dialog hiện mã tham chiếu sau khi chọn gói trả phí |
| Tạo giao dịch nạp Credit | ✅ `POST /payments/credit-topup` (quy đổi tạm 1 Credit = 1.000đ) — chặn trùng theo tổ chức | ✅ `/credit` — dialog "Nạp thêm Credit" |
| Xem trạng thái 1 giao dịch | ✅ `GET /payments/{id}` | ⬜ **chưa có UI** tra cứu riêng — chỉ dùng nội bộ ngay sau khi tạo |
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
| Cộng Credit thủ công | ✅ `POST /ops/organizations/{id}/credit-bonus` | ⬜ chưa có UI — chỉ gọi được qua API trực tiếp |
| Hoàn Credit khi tranh chấp | ⬜ backend chưa có (`/ops/organizations/{id}/credit-refund`, khác `credit-bonus` — dùng khi tranh chấp cụ thể) | ⬜ |
| Xử lý báo cáo vi phạm | ✅ `GET /ops/reports`, `POST /ops/reports/{id}/resolve` | ✅ `/ops/reports` — nối API thật, không còn mock |
| Quản lý người dùng hệ thống (khoá/mở khoá) | ✅ `GET /ops/users`, `POST /ops/users/{id}/{suspend,unsuspend}` | ✅ `/ops/users` — tìm theo email + nút Khóa/Mở khóa |
| Dashboard tổng quan Vận hành | ✅ `GET /ops/dashboard/stats` (đếm đơn giản, không có xu hướng theo thời gian) | ✅ `/ops/dashboard` |
| Sidebar tách theo role (Admin vs Vận hành) | — | ✅ `getSidebarData(role)` — employer chỉ thấy `employerGroup`, admin/moderator chỉ thấy `opsGroup`. Chưa lọc nhóm `Pages`/`Other` (tàn dư template, ngoài phạm vi) |

---

## 11. Thông báo, tin nhắn & hạ tầng UI khác

| Chức năng | Backend | `web/` | `web-admin/` |
|---|---|---|---|
| Thông báo trong ứng dụng (`Channel = InApp`) | ✅ `GET /notifications` (`?unreadOnly`), `PATCH .../read`, `PATCH .../read-all` | ✅ `NotificationBell` (dropdown, poll 30s) + trang `/notifications` (lịch sử đầy đủ + đánh dấu tất cả đã đọc) | ✅ tương tự — `NotificationBell` + route `/notifications` |
| Thông báo email | ⬜ | ⬜ | ⬜ |
| Nhắn tin NTD ↔ ứng viên | ⬜ | ⬜ | ⬜ `/chats` chỉ đọc `data/convo.json` tĩnh (tàn dư template), không gọi API nào |
| Trang `/tasks`, `/apps` (template mẫu Jira/App Store) | — | — | ⬜ tàn dư template shadcn-admin gốc, dùng `@faker-js/faker`, không thuộc nghiệp vụ — nên gỡ khỏi sidebar/điều hướng khi dọn dẹp |
| Trang Settings cá nhân (account/appearance/display/notifications) | — | (xem mục 1) | ⬜ tàn dư template, chưa nối API cập nhật user thật |
| Link `/about` | — | ⛔ route không tồn tại, link chết trên header | — |

---

## Tổng hợp nhanh: nợ kỹ thuật lộ diện qua đợt khảo sát này

Những mục này **không nằm trong checklist cũ**, phát hiện khi rà soát code thật — cần dọn hoặc quyết định rõ ràng (làm tiếp hay bỏ):

1. **Link chết trên `web/`**: `/organizations` (index — trang liệt kê tổ chức) và `/about` đều 404. Header trỏ tới nhưng route chưa dựng.
2. **`web/lib/mock-data.ts`**: 8/11 export là code chết (không ai import) — `MOCK_JOBS`, `getJobById`, `getOrganizationById`, `getJobsByOrganization`, `MOCK_SPECIALTIES`, `MOCK_LOCATIONS`, `MOCK_EMPLOYMENT_TYPES`, `MOCK_APPLICATIONS`. Nên xóa khi dọn dẹp.
3. **Route Handler tồn tại nhưng chưa có UI gọi** ở `web/`: sửa/xoá CCHN (`PUT`/`DELETE /api/candidates/me/licenses/{id}`), thêm chuyên khoa (`POST /api/candidates/me/specialties`) — hồ sơ ứng viên hiện không có cách sửa CCHN hay thêm chuyên khoa qua giao diện dù backend + proxy đã sẵn.
4. **`applicationsApi` có hàm chưa được gọi** ở `web-admin/`: `getById` (xem chi tiết đơn riêng), `addNote`, `score`, `getHistory` — đã có API thật và đã nối vào `lib/api.ts` nhưng chưa có UI/nút bấm nào gọi tới (khác `jobsApi.update/close/renew` — đã nối UI ở `/jobs`, xem mục 5 ở trên).
5. **Chuỗi tiếng Việt hardcode** ở `web/` thay vì qua `next-intl`, rải rác ở: `profile-form.tsx`, `profile/page.tsx`, `license-section.tsx`, `settings/page.tsx`, `delete-account-button.tsx`, `dashboard/page.tsx`, home `page.tsx`, `jobs/[id]/page.tsx` (hàm `formatSalary`). Vi phạm CLAUDE.md mục 4 quy tắc #10 — **chưa dọn ở đợt rút chuỗi `web-admin/`**, vẫn còn tồn đọng riêng cho `web/`. Ở `web-admin/`, sau [ADR-0010](../kien-truc/adr/0010-da-ngon-ngu-cho-web-admin.md), đã rút xong cả 7 namespace (`common`/`jobs`/`applications`/`credit`/`candidates`/`members`/`ops`) — không còn chuỗi hardcode nào trong `features/**/*.tsx` thuộc các trang nghiệp vụ chính (trừ `chats/`, `tasks/`, `apps/`, `settings/` cá nhân — tàn dư template, xem điểm 8).
6. **`web-admin/` sidebar chưa tách theo role thật** — Admin (NTD) và Vận hành đang thấy chung 1 sidebar (có TODO comment xác nhận), chưa ẩn/hiện mục theo role JWT thật.
7. **Vận hành thiếu 3 màn hình**: cộng Credit thủ công (đã có API, chưa có UI), quản lý người dùng hệ thống (chưa có cả API lẫn UI — khác "Thành viên tổ chức"), dashboard tổng quan nền tảng.
8. **`/tasks`, `/apps`, `/chats`, trang Settings cá nhân** ở `web-admin/` là tàn dư template shadcn-admin gốc, không thuộc nghiệp vụ — cân nhắc gỡ khỏi điều hướng để tránh gây nhầm lẫn "đây có phải tính năng thật không".

---

## Cách dùng file này khi thêm tính năng mới

Theo đúng CLAUDE.md mục 5 — khi hoàn thành 1 chức năng, cập nhật ngay dòng tương ứng ở đây (đổi ⬜/🟨
→ ✅, ghi rõ endpoint + route), cùng lúc với `DANH-SACH-TINH-NANG.md` (đổi trạng thái giai đoạn) và
`TIEN-DO-DU-AN.md` (thêm log chi tiết đợt làm việc). Ba file phục vụ 3 mục đích khác nhau — đừng gộp
nội dung, chỉ tham chiếu chéo bằng link.
