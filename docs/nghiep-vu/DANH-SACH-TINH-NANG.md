# GiapTech.BlouseHiding — Danh sách tính năng

> Checklist tính năng theo giai đoạn, dùng để theo dõi tiến độ thực tế — cập nhật trạng thái khi triển
> khai, không để tài liệu này lệch khỏi code thật. Chi tiết nghiệp vụ từng mục xem
> [`PHAN-TICH-NGHIEP-VU.md`](./PHAN-TICH-NGHIEP-VU.md) mục 3; luồng & màn hình xem
> [`LUONG-NGHIEP-VU-MAN-HINH.md`](./LUONG-NGHIEP-VU-MAN-HINH.md). Muốn tra theo module (endpoint/route
> cụ thể nào đã nối API thật, còn mock, hay hoàn toàn chưa có) xem
> [`TIEN-DO-CHI-TIET.md`](./TIEN-DO-CHI-TIET.md).
>
> Trạng thái: ⬜ Chưa làm · 🟨 Đang làm · ✅ Hoàn thành

---

## Giai đoạn 0 — Khởi tạo

> **Thứ tự ưu tiên: dựng giao diện (0.1) trước, nối backend thật (0.2) sau** — mục tiêu trước mắt là
> thấy được hình hài sản phẩm nhanh, dùng mock data thay API thật, chưa cần toàn diện ngay. Danh sách
> chi tiết theo màn hình xem [`LUONG-NGHIEP-VU-MAN-HINH.md`](./LUONG-NGHIEP-VU-MAN-HINH.md) mục 2.

### 0.1 UI Shell (ưu tiên làm trước, không cần chờ backend)
- ✅ Khởi tạo `web/` (Next.js + shadcn/ui) — copy component cần dùng, áp token "Tin cậy lâm sàng"
  ([`../frontend/THIET-KE-GIAO-DIEN.md`](../frontend/THIET-KE-GIAO-DIEN.md) mục 5), tự host Be Vietnam Pro
- ✅ Khởi tạo `web-admin/` (shadcn-admin, Vite + TanStack Router) — áp token trung tính (ADR-0007), theo
  cấu trúc đã chốt ở [ADR-0008](../kien-truc/adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md)
- ✅ Khung đa ngôn ngữ ở `web/`: routing `next-intl` 6 locale (`localePrefix: always`), cấu trúc thư
  mục `messages/`, bộ chọn ngôn ngữ (header + footer)
- ✅ Dựng màn hình `web/` theo mục 2.1/2.2 LUONG-NGHIEP-VU-MAN-HINH.md: trang chủ, tìm việc, chi tiết
  tin, đăng ký/đăng nhập, dashboard ứng viên, hồ sơ + CCHN, cài đặt tài khoản — nay đã nối API thật
  (xem mục 0.2). Riêng `profile/cv` (CV Builder) vẫn giữ mock, chờ backend CV Builder
- ✅ Dựng màn hình `web-admin/` phần Admin theo mục 2.3: dashboard NTD, đăng/sửa tin, danh sách tin,
  ATS Kanban, ví Credit, tìm ứng viên chủ động, thành viên tổ chức — nay đã nối API thật (mục 0.2)
- ✅ Dựng màn hình `web-admin/` phần Vận hành theo mục 2.4: dashboard tổng quan, 3 hàng đợi duyệt
  (CCHN/doanh nghiệp/tin), đối soát thanh toán, quản lý danh mục/gói, quản lý người dùng hệ thống
  (`features/ops-users/` — tách riêng khỏi trang Thành viên tổ chức), xử lý report (`features/
  ops-reports/`) đều đã nối API thật — dòng ghi "chưa làm" trước đây đã lỗi thời, đã xác nhận lại qua
  code (2026-08-06)
- ✅ Nhật ký kiểm toán (Audit log, mục 2.4 LUONG-NGHIEP-VU-MAN-HINH.md) — bounded context thật (bảng
  `AuditLogEntries` + `GET /ops/audit-logs`), ghi tường minh trong 4/5 Command handler nhạy cảm (duyệt
  CCHN/tổ chức, xử lý báo cáo, khóa/mở khóa user — xóa tài khoản để lại sau, không qua Mediator Command),
  `features/ops-audit/` đã nối API thật thay mock (2026-08-06)
- ⬜ Rà lại responsive + dark mode + contrast WCAG AA trên toàn bộ màn hình — chưa làm riêng thành 1
  đợt, mới rà thủ công từng màn hình lúc dựng

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
- 🟨 Nối API thật thay mock data. `web/` (Client) đã nối **Identity + Jobs + Applications + Hồ sơ ứng
  viên (core)**: đăng ký/OTP/đăng nhập/quên-đặt lại mật khẩu (`app/api/auth/*` proxy route → httpOnly
  cookie), tìm/xem tin + ứng tuyển thật, dashboard/hồ sơ/CCHN thật (`GET/PUT /candidates/me`,
  `POST /candidates/me/licenses`), xóa tài khoản. `SiteHeader` tự nhận diện đăng nhập qua cookie,
  route `dashboard/profile/settings` redirect login nếu chưa đăng nhập. **Chưa nối**: `profile/cv`
  (CV Builder — giữ mock, chờ backend), OAuth Google/Zalo, đổi mật khẩu (chưa có endpoint riêng).
  `web-admin/` (Vận hành/Admin) đã nối **đăng nhập + route protection (Zustand + cookie thường +
  axios interceptor tự refresh), đăng tin/danh sách tin, ATS Kanban theo tin (kéo-thả đổi giai đoạn
  thật), dashboard NTD, ví Credit (số dư + lịch sử), hàng đợi duyệt Vận hành (CCHN/tổ chức/tin, có
  dialog nhập lý do khi từ chối), tìm/mở hồ sơ ứng viên chủ động (`features/candidates/`, filter
  chuyên khoa/địa điểm, ẩn liên hệ tới khi mở, nút mở hồ sơ trừ Credit thật, báo lỗi rõ khi không đủ
  Credit), form thêm/sửa danh mục ở `/ops/catalog` (chuyên khoa + địa điểm dạng cây cha-con qua dialog,
  gói tin Free/Eco/Pro/Max — hạng gói không đổi được sau khi tạo, đúng ràng buộc backend), thành viên
  tổ chức (`features/users/` viết lại hoàn toàn — bỏ data-table generic + faker của shadcn-admin gốc,
  thay bằng bảng thành viên + lời mời đang chờ, dialog mời thành viên mới, xoá thành viên có xác nhận,
  không cho xoá owner), đối soát thanh toán (`ops-payments/`, hàng đợi thật từ `GET /ops/payments`,
  nút Xác nhận/Không khớp gọi `POST /ops/payments/{id}/confirm|reject`), nạp Credit (dialog ở trang Ví
  Credit — nhập số Credit, hiện số tiền quy đổi + mã tham chiếu để chuyển khoản, nút "Nạp thêm Credit"
  không còn `disabled` như trước), đăng tin gói trả phí (`jobs/new.tsx` — chọn gói Eco/Pro/Max gọi
  `POST /payments/job-package` sau khi submit, hiện dialog thông tin chuyển khoản)**. **Chưa nối**:
  `/ops/reports` (chưa có bounded context backend).

## Giai đoạn 1 — MVP

### Tài khoản & định danh
- ✅ Đăng ký/đăng nhập email, OTP — OTP hiện là **driver giả lập nội bộ** (log thay vì gửi email/SMS
  thật, xem docs/nghiep-vu/TIEN-DO-DU-AN.md), chưa chốt nhà cung cấp SMTP/SMS thật. Xác thực SĐT
  (`phone_verified_at`) chưa làm — mới có email
- ⬜ OAuth Google
- ⬜ OAuth Zalo
- ✅ RBAC (`candidate`/`employer`/`admin`/`moderator` — site Client/Admin/Vận hành) — JWT access token
  (15 phút) + refresh token xoay vòng (hash lưu DB), claim role dùng cho `[Authorize(Roles=...)]`
- ✅ Đa thành viên HR trong 1 tổ chức — `POST /organizations` (tạo tổ chức lần đầu → owner member),
  `GET/POST /organizations/{id}/members` (list + invite), `DELETE .../members/{id}` (không xoá được
  owner), `POST /invitations/{token}/accept`
- ✅ Mời thành viên HR qua email (kể cả email chưa có tài khoản — `organization_invitations`, token
  hash SHA-256 lưu DB, hết hạn sau 7 ngày). Gửi email hiện là **driver giả lập nội bộ** (log token thay
  vì gửi thật, cùng quyết định với OTP — xem `docs/nghiep-vu/TIEN-DO-DU-AN.md`)
- ✅ Chọn/ghi nhớ ngôn ngữ giao diện (`PUT /users/me/locale`, `GET /users/me` trả kèm `locale`) — cả
  `web/` và `web-admin/` tự đồng bộ khi đổi ngôn ngữ/đăng nhập lại. Email/thông báo gửi đúng ngôn ngữ đã
  chọn chưa làm (chưa có bounded context Thông báo)
- ✅ Xóa tài khoản (`DELETE /users/me` — soft-delete + anonymize PII, giữ audit log/application đã ẩn danh)

### Hồ sơ ứng viên
- ✅ Hồ sơ cơ bản (`candidate_profiles` — tên, mô tả ngắn, tiểu sử; tự tạo lúc `PUT /candidates/me` lần
  đầu). Học vấn/kinh nghiệm/kỹ năng (`experiences`/`educations`) chưa làm — dời sang đợt sau
- ✅ Quản lý CCHN — thêm/sửa (chỉ khi `pending`/`rejected`)/xóa (chỉ khi chưa `verified`); upload ảnh/scan
  CCHN thật qua MinIO (`POST /uploads/presigned-url` + `PUT` thẳng lên MinIO, `web/` đã nối UI thật).
  Sửa/xóa CCHN qua UI (Route Handler proxy đã có sẵn) — chưa nối
- ✅ Gắn chuyên khoa + trình độ (`profile_specialties`, không trùng chuyên khoa)
- ⬜ CV Builder (mẫu dựng sẵn) + upload PDF
- ✅ Hàng đợi duyệt CCHN (Vận hành) — `GET /ops/licenses`, `POST /ops/licenses/{id}/verify` (duyệt/từ
  chối kèm lý do bắt buộc khi từ chối)

### Cơ sở y tế
- ✅ Đăng ký hồ sơ tổ chức (`POST /organizations` — tạo tổ chức lần đầu → owner member). Đã nối UI ở
  `web-admin/` (trang "Hồ sơ tổ chức", `/organization`) — trước đó chỉ có backend, employer không có
  cách tạo tổ chức qua UI
- ✅ Upload giấy phép hoạt động — entity `OrganizationDocument` mới (`GET/POST
  /organizations/{id}/documents`), Vận hành xem file thật ngay trong hàng đợi duyệt tổ chức
  (`GET /ops/organizations` trả kèm `documents`), cảnh báo rõ khi tổ chức chưa upload gì
- ✅ Hàng đợi duyệt tổ chức (Vận hành) — `GET /ops/organizations`, `POST /ops/organizations/{id}/verify`
  (action `Verify`/`Reject`/`Suspend`). Rút xác thực (`Suspend`) **tự động** chuyển mọi tin `published`
  của tổ chức sang `suspended` trong cùng transaction (ERD mục 4.6) — không thao tác riêng từng tin
- ✅ Trang công khai cơ sở y tế — `GET /organizations/{id}` (Public, mới thêm khi nối `web/`, theo đúng
  API-DESIGN.md mục 4 đã thiết kế từ đầu nhưng chưa implement) + `web/organizations/[id]` nối API thật
- ✅ Danh sách cơ sở y tế công khai — `GET /organizations?q=` (chỉ trả `Verified`, filter theo tên
  optional) + `web/organizations` nối API thật thay suy diễn từ `getJobs()` (`deriveOrganizationsFromJobs`
  vẫn giữ lại cho tương thích cũ, không xoá — trang chủ có thể còn dùng)
- ✅ Ops tìm tổ chức theo tên (mọi trạng thái xác thực, khác endpoint public chỉ trả `Verified`) —
  `GET /ops/organizations/search?q=`, dùng cho màn "Cộng Credit thủ công" (`features/ops-credit/`)
  thay `MOCK_ORGANIZATIONS` cũ

### Tin tuyển dụng
- ✅ Tạo/sửa tin (draft/rejected — `CanEdit` invariant), đóng tin sớm (`close`), gia hạn (`renew` — tạo
  `jobs` row mới, tin gốc chuyển `closed`, không tái sử dụng `job_id`)
- ✅ Nộp duyệt (`submit`) — gói Free thẳng `pending`; gói Eco/Pro/Max → `pending_payment`, chờ
  `POST /payments/job-package` tạo giao dịch
- ✅ Quy trình thanh toán thủ công (mã tham chiếu + Vận hành xác nhận/từ chối qua `/ops/payments/{id}`)
  — xem mục "Thanh toán" bên dưới
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
- ✅ Nạp Credit — `POST /payments/credit-topup` (quy đổi tạm thời 1 Credit = 1.000đ), Vận hành xác
  nhận qua `POST /ops/payments/{id}/confirm` cộng `credit_wallets.balance` (`reason=purchase`).
  `POST /ops/organizations/{id}/credit-bonus` (Vận hành cộng thủ công, `reason=bonus`) vẫn giữ song
  song — dùng cho khuyến mãi/hỗ trợ, khác mục đích với nạp qua thanh toán
- ⬜ Hoàn Credit thủ công khi có tranh chấp (`/ops/organizations/{id}/credit-refund`) — chưa làm, khác
  `credit-bonus` (dùng khi tranh chấp/lỗi hệ thống, không phải nạp thường)

### Thanh toán
- ✅ Gói tin trả phí (`POST /payments/job-package`) — tạo `payments` (`type=job_package`,
  `provider=manual_transfer`) + `job_purchases` (liên kết ngay, `payment_id` gắn từ lúc tạo, không đợi
  confirm) + sinh `reference_code` (dạng `PAY-XXXXXX`, loại bỏ ký tự dễ nhầm 0/O/1/I vì NTD gõ tay vào
  nội dung chuyển khoản). Chặn tạo trùng khi tin đã có giao dịch `pending` khác
- ✅ Nạp Credit (`POST /payments/credit-topup`) — tạo `payments` (`type=credit_topup`). Chặn tạo trùng
  khi tổ chức đã có giao dịch nạp Credit `pending` khác
- ✅ Vận hành đối soát (`GET /ops/payments` hàng đợi, `POST /ops/payments/{id}/confirm|reject`) —
  confirm: `job_package` → `Job.ConfirmPayment()` (chuyển `pending_payment→pending`); `credit_topup` →
  cộng ví. Reject: `job_package` → `Job.RejectPayment()` (`pending_payment→draft`); `credit_topup` →
  chỉ đánh dấu `failed`, không cộng gì
- ⬜ Cổng thanh toán tự động (VNPay/Momo/ZaloPay) — hoãn theo [ADR-0003](../kien-truc/adr/0003-hoan-cong-thanh-toan-tu-dong.md),
  schema/enum `provider` đã dự phòng sẵn cho khi chọn cổng

### Thông báo
- ✅ Thông báo trong ứng dụng — backend (`GET /notifications`, `PATCH /notifications/{id}/read`), kích
  hoạt khi duyệt/từ chối CCHN, chuyển giai đoạn ứng tuyển (trừ khi "âm thầm"), duyệt/từ chối tin tuyển
  dụng. Đã nối UI chuông thông báo (poll 30s, đánh dấu đã đọc) ở cả `web/` và `web-admin/`. Chưa có nút
  "đánh dấu tất cả đã đọc" hay trang xem toàn bộ lịch sử (chỉ có dropdown hiện thông báo chưa đọc)
- ⬜ Thông báo email — hạ tầng driver giả lập đã có (`INotificationEmailSender`), nhưng chưa trigger nào
  dùng `Channel = Email` (cả 3 trigger hiện tại đều `InApp`); chưa chốt nhà cung cấp SMTP thật

### Vận hành
- ✅ Dashboard số liệu cơ bản (`GET /ops/dashboard/stats` — tổng ứng viên/NTD, tin theo trạng thái, tổ
  chức theo trạng thái xác thực, báo cáo/thanh toán chờ xử lý). Chưa có biểu đồ xu hướng theo thời gian
- ✅ Quản lý người dùng hệ thống (khóa/mở khóa) — `GET /ops/users?email=`,
  `POST /ops/users/{id}/{suspend,unsuspend}`, tách hẳn khỏi "Thành viên tổ chức" (`/users`, scope theo
  1 organization). Đã nối UI ở `web-admin/` (`/ops/users`)
- ✅ Xử lý báo cáo vi phạm — `POST /reports` (tạo, không giới hạn role), `GET /ops/reports`,
  `POST /ops/reports/{id}/resolve` (4 action: `dismissed`/`warned`/`content_removed`/
  `account_suspended` — `dismissed` thêm ngoài thiết kế gốc cho nút "Bỏ qua"). `content_removed` chỉ
  trigger state change thật (đóng tin) khi `targetType=Job` — `Organization`/`Profile`/`Message` chưa
  có cơ chế ẩn/gỡ tương ứng (giới hạn MVP). Đã nối UI ở `web-admin/` (`/ops/reports`, thay hoàn toàn
  mock cũ)

## Giai đoạn 2 — Hoàn thiện

- ⬜ Chuyển tìm kiếm sang OpenSearch
- 🟨 Gợi ý việc làm / matching ứng viên — cả 2 hướng đã làm, cùng độ đơn giản đã chốt (chỉ so chuyên
  khoa trùng tên, không tính địa điểm/kinh nghiệm/lịch sử xem tin):
  - `web/dashboard` hiện tối đa 4 tin "Việc phù hợp với bạn" (`lib/job-matching.ts`, so
    `ApiJob.specialtyName` với `ApiProfileSpecialty.name`), loại tin đã ứng tuyển khỏi gợi ý.
  - Hướng ngược lại: `web-admin/features/applications/suggested-candidates-sheet.tsx` — trang ATS 1
    tin có nút "Ứng viên gợi ý" mở Sheet, gọi lại `GET /candidates/search` (không có API mới), tự tra
    `specialtyId` từ `job.specialtyName` qua danh mục vì response `ApiJob` không có `specialtyId`,
    loại ứng viên đã ứng tuyển tin đó, có unlock Credit ngay trong Sheet (2026-08-07)
- ⬜ Chat realtime (SignalR)
- ⬜ Mobile app (Flutter/React Native)
- 🟨 Test đánh giá năng lực chuyên môn theo vị trí — UI đầy đủ (`web/` `/tools/competency-test`,
  chọn chuyên khoa từ danh mục thật → làm 5 câu hỏi → kết quả), nhưng bộ câu hỏi hiện tại chỉ là
  MINH HỌA dùng kiến thức an toàn/đạo đức nghề y tế chung — **chưa phải bộ đề chuyên môn lâm sàng
  thật theo từng chuyên khoa** (dược lâm sàng/điều dưỡng/cấp cứu...) như tài liệu yêu cầu, vì cần
  chuyên gia y tế soạn và kiểm định nội dung, ngoài khả năng tự viết đúng chuyên môn
- 🟨 Công cụ tính phụ cấp trực/độc hại/thuế TNCN — `web/` `/tools/salary-calculator` tính thật
  100% client-side (biểu thuế TNCN lũy tiến 7 bậc + giảm trừ gia cảnh theo luật hiện hành, không cần
  backend vì đây là công thức pháp luật công khai), có disclaimer rõ "không thay thế tư vấn thuế
  chính thức". Cả 2 công cụ (tính lương + test năng lực) gộp chung 1 trang hub `/tools` (nav header),
  tránh header có quá nhiều link riêng lẻ
- ✅ Đánh giá cơ sở y tế (review, có kiểm duyệt) — bounded context thật cả 2 chiều: bảng
  `OrganizationReviews` (`status` Pending/Approved/Rejected, unique 1 review/candidate/tổ chức) +
  `GET/POST /organizations/{id}/reviews` (candidate gửi, `web/organizations/[id]` đã nối API thật thay
  mock) + `GET /ops/organization-reviews` + `POST .../moderate` (Vận hành duyệt/từ chối, UI mới
  `web-admin/features/ops-reviews/`, chưa từng có kể cả dạng mock trước đây) — review "Pending" không
  bao giờ lộ ra endpoint public, review đã duyệt ẩn danh người viết (2026-08-06)

## Giai đoạn 3 — Mở rộng

- ⬜ Sự kiện/hội thảo/CME
- ⬜ Chuyên mục nội dung (tin tức/kỹ năng nghề y)
- ⬜ Cộng đồng sinh viên y khoa (mô hình CTV)
- ⬜ Phân tích/BI, employer branding
- ⬜ Matching bằng ML
- ⬜ Tích hợp/đối soát xác thực CCHN theo dữ liệu ngành

## Chưa có mốc thời gian cụ thể (chờ quyết định)

- ⬜ Tích hợp cổng thanh toán tự động (xem [ADR-0003](../kien-truc/adr/0003-hoan-cong-thanh-toan-tu-dong.md))
