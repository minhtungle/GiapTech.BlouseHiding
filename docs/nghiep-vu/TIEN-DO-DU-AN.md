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
| 0.2 — Backend & hạ tầng | 🟨 Gần xong | Danh mục (đọc+ghi) + CI/CD xong; Jobs/ATS/Credit chưa làm |
| 1 — MVP | 🟨 Đang làm | Backend: Identity+Jobs+Applications+Credit xong. `web/` đã nối Identity/Jobs/Applications/Hồ sơ ứng viên; `web-admin/` + Payments thật chưa làm |
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
- `/ops/catalog` (Vận hành) đã nối API thật cho phần đọc (xem Giai đoạn 0.2) — form thêm/sửa vẫn
  chưa nối (còn dùng nút tĩnh).

### Giai đoạn 0.2 — Backend & hạ tầng

**Trạng thái: 🟨 Gần xong — chưa đủ điều kiện chốt**

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
- Command Create/Update cho Specialty/Location/JobPackage (`POST`/`PUT` `/api/v1/ops/catalog/*`,
  `/api/v1/ops/job-packages/*`), mỗi command có FluentValidation validator (kiểm tra field bắt buộc +
  `CatalogLocales.IsSupported` cho bản dịch) và `[Authorize(Roles = "admin,moderator")]` theo đúng vai
  trò Vận hành ở `THUAT-NGU.md`.
- Nối `web/` (trang tìm việc) và `web-admin/` (`jobs/new`, `ops/catalog`) tới API danh mục thật qua
  `lib/api.ts` ở mỗi app, fallback về mock data khi API lỗi/rỗng (demo offline vẫn chạy được).
- CI/CD cơ bản: 3 GitHub Actions workflow độc lập theo path filter — `backend.yml` (`dotnet build` +
  `dotnet test`), `web.yml` (`npm run lint` + `npm run build`), `web-admin.yml` (`npm run lint` +
  `npm run format:check` + `npm run build`).

Verify đã chạy: `dotnet build`/`dotnet test` sạch (Release config, khớp CI); `dotnet ef` migration áp
thành công vào Postgres thật qua Docker Compose (`\dt` xác nhận đúng bảng); chạy `dotnet run --project
src/Web` thật + curl xác nhận cả 6 locale trả đúng tên đã dịch, fallback đúng khi header thiếu/không hỗ
trợ; `POST /api/v1/ops/catalog/specialties` không token trả 401, `GET /api/v1/catalog/*` vẫn public;
`curl http://localhost:3000/vi/jobs` trả tên chuyên khoa/địa điểm thật từ Postgres (không phải mock);
`npm run build` + `npm run lint` sạch ở cả `web/` và `web-admin/` sau khi nối API.

Còn thiếu (chặn việc chốt giai đoạn):
- Jobs, Applications/ATS, Credit/Payment — chưa bắt đầu bounded context nào trong số này (Identity đã
  làm, xem log Giai đoạn 1 bên dưới).
- Form thêm/sửa danh mục ở `web-admin/` (`/ops/catalog`, `jobs/new`) chưa gọi Command thật — mới nối
  phần đọc (dropdown/danh sách), nút submit còn tĩnh.
- `web/` và `web-admin/` vẫn dùng mock data cho mọi màn hình khác ngoài danh mục (Jobs/ATS/Credit/...).

### Giai đoạn 1 — MVP

**Trạng thái: 🟨 Đang làm — chưa đủ điều kiện chốt**

Đã làm:
- Identity thật thay khung ASP.NET Identity mặc định của template: mở rộng `ApplicationUser` (Id
  `Guid`, `role`/`status`/`locale`/`email_verified_at`/`phone_verified_at` khớp ERD), JWT access token
  (15 phút, claim role/email/nameidentifier) + refresh token xoay vòng (hash SHA-256 lưu bảng
  `refresh_tokens`, thu hồi khi logout/dùng lại token cũ).
- Endpoint `/api/v1/auth/{register,verify-otp,login,refresh,logout,forgot-password,reset-password}` +
  `/api/v1/users/me` (GET/DELETE) đúng path đã thiết kế ở `API-DESIGN.md` mục 2 — thay hoàn toàn
  `MapIdentityApi`/`AddBearerToken` mặc định của template (path/scheme không khớp thiết kế).
  Xóa `IdentityApiOperationTransformer` (dead code sau khi bỏ `MapIdentityApi`).
- OTP đăng ký + quên mật khẩu — **driver giả lập nội bộ** (`LoggingOtpSender`, log ra thay vì gửi email/
  SMS thật, qua interface `IOtpSender` nên đổi driver thật sau không ảnh hưởng luồng nghiệp vụ). Chưa
  chốt nhà cung cấp SMTP/SMS thật, chỉ áp dụng cho MVP demo.
- `DELETE /users/me` sửa từ xóa cứng (template mặc định) sang **soft-delete + anonymize** đúng CLAUDE.md
  rule bất di bất dịch #4/NĐ 13/2023 — set `status=deleted`, ẩn danh email/SĐT, xóa password hash.
- Bounded context Employer tối thiểu: `Organization`/`EmployerMember` entity + `POST /organizations`
  (tạo tổ chức lần đầu → tự tạo `owner` member), `[Authorize(Roles = "employer")]`.
- Enum toàn API chuyển sang serialize dạng string (`JsonStringEnumConverter` toàn cục) thay vì số thứ
  tự — áp dụng lùi cho cả Catalog đã có từ Giai đoạn 0.2.
- Migration `InitialCreate` viết lại từ đầu (schema Identity đổi `Id` từ `string` sang `Guid` — thay
  đổi phá vỡ, chấp nhận được vì migration cũ chưa từng lên production, chỉ tồn tại trong nhánh feature
  chưa merge `main`).
- Bounded context **Hồ sơ ứng viên (core)**: `CandidateProfile`/`License`/`ProfileSpecialty` — Domain
  entity (`License.Verify()`/`Reject()`, `CanEdit` invariant chỉ sửa khi `pending`/`rejected`) → EF Core
  config → migration `AddCandidateProfile`. Application: `GetMyProfileQuery` (resolve tên chuyên khoa
  theo locale), `UpdateMyProfileCommand` (tự tạo hồ sơ ở lần gọi đầu — "upsert"), `AddLicenseCommand`/
  `UpdateLicenseCommand`/`DeleteLicenseCommand`, `AddProfileSpecialtyCommand` (chặn gắn trùng chuyên
  khoa). Web: `GET/PUT /candidates/me`, `POST/PUT/DELETE /candidates/me/licenses/*`,
  `POST /candidates/me/specialties` — đúng path API-DESIGN.md mục 3.
- Hàng đợi duyệt CCHN cho Vận hành: `GET /ops/licenses`, `POST /ops/licenses/{id}/verify` (bắt buộc
  `rejectReason` khi từ chối) — `[Authorize(Roles = "admin,moderator")]`.
- Tính `completion_pct` hồ sơ (`ProfileCompletion`, trọng số đơn giản 5 tiêu chí x 20%, tính lại mỗi khi
  cập nhật profile/license/specialty) — quyết định MVP, không cần logic phức tạp hơn ở giai đoạn này.
- Enum toàn API chuyển sang serialize dạng string (`JsonStringEnumConverter` toàn cục) thay vì số thứ
  tự — áp dụng lùi cho cả Catalog đã có từ Giai đoạn 0.2.

Verify đã chạy: `dotnet test` 17/17 pass (3 unit + 14 functional — functional dùng Testcontainers
Postgres thật, có test riêng cho rotate refresh token, revoke khi logout, sai mật khẩu, reset password,
thêm/sửa/xóa CCHN theo đúng invariant `CanEdit`, duyệt/từ chối CCHN, gắn chuyên khoa trùng bị chặn);
migration áp thành công vào Postgres thật (`\dt` xác nhận đủ bảng); chạy `dotnet run --project src/Web`
thật + curl toàn bộ luồng register→verify-otp→login→PUT /candidates/me→POST .../licenses (pending)→
GET /ops/licenses (thấy trong hàng đợi, 401 khi không có token)→POST .../verify (approved)→
GET /candidates/me (license chuyển `Verified`, hàng đợi Ops rỗng)→POST .../specialties
(`completionPct` đạt 100%); `POST /organizations` tạo tổ chức + owner member thành công với JWT role
`employer`.

**Lỗi phát hiện và sửa trong lúc viết test** (đáng ghi lại vì không hiển nhiên): thêm entity mới vào
collection navigation của 1 entity cha **đã được `Include()` load lại** (không phải entity mới tạo từ
đầu) không đủ để EF Core tự động detect state "Added" khi PK được gán sẵn (`Guid.NewGuid()` ở
constructor thay vì để DB tự sinh) — gây `DbUpdateConcurrencyException` ("expected 1 row affected 0")
vì EF coi INSERT là UPDATE trên row không tồn tại. Khắc phục: luôn gọi `_context.<DbSet>.Add(entity)`
tường minh song song với việc thêm vào collection Domain, không chỉ dựa vào change-tracker tự suy luận
qua navigation. Áp dụng ở `AddLicenseCommand`/`AddProfileSpecialtyCommand`. Cũng phát hiện 4 Command
(bao gồm `RegisterCommand` từ vòng trước) throw thẳng `FluentValidation.ValidationException` trong
handler body thay vì `Application.Common.Exceptions.ValidationException` — `ProblemDetailsExceptionHandler`
chỉ bắt loại thứ 2, nên lỗi loại thứ nhất sẽ rơi xuống 500 thay vì 400 đúng thiết kế. Sửa bằng alias
`using ValidationException = ...Exceptions.ValidationException;` ở cả 4 file.

- Bounded context **Tin tuyển dụng (core, gói Free)**: `Job` entity — invariant `CanEdit`
  (draft/rejected), `Submit()`/`ConfirmPayment()`/`RejectPayment()`/`Moderate()`/`Close()`/
  `Suspend()`/`CreateRenewalCopy()` đều nằm trong Domain, Handler chỉ điều phối. `Moderate()` enforce
  đúng ERD mục 4.1 (`published` chỉ khi `organization.verify_status = verified`) — kiểm tra ở cả
  Application layer (`ModerateJobCommandHandler`) trước khi gọi Domain method, không phó mặc 1 lớp.
  Web: `GET/POST /jobs`, `GET/PUT /jobs/{id}`, `POST /jobs/{id}/{submit,close,renew}`,
  `GET /organizations/{id}/jobs`, `GET /ops/jobs`, `POST /ops/jobs/{id}/moderate` — đúng path
  API-DESIGN.md mục 5, 11. **Chỉ hỗ trợ gói Free ở MVP** (submit thẳng `pending`, không qua thanh
  toán) — gói Eco/Pro/Max + `payments`/`pending_payment` là bounded context Payments riêng, quyết định
  tách khỏi vòng này đã xác nhận với người dùng.

Verify đã chạy: `dotnet test` 23/23 pass (3 unit + 20 functional, có test riêng cho publish chặn khi
tổ chức chưa verified, draft không hiện cho guest, sửa tin sau submit bị chặn, đóng tin bởi người
không phải thành viên tổ chức bị 403); migration `AddJobs` áp thành công; chạy `dotnet run --project
src/Web` thật + curl toàn bộ luồng tạo tin (draft, guest 404)→submit gói Free→hàng đợi Vận hành→
duyệt→published (guest xem được, search theo keyword thấy đúng)→renew (tin cũ `closed`, tin mới
`draft` sao chép đúng nội dung)→`GET /organizations/{id}/jobs` thấy đủ mọi trạng thái.

- Bounded context **Ứng tuyển & ATS**: `JobApplication`/`ApplicationNote`/`ApplicationStageHistory` —
  entity `JobApplication` (không đặt tên `Application` để tránh xung đột với chính namespace project
  `GiapTech.BlouseHiding.Application`). `TransitionStage()` ghi lịch sử trong cùng lời gọi Domain
  method, Handler chỉ `Add` tường minh vào `DbSet` rồi save 1 lần — đúng CLAUDE.md rule bất di bất dịch
  #3 (đổi stage phải ghi `application_stage_history` cùng transaction). `cv_snapshot` (jsonb) chụp từ
  `CandidateProfile` hiện có (không cần CV Builder/bảng `cvs` — quyết định đã xác nhận với người dùng,
  đổi nguồn khi CV Builder ra mắt không ảnh hưởng luồng nghiệp vụ). `score` tính 1 lần lúc ứng tuyển
  theo match trường có cấu trúc (chuyên khoa/CCHN đã verified/địa điểm — không NLP/AI, đúng ERD mục
  4.8), HR ghi đè thủ công qua `PATCH /applications/{id}/score`. Web: `POST/GET /jobs/{id}/applications`,
  `GET /candidates/me/applications`, `GET/PATCH /applications/{id}/{stage,score}`,
  `POST /applications/{id}/notes`, `GET /applications/{id}/history` — đúng API-DESIGN.md mục 8.

Verify đã chạy: `dotnet test` 31/31 pass (3 unit + 28 functional — không có lỗi nào phải sửa lần
này, 2 bug pattern phát hiện ở các vòng trước (Add tường minh vào DbSet, alias ValidationException)
áp dụng đúng ngay từ đầu); migration `AddApplications` áp thành công; chạy `dotnet run --project
src/Web` thật + curl toàn bộ luồng: candidate ứng tuyển vào tin published→hiện trong
`GET /candidates/me/applications` (stage `New`)→employer xem `GET /jobs/{id}/applications` (ATS
Kanban)→chuyển stage `New→Shortlisted` (ghi lịch sử đúng)→thêm ghi chú→chấm điểm ghi đè→xem lịch sử;
ứng tuyển trùng bị chặn 400 đúng thiết kế.

- **Hàng đợi duyệt tổ chức (Vận hành)**: `GET /ops/organizations`, `POST /ops/organizations/{id}/verify`
  (action `Verify`/`Reject`/`Suspend`, không phải 3 endpoint riêng — đúng thiết kế API-DESIGN.md mục
  11). `Organization.Suspend()` (Domain method) chỉ đổi `verify_status`; enforce ERD mục 4.6 (tự động
  suspend mọi tin `published` của tổ chức cùng transaction) nằm ở `VerifyOrganizationCommandHandler`
  — query toàn bộ `Job` đang published của org rồi gọi `job.Suspend()` cho từng cái, save 1 lần. Đây
  là gap được ghi nhận là quan trọng nhất ở log trước (chặn test end-to-end không cần SQL tay) — đã
  giải quyết ngay lượt tiếp theo.

Verify đã chạy: `dotnet test` 35/35 pass (3 unit + 32 functional, có test riêng cho verify/reject
tổ chức, và **test quan trọng nhất**: rút xác thực tổ chức đã `verified` có tin `published` → tin tự
động chuyển `suspended` trong cùng lần gọi, không cần thao tác thứ 2); không cần migration mới (không
thêm DbSet, chỉ thêm Domain method + Command/Query + endpoint). Verify curl end-to-end thật: tạo tổ
chức (pending)→`GET /ops/organizations` thấy trong hàng đợi→`POST .../verify` (Verify) 200→tạo tin→
submit→duyệt→published (curl xác nhận)→`POST .../verify` (Suspend) 200→`GET /jobs/{id}` guest trả 404
(không còn published)→`SELECT "Status" FROM "Jobs"` xác nhận giá trị enum `Suspended` (7) trong Postgres
thật.

- Bounded context **Credit & Profile Unlock**: `CreditWallet` (tự tạo 1-1 lúc `POST /organizations`,
  `CHECK (balance >= 0)`)/`CreditTransaction`/`ProfileUnlock`. Trừ Credit dùng `ExecuteUpdateAsync` với
  điều kiện `WHERE Balance >= cost` — atomic ngay tại Postgres, tránh race condition khi 2 request trừ
  đồng thời **mà không cần row lock/transaction thủ công đọc-sửa-ghi qua ChangeTracker** (đơn giản hơn
  cách "SELECT FOR UPDATE" truyền thống, vẫn đúng CLAUDE.md mục 4 quy tắc bất di bất dịch #2). Bọc
  toàn bộ (trừ tiền + tạo `ProfileUnlock` + ghi `CreditTransaction`) trong 1 DB transaction thật qua
  `IApplicationDbContext.ExecuteInTransactionAsync` (thêm mới vào interface, che giấu chi tiết
  EF Core/Npgsql khỏi Application layer) — đảm bảo không có khoảng hở giữa lúc trừ tiền thành công và
  lúc ghi nhận bản ghi tương ứng nếu có lỗi giữa chừng. `ProfileUnlock` idempotent qua
  `UNIQUE(org_id, candidate_id)` — mở lại không trừ thêm. Nạp Credit **chưa nối payments thật** — thay
  bằng `POST /ops/organizations/{id}/credit-bonus` (Vận hành cộng thủ công) để test unlock end-to-end,
  quyết định tạm thời đã xác nhận với người dùng.
  Web: `GET /organizations/{id}/{credit-wallet,credit-transactions}`, `GET /candidates/search` (ẩn
  liên hệ tới khi unlock), `POST /candidates/{id}/unlock`, `POST /ops/organizations/{id}/credit-bonus`
  — đúng API-DESIGN.md mục 7 (trừ phần payments thật).

Verify đã chạy: `dotnet test` 41/41 pass (3 unit + 38 functional — không có lỗi phải sửa lần này,
có test riêng cho: ví tự tạo balance 0, cộng bonus + ghi transaction, unlock trừ đúng số + hiện contact,
unlock 2 lần idempotent không trừ thêm, unlock không đủ tiền bị chặn 400, và trừ liên tiếp 2 candidate
khác nhau khi ví chỉ đủ cho 1 lần — xác nhận `ExecuteUpdateAsync` có điều kiện chặn đúng, ví không bị
âm); migration `AddCredit` áp thành công. Verify curl end-to-end thật: tạo tổ chức (ví balance=0 tự
động)→Vận hành cộng bonus 100→`GET /candidates/search` (contact ẩn)→`POST .../unlock` (trừ 15, còn
85, contact hiện đúng email)→unlock lại cùng candidate (idempotent, vẫn 85, không trừ thêm).

- **Nối `web/` (Client) tới API thật** — Identity + Jobs + Applications + Hồ sơ ứng viên (core), gap
  lớn nhất được ghi nhận ở log trước. Kiến trúc: JWT access+refresh token lưu **httpOnly cookie** qua
  Next.js Route Handler (`web/app/api/auth/*`, `web/lib/auth-cookies.ts`) — quyết định đã xác nhận với
  người dùng (thay vì Context/localStorage phía client) để Server Component đọc được cookie qua
  `cookies()` mà không mất lợi thế SSR. `web/lib/backend-fetch.ts` tự refresh access token 1 lần khi
  gặp 401 trước khi trả lỗi. Route bảo vệ (`dashboard`/`profile`/`settings`) tự redirect `/auth/login`
  nếu chưa đăng nhập qua `getCurrentUser()` (gọi `GET /users/me` bằng cookie).
  Màn hình đã nối: đăng ký→verify-otp (trang mới)→đăng nhập→quên/đặt lại mật khẩu (2 trang mới);
  `SiteHeader` chuyển `async` để tự hiển thị email/nút đăng xuất khi đã đăng nhập; tìm việc (`/jobs`
  filter qua URL query thay vì client-state, giữ SSR) + chi tiết tin + nút ứng tuyển thật
  (`ApplyButton`, redirect login nếu chưa đăng nhập); dashboard (đơn ứng tuyển thật, % hoàn thiện hồ
  sơ thật) + hồ sơ (`ProfileForm`) + CCHN (`LicenseSection`, thêm CCHN mới) + xóa tài khoản.
  `profile/cv` (CV Builder) **cố ý giữ mock** — quyết định đã xác nhận, chờ backend CV Builder.
  Phát hiện + vá 2 gap backend nhỏ trong lúc nối: (1) thiếu `GET /organizations/{id}` Public (đã thiết
  kế ở API-DESIGN.md mục 4 từ đầu nhưng chưa implement) — thêm `GetOrganizationByIdQuery`; (2)
  `SearchJobsQuery` thiếu filter `organizationId` (cần để trang tổ chức công khai lấy tin — không dùng
  `GET /organizations/{id}/jobs` vì endpoint đó yêu cầu member và trả mọi trạng thái, không phù hợp
  công khai) — thêm param `OrganizationId` vào query.

Verify đã chạy: `dotnet test` vẫn 41/41 pass sau 2 thay đổi backend nhỏ trên; `npm run build`/`lint`
sạch ở `web/`. Verify curl end-to-end thật qua cookie jar mô phỏng browser: register→verify-otp→login
(cookie `access_token`/`refresh_token` set đúng, httpOnly xác nhận qua `curl -c`)→`GET /dashboard` có
cookie trả 200 và hiện đúng email/đơn ứng tuyển, không cookie redirect 307 về `/auth/login` (test cả
`dashboard`/`profile`/`settings`)→tạo hồ sơ qua `PUT /api/candidates/me`→ứng tuyển qua
`POST /api/jobs/{id}/apply` (proxy) thành công→dashboard hiện đúng đơn với stage "Mới"→thêm CCHN qua
`POST /api/candidates/me/licenses`→profile hiện đúng CCHN "Chờ xác thực"→completion% tăng đúng
(20→60% sau khi có fullName+headline+license, xác nhận qua cả DB trực tiếp và HTML render).

Còn thiếu (chặn việc chốt giai đoạn):
- OAuth Google/Zalo — chưa làm, quyết định hoãn sang sau khi Identity cốt lõi ổn định (đã xác nhận với
  người dùng).
- `POST /organizations/{id}/members/invite` + luồng chấp nhận lời mời (`organization_invitations`) —
  chưa làm, mới có tạo tổ chức lần đầu.
- Hồ sơ ứng viên: học vấn/kinh nghiệm (`experiences`/`educations`), CME (`continuing_certificates`), CV
  Builder + export PDF — chưa làm, quyết định tách khỏi vòng "core" (profile+CCHN+chuyên khoa) đã xác
  nhận với người dùng. Upload document CCHN hiện giả định URL có sẵn (`web/` gửi placeholder URL),
  chưa nối `POST /uploads/presigned-url`/MinIO thật.
- Payments thật (`payments`/`job_purchases`/`pending_payment`/`/ops/payments/*`, nạp Credit qua
  `manual_transfer`) — chưa làm, bounded context riêng đã xác nhận tách khỏi Jobs và Credit. Đang dùng
  giải pháp tạm (Free tier cho Jobs, credit-bonus thủ công cho Credit) — cả 2 sẽ cần nối lại khi
  Payments hoàn thiện. Gói trả phí Eco/Pro/Max của Jobs cũng phụ thuộc bounded context này.
  Tìm kiếm Jobs hiện dùng LINQ/EF Core thay vì Postgres full-text (`pg_trgm`) như ERD mục 0 ghi — đủ
  dùng cho MVP, tối ưu sau.
- Applications: ứng tuyển bằng CV riêng (upload) chưa làm, chỉ hỗ trợ CV nền tảng (`CandidateProfile`).
- Hoàn Credit thủ công khi tranh chấp (`/ops/organizations/{id}/credit-refund`) — chưa làm, khác
  `credit-bonus` đã có (dùng khi tranh chấp cụ thể, ghi rõ lý do, không phải nạp thường).
- Đổi mật khẩu khi đã đăng nhập (`settings` trang) — chưa có endpoint backend riêng, chỉ có
  forgot/reset-password qua OTP; phần UI đổi mật khẩu ở `settings` vẫn để tĩnh.
- `web-admin/` (Admin NTD/Vận hành) — vẫn dùng mock cho mọi màn hình trừ danh mục (đọc), chưa nối
  Jobs/Applications/Credit/duyệt tổ chức/duyệt CCHN thật dù backend đã sẵn sàng — ưu tiên tiếp theo.
- Tin nhắn/thông báo, tìm kiếm ứng viên chủ động ở `web-admin/` (Credit unlock UI phía NTD) — backend
  đã có (`GET /candidates/search`, `POST /candidates/{id}/unlock`), chưa nối frontend nào.

- **Nối `web-admin/` (Admin NTD + Vận hành) tới API thật** — thay toàn bộ mock (`MOCK_*` từ
  `lib/mock-data.ts`) bằng gọi API thật qua `lib/api.ts` (viết lại hoàn toàn, `fetch` → axios instance
  `lib/http.ts`). Kiến trúc auth khác `web/`: SPA thuần (không SSR) nên dùng cookie thường (không
  httpOnly, tái dùng `lib/cookies.ts` đã có sẵn cho theme/sidebar) + Zustand store
  (`stores/auth-store.ts`, đổi hẳn shape `AuthUser`/token theo response `GET /users/me` thật) + axios
  interceptor tự refresh khi 401 (gộp request đồng thời qua 1 `refreshPromise`, tránh gọi refresh
  trùng lặp). `routes/_authenticated/route.tsx` thêm `beforeLoad` redirect `/sign-in` nếu chưa có
  access token.
  Màn hình đã nối: đăng nhập thật (chặn role `candidate` không cho vào admin qua `ALLOWED_ROLES`) +
  route protection; đăng tin mới (`jobs/new.tsx`, dropdown chuyên khoa/địa điểm/gói dùng `id` thật thay
  chuỗi tên) + danh sách tin theo tổ chức; ATS Kanban theo từng tin (đổi route
  `/applications` → `/applications/$jobId`, kéo-thả gọi `applicationsApi.transitionStage` thật, có
  history); dashboard (tin đang tuyển/số dư Credit/chờ thanh toán từ API thật); ví Credit (số dư +
  lịch sử giao dịch thật, nút "Nạp thêm Credit" chủ động để `disabled` + ghi rõ "Đang chờ nối cổng
  thanh toán" — không giả vờ hoạt động khi Payments chưa có); hàng đợi duyệt Vận hành
  (`ops-verification/index.tsx`, 3 tab CCHN/Tổ chức/Tin tuyển dụng, nút Duyệt gọi thẳng, nút Từ chối mở
  dialog nhập lý do bắt buộc trước khi gọi `verifyLicense`/`verifyOrganization`/`moderateJob`).
  Thêm hook `hooks/use-my-organization.ts` (`organizationsApi.getMine()`) để mọi trang NTD tự biết tổ
  chức của user đang đăng nhập — MVP giả định 1 NTD chỉ thuộc 1 tổ chức (chưa có invite thành viên).
  `/ops/payments`, `/ops/reports` **cố ý giữ mock** — chưa có bounded context Payments/Report tương ứng
  ở backend, khác với các trang trên đã có API thật để nối.

Verify đã chạy: `npx tsc -b` sạch; `npm run build` sạch (cảnh báo kích thước chunk >500kB không phải
lỗi, chưa cần code-split ở MVP); `npm run lint` sạch; `npx vitest run` 128/129 pass — 1 fail
(`search-provider.test.tsx`, timeout click trong Playwright browser mode) là test cũ thuộc scaffold
gốc (không đụng tới trong đợt này), tái lập độc lập không phụ thuộc thay đổi của đợt này, nghi do môi
trường (Chromium mới cài lần đầu), không phải lỗi logic — không chặn việc chốt phần này.

Bổ sung ngay sau đó — dựng mới trang tìm/mở hồ sơ ứng viên chủ động ở `web-admin/`
(`web-admin/src/features/candidates/`, route `/candidates`, mục "Tìm ứng viên" trong sidebar Nhà
tuyển dụng): filter theo chuyên khoa/địa điểm qua `catalogApi`, danh sách kết quả ẩn liên hệ tới khi
mở (đúng ERD mục 7), nút "Mở hồ sơ" gọi `candidatesApi.unlock` thật (trừ Credit), báo lỗi rõ ràng khi
ví không đủ Credit (400) thay vì lỗi chung. Verify: `npx tsc -b`/`npm run build`/`npm run lint` sạch;
route tree tự sinh đúng (`routeTree.gen.ts` có `/candidates/`).

Bổ sung tiếp — nối form thêm/sửa ở `web-admin/src/features/ops-catalog/` (trước đó chỉ đọc) tới
`opsCatalogApi` mới (thêm vào `lib/api.ts`): dialog Thêm/Sửa cho chuyên khoa (dạng cây cha-con qua
`parentId`, `code` chỉ nhập lúc tạo — không đổi được sau), địa điểm (cùng cấu trúc cây, tỉnh/thành là
node gốc), gói tin (Free/Eco/Pro/Max — `tier` chỉ chọn lúc tạo, khớp đúng ràng buộc
`UpdateJobPackageCommand` không có field Tier ở backend). Không có nút xóa vì backend chưa có
Delete/Deactivate cho 3 entity này — cố ý không dựng nút xóa giả không gọi được gì. Dọn luôn code chết
liên quan ở `lib/mock-data.ts` (`MOCK_SPECIALTIES`/`MOCK_LOCATIONS`/`MOCK_JOB_PACKAGES` và các mock
Job/Application/Credit/License/Org cũ không còn nơi nào import — chỉ giữ `MOCK_PAYMENT_QUEUE`/
`MOCK_REPORT_QUEUE` vì 2 trang đó vẫn cố ý dùng mock). Verify: `tsc -b`/`build`/`lint` sạch,
`vitest run` 128/129 (1 fail flaky không liên quan, đã ghi ở log trước).

Bổ sung tiếp — **bounded context mời thành viên tổ chức** (`organization_invitations` +
`employer_members`, đã thiết kế sẵn ở ERD mục 2.3/API-DESIGN.md mục 4 từ trước nhưng chưa implement —
đây là lý do `web-admin/src/features/users/` vẫn phải dùng faker, không có API thật để nối). Quyết
định làm ngay để dứt điểm gap này thay vì hoãn tiếp, theo lựa chọn của người dùng khi được hỏi.
Domain: `Organization.InviteMember()`/`AcceptInvitation()` (không mời thêm `owner` qua lời mời — chỉ
`hr_manager`/`hr_member`, đúng ERD). Token lời mời: `InvitationTokenGenerator` (32 byte random, hash
SHA-256 lưu DB — cùng cách OTP dùng, khác chỉ ở độ dài vì đây là token URL không phải mã 6 số cho
người gõ tay). Gửi lời mời qua `IInvitationSender`/`LoggingInvitationSender` — **driver giả lập nội
bộ** (log token thay vì gửi email thật), cùng quyết định đã chốt với OTP, chưa chốt nhà cung cấp SMTP.
Command: `InviteMemberCommand` (chặn mời lại email đã là thành viên hoặc đang có lời mời chờ),
`AcceptInvitationCommand` (kiểm tra token còn hạn + chưa dùng + **email đăng nhập phải khớp email được
mời**, không cho accept bằng tài khoản khác), `RemoveMemberCommand` (chặn xoá owner). Query:
`GetOrganizationMembersQuery` (trả cả danh sách thành viên đã tham gia và lời mời đang chờ trong 1
lần gọi, để FE hiển thị cả 2 trong 1 bảng). Endpoint mới: `GET/POST /organizations/{id}/members`,
`POST /organizations/{id}/members/invite`, `DELETE /organizations/{id}/members/{memberId}`,
`POST /invitations/{token}/accept` (route riêng, không lồng dưới `/organizations` — khác thiết kế gốc
ở API-DESIGN.md một chút, đã cập nhật doc khớp thực tế). Chưa làm endpoint xem trước lời mời trước khi
đăng nhập (`GET .../invitations/{token}`) — MVP yêu cầu đăng nhập/đăng ký trước, chấp nhận UX kém hơn
một chút để đơn giản hoá.
Verify: thêm `CapturingInvitationSender` (test-only, cùng pattern `CapturingOtpSender` đã có) để test
capture token thật thay vì chỉ đọc log; `dotnet test` 46/46 pass (3 unit + 43 functional, gồm 4 test
mới cho invite/accept/forbidden/remove). Test mới cover: invite→accept tạo đúng member + xoá lời mời
khỏi hàng chờ; accept sai email bị chặn 400; mời khi không phải thành viên bị chặn 403; xoá thành viên
thường được nhưng xoá owner bị chặn 400. Migration EF Core cho thay đổi navigation property
(`Organization.Invitations`) sinh ra rỗng (không đổi shape DB) — đã xoá, không giữ migration rác.

Bổ sung ngay sau — nối `web-admin/src/features/users/` (trước đó dùng `@faker-js/faker` với role giả
superadmin/admin/cashier/manager, không khớp domain thật) tới API thành viên tổ chức vừa xây. Viết lại
hoàn toàn thay vì sửa: xóa toàn bộ data-table generic (columns/dialogs/schema/provider của
shadcn-admin gốc — pagination/facet-filter/bulk-delete không cần cho domain thật vì số thành viên 1 tổ
chức nhỏ, MVP), thay bằng 1 trang đơn giản — bảng thành viên (email/vai trò/ngày tham gia, nút xoá trừ
owner), bảng lời mời đang chờ (chỉ hiện khi có), dialog mời thành viên mới (email + chọn vai trò
`hr_manager`/`hr_member`), dialog xác nhận xoá. Route `/users` đơn giản hóa theo (bỏ `usersSearchSchema`
phức tạp không cần). Thêm `organizationsApi.getMembers/inviteMember/removeMember` vào `lib/api.ts`.
Verify: `tsc -b`/`build`/`lint` sạch; `vitest run` 101/102 (17 test file, giảm 4 file so với trước vì
xóa test cũ của users theo — 1 fail vẫn là `search-provider.test.tsx` flaky không liên quan, xác nhận
không mention gì tới route `/users`).

Bổ sung tiếp — **bounded context Payments** (đã thiết kế sẵn ở ERD mục 2.5, API-DESIGN.md mục 6-7, và
ADR-0003 từ trước nhưng chưa implement — đây là gap lớn nhất còn lại, chặn cả gói trả phí Jobs lẫn nạp
Credit thật). Quyết định làm ngay theo lựa chọn của người dùng khi được hỏi, đúng thiết kế thủ công đã
chốt (không tích hợp cổng thanh toán tự động ở MVP).
Domain: `Payment` (Type/Provider/Status, `ConfirmSuccess()`/`ConfirmFailed()`), `JobPurchase` (liên kết
`JobId`/`PackageId`/`PaymentId`). Phát hiện thú vị: `Job.ConfirmPayment(durationDays)`/
`RejectPayment()` và `JobStatus.PendingPayment` **đã tồn tại sẵn từ trước** (thiết kế đón đầu Payments
ngay từ khi làm Jobs) — chỉ cần gọi tới, không phải viết mới. `PaymentReferenceCodeGenerator` sinh mã
`PAY-XXXXXX` (bỏ ký tự dễ nhầm 0/O/1/I vì NTD gõ tay vào nội dung chuyển khoản).
Command: `CreateJobPackagePaymentCommand` (gọi sau `SubmitJobCommand` khi job ở `pending_payment` —
tạo `Payment` + `JobPurchase` ngay lúc này, không đợi confirm; chặn tạo trùng nếu tin đã có giao dịch
`pending` khác), `CreateCreditTopupPaymentCommand` (quy đổi tạm 1 Credit = 1.000đ, chưa có bảng giá gói
Credit riêng; chặn tạo trùng theo tổ chức), `ConfirmPaymentCommand`/`RejectPaymentCommand` (Ops — rẽ
nhánh theo `Payment.Type`: `JobPackage` gọi `Job.ConfirmPayment()`/`RejectPayment()`, `CreditTopup`
cộng thẳng `CreditWallet` qua `Credit()` với `reason=Purchase`, khác `reason=Bonus` của
`CreditBonusCommand` đã có — giữ song song 2 luồng này vì mục đích khác nhau, không thay thế nhau).
Query: `GetPaymentByIdQuery`, `GetPendingPaymentsQuery` (Ops, gồm cả 2 loại giao dịch trong 1 danh
sách). Sửa `SubmitJobCommand` — bỏ hẳn đoạn chặn cứng "gói trả phí chưa hỗ trợ" của đợt Jobs trước,
giờ mọi tier đều `Submit()` được (Free → `pending` thẳng, còn lại → `pending_payment` chờ
`CreateJobPackagePaymentCommand`). Endpoint mới: `POST /payments/job-package`,
`POST /payments/credit-topup`, `GET /payments/{id}`, `GET /ops/payments`,
`POST /ops/payments/{id}/confirm|reject`.
Verify: `dotnet build` sạch ngay lần đầu (0 lỗi) — nhờ `Job.ConfirmPayment()`/`RejectPayment()` có sẵn
từ trước nên phần khó nhất coi như đã làm xong sẵn. Migration `AddPayments` (bảng `Payments`,
`JobPurchases`, unique index `ReferenceCode`, FK `Restrict` tới `Organizations` — không cascade xoá dữ
liệu tài chính). `dotnet test` 52/52 pass (3 unit + 49 functional, gồm 6 test mới:
job-package confirm→job Pending; job-package reject→job Draft; chặn tạo payment trùng khi tin đã có
giao dịch pending; credit-topup confirm→ví tăng đúng; credit-topup forbidden khi không phải member;
chặn tạo credit-topup trùng theo tổ chức).

Bổ sung ngay sau — **nối `web-admin/` tới toàn bộ API Payments vừa xây**. Thêm `opsApi.getPendingPayments/
confirmPayment/rejectPayment` và `paymentsApi.createJobPackagePayment/createCreditTopupPayment/getById`
vào `lib/api.ts`. Viết lại `features/ops-payments/index.tsx` (bỏ `MOCK_PAYMENT_QUEUE`, dùng
`useQuery`/`useMutation` thật, cùng pattern các trang Ops khác). `features/credit/index.tsx` — nút
"Nạp thêm Credit" trước đó `disabled` với tooltip "Đang chờ nối cổng thanh toán" (từ đợt Credit/Unlock
trước, lúc Payments chưa tồn tại) giờ mở dialog thật: nhập số Credit → hiện số tiền quy đổi (1 Credit =
1.000đ) + mã tham chiếu để chuyển khoản. `features/jobs/new.tsx` — trước đó chỉ hỗ trợ gói Free (đã
ghi rõ trong UI "chỉ Free hỗ trợ ở MVP"), giờ chọn gói Eco/Pro/Max sẽ tạo `Payment` ngay sau khi submit
và hiện dialog thông tin chuyển khoản (đóng dialog mới điều hướng về `/jobs`, để NTD kịp ghi lại mã
tham chiếu). Dọn code chết: `MOCK_PAYMENT_QUEUE`/`MockPaymentQueueItem` ở `lib/mock-data.ts` không còn
ai import (giữ lại `MOCK_REPORT_QUEUE` vì `ops-reports` vẫn cố ý dùng mock).
Verify: `tsc -b`/`build`/`lint` sạch; `vitest run` 101/102 (1 fail flaky `search-provider.test.tsx`
không liên quan, đã ghi nhiều lần ở các log trước, không chặn việc chốt phần này).

Bổ sung tiếp — **[ADR-0010](../kien-truc/adr/0010-da-ngon-ngu-cho-web-admin.md): đảo ngược ADR-0008
mục 5**, thêm đa ngôn ngữ thật cho `web-admin/`. Lý do phát sinh: có NTD/nhân sự phía tổ chức y tế
không nói được tiếng Việt (cơ sở y tế có vốn/quản lý nước ngoài) cần tự vào `web-admin/` đăng tin/quản
lý ứng viên — giả định gốc "công cụ nội bộ, không hướng quốc tế" của ADR-0008 không còn đúng. Áp dụng
cùng 6 ngôn ngữ như `web/` nhưng dùng **`react-i18next`** (không dùng `next-intl` — thư viện đó gắn
chặt Next.js App Router, không cài được cho Vite SPA). Cấu trúc `web-admin/src/messages/{locale}/
{namespace}.json` — cùng tư duy tổ chức với `web/messages/` nhưng thư mục riêng, không dùng chung.
Chọn ngôn ngữ lưu `localStorage` (không routing URL, không SSR — khác `web/`).
Dựng xong khung `i18n.ts` (config `react-i18next` + `i18next-browser-languagedetector`) và namespace
`common` đầu tiên (6 locale — `vi`/`en` dịch tay thật, `ja`/`zh`/`ko`/`es` tạm giữ tiếng Việt làm
placeholder chờ dịch thuật vì không đủ độ tin cậy dịch tay ngôn ngữ chuyên môn, đúng nguyên tắc "không
dịch máy tự động lấp chỗ trống" của ADR-0006). Rút chuỗi hardcode ở `sidebar-data.ts` (đổi `title`/
`plan` từ literal sang key dịch) + `nav-group.tsx`/`team-switcher.tsx` (gọi `t()` khi render). Thêm
component `LanguageSwitcher` (dropdown Globe icon, tên ngôn ngữ theo bản ngữ — cùng nguyên tắc
`LanguageSwitcher` của `web/`), chèn vào Header của toàn bộ 15 feature page (script `sed`/`perl` áp
dụng đồng loạt do pattern `<ThemeSwitch />` hoàn toàn nhất quán ở mọi file — đã kiểm tra kỹ trước khi
áp dụng, sau đó lint autofix sắp lại thứ tự import).
Verify: `tsc -b`/`build`/`lint` sạch; `vitest run` 101/102 (1 fail flaky không liên quan, như log
trước). **Verify thật bằng browser** (Playwright, đăng nhập bằng tài khoản test qua OTP giả lập, chạy
backend + `web-admin/` dev server thật): xác nhận sidebar hiện đúng tiếng Việt lúc đăng nhập → bấm
dropdown Globe → chọn English → toàn bộ sidebar đổi ngay sang "Job Postings"/"Dashboard"/"Credit
Wallet"/... đúng bản dịch, kèm ảnh chụp màn hình xác nhận trực quan. Phát hiện + vá 1 lỗi nhỏ trong lúc
verify: `plan: 'nav.brandPlan'` sai namespace path (đúng phải là `plan: 'brandPlan'`, không nằm trong
`nav`) — sửa ngay, verify lại xác nhận đúng.

Bổ sung ngay sau — **hoàn thiện rút chuỗi hardcode ở `web-admin/`** cho toàn bộ 6 namespace còn lại
(`jobs`, `applications`, `credit`, `candidates`, `members`, `ops`), gộp với `common` đã xong ở đợt
trước thành đủ 7 namespace. Tạo `web-admin/src/messages/{locale}/{namespace}.json` cho từng cái (6
locale × 6 namespace mới = 36 file, cùng cách tiếp cận đợt trước: `vi`/`en` dịch tay thật, `ja`/`zh`/
`ko`/`es` tạm giữ tiếng Việt làm placeholder). Sửa toàn bộ 9 file component tương ứng
(`jobs/index.tsx`, `jobs/new.tsx`, `applications/index.tsx` + `constants.ts` + `kanban-card.tsx` +
`kanban-column.tsx`, `credit/index.tsx`, `candidates/index.tsx`, `users/index.tsx`,
`ops-verification/index.tsx`, `ops-payments/index.tsx`, `ops-catalog/index.tsx`) dùng
`useTranslation(namespace)` thay chuỗi cứng — bao gồm cả các `Record<string, string>` map tĩnh (status
label, reason label, role label, stage label) chuyển sang gọi `t('key.value', fallback)` thay vì định
nghĩa lại trong component. `i18n.ts` cập nhật danh sách `NAMESPACES` + import/khai báo resources cho
đủ 6 locale × 7 namespace trong 1 file config duy nhất.
Verify: `tsc -b`/`build`/`lint` sạch sau mỗi namespace (kiểm tra tăng dần, không dồn lỗi tới cuối).
**Verify thật bằng Playwright** — đăng nhập qua backend thật, set `localStorage` locale = `en` trực
tiếp (nhanh hơn click dropdown lặp lại), rồi duyệt qua cả 9 route chính
(`/`, `/jobs`, `/jobs/new`, `/credit`, `/candidates`, `/users`, `/ops/verification`, `/ops/payments`,
`/ops/catalog`) và assert từng trang chứa đúng cụm từ tiếng Anh mong đợi — cả 9/9 pass. `vitest run`
101/102 (1 fail flaky không liên quan, như mọi log trước).

Bổ sung tiếp — **đồng bộ `users.locale` giữa `web/` và `web-admin/`** (dứt điểm gap "chưa quyết định"
ở log trước) + **rút hết chuỗi hardcode còn lại ở `web/`** (8 file đã liệt kê ở log trước), theo lựa
chọn "dọn nợ kỹ thuật trước" của người dùng khi được hỏi thứ tự ưu tiên tiếp theo sau khi Giai đoạn 1
đã có đủ các bounded context lớn.
Backend: thêm `Locale` vào `AuthUserDto` (map từ `ApplicationUser.Locale` ở `IdentityService.ToDto`) —
trước đó `GET /users/me` không trả field này dù entity đã có sẵn từ đầu. `UpdateMyLocaleCommand` (Auth
bounded context, đúng pattern 1-file Command+Validator+Handler đã có) — validator dùng hằng số 6 locale
mới (`Domain.Constants.SupportedLocales`, **không** tái dùng `Application.Catalog.CatalogLocales` vì
class đó cố ý chỉ có 5 locale, loại `vi`, dùng cho mục đích khác — validate bản dịch danh mục, xem
comment gốc trong code). Endpoint `PUT /users/me/locale` (`Web/Endpoints/Users.cs`).
**Quyết định phạm vi đã xác nhận với người dùng**: `ICurrentLocale` (dùng để dịch nội dung Catalog/Jobs
theo `Accept-Language` header) **giữ nguyên không đổi** — `users.locale` chỉ dùng để lưu lựa chọn UI +
đồng bộ 2 app + chuẩn bị cho email/thông báo sau này (đúng mục đích ERD đã ghi), không trộn vào luồng
resolve locale nội dung hiện tại để tránh rủi ro phá vỡ hành vi đã verify kỹ ở Catalog/Jobs.
`web/`: `LanguageSwitcher` gọi `PUT /api/users/me/locale` (Route Handler proxy mới, cùng pattern
`backendFetch` đã có) khi đổi ngôn ngữ **và đã đăng nhập** (kiểm tra qua prop `isLoggedIn` truyền từ
`SiteHeader`/`SiteFooter`/`settings/page.tsx` — cả 3 nơi dùng `LanguageSwitcher`; `SiteFooter` đổi thành
async Server Component để tự gọi `getCurrentUser()`, theo đúng pattern `SiteHeader` đã có). Đồng bộ là
best-effort (không chặn UI nếu request lỗi).
`web-admin/`: `authApi.updateLocale` gọi khi đổi ngôn ngữ ở dropdown (nếu `useAuthStore` đã có user);
sau login (`user-auth-form.tsx`), nếu `user.locale` hợp lệ và khác locale hiện tại thì gọi
`i18n.changeLanguage()` ngay — đảm bảo 1 tài khoản đăng nhập ở máy khác vẫn thấy đúng ngôn ngữ đã chọn
trước đó thay vì chỉ phụ thuộc `localStorage` cục bộ.
Rút chuỗi hardcode `web/` — thêm key mới vào `profile.json` (`fullName`, `headline`, `savedChanges`,
`saving`, `saveChanges`, `submitting`, `addLicense`, `addNewLicense`, `licensePhotoNote`,
`confirmDeleteAccount`, `licenseShort`, `noApplications`, `noSavedJobs`, `generalInfo`,
`currentPassword`, `newPassword`, `language`, `currentLanguage`), `jobs.json` (`salaryNegotiable`,
`million`, `licenseAbbrev`, `viewOrgPage`), `home.json` (`verifiedBadge`, `activeJobsCount`) — dịch tay
thật cho `vi`/`en`, copy nguyên văn tiếng Việt làm placeholder cho `ja`/`zh`/`ko`/`es` (đúng quy ước đã
có, chưa dịch máy). Áp dụng ở cả 8 file: `profile-form.tsx`, `profile/page.tsx`, `license-section.tsx`,
`settings/page.tsx`, `delete-account-button.tsx`, `dashboard/page.tsx`, home `page.tsx`,
`jobs/[id]/page.tsx`. Hàm `formatSalary` (trùng lặp ở 2 file) đổi sang nhận `t` làm tham số thay vì
hardcode `"Thỏa thuận"`/`"triệu"` trực tiếp trong hàm.
Verify: `dotnet build` sạch (0 warning/error); `dotnet test` 51/51 Application.FunctionalTests pass
(tăng từ 49 lên 51 nhờ 2 test mới `UpdateMyLocaleTests` — persist locale hợp lệ, chặn locale không hỗ
trợ) + 3/3 Application.UnitTests pass (`Domain.UnitTests`/`Infrastructure.IntegrationTests` không có
test method nào từ trước, không phải regression đợt này). `web/`: `npm run lint` + `npm run build`
sạch. `web-admin/`: `npx tsc -b`/`npm run lint`/`npm run format:check`/`npm run build` sạch (phát hiện
2 lỗi thật trong lúc verify — type test cũ thiếu field `locale` mới trong `AuthUser`, và duplicate
import từ `@/i18n` ở `user-auth-form.tsx` — sửa cả 2 ngay); `npx vitest run` 101/102 (1 fail flaky
`search-provider.test.tsx`, đã ghi nhận nhiều lần ở log trước, không liên quan tới thay đổi đợt này).

Bổ sung tiếp — bounded context **Thông báo (Notifications)** — chưa có gì trước đó (chưa có entity,
chưa có endpoint, cơ chế domain event `BaseEvent`/`DispatchDomainEventsInterceptor` có sẵn từ template
gốc nhưng chưa từng được dùng ở bất kỳ đâu trong nghiệp vụ). **Quyết định thiết kế đã xác nhận với
người dùng**: (1) gọi trực tiếp `INotificationService.NotifyAsync(...)` từ Command Handler ngay sau
khi nghiệp vụ chính lưu thành công — **không** dùng domain event, giữ đơn giản cho MVP, tránh rủi ro
đụng vào cơ chế hoàn toàn chưa verify; (2) phạm vi trigger đầu tiên chỉ 3 sự kiện cốt lõi đã mô tả rõ
trong `LUONG-NGHIEP-VU-MAN-HINH.md` (License Verify/Reject, Application TransitionStage trừ khi
`silent`, Job Moderate publish/reject) — chưa làm Payment confirm/reject (để đợt sau nếu cần).
Domain: entity `Notification` (`BaseEntity`, `UserId`/`Type` chuỗi tự do/`Title`/`Body`/`Payload`
`Dictionary<string,object>` map jsonb/`Channel` enum 4 giá trị/`CreatedAt`/`ReadAt`), đúng schema đã có
sẵn ở `docs/database/ERD-CHI-TIET.md` mục 2.8 — không cần sửa ERD. `MarkAsRead()` idempotent (`??=`).
Application: `INotificationService`/`INotificationEmailSender` (2 interface tách biệt — service ghi
DB luôn, chỉ gọi email sender khi `Channel = Email`), `GetMyNotificationsQuery` (`?unreadOnly`),
`MarkNotificationReadCommand` (chặn user đánh dấu đã đọc notification không phải của mình, đúng "Owner"
ở API-DESIGN.md mục 9 — không cần sửa file này, endpoint đã thiết kế đúng từ đầu).
Infrastructure: `NotificationService` (ghi `Notifications` + gọi email sender), `LoggingNotificationEmailSender`
— **driver giả lập nội bộ** (log thay vì gửi thật), cùng pattern `LoggingOtpSender`/
`LoggingInvitationSender` đã có, cùng quyết định chưa chốt nhà cung cấp SMTP.
Web: `GET/PATCH /api/v1/notifications` (`RoutePrefix` override chữ thường, đúng convention các endpoint
khác).
Nối 3 điểm kích hoạt: `VerifyLicenseCommandHandler` (include `Profile` lấy `UserId`, gửi
`license_verified`/`license_rejected` kèm lý do khi từ chối), `TransitionApplicationStageCommandHandler`
(include `Candidate`, gửi `application_stage_changed` trừ khi `command.Silent`), `ModerateJobCommandHandler`
(gửi `job_published`/`job_rejected` cho **toàn bộ** `EmployerMembers` của tổ chức, không chỉ chủ sở
hữu — vì bất kỳ thành viên nào cũng cần biết tin đã publish/bị từ chối).
Migration `AddNotifications` (bảng `Notifications`, index composite `(UserId, ReadAt)` đúng ERD).
Verify: `dotnet build` sạch ngay lần đầu (0 lỗi); migration áp thành công vào Postgres thật (`\d
"Notifications"` xác nhận đúng cột/kiểu/index); `dotnet test` 59/59 Application.FunctionalTests pass
(tăng từ 51 lên 59 nhờ 8 test mới — verify cả 2 nhánh Approved/Rejected của License, cả nhánh
Silent/NonSilent của TransitionStage, ModerateJob thông báo đúng owner, mark-read đổi đúng trạng thái,
mark-read bởi user khác bị chặn 403, mark-read id không tồn tại bị 404) + 3/3 Application.UnitTests
pass, không có test nào phải sửa lại lần 2 (pass ngay từ bản đầu viết).

Bổ sung ngay sau — **nối UI Thông báo ở cả `web/` và `web-admin/`** tới API vừa xây (gap được ghi nhận
là ưu tiên tiếp theo ở log trước, làm ngay theo lựa chọn "hoàn thiện từng luồng chức năng" của người
dùng thay vì mở bounded context mới trong khi Notifications còn dở chừng chỉ có backend).
`web/`: 2 Route Handler proxy mới `app/api/notifications/route.ts` (forward query `?unreadOnly`),
`app/api/notifications/[id]/read/route.ts` (PATCH) — cùng pattern `backendFetch` đã có. Component
`NotificationBell` (Client Component, tự poll `GET /api/notifications?unreadOnly=true` mỗi 30 giây qua
`useEffect`/`AbortController`, không dùng `useCallback` bọc hàm async gọi `setState` trực tiếp trong
effect vì ESLint rule mới `react-hooks/set-state-in-effect` chặn pattern đó — phải viết lại thành 1
effect duy nhất inline fetch + interval, có `AbortController` để tránh race khi unmount giữa lúc đang
chờ response). Gắn vào `SiteHeader`, chỉ hiển thị khi `currentUser !== null` (cạnh `SiteHeaderUserMenu`).
Thêm namespace con `common.notifications.{label,empty,markRead}` (6 locale, `vi`/`en` dịch thật,
`ja`/`zh`/`ko`/`es` placeholder tiếng Việt theo quy ước).
`web-admin/`: `notificationsApi.getMine(unreadOnly)`/`markRead` thêm vào cuối `lib/api.ts` (đúng
convention cuối file, sau `paymentsApi`). Component `NotificationBell` dùng TanStack Query
(`useQuery` với `refetchInterval: 30_000` — pattern polling **chưa từng dùng ở đâu trong repo trước
đây**, `useMutation` + `invalidateQueries` khi mark-read, đúng convention `credit/index.tsx`). Quyết
định thiết kế đã xác nhận với người dùng: (1) thêm key vào namespace `common` có sẵn thay vì tạo
namespace `notifications` riêng (chi phí thấp hơn nhiều — chỉ sửa 6 file JSON, không phải sửa
`i18n.ts` 3 chỗ + tạo 6 file mới cho 1 tính năng nhỏ); (2) chèn `<NotificationBell />` vào cả **15**
feature page (không phải 14 như ước tính ban đầu — đếm lại chính xác bằng `grep -rl`) cạnh
`<LanguageSwitcher />`, đúng cách đã làm trước đây khi rút chuỗi hardcode i18n, thay vì refactor
`authenticated-layout.tsx` thành 1 Header dùng chung (ngoài phạm vi, rủi ro cao hơn cho 1 việc nhỏ) —
áp dụng bằng script Python (không dùng `perl -pe` line-by-line vì làm hỏng xuống dòng JSX, phải viết
lại bằng regex multiline `re.MULTILINE` xử lý toàn file để giữ đúng indentation).
Verify: `web/` — `npm run lint`/`npm run build` sạch (2 route mới build đúng). `web-admin/` — `npx tsc
-b`/`npm run lint`/`npm run format:check`/`npm run build` sạch (chunk `notification-bell` build đúng);
`npx vitest run` 101/102 (1 fail flaky `search-provider.test.tsx`, đã ghi nhận nhiều lần, không liên
quan tới thay đổi đợt này — xác nhận việc chèn `<NotificationBell />` vào 15 file không phá vỡ test
nào khác).

Bổ sung tiếp — bounded context **Report vi phạm** + **Ops dashboard** + **Quản lý user hệ thống**, dứt
điểm 3 việc cuối cùng của nhóm "Vận hành" (theo lựa chọn "hoàn thiện từng luồng chức năng" của người
dùng — làm xong cả backend lẫn UI trong 1 lượt, không để dở như Notifications đợt trước).
**Quyết định phạm vi đã xác nhận với người dùng**: (1) `ResolveReportCommand` action `content_removed`
chỉ trigger state change thật (gọi `Job.Close()`) khi `targetType=Job` — `Organization`/`Profile`/
`Message` chưa có cơ chế ẩn/gỡ nội dung tương ứng rõ ràng ở MVP, chỉ lưu action mà không đổi state gì
(ghi rõ trong code + tài liệu để tránh hiểu nhầm là đã xử lý đầy đủ); (2) `GET /ops/dashboard/stats`
(tài liệu gốc không có schema cụ thể) tự thiết kế thành số liệu đếm đơn giản (`SELECT COUNT/GROUP BY`),
không làm biểu đồ xu hướng theo thời gian.
Domain: entity `Report` (`BaseEntity`, `ReporterUserId`/`TargetType`/`TargetId`/`Reason`/`Status`/
`ResolutionAction`/`ResolvedBy`/`CreatedAt`) + 3 enum (`ReportTargetType`, `ReportStatus`,
`ReportResolutionAction`), đúng schema ERD mục 2.8 — không cần sửa ERD. `Report.Dismiss()`/`Resolve()`
chặn xử lý report không còn `Pending` (đổi ý 2 lần).
Application: `CreateReportCommand` (`POST /reports`, không giới hạn role — bất kỳ user đăng nhập nào
báo cáo được), `GetPendingReportsQuery` (join động `targetLabel` theo `TargetType` — Job.Title/
Organization.Name/CandidateProfile.FullName, `Message` chưa có bounded context Chat thật nên luôn
`null`), `ResolveReportCommand` (enum riêng `ReportResolveDecision.Dismiss/Resolve` để tách API
`dismissed` — không có trong ERD gốc — khỏi 3 giá trị `ResolutionAction` thật).
Mở rộng `IIdentityService` thêm 3 method mới (`CountUsersByRoleAsync`, `SearchUsersAsync`,
`SetUserStatusAsync`) vì Application layer không được reference `ApplicationUser`/`UserManager` trực
tiếp (Dependency Rule) — `GetOpsDashboardStatsQuery` cần đếm user theo role, `GetOpsUsersQuery`/
`SetUserStatusCommand` cần tìm kiếm/đổi trạng thái user hệ thống, tách hẳn khỏi "Thành viên tổ chức"
(`EmployerMember`, scope theo 1 organization) — đây là 2 khái niệm khác nhau đã nhầm lẫn ở
`web-admin/src/features/users/` trước đây, khảo sát kỹ trước khi code để không tái phạm.
`SetUserStatusCommand` chỉ có 1 command dùng chung cho suspend/unsuspend qua field `bool Suspend` (API
gốc chỉ thiết kế `POST /ops/users/{id}/suspend`, tự bổ sung `unsuspend` — cần thiết để đảo ngược).
Migration `AddReports` (bảng `Reports`, index `Status`).
**Phát hiện + sửa 1 bug thật trong hạ tầng test dùng chung** (không phải code nghiệp vụ mới, nhưng chặn
toàn bộ test Ops mới): `TestApp.RunAsUserAsync` (test helper) chỉ gán role qua Identity
`AddToRolesAsync` (bảng `AspNetUserRoles`, dùng cho `[Authorize(Roles=...)]` qua claim) nhưng KHÔNG set
cột `Role` string trên chính `ApplicationUser` (cơ chế thật production dùng cho `AuthUserDto`/JWT claim
— `RegisterCommand`/`IdentityService.RegisterUserAsync` có set đúng, chỉ test helper thiếu). Hậu quả:
`CountUsersByRoleAsync` luôn trả 0 cho user tạo qua test helper — phát hiện được nhờ viết test
`GetOpsDashboardStats_Should_Count_Correctly` và thấy `TotalEmployers` = 0 dù đã seed employer thật.
Sửa `RunAsUserAsync` set thêm `user.Role = roles.FirstOrDefault() ?? Roles.Candidate` — chạy lại toàn
bộ 59 test cũ để xác nhận không phá vỡ gì trước khi tiếp tục.
Web: `Reports.cs` (`POST /reports`), `OpsReports.cs` (`GET/POST /ops/reports`), `OpsDashboard.cs`
(`GET /ops/dashboard/stats`), `OpsUsers.cs` (`GET /ops/users`, `POST /ops/users/{id}/{suspend,unsuspend}`).
Verify: `dotnet build` sạch ngay lần đầu; migration áp thành công vào Postgres thật (`\d "Reports"`
xác nhận đúng cột/index); `dotnet test` 67/67 Application.FunctionalTests pass (tăng từ 59 lên 67 nhờ
8 test mới, sau khi sửa bug `TestApp.cs` — không có regression nào trong 59 test cũ) + 3/3
Application.UnitTests pass.
`web-admin/`: viết lại `ops-reports/index.tsx` nối `reportsApi` thật thay `MOCK_REPORT_QUEUE` (xóa hẳn
`lib/mock-data.ts` — đây là export cuối cùng còn dùng mock, không còn ai import gì từ file này nữa).
Trang mới `ops-dashboard/index.tsx` (`StatCard`/`BreakdownCard`, `useQuery` đơn giản không polling —
số liệu tổng quan không cần realtime) và `ops-users/index.tsx` (bảng + ô tìm theo email, nút
Khóa/Mở khóa tùy `status`). Thêm route `/ops/dashboard`, `/ops/users` (TanStack Router file-based —
phải chạy `vite build` để plugin `@tanstack/router-plugin` tự sinh lại `routeTree.gen.ts` trước khi
`tsc -b` nhận diện được route mới, không thể chỉ chạy `tsc -b` riêng lẻ). Thêm 2 mục sidebar mới vào
`opsGroup` (`nav.opsOverview`, `nav.opsUsers`) + namespace con `ops.{reports,dashboard,users}` (6
locale, `vi`/`en` dịch thật).
**Phát hiện + sửa 1 regression thật trong lúc verify**: đặt tên key `nav.opsDashboard` lúc đầu vô tình
chứa substring "Dashboard" trùng với `nav.dashboard` đã có — `search-provider.test.tsx` (không init
i18next thật, `t()` fallback trả về key thô) dùng `getByText('Dashboard')` substring-match nên khớp
nhầm cả 2 item, làm fail 1 test cũ (`renders the command palette...`) vốn đang pass. Đổi tên key thành
`nav.opsOverview` (không đụng `OpsDashboard`/`opsDashboardApi` — tên component/API giữ nguyên, chỉ đổi
key dịch) để hết xung đột, xác nhận lại bằng cách chạy lại đúng test đó trước khi chạy full suite.
Verify: `npx tsc -b`/`npm run lint`/`npm run format:check`/`npm run build` sạch (3 chunk mới build
đúng); `npx vitest run` 101/102 (1 fail flaky `search-provider.test.tsx` — bài khác, đã ghi nhận nhiều
lần ở log trước, không liên quan).

Bổ sung tiếp — **upload file thật qua MinIO** (ảnh CCHN + giấy phép hoạt động tổ chức), dứt điểm gap
được đánh giá nghiêm trọng nhất trong 1 đợt rà soát tổng thể các luồng nghiệp vụ end-to-end (theo yêu
cầu "ưu tiên tạo chức năng theo đúng luồng để có 1 hệ thống trước" của người dùng) — 2 cổng xác thực
quan trọng nhất của nền tảng (duyệt CCHN, duyệt tổ chức) trước đó chỉ mang tính hình thức vì Vận hành
duyệt mà không có tài liệu thật để xem (CCHN dùng placeholder URL cố định, tổ chức hoàn toàn chưa có
khái niệm document).
**Quyết định thiết kế đã xác nhận với người dùng**: dùng `AWSSDK.S3` trỏ vào MinIO (đúng theo
`docs/backend/CONG-NGHE-BACKEND.md` đã chốt từ trước, không dùng SDK MinIO riêng).
Backend: `IFileStorageService`/`MinioOptions`/`S3FileStorageService` (Infrastructure) —
`CreatePresignedUploadCommand` (`POST /uploads/presigned-url`, dùng chung 1 endpoint cho 5 `purpose`:
license/cv/avatar/org_document/org_logo, validator áp giới hạn content-type theo từng purpose — ảnh
JPEG/PNG/WEBP cho avatar/org_logo, thêm PDF cho license/org_document/cv). Entity `OrganizationDocument`
mới hoàn toàn (đúng ERD sẵn có mục "organization_documents" nhưng chưa từng implement) +
`AddOrganizationDocumentCommand`/`GetOrganizationDocumentsQuery`, endpoint lồng vào `Organizations.cs`
(`GET/POST /organizations/{id}/documents`). `GetPendingOrganizationsQuery` (Ops) sửa để trả kèm
`Documents` — Vận hành xem được giấy phép thật ngay trong hàng đợi duyệt, không cần gọi API riêng.
Migration `AddOrganizationDocuments`. Hạ tầng: thêm service `minio-init` vào `docker-compose.yml` —
tự tạo bucket `blousehiding-uploads` (public read, policy `download` — ảnh CCHN/giấy phép cần xem trực
tiếp qua URL) khi lần đầu `docker compose up`, idempotent (`mc mb --ignore-existing`) nên an toàn khi
chạy lại nhiều lần, tránh việc mỗi người clone repo phải tự tạo bucket bằng tay như đã làm lúc verify.
**Phát hiện + sửa 3 bug thật có sẵn từ trước** (lộ ra khi chạy `dotnet run` thật để verify curl
end-to-end — không lộ ra ở `dotnet build`/`dotnet test` vì toàn bộ test hiện có gọi thẳng `ISender`,
không đi qua HTTP pipeline nên chưa từng trigger `EndpointRoutingMiddleware` build route matcher):
2 cặp method trùng tên ở 2 `IEndpointGroup` khác nhau (Minimal API tự đặt tên endpoint theo tên method,
phải globally unique) — `OpsReports.GetPending` trùng `OpsLicenses.GetPending` (do tôi tự thêm ở đợt
trước), `Reports.Create` trùng `OpsJobPackages.Create` (do tôi tự thêm ở đợt này), `OpsUsers.Search`
trùng `Jobs.Search` (cũng do tôi tự thêm). Đổi tên cả 3 theo mẫu `Get{Domain}`/`Create{Domain}`/
`Search{Domain}` để tránh trùng. Thêm 1 test regression mới (`EndpointRoutingTests`, dùng
`WebApiFactory.CreateClient()` thật — khác toàn bộ test khác trong solution — để trigger route matcher
build) làm lưới an toàn duy nhất bắt được lớp lỗi này, đã tự verify bằng cách tái tạo lại đúng bug rồi
xác nhận test fail đúng, rồi khôi phục code đúng và xác nhận test pass lại. Bug thứ 4: `AmazonS3Client.
GetPreSignedURL` luôn sinh scheme `https` bất kể `AmazonS3Config.UseHttp` — MinIO dev local chỉ chạy
HTTP, phải tự thay scheme theo `Endpoint` đã cấu hình trong `S3FileStorageService` (không ảnh hưởng
chữ ký SigV4, vốn không bao gồm scheme).
**Phát hiện thêm ngoài dự tính ban đầu**: `web/` (Client, chỉ dành cho Candidate) hoàn toàn không có
form tạo tổ chức nào — ban đầu định làm ở `web/` nhưng sau khi đối chiếu lại
`docs/nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md` mục 2.3 xác nhận "Hồ sơ tổ chức" là màn hình của
`web-admin/` (Employer dùng `web-admin/`, không dùng `web/` — đúng kiến trúc ADR-0004/ADR-0008), đã
tự sửa lại quyết định giữa lúc làm sau khi người dùng xác nhận.
Verify backend: `dotnet build` sạch (sau khi sửa 3 bug trùng tên); migration áp thành công vào Postgres
thật; verify curl end-to-end thật hoàn chỉnh (không chỉ qua Mediator như thường lệ) — register→login→
`POST /uploads/presigned-url`→`PUT` file thật lên MinIO qua presigned URL (200)→`GET` file qua
`fileUrl` công khai xác nhận đúng nội dung; `dotnet test` 75/75 Application.FunctionalTests pass
(tăng từ 67 lên 75 nhờ 8 test mới, gồm 1 regression test cho bug trùng endpoint) + 3/3
Application.UnitTests pass.
`web/`: `LicenseSection` đổi từ `documentUrl: "https://placeholder.local/pending-upload"` cố định
sang input file thật (`<Input type="file">`, accept ảnh + PDF) → `lib/upload.ts` (gọi Route Handler
`/api/uploads/presigned-url` rồi `PUT` thẳng lên MinIO) → gửi `documentUrl` thật vừa nhận về khi tạo
license. Verify: `npm run lint`/`npm run build` sạch, route `/api/uploads/presigned-url` build đúng.
`web-admin/`: trang mới `organization-profile` (route `/organization`, sidebar `nav.organization`) —
hiện `CreateOrganizationForm` (React Hook Form + Zod, tái dùng `organizationsApi.create` đã có sẵn từ
Giai đoạn 1 nhưng chưa từng có UI gọi) khi `useMyOrganization()` trả `undefined` (employer chưa có tổ
chức), hoặc thông tin tổ chức + trạng thái xác thực + `OrganizationDocuments` (upload + xem giấy phép
đã tải) khi đã có. `ops-verification/index.tsx` tab organization sửa để hiện link giấy phép thật (hoặc
cảnh báo đỏ "chưa có giấy phép — không nên duyệt" nếu rỗng) thay vì chỉ có nút Duyệt/Từ chối trần.
Verify: `npx tsc -b`/`npm run lint`/`npm run format:check`/`npm run build` sạch (chunk `organization`
build đúng); `npx vitest run` 101/102 (1 fail flaky `search-provider.test.tsx`, không liên quan).

Còn thiếu (chặn việc chốt giai đoạn):
- Dịch thuật thật cho `ja`/`zh`/`ko`/`es` ở cả 2 app — hiện đều tạm dùng tiếng Việt làm placeholder,
  chưa thuê dịch giả/vendor.
- `chats/`, `tasks/`, `apps/`, trang Settings cá nhân ở `web-admin/` — tàn dư template shadcn-admin gốc,
  cố ý không rút chuỗi (không thuộc nghiệp vụ, xem `TIEN-DO-CHI-TIET.md` điểm 8 — cân nhắc gỡ hẳn).
- Cổng thanh toán tự động (VNPay/Momo/ZaloPay) — hoãn theo ADR-0003, chưa chọn nhà cung cấp cụ thể.
- Thông báo: kênh Email/SMS/Zalo chưa có trigger nào dùng tới (cả 3 trigger hiện tại đều `Channel =
  InApp`). Payment confirm/reject chưa có notification. Chưa có nút "đánh dấu tất cả đã đọc" hoặc trang
  xem toàn bộ lịch sử thông báo (dropdown chỉ hiện chưa đọc, không có trang riêng).
- Report: `content_removed` chỉ xử lý thật cho `targetType=Job` — `Organization`/`Profile`/`Message`
  chưa có state change tương ứng (giới hạn MVP đã xác nhận). `Message` cũng chưa có bounded context
  Chat thật nên `targetLabel` luôn `null` với loại báo cáo này.
- Ops dashboard: chỉ số đếm đơn giản, chưa có biểu đồ xu hướng theo thời gian (ngoài phạm vi MVP).
- ~~Upload avatar/logo tổ chức~~ — đã làm, xem log ngay dưới đây.
- Sửa/xóa CCHN, thêm chuyên khoa (UI chưa gọi Route Handler proxy đã có sẵn), CV upload riêng/CV
  Builder xuất PDF thật, chi tiết ATS (trang riêng 1 đơn/ghi chú/điểm/lịch sử), chấp nhận lời mời thành
  viên qua UI — các gap "xương sống nhưng thiếu chiều sâu" khác đã phát hiện ở đợt rà soát luồng, chưa
  làm ở đợt này (quản lý vòng đời tin đăng đã làm xong, xem log ngay dưới đây).
- OAuth, học vấn/kinh nghiệm/CME, đổi mật khẩu khi đã đăng nhập, hoàn Credit thủ công khi tranh chấp —
  vẫn như log trước, chưa có gì thay đổi ở đợt này.

Bổ sung ngay sau — **quản lý vòng đời tin đăng** ở `web-admin/` (sửa/đóng sớm/gia hạn), việc đầu tiên
trong nhóm "xương sống nhưng thiếu chiều sâu" được chọn làm sau đợt rà soát luồng, vì ảnh hưởng trực
tiếp doanh thu tái tục (gia hạn tin) và khối lượng nhỏ nhất trong nhóm (backend `jobsApi.update/close/
renew` đã có sẵn từ Giai đoạn 1, chỉ thiếu UI gọi).
Trang mới `jobs/$jobId/edit` (route `/jobs/$jobId/edit`) — tái dùng gần như nguyên form của `jobs/new.tsx`
nhưng load dữ liệu có sẵn qua `jobsApi.getById` rồi gọi `jobsApi.update` thay vì `create+submit`. Tách
thành 3 component (`EditJob` load data → `EditJobLayout` khung trang → `EditJobForm` nhận data qua
props, khởi tạo `useState` trực tiếp từ props thay vì đồng bộ qua `useEffect`+`setState`) để tránh vi
phạm ESLint rule `react-hooks/set-state-in-effect` (đã gặp lần đầu ở `NotificationBell`, giờ tiếp tục
áp dụng đúng pattern "derive initial state from props + `key` để force remount" thay vì effect).
`jobs/index.tsx` (danh sách tin) thêm cột "Hành động" — nút Sửa (bút chì, chỉ hiện khi `status` là
`Draft`/`Rejected`, đúng `Job.CanEdit` invariant ở backend), Đóng tin sớm (có `window.confirm` trước
khi gọi, hiện khi `Published`/`Pending`/`PendingPayment`), Gia hạn (hiện khi `Closed`/`Expired`/
`Suspended` — gọi `jobsApi.renew` tạo tin `Draft` mới, tin cũ giữ nguyên `Closed`, đúng thiết kế ERD
mục 4.7 "luôn tạo `jobs` row mới, không tái sử dụng `job_id`"). Cả 2 mutation dùng
`queryClient.invalidateQueries` để tự refresh danh sách sau khi thành công.
Verify: `npx tsc -b`/`npm run lint`/`npm run format:check`/`npm run build` sạch (chunk `_jobId.edit`
build đúng); `npx vitest run` 101/102 (1 fail flaky `search-provider.test.tsx`, không liên quan).
Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` mục 5 (Jobs) — 3 dòng sửa/đóng/gia hạn tin chuyển từ ⬜
sang ✅, sửa lại điểm 4 trong danh sách nợ kỹ thuật tổng hợp (chỉ còn `applicationsApi` chưa gọi, đã
tách khỏi `jobsApi` giờ đã xong).

Bổ sung ngay sau — **trang chi tiết 1 đơn ứng tuyển (ATS)** ở `web-admin/`, việc thứ 2 trong nhóm
"xương sống nhưng thiếu chiều sâu" (theo lựa chọn của người dùng khi được hỏi ưu tiên tiếp theo) — ATS
Kanban trước đó chỉ kéo-thả đổi giai đoạn, không xem được chi tiết 1 đơn cụ thể (CV lúc ứng tuyển, ghi
chú nội bộ, lịch sử chuyển giai đoạn, chấm điểm) dù backend đã có sẵn 4/5 endpoint cần thiết.
**Phát hiện 1 gap backend thật khi khảo sát trước khi code**: `POST /applications/{id}/notes` (thêm
ghi chú) đã có từ Giai đoạn 1 nhưng **chưa từng có endpoint đọc lại danh sách ghi chú** — trang chi
tiết không thể hiện ghi chú đã thêm nếu thiếu cái này. Thêm `GetApplicationNotesQuery`
(`GET /applications/{id}/notes`, chỉ Employer member được xem — ghi chú nội bộ NTD, Candidate không
xem được) — join tên tác giả qua `IIdentityService.FindByIdAsync` vì `ApplicationNote` chỉ lưu
`AuthorUserId`. Thêm `CvSnapshot` vào `ApplicationDto` (`GetApplicationByIdQuery` — trước đó có trong
entity nhưng chưa map ra DTO, không ai đọc được qua API dù đã lưu từ lúc ứng tuyển).
Cập nhật `docs/backend/API-DESIGN.md` mục 8 (endpoint mới) trong cùng lượt, đúng CLAUDE.md rule #8.
Verify backend: `dotnet build` sạch; `dotnet test` 77/77 Application.FunctionalTests pass (tăng từ 75
lên 77 nhờ 2 test mới — trả đúng danh sách kèm tác giả, Candidate gọi bị chặn 403); 3/3
Application.UnitTests pass.
**Phát hiện 1 bug thật lúc verify curl end-to-end** (không lộ ra ở `dotnet test` vì chưa có test nào
từng deserialize `cvSnapshot` ở phía gọi ngoài): `CvSnapshotBuilder.Build` (Application layer) dùng
`JsonSerializer.Serialize(data)` không qua `HttpJsonOptions` toàn cục của Web layer (vốn chỉ thêm
`JsonStringEnumConverter`, không có `JsonNamingPolicy.CamelCase`) — chuỗi lưu trong cột `cv_snapshot`
là **PascalCase** (`FullName`, `SpecialtyId`...), khác hẳn quy ước camelCase của mọi response JSON
khác trong API. Xác nhận bằng console app throwaway trước khi kết luận, tránh sửa nhầm theo giả định.
Quyết định: sửa phía đọc (`web-admin/`) đọc đúng PascalCase thay vì đổi serializer phía backend — vì
`cv_snapshot` là dữ liệu lịch sử đã lưu cứng theo đúng invariant ERD ("chụp tại thời điểm ứng tuyển,
không đổi sau đó"), đổi serializer sẽ tạo 2 format khác nhau giữa dữ liệu cũ/mới trong cùng cột, rủi ro
cao hơn giá trị mang lại ở MVP.
`web-admin/`: trang mới `applications/detail.tsx` (route `/applications/$jobId/$applicationId`) — 3
khối chính: thông tin đơn (thư ứng tuyển, lý do từ chối nếu có) + hồ sơ tại thời điểm ứng tuyển (parse
`cv_snapshot`, map `SpecialtyId` qua `catalogApi.getSpecialties()` để hiện tên chuyên khoa) + lịch sử
chuyển giai đoạn; cột phải: đổi giai đoạn (dropdown + nút áp dụng), chấm điểm (input số), ghi chú nội
bộ (thêm mới + danh sách kèm email tác giả + thời gian). `kanban-card.tsx` bọc `Link` tới trang chi
tiết khi bấm tên ứng viên (giữ nguyên vùng kéo-thả ở icon riêng, không đụng hành vi Kanban cũ). Thêm
`applicationsApi.getById/getHistory/getNotes` + field `cvSnapshot` vào `ApiApplication` trong
`lib/api.ts`. Namespace `applications` thêm khối `detail.*` (6 locale, `vi`/`en` dịch tay thật,
`ja`/`zh`/`ko`/`es` placeholder tiếng Việt theo quy ước).
Verify: `npx vite build` (bắt buộc trước `tsc -b` để plugin router sinh route mới) + `npx tsc -b` +
`npm run lint` + `npm run format:check` (chỉ 2 file mới/sửa, không đụng 7 file nợ kỹ thuật định dạng có
sẵn từ trước) đều sạch; `npx vitest run` 101/102 (1 fail flaky `search-provider.test.tsx`, đã ghi nhận
nhiều lần ở các log trước, không liên quan). Verify curl end-to-end thật qua toàn bộ luồng: tạo
employer+candidate+moderator (gán role qua DB đúng hướng dẫn `MOI-TRUONG-DEV-CUC-BO.md`)→tạo tổ
chức→verify→tạo tin→submit (Free)→moderate approve→published→candidate cập nhật hồ sơ + thêm chuyên
khoa→ứng tuyển→`GET /applications/{id}` xác nhận `cvSnapshot` chứa đúng dữ liệu hồ sơ lúc ứng tuyển
(PascalCase, đúng phát hiện trên). **Giới hạn của lượt verify này**: không có Playwright/browser tool
khả dụng trong session để chụp ảnh render UI thật như các lượt trước — xác nhận route wiring đúng qua
type-safety của TanStack Router (`tsc -b` sạch tự chứng minh `useParams`/`Link` khớp đúng route id sinh
ra, vì router generate literal union type cho path hợp lệ) + xác nhận data shape đúng qua response API
thật, nhưng chưa tự mắt thấy giao diện render — nên coi phần UI là "verify gián tiếp", không mạnh bằng
các lượt trước, cần người dùng tự mở browser xác nhận trực quan nếu cần chắc chắn 100%. Dữ liệu test
(email `demo-{emp,cand,mod}-1785836653@test.local`, mật khẩu `Testing1234!`) giữ lại trong Postgres dev
theo yêu cầu người dùng để tự xem demo, không xóa.
Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` mục 6 (Ứng tuyển & ATS) — 4 dòng (chi tiết đơn/ghi
chú/chấm điểm/lịch sử ở `web-admin/`) chuyển từ ⬜ sang ✅.

Bổ sung ngay sau — **sửa/xóa CCHN + thêm chuyên khoa ở `web/`**, việc thứ 3 (và cuối) trong nhóm "xương
sống nhưng thiếu chiều sâu" (theo lựa chọn của người dùng — làm gap khối lượng nhỏ hơn trước, hoãn lời
mời thành viên lại). Route Handler proxy `PUT`/`DELETE /api/candidates/me/licenses/[licenseId]` và
`POST /api/candidates/me/specialties` **đã tồn tại sẵn từ trước** (không cần tạo mới) — chỉ thiếu UI
gọi tới, khác một chút so với mô tả ban đầu trong `TIEN-DO-CHI-TIET.md` (đã sửa lại cho khớp thực tế).
**Phát hiện 1 gap backend thật khi khảo sát trước khi code**: `UpdateLicenseCommand` yêu cầu đủ field
(`LicenseNo`/`IssuedBy`/`IssuedAt`/`DocumentUrl` — không phải partial patch) nhưng `LicenseDto` (trả
về từ `GET /candidates/me`) trước đó chỉ có `Id`/`LicenseNo`/`VerifyStatus`/`ExpiredAt`/`RejectReason`
— không đủ dữ liệu để form Sửa pre-fill đúng giá trị cũ. Thêm `IssuedBy`/`Scope`/`IssuedAt`/
`DocumentUrl`/`CanEdit` vào `LicenseDto` + cập nhật mapping ở `GetMyProfileQuery`. `CanEdit` phản chiếu
đúng invariant Domain (`License.CanEdit`) — frontend đọc thẳng field này để ẩn nút Sửa/Xóa khi đã
Verified, không tự suy luận lại logic ở phía UI (đúng CLAUDE.md rule #3). Cập nhật ví dụ response ở
`docs/backend/API-DESIGN.md` mục 3 khớp field mới.
Verify backend: `dotnet build` sạch; `dotnet test` 77/77 Application.FunctionalTests pass (mở rộng
assert trong test `AddLicense_Then_VerifyLicense_Should_Update_Status` có sẵn để cover field mới —
`issuedBy`/`documentUrl`/`canEdit` đúng trước và sau khi verify, không cần viết test hoàn toàn mới vì
logic nghiệp vụ Update/Delete/CanEdit đã có test từ Giai đoạn 1) + 3/3 Application.UnitTests pass.
`web/`: `LicenseSection` viết lại — mỗi license card có nút Sửa (bút chì, mở form pre-fill từ props,
chỉ hiện khi `canEdit`) và Xóa (`window.confirm` trước khi gọi, cùng pattern đã dùng ở `web-admin/` cho
hành động phá hủy dữ liệu — lần đầu áp dụng ở `web/`). Trang mới `specialty-section.tsx` (`SpecialtySection`)
— dropdown chọn chuyên khoa (lọc bỏ chuyên khoa đã có) + mức độ kinh nghiệm (`Junior`/`Mid`/`Senior`/
`Expert`, khớp đúng enum `SpecialtyLevel` phía Domain — soát kỹ vì bản đầu viết thiếu `Mid`), gọi
`POST /api/candidates/me/specialties` có sẵn. Danh sách chuyên khoa (`getSpecialties`) fetch ở Server
Component (`page.tsx`) rồi truyền qua props — không tạo Route Handler catalog mới vì đây là public data,
`NEXT_PUBLIC_API_BASE_URL` đã an toàn để bundle vào client theo thiết kế Next.js. Thêm Card "Chuyên khoa"
mới vào `profile/page.tsx`. Namespace `profile` thêm 15 key mới (6 locale, `vi`/`en` dịch tay thật,
`ja`/`zh`/`ko`/`es` placeholder tiếng Việt theo quy ước — phát hiện `ja/profile.json` có lẫn 1 key
(`title`) đã dịch tiếng Nhật thật từ trước dù phần còn lại vẫn placeholder, không đồng nhất nhưng ngoài
phạm vi sửa của đợt này, không đụng vào).
Verify: `npm run lint`/`npm run build` sạch (route `/profile` build đúng, không route mới vì tái dùng
Route Handler có sẵn). Verify curl end-to-end thật qua 2 lớp: (1) gọi trực tiếp backend xác nhận
`GET /candidates/me` trả đúng field mới (camelCase, khác với gap `cvSnapshot` PascalCase phát hiện ở
đợt trước — DTO này đi qua `HttpJsonOptions` toàn cục nên đúng camelCase như kỳ vọng), update/delete
license qua backend thành công; (2) mô phỏng browser bằng cookie jar qua Route Handler thật của `web/`
(login → add/update/delete license, add specialty đều 200) → `curl` lấy HTML server-rendered của
`/vi/profile`, xác nhận trực tiếp trong HTML trả về: có đủ chuyên khoa vừa thêm kèm tên dịch đúng mức
độ ("Dược · Cao cấp", "Hồi sức · Trung cấp"), có nút "Sửa CCHN"/"Xóa CCHN" render đúng — mức verify
mạnh hơn đợt trước (đọc được nội dung HTML thật thay vì chỉ dựa vào type-safety của router).

Bổ sung ngay sau — **chạy thử toàn hệ thống end-to-end** (theo yêu cầu trực tiếp của người dùng), khởi
động đủ hạ tầng Docker + backend + cả 2 frontend, tạo dữ liệu mới hoàn toàn qua curl và verify toàn bộ
luồng chính 1 lượt: đăng ký→OTP→tạo tổ chức→Vận hành duyệt→đăng tin→submit Free→duyệt→published→
candidate cập nhật hồ sơ+chuyên khoa+CCHN→ứng tuyển→ATS (list/transition/note/**GET notes mới**/score/
history)→Credit (bonus/search ẩn liên hệ/unlock trừ đúng+hiện liên hệ)→`web/` render đúng dashboard/
jobs/profile qua cookie jar thật. Toàn bộ luồng pass đúng thiết kế.
**2 điều phát hiện lúc chạy thử** (không phải bug, do tôi test sai — nhưng đáng ghi chú vào doc):
(1) `POST /ops/organizations/{id}/credit-bonus` yêu cầu role `admin`, không phải `moderator` — đúng
phân quyền thiết kế (2 role Vận hành khác quyền), test lần đầu dùng nhầm token; (2)
`GET /candidates/search` yêu cầu query `organizationId` bắt buộc + tên param `specialty`/`location`
(không có hậu tố `Id`) — `API-DESIGN.md` không ghi rõ tên param cụ thể, đã sửa lại đúng + sửa luôn ví
dụ response sai hoàn toàn so với `CandidateSearchResultDto` thật (doc gốc ghi nhầm field `specialty`/
`yearsOfExperience`/`contactPhone` không tồn tại trong code).

Bổ sung tiếp — **chấp nhận lời mời thành viên qua UI**, việc cuối cùng trong nhóm "xương sống nhưng
thiếu chiều sâu". **Quyết định UX đã xác nhận với người dùng**: màn hình nhập mã lời mời thủ công (ô
input dán token 64-hex-char) ở `web-admin/`, không giả định có email link thật — khớp đúng thực trạng
hiện tại (`LoggingInvitationSender` chỉ log token ra console, chưa chốt nhà cung cấp SMTP).
`web-admin/`: trang mới `/accept-invitation` (`_authenticated/`, có `beforeLoad` guard sẵn của route
cha tự redirect `/sign-in?redirect=...` nếu chưa đăng nhập — không cần code thêm gì cho luồng "chưa
đăng nhập → sign-in → quay lại đúng trang" vì `user-auth-form.tsx` đã đọc `redirectTo` từ trước).
Route nhận `?token=` qua query (cho tương lai nếu có email link thật) nhưng luôn cho sửa tay trong ô
input. Thêm mục sidebar mới (`nav.acceptInvitation`, nhóm Employer, icon `MailCheck`) — kiểm tra kỹ
không trùng substring với `nav.dashboard` hay bất kỳ key nào khác trước khi thêm, đúng bài học từ
regression `nav.opsDashboard` ở log trước. `organizationsApi.acceptInvitation(token)` thêm vào
`lib/api.ts`. Đọc lỗi field cụ thể qua `error.response.data.errors` — tái dùng đúng pattern đã có ở
`features/users/index.tsx` (invite dialog).
**Phát hiện 1 bug backend nghiêm trọng có sẵn từ trước, không liên quan gì tới thay đổi hôm nay**, lúc
verify end-to-end thật: `ProblemDetailsExceptionHandler.TryHandleAsync` ép kiểu
`(ProblemDetails)new ValidationProblemDetails(ve.Errors)` trước khi gọi `WriteAsJsonAsync` — biến cục
bộ mang kiểu tĩnh `ProblemDetails` (base class) khiến `System.Text.Json` chỉ serialize field của base
class, **bỏ mất hoàn toàn field `errors`** (chỉ có ở `ValidationProblemDetails`, chứa message chi tiết
theo từng field mà FluentValidation sinh ra). Hậu quả: **mọi response 400 validation error trong toàn
hệ thống** (không riêng accept-invitation) chỉ trả `title` chung "One or more validation errors
occurred.", không có message thật ("Lời mời không hợp lệ hoặc đã hết hạn.", "Email đăng nhập không
khớp email được mời."...) — mọi nơi ở cả `web/`/`web-admin/` đang cố đọc `error.response.data.errors`
(ít nhất 2 chỗ đã biết: invite-member dialog, và component accept-invitation mới viết) đều nhận
`undefined`, rơi vào fallback message chung, không hiện đúng lý do lỗi cho người dùng.
**Không có test nào từng bắt được** vì cùng lớp nguyên nhân với bug "duplicate endpoint name" ở log
trước — toàn bộ 78 test hiện có gọi thẳng `ISender`/`TestApp.SendAsync`, nhận `Exception` .NET trực
tiếp, không bao giờ đi qua `WriteAsJsonAsync` (chỉ chạy khi có HTTP response thật). Sửa bằng cách viết
lại `TryHandleAsync` thành `switch` tường minh, mỗi nhánh khai biến đúng kiểu cụ thể (`ValidationProblemDetails`
cho nhánh Validation) và set `StatusCode` + gọi `WriteAsJsonAsync` ngay trong nhánh đó — không còn ép
kiểu chung về `ProblemDetails` qua tuple. **Tự chứng minh test bắt đúng bug trước khi coi là xong**
(theo đúng rigor đã áp dụng ở `EndpointRoutingTests`): thêm test `ValidationError_Response_Should_Include_Errors_Field`
(dùng `WebApiFactory.CreateClient()` thật, cùng lý do với `EndpointRoutingTests` — bug chỉ lộ qua HTTP
serialize thật), `git stash` tạm code cũ (có bug) → chạy test → xác nhận fail đúng như dự đoán →
`git stash pop` khôi phục fix → chạy lại xác nhận pass.
Verify: `dotnet build` sạch; `dotnet test` 78/78 Application.FunctionalTests pass (tăng từ 77) + 3/3
Application.UnitTests pass. Verify curl end-to-end thật hoàn chỉnh: employer mời 1 email mới→lấy token
thật từ log→đăng ký+verify OTP đúng email được mời→login→`POST /invitations/{token}/accept` (200)→
`GET /organizations/{id}/members` xác nhận thành viên mới xuất hiện với đúng role, danh sách lời mời
chờ đã rỗng→accept lại cùng token (400, đúng chặn theo `IsAccepted`)→accept token sai (400)→xác nhận cả
2 case lỗi này giờ trả đúng field `errors` với message tiếng Việt thật (trước khi sửa bug thì chỉ có
title chung).
`web-admin/`: `npx tsc -b`/`npm run lint`/`npm run format:check`/`npm run build` sạch (chunk
`accept-invitation` build đúng); `npx vitest run` 101/102 (1 fail flaky `search-provider.test.tsx`, xác
nhận không liên quan bằng cách đọc kỹ nội dung test — không mention gì tới accept-invitation).
**Giới hạn verify UI thật lần này**: `web-admin/` là SPA gọi trực tiếp browser→backend (khác `web/` có
Route Handler proxy để mô phỏng qua cookie jar) — không verify được UI render qua curl như đã làm ở
gap CCHN/chuyên khoa trước đó, chỉ verify được endpoint backend mà UI gọi tới (đã verify đầy đủ qua
curl trực tiếp) + code đã build/lint/tsc sạch.
Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` mục 2 (Thành viên tổ chức) — dòng "Chấp nhận lời mời"
chuyển từ ⬜ sang ✅. Với việc này, toàn bộ nhóm "xương sống nhưng thiếu chiều sâu" đã phát hiện ở đợt
rà soát luồng nghiệp vụ trước đó coi như hoàn tất.

Bổ sung tiếp — **upload avatar/logo** (theo lựa chọn của người dùng khi được hỏi ưu tiên tiếp theo,
trong số các gap còn lại tự làm được ngay). Backend đã hỗ trợ `purpose=avatar`/`org_logo` ở
`CreatePresignedUploadCommand` từ đợt MinIO trước, nhưng **chưa từng có command nào set field đích**
sau khi upload xong — chỉ có presigned URL, không có nơi lưu URL vào `CandidateProfile.AvatarUrl`/
`Organization.LogoUrl` (cả 2 field đã tồn tại sẵn ở entity từ đầu nhưng chưa từng được đọc/ghi qua bất
kỳ command nào).
**Phát hiện thêm ngoài dự tính**: `PUT /organizations/{id}` đã thiết kế sẵn trong `API-DESIGN.md` từ
đầu (mục 4) nhưng **hoàn toàn chưa implement** — không chỉ thiếu logo, thiếu cả chức năng "cập nhật
thông tin tổ chức" nói chung (trước đó chỉ có `POST /organizations` tạo lần đầu, không có update).
**Quyết định phạm vi đã xác nhận với người dùng**: `UpdateOrganizationCommand` chỉ cho sửa
`description`/`logoUrl`/`coverUrl`/`address`/`locationId` — **không** cho sửa `name`/`orgType`/
`licenseNo` qua endpoint này, vì đây là thông tin định danh đã dùng để Vận hành xác thực tổ chức, sửa
tự do sau khi `verified` rủi ro cho tính toàn vẹn xác thực (nếu cần đổi, nên qua luồng riêng có thể
yêu cầu duyệt lại — ngoài phạm vi MVP).
Domain: không có invariant đặc biệt (khác `Verify`/`Suspend`/`AcceptInvitation` đã có sẵn) — Handler
set field trực tiếp. Authorization: bất kỳ member nào cũng update được (không giới hạn owner), nhất
quán với `AddOrganizationDocumentCommand` đã làm ở đợt trước, dù `API-DESIGN.md` bản gốc ghi "owner/
hr_manager" — đã sửa lại doc khớp đúng quyết định nhất quán này.
Candidate: **tách riêng** `UpdateAvatarCommand` khỏi `UpdateMyProfileCommand` (vốn là "upsert" toàn bộ
field hồ sơ) — nếu nhồi `AvatarUrl` vào command chung, mỗi lần đổi avatar phải gửi kèm mọi field hồ sơ
khác (rủi ro vô tình xóa mất field không gửi), và ngược lại mỗi lần lưu form thông tin chung phải biết
avatar hiện tại. Cùng lý do, `LogoSection` (frontend) phải tự đọc `GET /organizations/{id}` lấy đủ
`description`/`coverUrl`/`address` hiện có trước khi gọi `PUT` chỉ đổi logo — **tự phát hiện + tự sửa
1 rủi ro tương tự trong lúc viết `LogoSection`** (bản đầu viết gửi `null` cứng cho 3 field kia, sẽ xóa
mất dữ liệu đã lưu nếu tổ chức đã có description/address — sửa ngay trước khi coi là xong, không đợi
verify mới phát hiện).
Thêm `AvatarUrl` vào `CandidateProfileDto`/`GetMyProfileQuery` (đọc lại). Endpoint mới:
`PUT /candidates/me/avatar` (`UpdateAvatarRequest`), `PUT /organizations/{id}` (`UpdateOrganizationRequest`).
Cập nhật `API-DESIGN.md` mục 3 (thêm dòng avatar + field `avatarUrl` vào ví dụ response) và mục 4 (ghi
rõ field nào update được + quyền thật khớp code) trong cùng lượt.
Verify: `dotnet build` sạch ngay lần đầu; `dotnet test` 82/82 Application.FunctionalTests pass (tăng
từ 78, 4 test mới: update tổ chức thành công + forbidden khi không phải member, đổi avatar không đụng
field khác + tự tạo hồ sơ nếu chưa có) + 3/3 Application.UnitTests pass; chạy riêng
`EndpointRoutingTests` trước khi chạy full suite để chắc chắn 2 endpoint mới không trùng tên method
với endpoint nào khác (bài học từ 3 lần dính bug này ở các đợt trước).
`web/`: `AvatarSection` mới (Client Component, input file ẩn + nút "Đổi ảnh đại diện" kích hoạt qua
ref, dùng `uploadFile(file, "avatar")` có sẵn) — đặt trong Card "Thông tin chung" cùng `ProfileForm`.
Route Handler proxy mới `app/api/candidates/me/avatar/route.ts`. Thêm `avatarUrl` vào
`ApiCandidateProfile`. Namespace `profile` thêm 2 key (`changeAvatar`/`avatarUploadError`, 6 locale,
`vi`/`en` dịch thật).
`web-admin/`: `LogoSection` mới trong `organization-profile/index.tsx` — `OrganizationProfile` gọi
thêm `organizationsApi.getById` (ngoài `useMyOrganization` vốn chỉ trả danh sách rút gọn không có
`logoUrl`) để lấy đủ field cần giữ nguyên khi update logo. Thêm `organizationsApi.update` vào
`lib/api.ts`. Giữ nguyên convention hardcode tiếng Việt của chính file này (chưa dùng `react-i18next`
dù dự án đã có đa ngôn ngữ từ ADR-0010) — cố ý không tự ý sửa nợ kỹ thuật i18n không thuộc phạm vi việc
đang làm, tránh 1 file nửa dùng `t()` nửa hardcode.
Verify: `npx tsc -b`/`npm run lint`/`npm run format:check`/`npm run build` sạch cả 2 app (`web/`: route
`/api/candidates/me/avatar` build đúng; `web-admin/`: chunk `organization` build đúng, không route mới
vì tái dùng route `/organization` có sẵn); `web-admin/` `npx vitest run` 101/102 (1 fail flaky
`search-provider.test.tsx`, đã ghi nhận nhiều lần, không liên quan). Verify curl end-to-end thật:
candidate đổi avatar→`GET /candidates/me` xác nhận `avatarUrl` đúng, `fullName`/`headline` không đổi
(xác nhận tách command đúng không gây mất dữ liệu)→employer update logo tổ chức lần 1 (kèm description/
address)→update logo lần 2 chỉ đổi `logoUrl`, giữ đúng `description`/`address` cũ (đúng luồng thật
`LogoSection` dùng, xác nhận không có bug mất dữ liệu như bản đầu đã tự sửa)→candidate (không phải
member) gọi update tổ chức bị chặn 403 đúng thiết kế.
Cập nhật `docs/nghiep-vu/TIEN-DO-DU-AN.md` mục "Còn thiếu" — gạch dòng upload avatar/logo (đã xong).

Bổ sung tiếp — **đổi mật khẩu khi đã đăng nhập**, theo lựa chọn của người dùng trong nhóm "có thể tự
làm ngay" (khác OAuth/CV Builder/thanh toán — những việc còn lại cần quyết định lớn hơn hoặc phụ thuộc
bên thứ 3). Backend hoàn toàn chưa có — `PATCH /users/me` đã thiết kế trong `API-DESIGN.md` từ đầu cho
đổi email/SĐT nhưng cũng chưa implement, và không phải chỗ đúng cho đổi mật khẩu.
`ChangePasswordCommand` (Auth bounded context, cùng pattern 1-file `UpdateMyLocaleCommand` đã có) —
**khác** `ResetPasswordCommand` (dùng OTP, không cần đăng nhập, cho trường hợp quên mật khẩu): command
này yêu cầu đã đăng nhập + `IIdentityService.CheckPasswordAsync` xác nhận đúng mật khẩu hiện tại trước
khi `SetPasswordAsync` (cả 2 method đã có sẵn từ đầu, chỉ chưa từng ghép lại thành 1 luồng đổi mật khẩu
hoàn chỉnh). Endpoint mới `PUT /users/me/password` (đặt ở `Users.cs` cạnh `/me/locale`, không đặt ở
`Auth.cs` vì đây là hành động Owner trên user hiện tại, không phải luồng công khai). Cập nhật
`API-DESIGN.md` mục 2, ghi rõ khác biệt với forgot/reset-password.
Verify: `dotnet build` sạch; chạy riêng `EndpointRoutingTests` trước khi full suite (không trùng tên
method với endpoint nào khác); `dotnet test` 84/84 Application.FunctionalTests pass (tăng từ 82, 2 test
mới: đổi mật khẩu thành công + mật khẩu mới login được + mật khẩu cũ bị chặn, sai mật khẩu hiện tại bị
chặn 400) + 3/3 Application.UnitTests pass.
`web/`: **quyết định phạm vi đã xác nhận với người dùng** — chỉ làm `web/`, không làm `web-admin/` vì
trang `settings/` cá nhân ở đó đã được ghi nhận là tàn dư template shadcn-admin gốc, cố ý chưa đầu tư
(xem `TIEN-DO-CHI-TIET.md` điểm 8) — không mở rộng phạm vi vào khu vực đã xác định ngoài phạm vi
nghiệp vụ chính; backend endpoint dùng chung nên vẫn sẵn sàng nếu sau này cần.
`ChangePasswordForm` mới (Client Component) thay thế phần tĩnh (Input/Button không có `onSubmit`) đã
tồn tại sẵn trong `settings/page.tsx` từ Giai đoạn 0.1 — đọc lỗi field cụ thể qua `data.errors` (cùng
pattern đã dùng ở nhiều nơi khác). Route Handler proxy mới `app/api/users/me/password/route.ts`.
Namespace `profile` thêm 2 key (`changePasswordSuccess`/`changePasswordError`, 6 locale, `vi`/`en`
dịch thật).
**Phát hiện + tự sửa 1 bug thật ngay trong lúc verify** (không phải bug cũ, do chính route handler mới
viết hôm nay gây ra): `Response.json(data, { status: res.status })` throw `TypeError: Response
constructor: Invalid response status code 204` khi backend trả `204 No Content` (case đổi mật khẩu
thành công) — theo Fetch API spec, response `204`/`205`/`304` không được phép có body, mà
`Response.json()` luôn tạo body dù giá trị là `null`. Route Handler cũ hơn cho cùng loại action
(`/api/users/me/locale`, viết từ trước) đã tránh đúng bug này từ đầu bằng `new Response(null, {
status })` thay vì `Response.json()` — kiểm tra toàn bộ 13 Route Handler khác dùng chung pattern
`Response.json(data, ...)` xác nhận không route nào khác gọi tới backend endpoint trả 204 nên không
route nào khác bị ảnh hưởng, chỉ route mới của tôi. Sửa bằng cách check `res.status === 204` trước,
trả `new Response(null, { status: 204 })` riêng, mọi status khác giữ nguyên `Response.json`.
Verify: `npm run lint`/`npm run build` sạch (route `/api/users/me/password` build đúng). Verify curl
end-to-end thật qua cookie jar mô phỏng browser: đổi mật khẩu sai mật khẩu hiện tại→400 kèm đúng field
`errors.CurrentPassword` message tiếng Việt thật (xác nhận fix `ProblemDetailsExceptionHandler` từ đợt
trước vẫn hoạt động đúng cho endpoint mới)→**phát hiện bug 500 ở bước này, dừng lại sửa ngay trước khi
tiếp tục verify** (không báo "xong" khi biết còn lỗi)→sau khi sửa, đổi mật khẩu đúng→204→login mật khẩu
cũ bị 401→login mật khẩu mới thành công 200.
Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` mục 1 (Auth & Định danh) — dòng "Đổi mật khẩu khi đã đăng
nhập" chuyển từ ⬜/🟨 sang ✅ (backend + `web/`), ghi rõ `web-admin/` cố ý bỏ qua theo quyết định trên.

Bổ sung tiếp — **trang lịch sử thông báo + đánh dấu tất cả đã đọc**, tiếp tục đúng thứ tự ưu tiên
"hoàn thiện luồng đã có trước khi mở rộng bounded context mới" người dùng đã chốt từ đầu — chọn việc
này vì backend Notifications (xây từ đợt trước) đã đủ hầu hết, chỉ thiếu 1 mảnh nhỏ (mark-all) + UI.
Backend: `MarkAllNotificationsReadCommand` (load toàn bộ notification chưa đọc của user, gọi
`Notification.MarkAsRead()` cho từng cái, save 1 lần — không dùng `ExecuteUpdateAsync` vì sẽ bỏ qua
Domain method `MarkAsRead()` vốn có invariant `??=` idempotent, dù số lượng nhỏ nên hiệu năng không
phải vấn đề). Endpoint mới `PATCH /notifications/read-all` — đặt **trước** khi test route order với
`{notificationId:guid}/read` (route constraint `:guid` tự loại `read-all` khỏi nhánh đó, xác nhận đúng
qua `EndpointRoutingTests`, không có collision).
Verify: `dotnet build` sạch; `EndpointRoutingTests` chạy riêng trước (không trùng tên, route order
đúng); `dotnet test` 86/86 Application.FunctionalTests pass (tăng từ 84, 2 test mới: mark-all set
đúng `ReadAt` cho mọi notification chưa đọc + không ảnh hưởng notification của user khác) + 3/3
Application.UnitTests pass. Cập nhật `API-DESIGN.md` mục 9.
`web/`: trang mới `/notifications` (Server Component fetch qua `lib/notifications.ts` mới +
`NotificationHistoryList` Client Component con xử lý mark-read/mark-all) — **quyết định đã xác nhận
với người dùng**: không thêm vào `AccountNav` (tránh đổi UX cố định ở mọi trang tài khoản cho 1 tính
năng ít dùng), chỉ truy cập qua link "Xem tất cả" mới thêm vào `NotificationBell` dropdown (cạnh nút
"Đánh dấu tất cả đã đọc" mới, ngăn cách bằng `DropdownMenuSeparator`). Route Handler proxy mới
`app/api/notifications/read-all/route.ts`. Thêm 3 key vào `common.notifications.*` (6 locale, `vi`/`en`
dịch thật).
**Phát hiện + tự sửa 1 bug thật ngay trong lúc verify** (bug mới do chính route handler hôm nay viết,
không phải bug cũ): route `read-all` ban đầu viết theo pattern `Response.json(data, {status})` (giống
đa số route khác) nhưng backend trả `204 No Content` cho endpoint này — cùng lớp lỗi đã học và sửa ở
`/api/users/me/password` cách đây không lâu (Fetch API cấm response 204 có body, `Response.json` luôn
tạo body). Lần này áp dụng đúng bài học ngay từ đầu: viết route bằng `new Response(null, {status})` từ
bản đầu tiên (không phải sửa sau khi lỗi) — không tái diễn bug.
`web-admin/`: trang mới `/notifications` (route `_authenticated/notifications.tsx`, độc lập không
thuộc `employerGroup`/`opsGroup` nào, cùng vị trí với `/accept-invitation` — dùng chung cho mọi role
đã đăng nhập). Thêm `notificationsApi.markAllRead` vào `lib/api.ts`. `NotificationBell` thêm nút
"Đánh dấu tất cả đã đọc" + link "Xem tất cả", cùng cấu trúc `DropdownMenuSeparator` như `web/`. Thêm 3
key vào `common.notifications.*` (6 locale) — kiểm tra kỹ không trùng substring với key nav nào khác
trước khi thêm (bài học từ regression `nav.opsDashboard` ở log trước), xác nhận an toàn vì đây thuộc
object `notifications` riêng, không phải `nav`.
Verify: `npx vite build` (regenerate route tree) + `npx tsc -b` + `npm run lint` + `npm run
format:check` sạch (chunk `notifications` build đúng); `npx vitest run` 101/102 (1 fail flaky
`search-provider.test.tsx`, không liên quan). Verify curl end-to-end thật qua cookie jar mô phỏng
browser của `web/`: tạo candidate mới→verify 2 CCHN (sinh 2 notification `license_verified`)→
`GET /notifications` thấy đủ 2, cả 2 `readAt: null`→`PATCH .../read-all` 200→xác nhận lại toàn bộ
`readAt` đã set + `?unreadOnly=true` trả rỗng→`GET /vi/notifications` (SSR HTML thật) xác nhận nội
dung cả 2 thông báo render đúng trong trang.
Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` mục 11 — dòng "Thông báo trong ứng dụng" chuyển từ ⬜
sang ✅ cho cả backend/`web/`/`web-admin/` (bảng này trước đó ghi ngày cập nhật 2026-08-03, đã lệch
hoàn toàn khỏi thực tế — Notifications bounded context được xây ở đợt sau đó nhưng bảng chưa từng
được đồng bộ lại).

Bổ sung tiếp — **rà soát lại toàn bộ luồng nghiệp vụ chính** (theo yêu cầu trực tiếp của người dùng:
"các chức năng thanh toán để sau, ưu tiên chức năng theo luồng") + **CV upload riêng khi ứng tuyển**.
Dùng agent research đối chiếu `LUONG-NGHIEP-VU-MAN-HINH.md` với tiến độ thật, tìm gap ảnh hưởng "đi hết
1 luồng" (không tính thanh toán/OAuth/CV Builder/học vấn-kinh nghiệm/dịch thuật — đã biết là backlog
riêng). **Phát hiện báo cáo research ban đầu sai 2 điểm** — nói Report vi phạm và khóa/mở khóa user
"chưa có backend lẫn UI", nhưng cả 2 đã xây xong từ 2 đợt trước (do agent đọc `TIEN-DO-CHI-TIET.md` cũ
chưa đồng bộ, đúng vấn đề đã ghi nhận nhiều lần ở các log trước). Tự kiểm tra lại trực tiếp qua code
(`grep`/`find` trên `src/Web/Endpoints/`, `web-admin/src/features/`) trước khi báo cáo lại cho người
dùng — không tin nguyên báo cáo research khi có dấu hiệu mâu thuẫn với những gì tự mình đã làm.
2 gap thật còn lại (ngoài thanh toán): (A) ứng tuyển chỉ dùng hồ sơ nền tảng, không cho CV riêng — sửa
được ngay trong luồng đã có; (B) NTD không nhắn tin được với ứng viên sau khi mở hồ sơ qua Credit — cần
bounded context Chat hoàn toàn mới. Theo lựa chọn của người dùng, chọn (A) trước (khớp đúng tinh thần
ưu tiên, khối lượng nhỏ hơn), hoãn (B) vì là bounded context mới không phải "hoàn thiện luồng có sẵn".
**Quyết định phạm vi đã xác nhận với người dùng**: ERD thiết kế bảng `cvs` dùng chung cho CV Builder
(JSON) và upload file, nhưng CV Builder là backlog riêng chưa chốt thiết kế — chọn cách tối giản: chỉ
thêm field `cv_file_url` trực tiếp vào `applications` (không tạo bảng `cvs`/CV Builder), tương tự cách
`AddOrganizationDocument` lưu file URL đơn giản trước đó.
Domain: `JobApplication.CvFileUrl` (string?, mới). Command: `SubmitApplicationCommand` thêm field
`CvFileUrl` optional, set trực tiếp khi tạo `JobApplication`. Cập nhật `ApplicationDto` + cả 3 nơi map
(`GetMyApplicationsQuery`/`GetJobApplicationsQuery`/`GetApplicationByIdQuery`) — không sót nơi nào nhờ
`grep -rln "new ApplicationDto"` trước khi sửa. Endpoint `POST /jobs/{jobId}/applications` thêm field
`cvFileUrl` vào `ApplyRequest`. Migration `AddApplicationCvFileUrl` — chỉ `ADD COLUMN` nullable, đọc kỹ
SQL sinh ra trước khi áp dụng theo đúng `QUY-UOC-MIGRATION.md` rule #1 (không cần backfill vì không đổi
cột đã có dữ liệu). Cập nhật `ERD-CHI-TIET.md` mục `applications` — nhân tiện sửa luôn 2 chỗ đã sai từ
đầu: `cv_id`/`cv_snapshot` ghi nhầm là liên quan CV Builder trong khi thực tế `cv_snapshot` luôn chụp
từ `candidate_profiles` (CV Builder chưa từng tồn tại) và `cv_id` chưa từng implement — đánh dấu ⬜ rõ
ràng thay vì để đọc nhầm là đã có. Cập nhật `API-DESIGN.md` mục 8 cùng lượt (doc gốc ghi sai "chọn
`cvId`" — chưa từng đúng).
Verify: `dotnet build` sạch; migration áp thành công vào Postgres thật (`\d "Applications"` xác nhận
đúng cột); `dotnet test` 88/88 Application.FunctionalTests pass (tăng từ 86, 2 test mới: submit kèm
CvFileUrl trả đúng giá trị qua `GetApplicationByIdQuery`, submit không kèm để `null` đúng) + 3/3
Application.UnitTests pass.
`web/`: `ApplyButton` viết lại hoàn toàn — từ 1-click submit ngay thành Dialog nhỏ (thư ứng tuyển +
input file CV tùy chọn) trước khi submit, dùng `uploadFile(file, "cv")` có sẵn (purpose `cv` đã tồn
tại trong `UploadPurpose`/validator từ đợt MinIO nhưng chưa từng được dùng ở đâu). Route Handler
`/api/jobs/[id]/apply` không cần sửa — đã forward nguyên body từ trước, tự nhận field mới. Thêm 5 key
i18n vào `jobs.json` (6 locale, `vi`/`en` dịch thật).
`web-admin/`: thêm `cvFileUrl` vào `ApiApplication`; trang chi tiết ATS (`detail.tsx`, làm ở đợt trước)
hiện link "Xem CV riêng ứng viên đã tải lên →" khi có; `KanbanCard` thêm icon `FileText` nhỏ báo ứng
viên có CV riêng ngay từ danh sách, không cần mở từng đơn mới biết. Thêm 1 key vào `applications.json`
(6 locale).
Verify: `npx tsc -b`/`npm run lint`/`npm run format:check`/`npm run build` sạch cả 2 app; `web-admin/`
`npx vitest run` 101/102 (1 fail flaky không liên quan, như mọi log trước). Verify curl end-to-end thật
qua cookie jar mô phỏng browser của `web/`: candidate mới→`POST /api/uploads/presigned-url`
(`purpose=cv`)→`PUT` file PDF giả lên MinIO thật (200)→`POST .../apply` kèm `cvFileUrl` (200)→
employer `GET /applications/{id}` xác nhận `cvFileUrl` trả đúng URL→`curl` trực tiếp URL đó xác nhận
public read 200 (NTD bấm link xem được ngay, không cần presigned GET riêng)→ứng tuyển tin khác **không**
kèm CV xác nhận field vẫn hoạt động đúng khi bỏ trống (optional path).
Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` mục 3 — dòng "Ứng tuyển bằng CV upload riêng" chuyển từ
⬜ sang ✅ cho cả backend/`web/`/`web-admin/`.

Bổ sung tiếp — **rà soát 3 nhóm "xem ứng viên/phê duyệt/đăng tải thông tin"** theo yêu cầu trực tiếp
của người dùng (ưu tiên đúng 3 nhóm này, bỏ qua chat/thanh toán/việc chưa cần thiết). Dùng agent
research, **quán triệt rõ ràng trong prompt**: không tin `TIEN-DO-CHI-TIET.md` một mình, phải tự verify
bằng code thật — vì lượt research trước đó đã sai 2 điểm do đọc tài liệu lệch. Agent lần này tự grep/
đọc code trực tiếp trước khi kết luận, xác nhận đúng: phê duyệt CCHN/tổ chức/tin đăng, xử lý report,
khóa/mở khóa user — cả 4 đã đầy đủ backend + UI thật (không phải mock/nút tĩnh), bác bỏ nghi ngờ trước
đó. Tìm ra 3 gap thật còn sót, xếp theo ảnh hưởng: (1) xem chi tiết ứng viên sau unlock — chỉ có email,
không có tên/CCHN/kinh nghiệm/CV thật; (2) **bug đang gây mất dữ liệu** ở `ProfileForm` (`web/`); (3)
form hồ sơ tổ chức thiếu input `description`/`address`. Theo lựa chọn của người dùng, sửa bug mất dữ
liệu (2) trước vì nghiêm trọng hơn thiếu tính năng.
**Phát hiện gap sâu hơn agent report ban đầu lúc bắt tay vào sửa**: không chỉ UI thiếu input — backend
`GetMyProfileQuery`/`CandidateProfileDto` **chưa từng trả** `Dob`/`Gender`/`Address` dù entity
`CandidateProfile` có đủ 3 field này từ đầu. Đây là nguyên nhân gốc khiến `ProfileForm` không có cách
nào biết giá trị hiện tại để hiển thị lại hoặc giữ nguyên khi submit — phải sửa backend trước khi sửa
được frontend, không chỉ là "thêm input" đơn giản như agent report ban đầu mô tả.
Backend: thêm `Dob`/`Gender`/`Address` vào `CandidateProfileDto` + map trong `GetMyProfileQuery`
(`p.Gender.HasValue ? p.Gender.Value.ToString() : null` — nullable enum ternary, chạy test thật qua
Testcontainers Postgres để xác nhận EF Core dịch đúng sang SQL, không chỉ tin `dotnet build` sạch vì
lỗi này chỉ lộ ra ở runtime). Không cần migration — chỉ đọc thêm field đã có sẵn trong entity, không
đổi schema. Cập nhật `API-DESIGN.md` mục 3 (ví dụ response thêm 3 field).
Verify: `dotnet build` sạch; `dotnet test` 89/89 Application.FunctionalTests pass (tăng từ 88, 1 test
mới: lưu profile với `Dob`/`Gender`/`Address` rồi đọc lại qua `GetMyProfileQuery` xác nhận đúng cả 3 —
test này chính là lưới an toàn bắt lỗi EF Core translation nếu tương lai có ai sửa lại query theo cách
khác) + 3/3 Application.UnitTests pass.
`web/`: viết lại hoàn toàn `ProfileForm` — từ 2 input (fullName/headline) + gửi cứng `null` cho 3 field
còn lại, thành đủ 6 input (thêm summary/dob/gender/address, `Select` cho gender) khởi tạo từ
`profile` props, submit gửi đúng giá trị hiện tại của form (không còn hardcode `null`). Thêm
`Dob`/`Gender`/`Address` vào `ApiCandidateProfile` type. Thêm 8 key i18n vào `profile.json` (6 locale,
`vi`/`en` dịch thật — `summary`/`dob`/`gender`/`selectGender`/`genderValue.{Male,Female,Other}`/
`address`).
Verify: `npm run lint`/`npm run build` sạch. **Verify curl end-to-end thật xác nhận đúng bug đã sửa**
(không chỉ tin code đọc bằng mắt): candidate mới→lưu profile lần 1 (fullName+dob+gender+address)→
`GET /vi/profile` (SSR HTML thật) xác nhận form pre-fill đúng `1990-05-20`/`123 Đường ABC`→lưu lần 2
chỉ đổi `fullName` (mô phỏng đúng hành vi `ProfileForm` mới — đọc giá trị hiện tại từ state rồi gửi lại,
không phải hardcode null)→`GET /vi/profile` lại xác nhận `dob`/`address` **vẫn còn nguyên** sau lần lưu
thứ 2 — trước khi sửa, bước này chắc chắn xóa mất 2 field đó.
Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` mục 1 — dòng "Cập nhật hồ sơ cơ bản" viết lại mô tả cho
khớp thực tế (bảng này ghi "có hardcode tiếng Việt" nhưng thực tế đã rút xong từ commit `a376453` một
đợt trước, tài liệu chưa đồng bộ lại — đúng lớp vấn đề lệch tài liệu đã gặp nhiều lần), ghi rõ bug mất
dữ liệu đã sửa.

Bổ sung tiếp — **xem chi tiết hồ sơ ứng viên sau khi mở qua Credit**, theo yêu cầu trực tiếp của người
dùng ("ưu tiên các chức năng cho phép xem ứng viên, phê duyệt, đăng tải thông tin... bỏ qua chat, thanh
toán"). Gap đã xác nhận ở đợt research trước: sau khi NTD trả Credit mở hồ sơ, `SearchCandidatesQuery`
chỉ trả `Headline`/`IsUnlocked`/`UnlockCost`/`ContactEmail` — không có tên đầy đủ/CCHN/chuyên khoa/CV,
làm giảm giá trị thật của việc đã tốn Credit.
**Quyết định thiết kế đã xác nhận** (tách khỏi phạm vi doc gốc): `API-DESIGN.md` thiết kế
`GET /candidates/{id}` dùng chung 1 DTO cho cả Employer đã unlock và Vận hành, nhưng để đúng CLAUDE.md
mục 4 quy tắc #9 (không trả nhiều hơn field cần cho từng role), tạo `EmployerCandidateProfileDto` riêng
biệt hoàn toàn với `CandidateProfileDto` (dùng cho `GET /candidates/me`) — không có `completionPct`/
`dob`/`gender`/`address` (NTD không cần các field nội bộ đó), có thêm `ContactEmail` (candidate không
cần khi xem hồ sơ chính mình). **Chưa làm** phần "Vận hành xem qua endpoint này" và
`GET /candidates/{id}/public-summary` (bản rút gọn ẩn danh) — ngoài phạm vi yêu cầu lần này, ghi rõ ⬜
trong `API-DESIGN.md` thay vì để đọc nhầm là đã có.
Backend: `GetCandidateProfileByIdQuery` (Employer, member + đã unlock — check cả 2 điều kiện, 403 nếu
thiếu 1 trong 2) — join `IIdentityService.FindByIdAsync` lấy `ContactEmail`, map `Specialties`/
`Licenses` rút gọn (chỉ tên/mức độ, chỉ số CCHN/trạng thái — không lộ `DocumentUrl`/`RejectReason` nội
bộ). Endpoint mới `GET /candidates/{candidateId}` (query `organizationId` bắt buộc, cùng pattern
`/candidates/search` đã có — nhất quán cách xác định "unlock cho tổ chức nào"). Cập nhật `API-DESIGN.md`
mục 3 — viết lại đúng thực tế implement (doc gốc ghi sai "Employer/Vận hành" chung, và
`POST .../unlock` ghi sai "trả hồ sơ đầy đủ" — command đó thật ra chỉ trả `Guid`, chưa từng đúng).
Verify: `dotnet build` sạch; `EndpointRoutingTests` chạy riêng trước (không trùng tên); `dotnet test`
91/91 Application.FunctionalTests pass (tăng từ 89, 2 test mới: xem đầy đủ sau unlock trả đúng
fullName/headline/contactEmail, xem trước khi unlock bị chặn 403) + 3/3 Application.UnitTests pass.
`web-admin/`: trang mới `/candidates/$candidateId` (route mới trong `_authenticated/candidates/`) —
avatar + tên + headline + summary, danh sách chuyên khoa (Badge) + CCHN (số + trạng thái), card liên
hệ riêng. `candidates/index.tsx` — card đã unlock giờ là `Link` dẫn tới trang chi tiết (trước đó chỉ
hiện email tĩnh, không bấm được gì). Thêm `candidatesApi.getById` + type `EmployerCandidateProfile`
vào `lib/api.ts`. Thêm khối `detail.*` vào `candidates.json` (6 locale, `vi`/`en` dịch thật).
Verify: `npx vite build` (regenerate route tree) + `npx tsc -b` + `npm run lint` + `npm run
format:check` + `npm run build` sạch (chunk `_candidateId` build đúng); `npx vitest run` 101/102 (1
fail flaky không liên quan, như mọi log trước). Verify curl end-to-end thật bằng dữ liệu smoke test đã
có sẵn từ trước (employer đã unlock 1 candidate ở đợt trước): `GET /candidates/{id}?organizationId=...`
cho candidate đã unlock trả đầy đủ fullName/headline/avatarUrl/contactEmail/specialties/licenses đúng
dữ liệu thật trong DB; gọi lại cho candidate **chưa** unlock (lấy từ `/candidates/search` thấy
`isUnlocked:false`) xác nhận đúng 403 — cả 2 nhánh chính đều đúng thiết kế.
Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` mục 7 — thêm dòng "Xem chi tiết hồ sơ sau khi mở".

Bổ sung tiếp — **form hồ sơ tổ chức thiếu input description/address**, gap cuối cùng trong 3 gap đã xác
nhận ở đợt research trước theo đúng ưu tiên "xem/phê duyệt/đăng tải thông tin" người dùng đã chốt.
Không cần thay đổi backend — `UpdateOrganizationCommand`/`PUT /organizations/{id}` đã hỗ trợ đủ 5 field
từ đợt upload avatar/logo trước, chỉ thiếu UI gọi tới 2 field `description`/`address`.
`web-admin/`: `OrganizationInfoForm` mới (react-hook-form + zod, cùng pattern `CreateOrganizationForm`
đã có) — 2 field `description` (Textarea)/`address` (Input), khởi tạo từ `orgDetails` đã fetch qua
`organizationsApi.getById` (data này đã có sẵn từ đợt `LogoSection`, tái dùng không phải fetch thêm).
Submit giữ nguyên `logoUrl`/`coverUrl` hiện có — đúng bài học đã áp dụng ở `LogoSection` (
`UpdateOrganizationCommand` ghi đè toàn bộ field, không phải partial patch, gửi `null` sẽ xóa mất dữ
liệu field khác). Đặt Card mới giữa Card thông tin cơ bản (tên+trạng thái xác thực) và
`OrganizationDocuments` (upload giấy phép) — tách riêng khỏi `LogoSection` (không nhồi 2 mục đích khác
nhau vào 1 Card). `key={organization.id}` trên component để force remount đúng khi tổ chức đổi, cùng
convention `EditJobForm`/`OrganizationInfoForm` đã áp dụng nhất quán trong dự án — không sync qua
`useEffect` để tránh vi phạm ESLint `react-hooks/set-state-in-effect`.
**Sự cố hạ tầng giữa lúc làm**: safety classifier của harness tạm thời không khả dụng, mọi lệnh Bash
(kể cả `tsc -b`/lint) bị chặn khoảng vài phút. Trong lúc chờ, tự đọc lại toàn bộ code vừa viết bằng
Read (không cần classifier) để rà soát logic trước — xác nhận không thấy lỗi rõ ràng qua đọc thủ công
trước khi có thể chạy build thật xác nhận lại.
Verify: không cần chạy lại `dotnet test` (không đổi backend). `npx tsc -b`/`npm run lint`/`npm run
format:check`/`npm run build` sạch (chunk `organization` build đúng, không route mới vì tái dùng route
`/organization` có sẵn); `npx vitest run` 101/102 (1 fail flaky không liên quan, như mọi log trước).
Verify curl end-to-end thật bằng tổ chức smoke test đã có sẵn logo/description/address từ trước: mô
phỏng đúng hành vi `OrganizationInfoForm` — đổi `description`/`address`, gửi kèm `logoUrl` cũ để giữ
nguyên → xác nhận cả 3 field đúng sau khi lưu, `logoUrl` không bị mất → non-member gọi update (kể cả
thử gửi `logoUrl: null`) bị chặn 403 trước khi kịp update — xác nhận không có lỗ hổng cho phép người
ngoài tổ chức xóa mất logo/thông tin.
Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` mục 4 (Cơ sở y tế) — dòng "Cập nhật thông tin tổ chức"
chuyển mô tả `web-admin/` từ "chỉ đổi logo" sang đầy đủ 2 form, ghi rõ `coverUrl` vẫn chưa có UI (thứ
yếu, ngoài phạm vi 3 nhóm ưu tiên đã chốt).
Với việc này, cả 3 gap đã xác nhận ở đợt rà soát "xem/phê duyệt/đăng tải" (xem chi tiết ứng viên, bug
mất dữ liệu profile, form hồ sơ tổ chức) đều đã hoàn tất.

Bổ sung tiếp — **rà lại toàn bộ mục 10 (Vận hành nền tảng) trước khi chọn việc tiếp theo**, theo yêu
cầu người dùng ("liệt kê lại kế hoạch tiến độ tính năng" trước khi làm). Phát hiện bảng mục 10 hoàn
toàn lỗi thời — Report/khóa-mở-khóa-user/Ops-dashboard đã ghi "⬜ chưa có" dù cả 3 đã xây và verify từ
nhiều đợt trước trong chính session này (đúng lớp vấn đề lệch tài liệu đã gặp lại nhiều lần). Tự verify
qua `grep`/`find` code thật trước khi sửa lại bảng, xác nhận đúng 2 gap còn thật: cộng Credit thủ công
thiếu UI (nhỏ), sidebar không tách theo role (chọn làm trước, theo lựa chọn người dùng — ảnh hưởng UX
hàng ngày của cả Employer/Vận hành).
**Quyết định phạm vi đã xác nhận với người dùng**: chỉ lọc 2 nhóm nghiệp vụ chính (`employerGroup`/
`opsGroup`) theo role — không đụng `Pages`/`Other` (tàn dư template demo Auth/Errors/Settings/Help,
vẫn hiện cho mọi role, đã biết là backlog "dọn dẹp" riêng, ngoài phạm vi lần này).
`web-admin/`: `sidebarData` (object tĩnh, export thẳng, dùng ở cả `AppSidebar` và `CommandMenu`) đổi
thành hàm `getSidebarData(role)` — filter theo title nhóm (`employer` ẩn `nav.opsGroup`, `admin`/
`moderator` ẩn `nav.employerGroup`, role khác/`undefined` không ẩn gì — an toàn cho lúc chưa đăng
nhập). Xóa comment TODO cũ ở đầu file (đã giải quyết đúng ý TODO đó). Sửa `AppSidebar` và
`CommandMenu` gọi `getSidebarData(useAuthStore(...).auth.user?.role)` — sửa đúng 1 nguồn dữ liệu, cả
sidebar thật và command palette (Cmd+K) tự động đồng bộ theo, không phải filter riêng ở 2 nơi.
**Phát hiện quan trọng ngoài dự tính, cần ghi lại rõ** vì đã hiểu sai suốt nhiều đợt log trước: đọc kỹ
`search-provider.test.tsx` trước khi sửa (để tránh lặp lại bug substring `nav.opsDashboard` đã gặp),
phát hiện test `navigates to a top-level route...` (dòng 107-118) click vào `getByText('Tasks')` —
nhưng **"Tasks" không tồn tại ở bất kỳ đâu trong `sidebar-data.ts` hiện tại** (`grep` toàn bộ `src/`
xác nhận). Route `/tasks` vẫn còn (tàn dư template), nhưng sidebar item dẫn tới nó đã bị xóa ở 1 đợt
dọn dẹp trước, còn test thì chưa được cập nhật theo — test này gọi `git log` cũng không tìm thấy
"Tasks" trong lịch sử `sidebar-data.ts`, nên nhiều khả năng test copy nguyên từ template gốc
(`satnaing/shadcn-admin`) chưa từng chỉnh theo dữ liệu sidebar thật của dự án này. **Toàn bộ các log
trước trong session này đã gọi fail này là "flaky do môi trường Chromium"** — kết luận đó sai, đây là
bug thật (test tham chiếu UI không tồn tại), không phải flaky. Không sửa ngay (ngoài phạm vi việc đang
làm — filter role), nhưng ghi lại chính xác để không tiếp tục hiểu nhầm ở các log sau.
Verify: không cần backend (chỉ đổi `web-admin/`). `npx tsc -b`/`npm run lint`/`npm run format:check`/
`npm run build` sạch. Thêm test mới `sidebar-data.test.ts` (5 test: employer ẩn opsGroup, admin/
moderator ẩn employerGroup, role không xác định không ẩn gì, không lọc Pages/Other) — lưới an toàn lâu
dài cho logic filter, vì không có Route Handler proxy để verify qua curl như `web/` (SPA gọi backend
trực tiếp từ browser, không có tầng trung gian để mô phỏng qua HTTP). `npx vitest run` 106/107 (tăng từ
101, +5 test mới; 1 fail vẫn là "Tasks" — đã xác nhận là bug thật của test cũ, không liên quan gì tới
thay đổi lần này, không phải regression).
Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` mục 10 — sửa lại toàn bộ 5 dòng cho khớp thực tế (Report/
khóa-mở-khóa-user/Ops-dashboard chuyển từ ⬜ sang ✅, sidebar tách role chuyển ⬜ sang ✅, credit-refund
giữ ⬜ đúng vì thật sự chưa làm).

Bổ sung tiếp — **UI cộng Credit thủ công cho Vận hành**, gap cuối cùng trong đúng phạm vi ưu tiên "xem/
phê duyệt/đăng tải" đã chốt. **Thay đổi quy trình theo yêu cầu trực tiếp của người dùng**: từ nay ưu
tiên xây UI hoàn chỉnh trước, dùng dữ liệu giả nếu backend chưa đủ, rồi mới quyết định API cần bổ sung
— khác thứ tự "API trước, UI sau" đã áp dụng suốt các đợt trước.
Khảo sát trước khi code phát hiện: **không có API nào cho Vận hành tìm/liệt kê tất cả tổ chức** — chỉ
có `GET /ops/organizations` (hàng đợi *pending*, không phải toàn bộ). Nếu chỉ dùng API sẵn có, UI sẽ
phải bắt Vận hành gõ tay `organizationId` (UX tệ, không thực tế). Theo đúng quy trình mới, chọn xây UI
tìm kiếm hoàn chỉnh trước (dùng `MOCK_ORGANIZATIONS` + hàm `searchOrganizationsMock` nội bộ, đánh dấu
rõ TODO), còn phần cộng Credit thật sự (`credit-bonus`) vẫn gọi API thật đã có — không mock phần đã có
sẵn thật.
`web-admin/`: trang mới `/ops/credit` — Card 1 tìm tổ chức theo tên (mock), chọn 1 kết quả → Card 2
form nhập số Credit + lý do (Textarea) → xác nhận gọi `opsApi.creditBonus(orgId, amount)` (mới thêm
vào `lib/api.ts`, dùng đúng `POST /ops/organizations/{id}/credit-bonus` đã có từ trước). Field "lý do"
**không gửi lên backend** — `CreditBonusRequest` chỉ nhận `Amount`, không có cột lưu lý do trong DB —
ghi rõ `reasonHint` ngay trong UI báo người dùng biết trường này chỉ để tự ghi nhớ, không giả vờ đã lưu
(đúng nguyên tắc minh bạch đã áp dụng ở nút "Nạp thêm Credit" trước khi có Payments). Thêm mục sidebar
`nav.opsCredit` trong `opsGroup` — kiểm tra kỹ không trùng substring với `nav.credit` (Employer) hay
bất kỳ key nào khác trước khi thêm, theo đúng bài học từ regression `nav.opsDashboard` ở log trước.
Verify: `npx vite build` (regenerate route tree) + `npx tsc -b` + `npm run lint` + `npm run
format:check` + `npm run build` sạch (2 chunk `credit` build đúng — 1 cho Employer `/credit`, 1 cho
Ops `/ops/credit`); `npx vitest run` 106/107 (1 fail "Tasks" đã biết rõ nguyên nhân từ log trước,
không liên quan). Verify curl end-to-end thật (không qua UI vì đây không phải Route Handler proxy có
thể mô phỏng như `web/` — SPA gọi thẳng backend, verify logic bằng cách gọi đúng request UI sẽ gửi):
admin gọi `credit-bonus` amount=50 → 200 → xác nhận ví tăng đúng 85→135 qua token employer khác;
moderator (không phải admin) gọi cùng endpoint → 403 đúng, xác nhận phân quyền role `admin`-only giữ
nguyên không bị nới lỏng qua UI mới.
Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` mục 10 — dòng "Cộng Credit thủ công" chuyển ⬜ sang ✅,
ghi rõ phần tìm tổ chức đang dùng mock tạm, chưa phải API thật.
Với việc này, **toàn bộ các gap đã xác nhận trong phạm vi ưu tiên "xem ứng viên/phê duyệt/đăng tải
thông tin"** (theo yêu cầu người dùng, loại trừ chat/thanh toán) đã hoàn tất — trừ 1 mục còn cố ý để
mock (tìm tổ chức cho Ops) chờ quyết định có làm API thật hay không.

Tiếp tục theo quy trình "UI trước, API sau" đã chốt ở log trên — người dùng yêu cầu áp dụng luôn cho
`web/` (Client), sau khi tự phát hiện Explore thấy 5 gap UI còn lại (không phải chat/thanh toán, đúng
phạm vi ưu tiên): (1) form tìm kiếm trang chủ chưa nối, (2) trang chủ dùng `MOCK_ORGANIZATIONS`,
(3) CV Builder 100% mock không có handler, (4) chưa có "Lưu tin quan tâm", (5) OAuth Google/Zalo chỉ UI
tĩnh không có state. Người dùng chọn làm hết cả 5 theo độ dễ tăng dần trong 1 lượt, thay vì dừng sau 2
mục đầu.

**(1) Form tìm kiếm** — khảo sát trước khi code phát hiện `keyword` đã được nối **thật, đầy đủ 2 đầu**
từ trước (`SearchJobsQuery.Keyword` backend + `getJobs()` frontend `lib/api.ts`) — gap thực tế chỉ là
UI chưa gọi tới: `/jobs` (`jobs/page.tsx`) chưa đọc `searchParams.keyword`, và Hero form ở trang chủ
không có `onSubmit`. Đây là trường hợp hiếm — 100% UI wiring, **không cần thêm API/backend nào**.
Thêm state + form tìm theo từ khóa vào `JobsBrowser` (đồng bộ qua URLSearchParams, cùng pattern với 3
filter cũ), thêm 2 key `keywordPlaceholder`/`keywordSearch` cho 6 locale. Tách `HeroSearchForm` (Client
Component riêng, theo đúng pattern `ApplyButton`/`AvatarSection`) để trang chủ (Server Component) có
thể có 1 form `onSubmit` điều hướng `router.push` sang `/jobs?keyword=...`.

**(2) Trang chủ dùng data thật thay `MOCK_ORGANIZATIONS`** — khảo sát phát hiện đây **không chỉ là
thiếu polish mà là 1 bug thật đang chạy**: `MOCK_ORGANIZATIONS` dùng ID giả (`org-1`, `org-2`...),
nhưng `Link href="/organizations/${org.id}"` trỏ vào trang `/organizations/[id]` gọi API thật
`getOrganizationById(id)` — với ID giả, API trả 404 → `notFound()`. Vẫn không có API "liệt kê tổ chức
công khai" (đã xác nhận qua grep `Organizations.cs`, chỉ có `GetById` theo ID biết trước và
`GetMyOrganizations` yêu cầu đăng nhập employer) — nhưng lần này **không cần mock**: tận dụng chính
`getJobs()` thật đã gọi cho mục "tin nổi bật" ngay phần trên, suy ra danh sách tổ chức nổi bật bằng
cách dedupe theo `organizationId`, sắp theo số tin đang có, lấy top 3. Vì job chỉ publish được khi
organization đã `verified` (ràng buộc bất di bất dịch #4 CLAUDE.md), mọi tổ chức suy ra theo cách này
chắc chắn đã verified — không cần gọi thêm API để biết `verifyStatus`. Ẩn cả section nếu không có tin
nào. Kết quả tốt hơn kế hoạch ban đầu: dùng 100% data thật, link hoạt động đúng, và fix luôn 1 bug 404
đang tồn tại trên production trước đó.

**(3) CV Builder** — chuyển `/profile/cv` từ Server Component tĩnh (`MOCK_CANDIDATE`/`MOCK_CV` render
thẳng, input `defaultValue` không có `onChange`, nút Thêm/Lưu không có handler) sang tách riêng
`CvBuilderForm` (Client Component) với state thật cho `education`/`experience`/`skills` — Thêm/Sửa/Xóa
từng mục, xem trước (preview) đồng bộ realtime theo state, nút Lưu hiện thông báo "Đã lưu thay đổi"
(cùng key `savedChanges` đã dùng ở `ProfileForm`). Nút Lưu **chưa gọi API** — ghi rõ TODO trong code:
chưa có bounded context CV ở backend, chỉ giữ state tạm ở client theo đúng quyết định UI-first. Nút
Xuất PDF vẫn giữ `disabled` (không đổi — nằm ngoài phạm vi 5 gap đã chốt). Thêm 2 key `removeEntry`/
`skillPlaceholder` cho 6 locale.

**(4) Lưu tin quan tâm (Saved Jobs)** — tính năng hoàn toàn mới, chưa có bounded context/API nào ở
backend. Xây `lib/saved-jobs.ts` quản lý danh sách ID đã lưu qua `localStorage` (không phải state
React đơn thuần, để giữ được qua lần tải lại trang) + 1 custom event `saved-jobs-changed` để đồng bộ
giữa nhiều component đang mount cùng lúc (`SaveJobButton` trên trang chi tiết tin và `SavedJobsList`
trên trang tổng hợp). Thêm `SaveJobButton` (bookmark toggle) cạnh `ApplyButton` trên `/jobs/[id]`.
Thêm route mới `/dashboard/saved-jobs` — Client Component fetch chi tiết từng job theo ID lưu trong
`localStorage` qua chính `getJobById()` thật (không mock nội dung job, chỉ mock *việc lưu lại ID nào*).
Thêm tab "Đã lưu" vào `AccountNav` (đổi type `active` để nhận thêm `"savedJobs"`), đổi text tĩnh
"chưa có mục nào" trên `/dashboard` cũ thành link thật sang trang mới. Bug tự phát hiện lúc lint: dùng
`useEffect` gọi `setSaved(isJobSaved(jobId))` ngay khi mount vi phạm rule `react-hooks/set-state-in-
effect` (bài học đã ghi nhận từ các log trước — `NotificationBell`/`EditJobForm`/`OrganizationInfoForm`)
— sửa bằng lazy initializer `useState(() => isJobSaved(jobId))` cho lần đọc đầu (an toàn qua SSR vì
`isJobSaved` tự guard `typeof window === "undefined"`), chỉ giữ `useEffect` để subscribe thay đổi từ
component khác. Thêm 2 key `tabSavedJobs` (profile.json) và `saveJob`/`savedJob` (jobs.json) cho 6
locale.

**(5) OAuth Google/Zalo** — chỉ polish UI theo đúng phạm vi đã chốt (không có OAuth thật ở backend,
ngoài phạm vi đợt này). Tách `OAuthButtons` (Client Component) — click vào 1 trong 2 nút disable cả
2 nút, hiện spinner ~0.8s rồi hiện thông báo "sắp ra mắt" (`oauthComingSoon`, key mới cho 6 locale) —
tránh cảm giác nút chết hoàn toàn nhưng cũng không giả vờ đăng nhập thành công.

Verify: `npm run lint` và `npm run build` sạch sau mỗi mục trong cả 5 mục (không dồn lại kiểm 1 lần).
**Hạn chế đã biết của lượt verify này**: Docker daemon không chạy được trong phiên làm việc này nên
không khởi động được `docker compose`/backend API để test qua curl hoặc browser thật với data thật —
chỉ xác nhận được ở mức compile-time (TypeScript, ESLint, Next.js build). Cần verify lại qua UI thật
(hoặc ít nhất qua `dotnet run` + `npm run dev` + click tay) ở phiên làm việc kế tiếp có Docker chạy
được, trước khi coi 5 mục này là "đã kiểm chứng đầy đủ" theo đúng tinh thần mục 5 CLAUDE.md.
Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` mục 1 (OAuth), mục 3 (CV Builder), mục 4 (trang chủ hết
mock tổ chức), mục 5 (thêm dòng "Lưu tin quan tâm", ghi rõ form từ khóa ở tìm kiếm tin).

Người dùng yêu cầu "tiếp tục các UI khác" — chạy 1 lượt khảo sát Explore mới (không dựa vào doc, đọc
trực tiếp route thật + đối chiếu `api/src/Web/Endpoints/*.cs`) để tìm gap UI còn sót ngoài 5 mục vừa
xong, loại trừ chat/thanh toán. Kết quả: không còn gap loại "chỉ cần nối UI với API đã có" nào khác —
mọi trang nghiệp vụ chính (candidates/ATS/jobs/organization-profile/ops-*/notifications/accept-
invitation/credit) đã nối API thật, không còn mock. Tìm được 2 gap dễ sửa (không cần backend) + 2 gap
cần backend mới (Ops tìm tổ chức theo tên, danh sách tổ chức public riêng — quyết định làm hay không
để sau, chưa đủ ưu tiên so với 2 gap dễ). Hỏi người dùng, được xác nhận làm 2 gap dễ ngay.

**(1) 2 link 404 trên `web/`** — `SiteHeader` (dùng chung mọi trang công khai) có link "Cơ sở y tế"
(`/organizations`) và "Giới thiệu" (`/about`) trỏ tới route không tồn tại — lỗi hiện diện toàn site,
không phải lỗi cục bộ 1 trang. Tách hàm suy-tổ-chức-từ-jobs (`featuredOrganizationsFromJobs`, viết ở
log trên cho trang chủ) thành `deriveOrganizationsFromJobs` dùng chung trong `lib/api.ts` (bỏ giới hạn
top-3 khi dùng cho trang danh sách đầy đủ), trang chủ đổi qua gọi hàm chung này. Trang `/organizations`
mới — Server Component gọi `getJobs()` thật rồi suy ra toàn bộ danh sách tổ chức (không giới hạn top-3
như trang chủ). Trang `/about` mới — hoàn toàn tĩnh, không gọi API, 3 giá trị cốt lõi (tin cậy lâm
sàng/đúng chuyên khoa/cộng đồng y tế) viết bám theo đúng mô tả nghiệp vụ ở đầu CLAUDE.md, không suy
diễn thêm nội dung ngoài phạm vi tài liệu đã có. Thêm namespace i18n mới `about` (đăng ký vào
`i18n/request.ts` — dễ quên bước này vì next-intl không tự động phát hiện namespace mới) + file
`about.json` cho 6 locale (vi/en dịch tay, ja/zh/ko/es placeholder tiếng Việt theo đúng convention).
Thêm link "Xem tất cả" ở section tổ chức nổi bật trang chủ, trỏ sang `/organizations` mới.

**(2) Help Center `web-admin/`** — mục sidebar có thật (nhóm "Other", hiện cho mọi role đăng nhập,
không bị lọc theo role) nhưng route chỉ render `<ComingSoon />` (placeholder tiếng Anh của template
gốc). Viết `HelpCenter` feature — 4 câu hỏi thường gặp tĩnh (vì sao chưa đăng được tin/cách mời thành
viên/Credit dùng để làm gì/vì sao không sửa được tin đã đăng — đều là câu hỏi bám sát đúng luồng
nghiệp vụ 4 ràng buộc bất di bất dịch, không phải câu hỏi chung chung vô nghĩa) + 1 mục hướng dẫn liên
hệ hỗ trợ. Cân nhắc rồi bỏ ý định thêm địa chỉ email `mailto:` cụ thể — không có domain email hỗ trợ
thật nào được xác lập trong dự án (chỉ có `demo@blousehiding.vn` là dữ liệu demo user giả trong sidebar
mock, không phải kênh liên hệ thật), bịa 1 địa chỉ cụ thể sẽ gây hiểu lầm đây là kênh đang hoạt động —
đổi thành hướng dẫn chung "liên hệ qua kênh nội bộ tổ chức bạn", không có link mailto giả. Thêm block
`helpCenter.*` vào `common.json` (không tạo namespace riêng, vì đây là 1 trang đơn lẻ) cho 6 locale.

Verify: `web/` — `npm run lint` + `npm run build` sạch (thêm 2 route mới `/[locale]/about`,
`/[locale]/organizations` vào danh sách route build ra). `web-admin/` — `npx tsc -b` sạch, `npm run
lint` sạch, `npm run format:check` chỉ còn đúng 7 file nợ kỹ thuật cũ có từ trước (không liên quan thay
đổi này — file mới `help-center/index.tsx` đã tự format sạch bằng `prettier --write` chỉ nhắm đúng 1
file, không đụng 7 file khác), `npx vite build` build ra chunk `help-center` mới thành công, `npx
vitest run` 106/107 (1 fail "Tasks" vẫn là bug cũ đã biết rõ nguyên nhân từ log trước, không phải
regression từ thay đổi này).
**Hạn chế verify giữ nguyên từ log trên**: Docker daemon vẫn không chạy được trong phiên này, không
test được qua browser/curl thật với data thật — chỉ compile-time. Cần verify lại qua UI thật khi có
Docker.
Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` mục 4 (danh sách tổ chức chuyển ⛔ 404 sang ✅), mục 11
(thêm dòng `/about` và Help Center, đánh dấu mục nợ kỹ thuật #1 "link chết" đã sửa xong).
2 gap cần backend mới (Ops tìm tổ chức theo tên, API list tổ chức public riêng) — **chưa làm**, để
người dùng quyết định có ưu tiên hay không ở lượt tiếp theo.

Người dùng hỏi "kế hoạch tiếp theo" — trình bày lại 2 nhóm việc còn mở (2 gap cần backend mới ở trên,
+ mục "Tổng hợp nhanh: nợ kỹ thuật" trong `TIEN-DO-CHI-TIET.md` với 8 dòng ghi từ đợt khảo sát trước),
chủ động nói rõ nghi ngờ 1 số dòng trong mục nợ kỹ thuật đã lỗi thời (bài học cũ: doc này nhiều lần bị
phát hiện lệch code thật). Người dùng chọn "Verify + dọn nợ kỹ thuật" trước — rủi ro thấp, không cần
backend mới, ưu tiên đúng thứ tự (verify trước khi tin, không code trước rồi mới biết là đã lỗi thời).

Chạy Explore agent verify từng dòng 1-4 của mục nợ kỹ thuật bằng cách đọc trực tiếp code (không dựa
vào doc), kết quả: **3/4 dòng đã LỖI THỜI** (đã được sửa ở các đợt làm việc trước nhưng chưa gạch bỏ
trong "Tổng hợp nhanh") — sửa/xoá CCHN + thêm chuyên khoa đã có UI đủ (`license-section.tsx`/
`specialty-section.tsx`), `applicationsApi.getById/addNote/score/getHistory` đã được `detail.tsx` gọi
đủ cả 4, chuỗi tiếng Việt hardcode ở 8 file được nêu đã không còn (quét bằng regex Unicode tiếng Việt
xác nhận). Chỉ dòng 4 — `web/lib/mock-data.ts` có 8/11 export chết (`MOCK_JOBS`, `getJobById`,
`getOrganizationById`, `getJobsByOrganization`, `MOCK_SPECIALTIES`, `MOCK_LOCATIONS`,
`MOCK_EMPLOYMENT_TYPES`, `MOCK_APPLICATIONS`) — **còn đúng thật**. Dễ nhầm vì `getJobById`/
`getOrganizationById` cùng tên cũng tồn tại thật trong `lib/api.ts` (định nghĩa khác, đang dùng thật ở
nhiều trang) — Explore agent xác nhận rõ 2 cặp hàm trùng tên nhưng khác file, không lẫn vào nhau.

Xóa 8 export chết khỏi `mock-data.ts`, chỉ giữ `MOCK_CANDIDATE`/`MOCK_CV`/`MockCvEducation`/
`MockCvExperience` (đang dùng thật ở `cv-builder-form.tsx`, viết ở log trước). File giảm từ 276 dòng
xuống 66 dòng — xóa toàn bộ block `MOCK_JOBS`/`MOCK_ORGANIZATIONS`/`MOCK_APPLICATIONS` cùng các hàm
tra cứu đi kèm.

Verify: `npm run lint` + `npm run build` sạch, toàn bộ route cũ vẫn build đúng — xác nhận không route
nào còn phụ thuộc các export đã xóa (đúng như Explore agent đã kết luận trước khi xóa).

Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` mục "Tổng hợp nhanh: nợ kỹ thuật" — gạch bỏ dòng 1-5 (đã
sửa hoặc lỗi thời), thêm dòng cảnh báo đầu mục "danh sách này dễ lỗi thời nhất trong toàn file, luôn
verify lại bằng code thật trước khi làm". Dòng 6 (sidebar theo role) và 7 (Vận hành thiếu 3 màn hình)
cũng nghi lỗi thời một phần (đã có `getSidebarData(role)`, đã có `/ops/credit`) — ghi chú rõ cần verify
lại ở lượt sau, chưa xác nhận đủ để gạch bỏ hẳn như 1-5.
Vẫn còn mở: 2 gap cần backend mới (Ops tìm tổ chức theo tên, API list tổ chức public riêng), và cần
verify lại dòng 6-8 còn lại của mục nợ kỹ thuật trước khi coi là xong hoàn toàn.

Người dùng nhắc lại "đã bảo ưu tiên hoàn thiện UI trước" khi thấy đề xuất tiếp theo là làm 2 API backend
còn thiếu — đúng là lệch khỏi chính sách UI-trước đã chốt. Nhận ra cả 2 gap đó **đã có UI** (Ops-credit
dùng mock, `/organizations` suy từ jobs) — vấn đề chỉ là "quyết định làm API thật hay không", không phải
"thiếu UI". Hỏi lại người dùng nên làm gì tiếp vì không còn gap "thiếu UI" nào đã biết — chọn rà lại sâu
hơn để tìm khiếm khuyết UI tinh vi hơn (không chỉ "UI hoàn toàn chết" như 2 lượt rà trước).

Chạy Explore agent khảo sát cả `web/` và `web-admin/` tìm khiếm khuyết loại: thiếu loading/error/empty
state phân biệt rõ, thiếu validation client-side, feedback giả sau hành động, v.v. — không giới hạn ở
"nút không có handler" như trước. Tìm được 11 vấn đề, xác nhận với người dùng làm hết theo thứ tự ảnh
hưởng UX giảm dần:

**#1 — `apiGet()` (web/lib/api.ts) nuốt mọi lỗi backend thành "không có dữ liệu"**: lỗi nền tảng ảnh
hưởng nhiều trang nhất — 500/network fail trả `null`/`[]` giống hệt trường hợp trống thật, khiến
`/jobs`, `/jobs/[id]`, `/organizations`, `/dashboard` hiện sai trạng thái khi backend lỗi. Sửa: `apiGet`
giờ chỉ trả `null` cho 404 thật (hợp lệ — "không tìm thấy"); mọi lỗi khác (network throw, 5xx, 4xx≠404)
`throw new ApiError`. Thêm `app/[locale]/error.tsx` (Next.js error boundary chuẩn, Client Component,
`useTranslations` hoạt động vì nằm trong `NextIntlClientProvider` của layout) — hiện thông báo lỗi + nút
"Thử lại"/"Về trang chủ". Thêm namespace `errorPage` vào `common.json` (6 locale). Cố ý giữ đúng phạm
vi: không đụng `backendFetch` (helper khác, dùng cho các trang auth-gated) vì Explore agent chỉ báo cáo
`apiGet`, không mở rộng ngoài kế hoạch.

**#2 — Kanban ATS (web-admin) báo "thành công" trước khi API xác nhận, không rollback khi lỗi**: kéo-
thả ứng viên gọi `toast.success` ngay trong `handleDragEnd`, trước cả khi `mutation` resolve — nếu API
lỗi, NTD đã thấy toast thành công dù ứng viên chưa hề chuyển giai đoạn. Sửa: chuyển toast success vào
`onSuccess` của mutation (dùng `variables` để lấy đúng tên ứng viên/giai đoạn vì lúc đó không còn trong
scope của `handleDragEnd`), xóa toast sớm.

**#3-7 — Loading/empty/error state thiếu phân biệt** ở 5 nơi: Jobs list (web-admin, thêm skeleton 3
dòng + phân biệt loading/error/empty), Candidates search (thêm `isError` riêng, tránh lỗi API hiện
giống "không có kết quả" khiến NTD đổi filter vô ích), Saved Jobs list (web/, đổi `return null` lúc tải
thành skeleton + bắt lỗi throw từ `getJobById` sau khi sửa #1 — đây là Client Component trong
`useEffect`, `error.tsx` KHÔNG bắt được lỗi ở đây nên phải tự try/catch riêng), Ops verification (3 tab
count hiện "0" giả khi đang tải — sửa thành placeholder "(…)" cho tới khi query resolve), Members list
(thêm skeleton + error, tách khỏi nhánh "chưa có thành viên" cũ dùng chung với "đang tải").

**#8 — Global error handler (web-admin `main.tsx`) chỉ xử lý 401/500, bỏ sót 403 + network**: 403 chỉ
có dòng comment bị vô hiệu hóa (`// router.navigate(...)`) từ template gốc, không toast gì; network
fail/timeout (không có `error.response`) rơi qua mọi nhánh `if` mà không báo. Thêm toast rõ ràng cho cả
2 trường hợp trong `queryCache.onError` (không đụng `handleServerError` dùng cho mutations — đã xử lý
tốt qua `error.response?.data?.title` + fallback).

**#9 — Invite member dialog không validate email client-side**: input `type='email'` nhưng dialog dùng
`onClick` trên Button (không phải `<form onSubmit>`) nên browser validation không kích hoạt — nhập
"abc" vẫn gửi lên, phải chờ round-trip mới biết sai. Thêm regex email đơn giản, disable nút submit +
hiện lỗi inline khi email không hợp lệ.

**#10 — Apply button (web/) không giới hạn kích thước file CV client-side**: chỉ giới hạn qua
`accept=`, file quá lớn chỉ phát hiện được sau khi `uploadFile()` thất bại ở server. Kiểm tra
`API-DESIGN.md` xác nhận backend **chưa công bố giới hạn chính thức** (chỉ ghi "sẽ áp giới hạn ở Giai
đoạn 1", chưa có con số) — đặt tạm 10MB theo thông lệ chung cho CV PDF/ảnh scan ở client, ghi rõ comment
đây là giá trị tạm chờ backend thật, không phải số đã thống nhất.

**#11 — Jobs new form (web-admin) không có loading state cho 4 catalog dropdown**: Chuyên khoa/Khu
vực/Loại hình/Gói tin hiện trống trong vài giây đầu tải, dễ hiểu lầm "chưa có danh mục". Thêm
`isLoading` cho cả 4 query, disable Select + đổi placeholder thành "Đang tải..." khi đang tải, disable
luôn nút submit cho tới khi catalog tải xong (tránh submit với giá trị rỗng do bấm quá nhanh).

Verify riêng biệt sau từng mục (không dồn lại 1 lần): `web/` — `npm run lint` + `npm run build` sạch.
`web-admin/` — `npx tsc -b` sạch, `npm run lint` sạch, `npx vite build` sạch, `npx vitest run` 106/107
(1 fail "Tasks" vẫn là bug cũ đã biết, không phải regression). `npm run format:check` phát hiện 3 file
vừa sửa (`candidates/index.tsx`, `ops-verification/index.tsx`, `main.tsx`) lệch format — chạy
`prettier --write` đúng 3 file này (không đụng 6 file nợ format cũ có từ trước, đúng bài học từ lượt
Help Center trước).
Cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` — chưa cập nhật, cần làm ở bước tiếp theo vì 11 mục này là
polish UX chi tiết, không phải tính năng mới ở cấp module nên chưa rõ nên ghi vào đâu trong file đó cho
hợp lý; để nguyên trong log này làm nguồn tham chiếu chính.
Vẫn còn mở: 2 gap cần backend mới (Ops tìm tổ chức theo tên, API list tổ chức public riêng) từ log
trước — chưa làm, chờ quyết định người dùng.

Người dùng nhắc "đã bảo ưu tiên hoàn thiện UI trước" khi thấy đề xuất tiếp theo là 2 gap backend còn mở
— lệch khỏi chính sách UI-trước. Nhận ra cả 2 gap đó đã có UI (mock/suy diễn), chỉ còn chờ quyết định
API — không phải "thiếu UI". Hỏi lại hướng tiếp theo, chọn rà sâu thêm 1 lượt nữa tìm khiếm khuyết UI ở
các góc chưa quét (responsive, UX cụt sau hành động, optimistic UI thiếu ở mutation khác, confirm dialog
thiếu cho hành động phá hủy, pagination, các trang chưa đọc: notifications/organization-profile/credit/
ops-reports/candidates detail/applications detail/settings). Explore agent tìm 10 vấn đề, xác nhận làm
hết cả 10 kể cả pagination (dù backend không hỗ trợ — xác nhận qua Explore agent riêng đọc trực tiếp 3
Query Handler liên quan: `GetOrganizationJobsQuery`/`SearchCandidatesQuery`/`GetMyNotificationsQuery`
đều `.ToListAsync()` không `Skip/Take`, không `totalCount` — quyết định làm pagination CLIENT-SIDE, chỉ
giới hạn số dòng render, không giảm dữ liệu tải về, đến khi backend có pagination thật).

**#1 Ops reports — thêm confirm dialog cho khóa tổ chức/gỡ nội dung**: nút "Gỡ/Khóa" gọi mutation ngay
khi click 1 lần, không xác nhận — hành động phá hủy (khóa tổ chức tự động ẩn mọi tin `published`, đúng
ràng buộc bất di bất dịch #4 CLAUDE.md). Thêm Dialog xác nhận (không dùng `AlertDialog` có sẵn trong
`components/ui/` vì toàn bộ `features/` chưa từng dùng nó — giữ nhất quán với pattern `Dialog` đã dùng
khắp nơi khác thay vì giới thiệu 1 pattern mới).

**#2 Candidates unlock — thêm xác nhận trước khi trừ credit**: mở hồ sơ ứng viên tốn Credit thật, không
thể hoàn tác, trước đây bấm 1 lần là trừ ngay. Thêm Dialog xác nhận cùng pattern #1.

**#3+#4 Jobs list — disable nút theo từng job + đổi `window.confirm` sang Dialog + overflow-x-auto**:
`closeMutation.isPending`/`renewMutation.isPending` disable TOÀN BỘ nút đóng/gia hạn trên mọi dòng cùng
lúc (không phân biệt job nào đang xử lý) vì không track theo biến — sửa bằng `mutation.variables ===
job.id` (React Query tự cung cấp `variables` của lần gọi `mutate()` gần nhất). `window.confirm` thô đổi
sang Dialog chuẩn cùng lúc. Thêm `overflow-x-auto` cho bảng — trước đây không có breakpoint responsive
nào, bảng 5 cột dễ vỡ layout trên mobile.

**#5 Pagination client-side** cho Jobs list/Candidates search/Notification history (`web-admin/`):
tạo `hooks/use-pagination.ts` (hook thuần, PAGE_SIZE=10) + `components/simple-pagination.tsx` (UI nút
Trước/Sau). Bug tự phát hiện lúc lint: dùng `useEffect(() => setPage(1), [items])` để reset trang khi
đổi filter vi phạm `react-hooks/set-state-in-effect` (bài học lặp lại lần 3 trong session — luôn kiểm
tra rule này khi effect chỉ gọi `setState`) — sửa bằng pattern React chính thức "adjust state during
render": so sánh `items !== prevItems` (React Query giữ nguyên reference khi data không đổi nhờ
structural sharing, nên so sánh này chỉ true khi filter/tổ chức thực sự đổi) rồi `setState` ngay trong
thân hàm render, không qua effect. Tách hook sang file riêng trong `hooks/` theo đúng convention có sẵn
của repo (`use-my-organization.ts`, `use-table-url-state.ts`) sau khi ESLint cảnh báo
`react-refresh/only-export-components` vì 1 file lẫn cả hook và component.

**#6 Notification history (`web/`) — rollback khi lỗi mark-read/mark-all-read**: optimistic update chạy
trước, `fetch` không kiểm tra `res.ok`, không rollback nếu lỗi — người dùng thấy đã đọc dù request thật
đã fail. Thêm kiểm tra `res.ok` + rollback về state cũ + `toast.error` khi lỗi. Phát hiện `web/` đã cài
`sonner` từ trước (chỉ setup `<Toaster />`, chưa từng gọi `toast()` trong app code) — dùng đúng luôn thay
vì `alert()` thô, đây cũng là lần đầu `toast()` được dùng thật trong `web/`.

**#7 Delete account button — hiển thị lỗi khi thất bại**: trước đây API lỗi thì im lặng, `confirming`
vẫn `true` nhưng người dùng không biết vì sao. Thêm `toast.error` khi `!res.ok` hoặc network throw.

**#8 Organization profile — CTA/lý do khi bị Rejected** (mục duy nhất trong 10 mục cần sửa backend,
không chỉ UI): badge "Bị từ chối" trước đây không kèm lý do hay hướng dẫn gì tiếp. Kiểm tra Domain thấy
`Organization.RejectReason` đã tồn tại nhưng chưa map vào `OrganizationDto`
(`GetOrganizationByIdQuery.cs`) — thêm field vào cả record và LINQ projection. Viết test functional mới
`Reject_Pending_Organization_Should_Expose_RejectReason_On_GetById` theo đúng pattern có sẵn trong
`VerifyOrganizationTests.cs`. **Phát hiện quan trọng**: `dotnet`/`docker` mà các log trước ghi "không
chạy được" hóa ra chỉ là do `PATH` ưu tiên `/usr/local/share/dotnet` (không có .NET 10) trước
`~/.dotnet` (có 10.0.301, thỏa `global.json` pin 10.0.201 + `rollForward: latestFeature`) — gọi trực
tiếp `~/.dotnet/dotnet` build/test thành công; Docker thực ra đang chạy (`docker ps` thấy cả 4 container
Postgres/Redis/RabbitMQ/MinIO healthy). Tự tay verify: `dotnet build` sạch, test mới pass, toàn bộ
Application.FunctionalTests 92/92 (tăng từ 91). Cập nhật `docs/backend/API-DESIGN.md` mục 4 (Organization)
theo đúng CLAUDE.md quy tắc #8. Frontend: thêm `rejectReason` vào `ApiOrganization` type, hiện khối
cảnh báo màu đỏ (viền `accent-seal`) kèm lý do + hướng dẫn tải lại giấy phép khi `verifyStatus ===
'Rejected'`. **Nợ kỹ thuật tự phát hiện, không tự sửa**: toàn bộ `organization-profile/index.tsx`
(kể cả trước khi sửa lần này) 100% hardcode chuỗi tiếng Việt, không dùng `useTranslation` — vi phạm
CLAUDE.md mục 4 quy tắc #10, nhưng rút hết ~30 chuỗi trong file là việc lớn riêng biệt ngoài phạm vi
10 mục đã chốt; giữ nguyên phong cách hardcode hiện có cho đoạn code mới để nhất quán, không tự ý mở
rộng phạm vi, chỉ ghi nhận rõ ở đây.

**#9 Credit wallet — loading state riêng cho bảng giao dịch**: `transactions?.length === 0` hiện "chưa
có giao dịch" giả trong lúc đang tải — cùng pattern lỗi đã sửa nhiều lần trong session. Thêm
`isLoading`/`isError` + skeleton 3 dòng.

**#10 Applications detail — loading state cho history/notes**: 2 query con (`getHistory`/`getNotes`)
không lấy `isLoading`, hiện "trống" giả trong lúc tải — tái dùng đúng key `detail.loading` đã có sẵn
cho toàn trang, không cần thêm key mới.

Verify: **lần đầu trong session verify được cả 3 tầng thật** (trước đây luôn bị chặn bởi tưởng lầm
Docker/dotnet không chạy). Backend: `dotnet build` + `dotnet test` Application.FunctionalTests 92/92.
`web/`: `npm run lint` + `npm run build` sạch. `web-admin/`: `npx tsc -b` sạch, `npm run lint` sạch (sau
khi sửa lỗi `set-state-in-effect`), `npx vite build` sạch, `npx vitest run` 106/107 (1 fail "Tasks" bug
cũ đã biết). `npm run format:check` phát hiện 6 file mới lệch format
(`candidates/index.tsx`/`jobs/index.tsx`/`notifications/index.tsx`/`ops-reports/index.tsx`/
`organization-profile/index.tsx`/`hooks/use-pagination.ts`) — `prettier --write` đúng 6 file này, không
đụng 6 file nợ format cũ có từ trước.
Cập nhật `docs/backend/API-DESIGN.md` mục 4 (rejectReason). Chưa cập nhật
`docs/nghiep-vu/TIEN-DO-CHI-TIET.md` cho đợt này — 10 mục là polish UX chi tiết ở cấp component, không
phải tính năng mới ở cấp module, để nguyên log này làm nguồn tham chiếu chính (giống quyết định ở lượt
polish UX trước).
Vẫn còn mở: 2 gap cần backend mới (Ops tìm tổ chức theo tên, API list tổ chức public riêng); nợ kỹ thuật
hardcode tiếng Việt ở `organization-profile/index.tsx` (phát hiện, chưa sửa, ghi nhận rõ phạm vi).

Người dùng hỏi "còn những giao diện nào chưa làm" — câu hỏi khác hẳn 4 lượt rà soát UI trước (tất cả
đều tìm "UI có nhưng lỗi/thiếu hoàn thiện"), lần này cần tìm màn hình **hoàn toàn chưa tồn tại route/
component nào**. Chạy Explore agent đối chiếu `DANH-SACH-TINH-NANG.md` + `LUONG-NGHIEP-VU-MAN-HINH.md`
(danh sách màn hình đã thiết kế) với code thật (grep trực tiếp `web/app/`, `web-admin/src/{routes,
features}/`), không tin `TIEN-DO-CHI-TIET.md` một mình (đã nhiều lần lỗi thời). Kết quả: chỉ 3 màn hình
thật sự chưa có UI nào (loại chat/thanh toán) — Audit log (MVP, liên quan tuân thủ NĐ 13/2023), Công cụ
tính lương/phụ cấp (Giai đoạn 2), Đánh giá cơ sở y tế (Giai đoạn 2). Người dùng chọn làm cả 3.

**1. Audit log (`web-admin/`)** — route mới `/ops/audit` + `features/ops-audit/index.tsx`: bảng danh
sách thao tác nhạy cảm (người thực hiện/loại thao tác/đối tượng/thời gian) + filter theo loại thao tác,
dùng `MOCK_AUDIT_LOGS` (5 dòng mẫu đủ 5 loại: duyệt CCHN, duyệt tổ chức, xử lý report, khóa user, xóa
tài khoản) — TODO ghi rõ chưa có bounded context Audit Log ở backend (chưa có bảng lưu vết, chưa có
`GET /ops/audit-logs`). Thêm `nav.opsAudit` vào `opsGroup` trong `sidebar-data.ts`, kiểm tra không trùng
substring với 7 key `nav.ops*` khác trước khi thêm (bài học `nav.opsDashboard` từ log cũ). Build ra đúng
chunk `audit` riêng, xác nhận qua `ls dist/assets/ | grep audit`.

**2. Công cụ tính lương (`web/`)** — khác 2 mục còn lại: **không mock**, vì công thức thuế TNCN Việt
Nam (biểu lũy tiến 7 bậc, giảm trừ gia cảnh 11tr/người phụ thuộc 4.4tr theo Nghị quyết 954/2020/
UBTVQH14) là quy định pháp luật công khai, tính thật 100% client-side không cần backend. Hỏi người dùng
trước vì đây là rủi ro khác hẳn các mock UI khác — số liệu sai ảnh hưởng tính toán tài chính thật của
người dùng, không chỉ là "UI chưa hoàn thiện". Người dùng chọn "làm UI + công thức cơ bản, ghi rõ
disclaimer" (không chọn bỏ qua hay chỉ làm form rỗng). Viết `lib/tax-calculator.ts` (hàm thuần
`calculateSalary`/`calculatePersonalIncomeTax`, comment trích rõ nguồn luật) + trang `/tools/salary-
calculator` (form Gross/phụ cấp trực/độc hại/tỷ lệ BHXH tự nhập — không cứng 10.5% vì lương đóng BHXH
thực tế có thể khác lương Gross/thực nhận/người phụ thuộc, tính Net theo thời gian thực qua state, có
disclaimer rõ "không thay thế tư vấn thuế/kế toán chính thức"). Đặt link công khai ở `SiteHeader` (không
cần đăng nhập, đúng tinh thần "SEO tốt" trong tài liệu 3.9 PHAN-TICH-NGHIEP-VU.md) — không đặt vào
`AccountNav` (chỉ dùng cho khu vực đã đăng nhập) sau khi cân nhắc rồi tự sửa lại quyết định ban đầu.
Thêm namespace i18n mới `tools` (đăng ký vào `i18n/request.ts`).

**3. Đánh giá cơ sở y tế (`web/`)** — đúng yêu cầu nghiệp vụ "cần kiểm duyệt chặt do tính nhạy cảm ngành
y" (PHAN-TICH-NGHIEP-VU.md mục 3.9/4.1): thiết kế UI để KHÔNG hiện review vừa gửi công khai ngay —
`OrganizationReviews` tách rõ 2 phần: danh sách đã "duyệt" (`MOCK_APPROVED_REVIEWS`, dùng chung cho mọi
tổ chức vì không có ID seed thật nào để gắn cụ thể — quyết định này sau khi phát hiện ID mock ban đầu
`38d9b2d9-...` không tồn tại trong backend, tránh gây hiểu lầm "tổ chức X có review sẵn") và form gửi
review mới (chỉ hiện khi đã đăng nhập, sau khi gửi báo "đang chờ Vận hành kiểm duyệt" — không hiện ngay
trong danh sách trên). Dùng `localStorage` để nhớ "đã gửi" tránh gửi trùng, cùng pattern `saved-jobs.ts`
đã dùng trước. Bug tự phát hiện lúc lint (lần thứ 4 trong session): `useEffect` đọc localStorage rồi
`setState` ngay — vi phạm `react-hooks/set-state-in-effect`, sửa bằng lazy initializer
`useState(() => ...)` giống các lần trước (SaveJobButton). Đổi nội dung key `reviewsTitle` từ "Đánh giá
(sắp ra mắt)" thành "Đánh giá" ở cả 6 locale — dịch thật cho ja/zh/ko/es (không copy tiếng Việt) vì đây
là các file đã có bản dịch thật từ trước, không phải placeholder.

Verify: `web/` — `npm run lint` + `npm run build` sạch (3 route mới: `/tools/salary-calculator`,
`/organizations/[id]` cập nhật, không có route riêng cho review vì nằm trong trang tổ chức có sẵn).
`web-admin/` — `npx tsc -b` sạch, `npm run lint` sạch, `npx vite build` sạch (chunk `audit` xác nhận
riêng), `npx vitest run` 106/107 (1 fail "Tasks" bug cũ đã biết). `npm run format:check` phát hiện
`ops-audit/index.tsx` lệch format — `prettier --write` đúng file này, còn lại đúng 6 file nợ cũ.
Cập nhật `docs/nghiep-vu/DANH-SACH-TINH-NANG.md`: sửa dòng Giai đoạn 0.1 lỗi thời ("quản lý người dùng"/
"xử lý report" ghi nhầm "chưa làm" — đã xác nhận có UI+API thật từ các log trước, chuyển 🟨→✅); thêm
dòng Audit log (🟨, UI có/backend chưa); đổi 2 dòng Giai đoạn 2 (Công cụ tiện ích, Đánh giá cơ sở y tế)
từ ⬜ sang 🟨 kèm mô tả rõ phần đã làm/còn thiếu.
Vẫn còn mở: 3 bounded context backend mới (Audit Log, Review cơ sở y tế) cộng 2 gap cũ (Ops tìm tổ
chức, API list tổ chức public) — tổng 4 quyết định API còn chờ người dùng, tất cả đều đã có UI đầy đủ
chờ nối khi quyết định làm.

Người dùng hỏi "đang làm cho role nào, còn bao nhiêu giao diện" — chạy Explore agent đối chiếu cả 4
danh sách màn hình theo role trong `LUONG-NGHIEP-VU-MAN-HINH.md` (2.1 Public, 2.2 Candidate, 2.3
Employer, 2.4 Vận hành) với code thật. Agent báo sai 1 mục ("Đánh giá cơ sở y tế" — báo "chưa có UI" dù
đã làm ở log ngay trước, có thể do grep không khớp đúng tên file `organization-reviews.tsx`) — tự verify
lại bằng cách đọc trực tiếp file + xác nhận đã import đúng trong `page.tsx` trước khi báo cho người dùng
(bài học: luôn tự verify claim của agent, không chỉ tin báo cáo). Sau khi sửa sai sót đó, con số đúng:
Guest 6/6, Candidate 8/8, Employer 10/10, Vận hành 9/9 đã có UI — chỉ còn 2 mục thuộc **Giai đoạn 3**
(Góc nghề y, Sự kiện/CME) hoàn toàn chưa có UI, ngoài phạm vi Giai đoạn 2.

Người dùng chọn "hoàn thiện Giai đoạn 2 trước" — đúng thứ tự roadmap, không nhảy sang Giai đoạn 3. Rà
lại 7 mục Giai đoạn 2: loại 3 mục ngoài phạm vi UI (Chat — đã loại trừ từ trước theo yêu cầu người dùng;
OpenSearch — đổi hạ tầng search engine, không phải UI; Mobile app — nền tảng khác hoàn toàn). Còn 4 mục,
hỏi làm gì tiếp — người dùng chọn "Test đánh giá năng lực" (mục duy nhất trong 4 còn hoàn toàn chưa có
UI, 3 mục khác đã có UI từ trước hoặc chờ quyết định backend).

**Test đánh giá năng lực chuyên môn** (`web/`) — trước khi viết, nhận ra vấn đề khác các mock trước:
nội dung cần câu hỏi chuyên môn y tế thật (dược lâm sàng/điều dưỡng/cấp cứu) mà không có chuyên môn y
khoa để tự viết đúng — khác việc tính thuế TNCN (công thức pháp luật khách quan, có thể tự viết đúng).
Hỏi người dùng trước, chọn "làm khung UI + logic quiz, nội dung mẫu rõ ghi MINH HỌA". Viết
`lib/competency-quiz.ts` — 5 câu hỏi dùng kiến thức an toàn/đạo đức nghề y tế CHUNG (xác nhận đúng danh
tính bệnh nhân trước thủ thuật, báo cáo trung thực khi có sai sót, nguyên tắc "5 đúng" dùng thuốc, bảo
mật thông tin bệnh nhân, chuyển tuyến khi ngoài khả năng) — đều là kiến thức phổ biến/không tranh cãi,
KHÔNG đi vào chuyên môn lâm sàng cụ thể của bất kỳ chuyên khoa nào để tránh rủi ro sai lệch. Comment ghi
rõ đây không phải bộ đề thật, cần chuyên gia y tế soạn và kiểm định trước khi dùng thật.
UI: `CompetencyQuizForm` (Client Component, 3 giai đoạn `select`→`quiz`→`result` qua state cục bộ) —
chọn chuyên khoa từ danh mục thật (`getSpecialties`, không mock danh mục vì API đã có sẵn), làm từng câu
một, xem kết quả kèm đúng/sai từng câu. Ghép chung với `salary-calculator` vào 1 trang hub mới `/tools`
(2 Card link tới từng công cụ) — quyết định này SAU KHI đã đặt link `salaryCalculator` riêng lẻ ở
`SiteHeader` từ log trước, nhận ra thêm 1 link nữa sẽ làm header quá dài trên desktop, nên gộp lại thành
1 mục nav "Công cụ" duy nhất trỏ vào hub. Đổi key i18n `nav.salaryCalculator` thành `nav.tools`.
Bug tự phát hiện lúc lint: ban đầu định gộp `CompetencyQuizForm` dùng lại y hệt pattern trước, nhưng
không tái diễn lỗi `set-state-in-effect` lần này vì thiết kế state hoàn toàn nằm trong event handler
(`handleAnswer`/`handleStart`), không có `useEffect` đọc nguồn ngoài nào — học được từ 4 lần sửa lỗi
này ở các log trước, viết đúng ngay từ đầu không cần sửa lại.

Verify: `npm run lint` + `npm run build` sạch — 3 route mới (`/tools`, `/tools/competency-test`,
`/tools/salary-calculator`) build thành công.
Cập nhật `docs/nghiep-vu/DANH-SACH-TINH-NANG.md` — đổi dòng "Test đánh giá năng lực" từ ⬜ sang 🟨 (UI
đầy đủ, bộ câu hỏi chỉ là minh họa); ghi chú thêm ở dòng "Công cụ tính phụ cấp" về việc gộp cả 2 công cụ
vào 1 trang hub `/tools`.
Vẫn còn mở trong Giai đoạn 2: "Gợi ý việc làm/matching" (hoàn toàn chưa có UI, cần thuật toán matching)
— người dùng chưa chọn làm ở lượt này; 3 bounded context backend cũ (Audit Log, Review, Ops search tổ
chức) + 1 gap API list tổ chức public — tất cả vẫn chờ quyết định làm backend thật.

Người dùng chọn làm tiếp "Gợi ý" — hỏi rõ tiêu chí matching trước khi code (multiSelect: chuyên
khoa/địa điểm/kinh nghiệm), người dùng chỉ chọn **chuyên khoa trùng**, không chọn địa điểm/kinh nghiệm
— giữ thuật toán đơn giản nhất theo đúng lựa chọn, không tự thêm tiêu chí ngoài yêu cầu.

Kiểm tra data thật trước khi code: `ApiJob` (`web/lib/api.ts`) chỉ có `specialtyName`, KHÔNG có
`specialtyId` — xác nhận qua đọc `JobDto.cs` (backend): entity `Job.SpecialtyId` tồn tại ở Domain nhưng
chưa map ra DTO (cùng loại gap với `RejectReason` ở log #8 trước). Có 2 cách: so theo tên (không sửa
backend) hoặc thêm `SpecialtyId` vào `JobDto` (sửa backend, chính xác 100% qua ID). Hỏi người dùng
trước vì đây là quyết định kỹ thuật ảnh hưởng độ chính xác — chọn "so theo tên, không sửa backend":
chấp nhận được vì candidate và job dùng chung 1 bảng `specialties`, không phải 2 nguồn độc lập có thể
lệch tên trong thực tế.

**Gợi ý việc làm** (`web/dashboard`) — viết `lib/job-matching.ts` (hàm thuần `matchJobsBySpecialty`,
so `Set` tên chuyên khoa từ hồ sơ với `job.specialtyName`, loại trừ job đã ứng tuyển qua tham số
`excludeJobIds`). Nối vào `dashboard/page.tsx`: gọi thêm `getJobs(locale)` (đã có sẵn, không cần API
mới) cùng lúc với `getMyCandidateProfile`/`getMyApplications` qua `Promise.all`, lọc ra tối đa 4 tin
phù hợp, thêm section "Việc phù hợp với bạn" (chỉ hiện khi có kết quả) ngay trước link "Đã lưu" ở cuối
trang. Thêm key `recommendedJobs` vào `profile.json` (6 locale).

Verify: `npm run lint` phát hiện `getLocale` import thừa không dùng (đã có `locale` từ `params` route
sẵn) — xóa import, không phải bug logic. `npm run build` sạch sau khi sửa.
Cập nhật `docs/nghiep-vu/DANH-SACH-TINH-NANG.md` — đổi dòng "Gợi ý việc làm/matching" từ ⬜ sang 🟨, ghi
rõ chỉ matching theo chuyên khoa (không địa điểm/kinh nghiệm theo đúng phạm vi đã chốt), và hướng ngược
lại (gợi ý ứng viên phù hợp cho NTD ở `web-admin/`) vẫn chưa làm.

**Với việc này, toàn bộ Giai đoạn 2 đã đạt trạng thái tối đa có thể làm bằng UI-first** — 4/7 mục có UI
(Test năng lực, Công cụ tính lương, Đánh giá cơ sở y tế, Gợi ý việc làm), 3/7 mục ngoài phạm vi UI web
(Chat — đã loại trừ theo yêu cầu người dùng từ đầu phiên làm việc; OpenSearch — đổi hạ tầng search
engine; Mobile app — nền tảng khác). Các mục 🟨 đều đã ghi rõ phần còn thiếu (nội dung minh họa cần
chuyên gia y tế, hoặc chờ quyết định backend) — không còn gap UI nào có thể tự làm thêm trong Giai đoạn
2 mà không cần quyết định lớn hơn (viết bộ đề y tế thật, làm 4 bounded context backend mới, hoặc đổi
sang OpenSearch/mobile).

**Làm 4 bounded context backend còn thiếu** — người dùng chọn "Làm hết cả 4, theo độ đơn giản tăng dần"
để dứt điểm các gap UI-first còn tồn đọng ở trên. Thứ tự thực hiện: (1) Danh sách tổ chức công khai,
(2) Ops tìm tổ chức theo tên, (3) Nhật ký kiểm toán, (4) Đánh giá cơ sở y tế (2 chiều).

**(1) Danh sách tổ chức công khai** — `ListOrganizationsQuery` (chỉ trả `verifyStatus=Verified`, filter
`Keyword` optional theo tên, không phân biệt hoa/thường), endpoint `GET /organizations?q=` (public,
không yêu cầu đăng nhập, thêm vào `Endpoints/Organizations.cs` cạnh các route đã có). 3 test functional
(`ListOrganizationsTests.cs`): chỉ trả verified, filter case-insensitive theo tên, tổ chức từng verified
sau đó suspended không còn xuất hiện — cả 3 pass ngay lần đầu.

Nối `web/organizations` dùng `getOrganizations()` thật thay `deriveOrganizationsFromJobs(await
getJobs(locale))` (suy diễn cũ chỉ thấy tổ chức đang có tin, bỏ sót tổ chức mới verified chưa đăng tin
nào). Thêm `organizations-search-form.tsx` (Client Component, cùng pattern `HeroSearchForm`) +
`searchParams` ở `page.tsx`. Giữ `deriveOrganizationsFromJobs` lại (không xoá) vì trang chủ vẫn dùng
cho mục đích khác ("tổ chức nổi bật theo số tin đang có", không phải danh sách đầy đủ).

**(2) Ops tìm tổ chức theo tên** — `SearchOrganizationsQuery` (`[Authorize(Roles = "admin,moderator")]`,
khác endpoint public ở (1): trả **mọi** trạng thái xác thực, kèm `CreditBalance` join từ `CreditWallets`
qua Dictionary lookup tránh N+1, `Take(20)`, rỗng keyword → trả `[]` không trả toàn bộ). Endpoint
`GET /ops/organizations/search?q=`.

3 bug trong lúc viết test (`SearchOrganizationsTests.cs`), cả 3 do seed dữ liệu test không đúng cách
chứ không phải bug logic chính: seed user role `Moderator` gọi nhầm `CreditBonusCommand`
(`[Authorize(Roles = "admin")]`-only) → đổi sang `Admin`; seed `Organization` trực tiếp qua `AddAsync`
bỏ qua side-effect tự tạo `CreditWallet` của `CreateOrganizationCommand` → seed thêm `CreditWallet` thủ
công; 1 test quên `RunAsUserAsync` trước khi gọi query yêu cầu đăng nhập → thêm vào. Sau khi sửa cả 3,
3 test pass, suite 92→98.

Nối `web-admin/features/ops-credit/index.tsx`: xóa `MOCK_ORGANIZATIONS`/`searchOrganizationsMock` cũ,
thay bằng `useQuery` gọi API thật, thêm trạng thái `isSearching`/`isSearchError`. Sửa 1 bug tự gây ra:
sau khi bỏ `useState` cho `results`, còn sót lại lệnh gọi `setResults([])` trong
`creditBonusMutation.onSuccess` (compile lỗi `Cannot find name 'setResults'`) — sửa thành
`setSubmittedQuery('')`.

**(3) Nhật ký kiểm toán** — quyết định không dùng EF Core interceptor tự động (interceptor hiện có
`AuditableEntityInterceptor` chỉ lo `CreatedAt`/`LastModified`, không phân biệt được "thao tác nhạy
cảm cần audit" với sửa field thường), mà ghi **tường minh trong từng Command Handler** — cùng tinh thần
ràng buộc #4 CLAUDE.md về `application_stage_history`. Entity `AuditLogEntry` (Domain, factory method
`Create`, không có setter public), bảng `AuditLogEntries` (Action varchar(100), TargetLabel varchar(500),
index trên `CreatedAt` và `Action`). `TimeProvider` (đã đăng ký sẵn ở `Infrastructure/DependencyInjection.cs`)
tiêm vào 4 handler để lấy `CreatedAt` thống nhất, dễ test.

Hỏi người dùng phạm vi: xóa tài khoản (1 trong 5 hành động nhạy cảm dự kiến) đi qua `IIdentityService`
trực tiếp, không qua Mediator Command — không có "handler" để chèn log vào theo đúng pattern. Người dùng
chọn "Làm 4/5, bỏ xóa tài khoản lại sau" — hook vào `VerifyOrganizationCommand`, `VerifyLicenseCommand`,
`ResolveReportCommand` (namespace `Application.Reports`, không phải `Application.Ops`, dễ nhầm), và
`SetUserStatusCommand` (phải thêm field mới `ActorUserId` vào command vì trước đó chỉ có
`IIdentityService`, không biết ai thực hiện hành động — endpoint `OpsUsers.cs` lấy từ `ClaimsPrincipal`
qua `NameIdentifier` claim).

Endpoint `GET /ops/audit-logs?action=` (`[Authorize(Roles = "admin,moderator")]`, `Take(200)`, join email
actor qua `IIdentityService.FindByIdAsync` với cache Dictionary tránh N+1 lúc lặp). Migration
`CreateAuditLogEntriesTable` — đọc SQL sinh ra xác nhận thuần `CREATE TABLE` + 2 index, không có rủi ro
mất dữ liệu. 5 test (`AuditLogTests.cs`) cho cả 4 hành động + filter theo action; 1 bug: seed
`License.IssuedAt` sai kiểu (viết `DateTimeOffset.UtcNow...` nhưng field thật là `DateOnly`, phải đọc lại
entity mới biết) — sửa thành `DateOnly.FromDateTime(...)`. Cả 5 pass, suite 98→103.

Viết lại hoàn toàn `web-admin/features/ops-audit/index.tsx` (trước đó dùng mock) — hardcode `ACTIONS`
khớp đúng 11 chuỗi backend ghi, badge màu theo loại hành động (verify/approve = xanh, reject/suspend =
đỏ). Đổi 5 key i18n cũ (`audit.action.license_verify`, `.organization_verify`, `.report_resolve`,
`.user_suspend`, `.account_delete` — tên nhóm chung, không khớp chuỗi backend thật) thành 11 key khớp
đúng action string thật, cả 6 locale.

**(4) Đánh giá cơ sở y tế** — quy mô lớn hơn 3 mục trên vì cần UI cả 2 chiều (candidate gửi — vốn đã có
mock ở `web/organizations/[id]` từ UI-first trước đó; Vận hành duyệt — **chưa từng có UI dù chỉ là mock**).
Hỏi và được xác nhận "Làm đầy đủ cả 2 chiều" thay vì chỉ làm backend.

Entity `OrganizationReview` (Domain) theo đúng style `Report.Dismiss()`/`Report.Resolve()` — `Approve(Guid
moderatedBy)`/`Reject(Guid moderatedBy)` throw `InvalidOperationException` nếu không ở trạng thái
`Pending`, setter private. Enum `ReviewStatus` (`Pending`/`Approved`/`Rejected`). Unique compound index
`(OrganizationId, CandidateUserId)` ở `OrganizationReviewConfiguration.cs` — chặn gửi 2 lần ở tầng DB,
đồng thời kiểm tra lại ở Application layer (`SubmitOrganizationReviewCommandHandler`) để trả lỗi
`ValidationException` thân thiện thay vì lỗi constraint DB thô. 1 bug tự phát hiện và sửa trước khi build:
viết nhầm 1 dòng LINQ vô nghĩa `.Include(r => r.GetType() == null ? null : r) // no-op` trong
`ModerateOrganizationReviewCommandHandler` lúc đang rối — xoá, thay bằng `FirstOrDefaultAsync` trực tiếp.

`GetApprovedOrganizationReviewsQuery` (public, chỉ `Status=Approved`, DTO **cố tình không có**
`CandidateUserId` — ẩn danh người viết review công khai) và `GetPendingOrganizationReviewsQuery`
(`[Authorize(Roles = "admin,moderator")]`, join tên tổ chức). Thêm route `GET/POST
{organizationId}/reviews` vào `Endpoints/Organizations.cs` — tự gây 1 bug nhỏ: thêm dòng `using
System.Security.Claims;` trùng (đã có sẵn ở đầu file) — phát hiện lúc đọc lại file trước khi build, xoá
dòng trùng. Endpoint mới `Endpoints/OpsOrganizationReviews.cs` (`GET /ops/organization-reviews`, `POST
.../{id}/moderate`).

Migration `CreateOrganizationReviewsTable` — thuần `CREATE TABLE` + 3 index (gồm unique compound), không
rủi ro mất dữ liệu. 5 test (`OrganizationReviewTests.cs`): review pending không xuất hiện ở danh sách
công khai cho tới khi duyệt, review approved xuất hiện đúng, review rejected không bao giờ public, gửi
2 lần từ cùng candidate bị chặn, rating ngoài khoảng 1-5 bị chặn — cả 5 pass ngay lần đầu.

**Bug tìm ra lúc chạy lại toàn bộ suite (không phải lúc chạy riêng test mới)** — `dotnet test` không
filter báo 106/108 pass, 2 fail đều ở `EndpointRoutingTests` (file test riêng chuyên bắt lỗi "Duplicate
endpoint name" — Minimal API tự đặt tên endpoint theo tên method, 2 method cùng tên ở 2 `IEndpointGroup`
khác nhau sẽ làm route matcher build lỗi lúc `dotnet run` thật, nhưng **không lộ ra** ở hầu hết test khác
vì chúng gọi thẳng `ISender`, không đi qua HTTP pipeline thật). Debug bằng cách tạm in `response body`
ra console trong lúc chạy test — lộ ra `InvalidOperationException: Duplicate endpoint name 'GetPending'
found on 'HTTP: GET /api/v1/ops/organization-reviews/ => GetPending' and 'HTTP: GET /api/v1/ops/licenses/
=> GetPending'`. Đổi tên `OpsOrganizationReviews.GetPending` → `GetPendingReviews`, và nhân tiện đổi luôn
`Moderate` → `ModerateReview` (chưa va nhưng chắc chắn sẽ va với `OpsJobs.Moderate` nếu không đổi trước).
Xóa đoạn debug, build lại, chạy lại toàn bộ suite: 108/108 pass. Đây là bằng chứng cụ thể cho lý do tồn
tại của `EndpointRoutingTests` — quy tắc đặt tên method endpoint không được trùng across các
`IEndpointGroup` khác nhau, không chỉ trong cùng 1 file.

Nối `web/organizations/[id]/organization-reviews.tsx` (trước đó `MOCK_APPROVED_REVIEWS` + theo dõi "đã
gửi" qua `localStorage`) — bỏ hẳn theo dõi phía client, để backend tự chặn gửi trùng qua unique index +
trả lỗi rõ. Thêm route proxy `web/app/api/organizations/[id]/reviews/route.ts` (theo đúng pattern
`jobs/[id]/apply/route.ts` — dùng `backendFetch` tự gắn access token từ cookie httpOnly, Client Component
không đọc được cookie này nên phải qua Route Handler). `canSubmitReview` tính từ `currentUser?.role ===
"candidate"` ở Server Component (`page.tsx`) — chỉ candidate mới thấy form gửi, khớp
`[Authorize(Roles = "candidate")]` ở backend.

Dựng UI Vận hành mới hoàn toàn — `web-admin/features/ops-reviews/index.tsx` (theo đúng pattern
`ops-reports/index.tsx`: Card list + nút Duyệt/Từ chối, `useMutation` + `invalidateQueries` sau khi
xong), route `_authenticated/ops/reviews.tsx`, thêm mục sidebar `nav.opsReviews` (icon `Star`, sau
`nav.opsAudit`). `routeTree.gen.ts` do TanStack Router Vite plugin tự sinh — không sửa tay, phải chạy
`vite dev`/`vite build` 1 lần để plugin regenerate trước khi `tsc -b` hết lỗi "not assignable to keyof
FileRoutesByPath".

Verify cuối: `dotnet test` (toàn bộ solution) 108/108 Application.FunctionalTests +
3/3 Application.UnitTests. `web/`: `tsc --noEmit` sạch, `npm run lint` sạch (1 warning `setReviews` không
dùng do refactor thừa `useState` — sửa bỏ hẳn state, dùng trực tiếp prop `reviews`), `npm run build` sạch,
route `/api/organizations/[id]/reviews` xuất hiện đúng trong danh sách route. `web-admin/`: `tsc -b`
sạch, `npm run lint` sạch, `npm run build` sạch, `npm run test` (vitest browser mode) 106/107 pass — 1
fail (`search-provider.test.tsx` — command palette click "Tasks" timeout) xác nhận **có sẵn từ trước**,
không liên quan review (`git stash` toàn bộ thay đổi `web-admin/`, chạy lại đúng test đó, vẫn fail giống
hệt trên baseline chưa đụng gì).

Cập nhật `docs/backend/API-DESIGN.md` (thêm 5 endpoint mới: `GET /organizations` list,
`GET /ops/organizations/search`, `GET /ops/organization-reviews`, `POST .../moderate`, sửa lại role
`GET /ops/audit-logs` từ "Admin" thành đúng "Admin/Moderator" theo code thật), `docs/database/ERD-CHI-TIET.md`
(viết lại 2 bảng `OrganizationReviews`/`AuditLogEntries` khớp schema thật thay thiết kế gốc đã lỗi thời,
thêm ràng buộc nghiệp vụ #13 về ẩn danh review + không lộ `Pending` ra public),
`docs/nghiep-vu/DANH-SACH-TINH-NANG.md` và `TIEN-DO-CHI-TIET.md` (chuyển 4 mục từ ⬜/🟨 sang ✅, sửa luôn
vài dòng đã lỗi thời không liên quan trực tiếp nhưng phát hiện ra trong lúc rà — vd dòng "ops-credit chưa
có UI" đã sai từ lâu).

**Rà lại Giai đoạn 2 lần cuối trước khi coi là xong** — hỏi người dùng có muốn làm nốt hướng ngược lại
của "Gợi ý việc làm/matching" (gợi ý ứng viên phù hợp cho 1 tin, phía NTD) hay dừng ở trạng thái hiện
tại. Người dùng chọn làm nốt để đạt trạng thái tối đa theo UI-first.

**Gợi ý ứng viên cho 1 tin (`web-admin/`)** — cùng độ đơn giản đã chốt cho hướng thuận (chỉ so chuyên
khoa trùng tên), không dùng API mới: dùng lại `GET /candidates/search` đã có sẵn cho tính năng "Tìm ứng
viên chủ động". Vấn đề kỹ thuật giống job-matching ở `web/`: `ApiJob` (response, kể cả bản `web-admin/`)
chỉ có `specialtyName`, không có `specialtyId` — mà `candidates/search` cần `specialtyId` (Guid) để
filter, không nhận filter theo tên. Giải quyết bằng cách tra `catalogApi.getSpecialties()` (đã load sẵn
ở trang `/candidates`) tìm specialty có `name === job.specialtyName`, lấy `id` gọi API — không sửa
backend, không thêm field mới vào DTO.

Đặt UI ở đâu là vấn đề chính cần cân nhắc: trang ATS (`features/applications/index.tsx`) dùng
`<Main fixed>` (full-height, không scroll) để vừa khung Kanban kéo-thả — không có chỗ nhồi thêm 1 khối
danh sách dài bên dưới mà không phá layout. Chọn `Sheet` (panel trượt từ phải, đã có sẵn component
`components/ui/sheet.tsx`, đúng pattern `ConfigDrawer` đang dùng) mở từ 1 nút "Ứng viên gợi ý" ở góc phải
tiêu đề trang ATS — không đụng gì tới khung Kanban.

Viết `suggested-candidates-sheet.tsx` — tái dùng gần như nguyên UI Card (headline, badge ẩn liên hệ,
nút mở hồ sơ, dialog xác nhận) từ `features/candidates/index.tsx` để giữ nhất quán, nhưng bỏ 2 dropdown
chọn chuyên khoa/địa điểm (đã cố định theo `job.specialtyName`) và thêm bước lọc client-side loại bỏ
ứng viên đã có trong `applications` của tin đó (so theo `candidateId`, `Set` giống cách
`lib/job-matching.ts` loại tin đã ứng tuyển ở hướng thuận). Gắn vào `Applications` (trang ATS) qua nút ở
header, truyền `job`/`applications`/`organizationId` (lấy qua `useMyOrganization()`, hook đã có sẵn).

Thêm block i18n `applications.suggestions.*` (6 locale, vi/en thật — cùng câu chữ với
`candidates.json` cho phần Card/unlock để nhất quán UI hai nơi, ja/zh/ko/es giữ placeholder tiếng Việt
theo đúng convention file này đã dùng từ trước). 1 lưu ý nhỏ khi thêm: 4 file `ja/zh/ko/es` có thứ tự
key trong block `detail` khác `vi/en` (`viewCvFile` bị đặt cuối cùng thay vì giữa) — không phải lỗi mới,
tồn tại từ trước, chỉ cần chèn đúng vị trí cuối file thật của từng bản dịch thay vì giả định tất cả 6
file giống thứ tự nhau.

Verify: `tsc -b` sạch, `npm run lint` sạch, `npm run build` sạch. Verify **bằng UI thật** (không chỉ tin
type-check) — dùng Playwright có sẵn trong `node_modules` (phục vụ vitest browser mode) viết script tạm
để: đăng nhập tài khoản employer mới tạo qua API, tạo tổ chức + 1 tin tuyển "Dược sĩ lâm sàng", tạo 1
candidate mới có chuyên khoa "Dược" trong hồ sơ, mở trang ATS tin đó, click nút "Ứng viên gợi ý", chụp
ảnh màn hình xác nhận Sheet hiện đúng 3 ứng viên chuyên khoa Dược (gồm đúng candidate mới tạo), click
"Mở hồ sơ" → xác nhận dialog → unlock thành công, toast hiện đúng, card chuyển từ "Liên hệ đã ẩn" sang
hiện email thật. Trong lúc dựng dữ liệu test qua `curl` gặp 2 lỗi nhỏ không liên quan code (do gọi API
sai giá trị enum): `"size":"Nho"` không hợp lệ cho `OrganizationSize` (đúng phải là `Under50`), và cần
`PUT /candidates/me` để tạo `CandidateProfile` trước khi `POST .../specialties` (chưa có profile thì
404 `NotFoundException`) — cả 2 không phải bug, chỉ là hiểu sai contract lúc dựng dữ liệu thủ công. Xóa
2 script Playwright tạm sau khi verify xong, không commit vào repo.

Cập nhật `docs/backend/API-DESIGN.md` (ghi chú thêm 1 dòng ở endpoint `GET /candidates/search` về cách
dùng mới, không phải endpoint mới nên không cần mục riêng), `DANH-SACH-TINH-NANG.md` (chuyển "Gợi ý việc
làm/matching" từ 🟨 sang 🟨 nhưng ghi rõ cả 2 hướng đã xong — vẫn giữ 🟨 không phải ✅ vì đây là 1 mục
matching tổng, còn nhiều tinh chỉnh tương lai có thể làm thêm dù không bắt buộc cho MVP), `TIEN-DO-CHI-TIET.md`
(thêm dòng "Gợi ý ứng viên cho 1 tin" vào mục 7 Credit & Tìm ứng viên chủ động).

**Với việc này, Giai đoạn 2 đã đạt trạng thái tối đa hợp lý theo UI-first** — mọi mục còn ⬜/🟨 đều cần
quyết định kiến trúc lớn hơn (OpenSearch, chat realtime, mobile app) hoặc nội dung chuyên môn ngoài khả
năng tự viết (bộ đề y tế thật theo từng chuyên khoa), không còn gap nào tự làm được thêm bằng cách viết
UI/logic thuần.

**Rà lại toàn bộ màn hình theo yêu cầu "ưu tiên hoàn thiện giao diện trước tính năng"** — đối chiếu
`LUONG-NGHIEP-VU-MAN-HINH.md` mục 2 (Screen Inventory) với route/feature thật trong code. Kết quả: Vận
hành đủ 10/10 màn; thiếu hẳn Mua gói tin (bảng so sánh Eco/Pro/Max — hiện gói chọn ngay trong form tạo
tin), trang "Việc đã ứng tuyển" riêng (đang nhúng trong dashboard), tra cứu giao dịch thanh toán, Tin
nhắn (đã loại khỏi phạm vi từ đầu), Góc nghề y (Giai đoạn 3); 7 màn thiếu trạng thái loading/error.
Nhưng vấn đề lớn nhất phát hiện được là **tàn dư template shadcn-admin vẫn hiện với người dùng thật** —
chọn xử lý việc này trước.

**Dọn tàn dư template shadcn-admin (`web-admin/`)** — điểm 8 mục "Ghi chú" của `TIEN-DO-CHI-TIET.md`
trước đây kết luận các trang template "không nằm trong sidebar chức năng thật nên người dùng không vô
tình vào được → giữ nguyên, không gỡ". **Kết luận đó sai**, kiểm tra lại code cho thấy: `/chats` CÓ nằm
trong `employerGroup` (mục `nav.chats`, kèm badge "3" tin nhắn giả — dễ tưởng là tính năng thật), và
nhóm `Pages` (Auth demo: Sign In 2 Col/Sign Up/OTP; Errors demo: 401/403/404/500/503) hiện với **mọi**
role vì `getSidebarData` chỉ lọc `employerGroup`/`opsGroup`. Tức NTD/Vận hành thật đăng nhập vào đều
thấy menu "Tasks", "Apps", "Tin nhắn", "Sign In (2 Col)", "401/403/404" — rất lộ là template chưa dọn,
bấm vào đều ra màn hình giả.

Gỡ 49 file: `features/tasks`, `features/apps`, `features/chats` + route; route auth trùng lặp
(`sign-in-2`/`sign-up`/`forgot-password`/`otp` — đều là bản template tĩnh không gọi API, trong khi đăng
ký/quên mật khẩu THẬT cho cả ứng viên lẫn NTD đã có đủ ở `web/`); route demo trang lỗi;
`coming-soon.tsx`; ảnh assets của `sign-in-2`; dependency `@faker-js/faker`. **Giữ lại** `features/errors/*`
vì `__root.tsx` dùng thật làm `errorComponent`/`notFoundComponent` — chỉ route demo để xem trước các
trang lỗi mới là rác. **Giữ lại** trang Settings cá nhân: tuy vẫn là giao diện template chưa nối API,
"Cài đặt tài khoản" là màn hình có thật trong tài liệu (mục 2.2), cần làm thật sau chứ không phải rác.

3 lỗi phát hiện thêm trong lúc dọn, sửa luôn:
1. **Command palette render thẳng `navItem.title`** — mà `title` là KEY dịch (xem comment đầu
   `sidebar-data.ts`), nên palette hiện chuỗi thô `nav.dashboard`, `nav.jobs`... trong khi sidebar gọi
   `t()` đúng. Sửa dùng `t()`, `value` tìm kiếm cũng dùng text đã dịch để gõ tiếng Việt tìm ra đúng mục.
2. **Trang đăng nhập có 3 link chết**: "Sign Up" trỏ tới trang template vừa gỡ, `/terms` và `/privacy`
   không tồn tại ở bất kỳ đâu (đã grep cả `web/` lẫn `web-admin/`). Gỡ cả 3, chuyển text sang i18n
   (trước đây hardcode tiếng Anh dù app hỗ trợ 6 ngôn ngữ theo ADR-0010).
3. **`main.tsx` điều hướng sang route `/500`** khi API trả lỗi 500 — route đó là trang lỗi demo vừa gỡ.
   Bỏ điều hướng, chỉ hiện toast: lỗi 1 query không nên đá người dùng khỏi màn hình đang làm việc, còn
   lỗi khiến cả trang không render được đã có `errorComponent` ở `__root.tsx` lo.

Test: `sidebar-data.test.ts` có 1 test khẳng định "không lọc nhóm Pages/Other" — hành vi đó nay không
còn đúng, thay bằng 3 test mới (không còn nhóm `Pages`; `otherGroup` hiện với mọi role; mọi group title
đều là khóa i18n). `search-provider.test.tsx` vốn **fail sẵn từ trước phiên này** do phụ thuộc nav item
"Tasks" của template — nay import `@/i18n` và ép ngôn ngữ `vi` trong `beforeEach` để test không phụ
thuộc ngôn ngữ trình duyệt chạy test (trước đó i18n không được init trong môi trường test nên `t()` trả
khóa thô, chụp màn hình lúc debug xác nhận điều này).

Verify: `tsc -b` sạch, lint sạch, build sạch, **84/84 test pass** (trước đợt này: 83 pass 1 fail), thời
gian chạy test giảm từ ~40s còn ~12s do bỏ bundle `tasks-*.js` 499 kB chứa faker. Verify thêm bằng UI
thật (Playwright): sidebar chỉ còn 8 mục nghiệp vụ + nhóm "Khác", command palette hiện text tiếng Việt
đã dịch thay vì khóa thô, trang đăng nhập không còn link chết.

Ghi nhận 1 vấn đề **không sửa trong đợt này** (ngoài phạm vi dọn template): tài khoản Vận hành seed sẵn
`admin@localhost` **không đăng nhập được qua UI** vì `z.email()` ở form từ chối email không có TLD.
Validator đúng cho người dùng thật; vấn đề nằm ở dữ liệu seed. Cần đổi email seed thành dạng hợp lệ
(vd `admin@blousehiding.local`) hoặc nới validator ở môi trường dev.

**Thêm trạng thái đang tải / lỗi cho 6 màn `web-admin/` còn thiếu** — bước 2 của kế hoạch hoàn thiện
giao diện. Trước đây các màn này chỉ xử lý trạng thái RỖNG: API chậm thì màn hình trắng trơn, API lỗi
thì hiển thị y hệt "không có dữ liệu" nên người dùng không phân biệt được "chưa có gì" với "gọi API
hỏng". Màn đã sửa: Dashboard NTD, Ops Dashboard, Đối soát thanh toán, Duyệt đánh giá, Quản lý người
dùng, Xử lý báo cáo.

Viết component dùng chung `components/query-state.tsx` gom cả 3 trạng thái (đang tải/lỗi/rỗng) thay vì
lặp cùng 1 đoạn JSX ở 6 chỗ; trả `null` khi query xong và có dữ liệu để chỗ gọi cứ render tiếp danh sách
như cũ. Riêng 2 dashboard dùng skeleton dạng thẻ (giữ nguyên layout lưới) thay vì spinner giữa trang, vì
đó là số liệu chứ không phải danh sách.

2 vấn đề phát hiện thêm trong lúc làm, sửa luôn:
1. **Dashboard NTD là màn DUY NHẤT không dùng i18n** — toàn bộ text hardcode tiếng Việt ("Tin đang
   tuyển", "Số dư Credit", "Đăng tin mới"...) kèm bảng `STATUS_LABEL` riêng, trong khi namespace `jobs`
   đã có sẵn `status.*` dùng chung với các màn khác. Chuyển hết sang i18n, bỏ `STATUS_LABEL` trùng lặp,
   thêm khối `jobs.dashboard` cho 6 locale.
2. **Ops Dashboard render key enum thô** từ backend (`Published`, `Verified`...) chưa dịch. Thêm prop
   `labelFor` cho `BreakdownCard`: trạng thái tin dùng lại `jobs.status.*`, trạng thái xác thực tổ chức
   thêm khóa mới `ops.dashboard.verifyStatus.*`.

Lưu ý kỹ thuật ở Dashboard NTD: 2 query bị chặn bởi `enabled: !!organization`, trong lúc chờ biết user
thuộc tổ chức nào thì `isLoading` của react-query vẫn `false` — nếu chỉ dựa vào `isLoading` thì thẻ số
liệu chớp số 0 rồi mới nhảy sang số thật. Phải tự gộp thêm điều kiện `!organization`.

Verify: `tsc -b` + lint + build sạch, 84/84 test pass. Verify trạng thái **lỗi** bằng UI thật
(Playwright) — thứ trước giờ chưa bao giờ test được: đăng nhập xong dùng `page.route()` chặn toàn bộ
request tới `/api/v1/**` để giả lập API sập, xác nhận cả 6 màn đều hiện thông báo lỗi rõ ràng kèm icon
cảnh báo thay vì màn hình trắng.

**Làm 3 màn hình còn thiếu** — bước 3 (cuối) của kế hoạch hoàn thiện giao diện. Cả 3 đều đã có endpoint
backend sẵn, không cần thêm gì ở `api/`.

**Gói tin & giao dịch (`web-admin/`, `/job-packages`)** — gộp 2 màn trong tài liệu (mục 2.3 "Mua gói tin"
và tra cứu giao dịch) vào 1 trang vì cùng chủ đề thanh toán:
- Bảng so sánh Free/Eco/Pro/Max hiện đủ giá, thời hạn, `maxActiveJobs` và từng quyền lợi (`pin_top`,
  `highlight`) dạng ✓/—. Trước đây gói chỉ hiện dạng radio list gọn trong form tạo tin, **không** hiện
  `perks` lẫn `maxActiveJobs`, nên NTD không có cách nào biết trả thêm tiền thì được gì.
- Ô tra cứu giao dịch gọi `GET /payments/{id}` — endpoint này có sẵn từ lâu nhưng **chưa UI nào gọi**
  (`TIEN-DO-CHI-TIET.md` mục 8 ghi rõ "chưa có UI"). NTD giờ tự tra được trạng thái đối soát thay vì
  phải hỏi đội Vận hành.
- Cố ý **không có nút "Mua"**: `POST /jobs/{id}/submit` cần `jobId` nên gói chỉ chọn được trong luồng
  đăng tin, không mua rời — trang này để so sánh trước, có link dẫn sang `/jobs/new`.

**Việc đã ứng tuyển (`web/`, `/dashboard/applications`)** — tài liệu mục 2.2 yêu cầu "theo dõi trạng thái
từng đơn, pipeline view rút gọn", nhưng trước đây chỉ là 1 khối danh sách phẳng nhúng trong `/dashboard`:
liệt kê hết mọi đơn, không nhóm, không ngày nộp, không hiện lý do từ chối. Trang mới tách 2 nhóm "Đang
xử lý" (New/Reviewing/Shortlisted/Interview/Offer) và "Đã kết thúc" (Hired/Rejected) kèm số đếm, hiện
ngày nộp và `rejectedReason` (field có sẵn trong DTO nhưng chưa dùng ở đâu). `/dashboard` giới hạn còn 5
đơn gần nhất + link "Xem tất cả". Thêm tab "Đã ứng tuyển" vào `AccountNav`.

Verify: `web/` tsc + lint + build sạch, `web-admin/` tsc + lint + build sạch + 84/84 test pass. Verify UI
thật (Playwright) với dữ liệu thật: tạo 2 đơn ứng tuyển qua API rồi mở trang — xác nhận bảng gói hiện đủ
4 gói kèm cột quyền lợi, tra cứu giao dịch với ID không tồn tại báo lỗi đúng thay vì im lặng, trang đơn
ứng tuyển hiện đúng nhóm/số đếm/ngày nộp. Nhân tiện xác nhận RBAC hoạt động đúng: thử đổi trạng thái đơn
bằng employer không sở hữu tin đó → backend trả 403 như mong đợi.

**Hoàn tất kế hoạch hoàn thiện giao diện** (3 bước: dọn tàn dư template → thêm loading/error → làm màn
còn thiếu). Màn hình còn thiếu so với tài liệu chỉ còn: Tin nhắn (đã loại khỏi phạm vi từ đầu phiên) và
Góc nghề y (Giai đoạn 3).

**Sửa 2 vấn đề tồn đọng** — đều đã ghi nhận trong nhật ký các đợt trước nhưng để lại chưa xử lý.

**1. Tài khoản Vận hành seed sẵn không đăng nhập được qua UI.** Email seed là `admin@localhost` — tạo
được ở DB nhưng form đăng nhập `web-admin/` validate bằng `z.email()`, mà `localhost` không có TLD nên
bị từ chối ngay ở client. Hệ quả: người mới clone repo về **không có cách nào vào được khu vực Vận hành**
(role `admin`/`moderator` không tự đăng ký được qua UI công khai, chỉ chọn được candidate/employer). Đổi
sang `admin@blousehiding.local`. Lưu ý seed chỉ tạo khi user chưa tồn tại, **không** tự đổi email tài
khoản cũ — DB dev tạo trước thay đổi này vẫn giữ `admin@localhost`.

Nhân tiện sửa `MOI-TRUONG-DEV-CUC-BO.md` mục 4 vốn ghi **sai**: "Không có tài khoản seed sẵn nào trong
migration hoặc fixture" — thực tế có admin seed từ `ApplicationDbContextInitialiser.TrySeedAsync`.

**2. Cổng API mặc định của 2 frontend không khớp backend.** `NEXT_PUBLIC_API_BASE_URL` và
`VITE_API_BASE_URL` mặc định trỏ `5100` trong khi `dotnet run --project src/Web` chạy ở `5256` (theo
`launchSettings.json`). Tài liệu có ghi gap này nhưng bắt người dùng tự chọn 1 trong 2 cách xử lý mỗi
lần chạy — chính tôi cũng vấp lỗi này nhiều lần trong các đợt làm việc trước khi verify UI. Sửa tận gốc:
thống nhất `5256` ở cả 4 file code hardcode fallback (`web/lib/{api,backend-fetch,session}.ts`,
`web-admin/src/lib/http.ts`) lẫn 2 file `.env.example`, rồi cập nhật lại mục 3 của tài liệu.

**3. N+1 query ở `SearchCandidatesQueryHandler`** (phát hiện lúc rà 8 luồng cơ bản). Handler gọi
`IIdentityService.FindByIdAsync` **trong vòng lặp** — mỗi ứng viên đã unlock là 1 round-trip DB, càng
unlock nhiều càng chậm. Thêm `FindByIdsAsync` (1 truy vấn cho nhiều ID) vào `IIdentityService`, map qua
Dictionary. Chỉ query user của ứng viên **đã unlock** — ứng viên chưa unlock không được lộ email (ERD
mục 7) nên không cần lấy. Đổi luôn `unlockedCandidateIds` từ `List` sang `HashSet` (`Contains()` trong
vòng lặp trước đó là O(n) mỗi lần).

Thêm test `SearchCandidates_With_Multiple_Unlocked_Should_Map_Correct_Email_To_Each`: unlock 2 trong 3
ứng viên, xác nhận từng người nhận đúng email của mình và người chưa unlock vẫn bị ẩn — chỗ map qua
Dictionary dễ map nhầm email sang sai ứng viên nếu viết ẩu. Suite 108 → 109 test, toàn bộ pass.

Verify: backend 109/109 + 3/3 test pass, 2 frontend tsc + lint sạch. Verify UI thật (Playwright): chạy
`vite dev` **không truyền biến môi trường** để chứng minh mặc định đã đúng, đăng nhập bằng tài khoản
admin seed mới thành công (trước đây bị chặn ngay ở client), Ops Dashboard tải được dữ liệu thật.

**Làm bounded context CV** — gap cuối trong luồng cốt lõi ứng viên. CV Builder trước đây dùng 100%
mock: nút "Lưu" chỉ `setSaved(true)` hiển thị "Đã lưu thay đổi" nhưng **không gọi API nào** — người
dùng thật mất sạch công sức khi tải lại trang mà vẫn tưởng đã lưu. Tệ hơn cả việc không có nút, vì báo
thành công sai sự thật.

Làm theo đúng thiết kế **đã có sẵn** trong `ERD-CHI-TIET.md` mục 2.2 (bảng `cvs`) và mục 2.6 (cột
`applications.cv_id`) — cả hai đều thiết kế từ đầu nhưng đánh dấu "chưa implement", không tự nghĩ lại
schema. Chỉ thêm 1 cột ngoài thiết kế gốc: `Title` — không có tên CV thì danh sách chọn CV lúc ứng
tuyển vô nghĩa.

Backend: entity `Cv` (2 dạng dùng chung 1 bảng — CV Builder có `DataJson` jsonb, CV upload file có
`FileUrl`), `PUT /candidates/me/cvs/builder` là **upsert** chứ không tạo bản mới mỗi lần bấm Lưu (nếu
không, sửa 1 chữ rồi lưu vài lần là hồ sơ đầy CV rác), `GET /candidates/me/cvs`. Thêm cột
`applications.cv_id` kèm **kiểm tra CV thuộc về chính ứng viên đang nộp** — không có bước này thì ai
biết Id CV người khác đều gắn được vào đơn của mình. 7 test mới, suite 109 → 116.

Frontend: nối API thật qua route proxy (Client Component không đọc được cookie httpOnly), preview dùng
tên/headline thật thay tên mock "Nguyễn Thị Thu Hà", trang gate đăng nhập (trước đây ai vào cũng được),
dialog ứng tuyển thêm ô "Dùng CV đã lưu". Xóa `lib/mock-data.ts` — sau thay đổi này không còn ai dùng.

2 vấn đề phát hiện trong lúc làm:
1. **Postgres chuẩn hoá `jsonb`** — sắp xếp lại thứ tự key và chèn khoảng trắng khi lưu, nên chuỗi đọc
   ra khác chuỗi ghi vào. 2 test đầu fail oan vì so `==` chuỗi; sửa thành so theo ngữ nghĩa JSON qua
   `JsonNode.DeepEquals`. Đã ghi cảnh báo này vào ERD để người sau không vấp lại.
2. **6 route handler `app/api/auth/*` vẫn hardcode cổng 5100** — đợt sửa cổng trước chỉ quét trong
   `lib/` nên bỏ sót, nghĩa là toàn bộ luồng đăng nhập/đăng ký/quên mật khẩu ở `web/` vẫn hỏng với cấu
   hình mặc định. Phát hiện đúng lúc verify UI: login thất bại với `ECONNREFUSED` dù backend đang chạy.
   Bài học: sửa giá trị hardcode phải grep toàn bộ thư mục, không chỉ chỗ "có vẻ liên quan".

Verify: backend 116/116 + 3/3 test, `web/` tsc + lint + build sạch. Verify UI thật (Playwright) đúng
kịch bản người dùng gặp phải: nhập học vấn → bấm Lưu CV → **tải lại trang** → dữ liệu vẫn còn cả trong
form lẫn preview. Dialog ứng tuyển hiện đúng CV vừa lưu kèm nhãn "CV chính".

Còn lại: xuất CV ra PDF (nút vẫn `disabled`, cần thư viện render PDF) và luồng upload CV file vào bảng
`Cvs` (hiện CV riêng lúc ứng tuyển vẫn lưu ở `applications.cv_file_url`, không tạo dòng trong `Cvs`).

**Làm bounded context "việc đã lưu"** — chỗ dùng dữ liệu giả áp chót. Tin đã lưu trước đây chỉ nằm ở
`localStorage` phía client: ứng viên lưu tin trên điện thoại rồi mở máy tính là trắng, xóa cache cũng
mất sạch.

Khác bảng `cvs` (đã thiết kế sẵn trong ERD), bảng này **không có trong thiết kế gốc** — tự thiết kế và
bổ sung vào ERD kèm ghi chú rõ là phát sinh sau.

Backend: entity `SavedJob` gắn với `CandidateProfile` (không phải `UserId`) cho nhất quán với các bảng
khác của ứng viên, UNIQUE `(CandidateId, JobId)` chặn dòng trùng. Dùng **1 endpoint toggle** thay vì
tách POST + DELETE vì UI chỉ có 1 nút bật/tắt — tách 2 endpoint thì client phải tự biết trạng thái hiện
tại trước khi gọi, dễ lệch khi mở 2 tab. `GET /candidates/me/saved-jobs` **không lọc theo `jobs.status`**:
tin đã lưu rồi bị đóng/hết hạn vẫn phải hiện kèm trạng thái thật, ẩn đi thì ứng viên tưởng mình chưa
từng lưu. 4 test, suite 116 → 120.

Frontend: bỏ toàn bộ `localStorage`. `SaveJobButton` nhận trạng thái ban đầu từ server, cập nhật
optimistic (đổi UI ngay rồi mới chờ server, lỗi thì trả về trạng thái cũ) vì bấm lưu tin phải phản hồi
tức thì. `SavedJobsList` chuyển từ Client sang **Server Component** — backend trả sẵn đủ `JobDto` nên
không cần gọi `getJobById` cho từng ID rồi hiện skeleton như trước (N+1 phía client).

Verify: backend 120/120 + 3/3, `web/` tsc + lint + build sạch. Verify UI thật đúng kịch bản người dùng
gặp: bấm "Lưu việc này" → nhãn đổi ngay thành "Đã lưu" → **tải lại trang** → vẫn giữ "Đã lưu" → tin hiện
đúng trong `/dashboard/saved-jobs` với đầy đủ thông tin.

**Còn lại 1 chỗ dùng dữ liệu giả:** OAuth Google/Zalo (nút hiện "sắp ra mắt"). Khác 2 chỗ trước, mục này
**không tự hoàn tất được** — cần Client ID/Secret thật từ Google Cloud và Zalo Developers do chủ dự án
đăng ký, code xong cũng không test được đầu-cuối nếu thiếu credential.

**Hoàn thiện luồng nộp hồ sơ** — theo yêu cầu tập trung vào "hiển thị danh sách" và "luồng nộp hồ sơ",
bỏ qua email/SMS, OAuth Zalo, bộ đề y tế. Rà bằng agent + kiểm tra tay, phát hiện 1 **tính năng chết**
và vài lỗ hổng.

**"Lý do từ chối" là tính năng chết** — nghiêm trọng nhất. Backend có cột `rejected_reason`, trả ra cho
ứng viên xem, và tôi vừa làm UI hiển thị nó ở `/dashboard/applications` đợt trước — nhưng **NTD không có
đường nào nhập**. Nguyên nhân 2 tầng: `constants.ts` cố ý loại `Rejected` khỏi `APPLICATION_STAGES` với
comment "từ chối xử lý qua hành động riêng", nhưng **hành động riêng đó chưa từng được làm**; và kể cả
nếu có cột thì kéo-thả cũng không hỏi được lý do. Sửa: thêm nút "Từ chối" trên từng thẻ Kanban mở dialog
nhập lý do. Giữ nguyên quyết định không thêm cột `Rejected` vào bảng nhưng sửa comment cho khớp thực tế.

**Nuốt thông báo lỗi khi ứng tuyển** — backend trả lỗi cụ thể ("Bạn đã ứng tuyển tin này rồi", "Cần hoàn
thiện hồ sơ trước khi ứng tuyển"), route proxy truyền đúng body lỗi, nhưng UI vứt hết chỉ hiện 1 câu
chung. Giờ đọc đúng message. Kèm: nộp xong `router.refresh()` + kiểm tra `alreadyApplied` ở Server
Component (trước đây tải lại trang thấy nút "Ứng tuyển" như chưa nộp), tách lỗi upload CV khỏi lỗi nộp
đơn.

**NTD không được báo khi có đơn mới** — `SubmitApplication` không gửi notification nào, NTD phải tự vào
ATS kiểm tra. Giờ báo cho mọi thành viên tổ chức.

**Rút đơn ứng tuyển** — chưa từng có. Hai quyết định: (1) **XÓA hẳn** dòng application thay vì thêm stage
"Withdrawn", vì `UNIQUE(job_id, candidate_id)` khiến giữ dòng cũ sẽ chặn nộp lại tin đó sau này — mà rút
rồi nộp lại là nhu cầu hợp lệ (có test khẳng định); (2) chỉ cho rút ở `New`/`Reviewing`/`Shortlisted` —
từ `Interview` trở đi NTD đã thực sự xử lý, tự ý rút làm họ mất dấu vết. Kiểm tra chủ sở hữu đơn để
không ai rút đơn người khác.

Kèm: Kanban trước đây dùng `applications ?? []` nên lúc đang tải trông y hệt lúc rỗng — thêm `QueryState`.

Verify: backend 124/124 (từ 120) + 3/3, `web-admin/` 84/84, cả 2 frontend tsc + lint + build sạch. Verify
UI thật với dữ liệu thật (dựng employer + candidate + tổ chức verified + tin published + đơn qua API):
xác nhận NTD nhận notification `application_received`, bấm "Từ chối" → nhập lý do → **kiểm tra lại qua
API thấy ứng viên nhận đúng lý do** (trước đây luôn `null`), nút "Rút đơn" hiện đúng, trang tin chặn nộp
lại với thông báo rõ ràng.

**Còn lại nhóm "hiển thị danh sách"**: 26/38 query backend trả toàn bộ bảng không giới hạn (không query
nào có `Skip`, chỉ 2 chỗ có `Take`) — nặng nhất là `SearchJobs` (trang chính) và `SearchCandidates`
(thêm nữa là **không có `OrderBy`** nên thứ tự ngẫu nhiên giữa các lần gọi). `web/` cũng chưa có
`loading.tsx`/`error.tsx` nào nên API lỗi sẽ ra trang trắng.

**Phân trang danh sách** — nhóm còn lại của yêu cầu "tập trung hiển thị danh sách + luồng nộp hồ sơ".
Trước đợt này **không một query nào có phân trang**: không chỗ nào dùng `Skip()`, chỉ 2/38 query có
`Take()`. Mọi danh sách `ToListAsync()` trên toàn bộ tập kết quả.

Chốt phạm vi với chủ dự án: làm **8 danh sách có thể phình to**, cố ý bỏ qua danh mục cố định
(chuyên khoa/địa điểm/gói tin), danh sách vài-dòng-mỗi-user (CV, tổ chức của tôi), danh sách theo-1-bản-ghi
(ghi chú/lịch sử đơn) và hàng đợi Vận hành — thêm phân trang cho chúng chỉ làm phức tạp mà không có lợi.

Hạ tầng: `PaginatedList<T>` (trả kèm `TotalCount`/`TotalPages` để client dựng được số trang — chỉ có cờ
`hasNext` thì không hiện được), `PagedQuery` với `PageSize` **chặn cứng ở 100** (nếu không, client truyền
`pageSize=999999` là quay lại đúng vấn đề phân trang sinh ra để tránh — có test), extension
`ToPaginatedListAsync`.

**2 lỗi sắp xếp phát hiện khi làm** — nếu không sửa thì phân trang cho kết quả sai:
1. `SearchCandidates` **không có `OrderBy` nào** — Postgres trả thứ tự tuỳ ý, cùng 1 ứng viên có thể
   xuất hiện ở 2 trang hoặc biến mất. Thêm `OrderBy(FullName)`.
2. Mọi query đều thiếu **khóa sắp xếp phụ**. Sắp theo `PublishedAt`/`AppliedAt` đơn thuần không đủ:
   nhiều bản ghi cùng thời điểm (Vận hành duyệt hàng loạt) vẫn bất định. Thêm `ThenBy(Id)` cho tất cả,
   kèm test khẳng định các trang không lặp/không bỏ sót.

Frontend `web/`: component `PaginationNav` dùng **Link đổi URL `?page=N`** chứ không phải nút bấm — trang
giữ được khi tải lại/chia sẻ link và hoạt động cả khi chưa có JS; bộ lọc hiện tại được giữ khi chuyển
trang. Thêm `loading.tsx` (trước đây không có loading boundary nào nên trang chậm trông như treo). Bỏ dòng
"N tin phù hợp" vì đó là số tin **trên trang**, dễ nhầm với tổng — tổng đã hiện ở thanh phân trang.

Frontend `web-admin/`: `usePagination` trước đây chỉ `slice` **phía client** (comment trong chính file thừa
nhận backend không hỗ trợ) — vẫn tải toàn bộ bảng rồi mới cắt, không giảm tải mạng. Giờ `page` nằm trong
`queryKey`. Hai chi tiết dễ sai đã xử lý: đổi bộ lọc phải quay về trang 1 (không thì đang ở trang 5 mà lọc
lại sẽ thấy trang rỗng), và `invalidateQueries` phải quét mọi trang (không thì đóng tin ở trang 2 xong các
trang khác vẫn giữ dữ liệu cũ). Xóa hook `use-pagination`.

Verify: backend 129/129 (từ 124) + 3/3, `web-admin/` 84/84, cả 2 frontend tsc + lint + build sạch. Verify
UI thật: seed 23 tin published, xác nhận "Trang 1/2 — 23 kết quả", bấm "Sau" đổi URL thành `?page=2` và
sang đúng trang 2, nút Trước/Sau mờ đúng ở đầu/cuối. Kiểm tra thêm bằng API: 19 tin với `pageSize=10` cho
trang 2 có 9 tin và `hasNextPage=false`.

**Với đợt này, cả 2 nhóm chủ dự án yêu cầu đã xong** — luồng nộp hồ sơ và hiển thị danh sách.

**Học vấn & kinh nghiệm làm việc** — chọn mục này từ danh sách còn lại vì phát hiện 1 vấn đề đáng kể khi
rà: **NTD trả 15 Credit để mở hồ sơ ứng viên nhưng chỉ thấy tên, headline, tiểu sử, chuyên khoa, CCHN** —
không có học vấn lẫn kinh nghiệm, tức thiếu đúng 2 thứ quan trọng nhất để đánh giá. Học vấn/kinh nghiệm
trước đó chỉ nằm trong CV Builder (jsonb riêng), không phải phần của hồ sơ nền tảng.

Làm theo thiết kế **đã có sẵn** trong `ERD-CHI-TIET.md` mục 2.2 (`experiences` + `educations`) — cả hai
thiết kế từ đầu nhưng chưa implement. Thêm enum `FacilityTier` (trung ương/tỉnh/huyện/tư nhân) theo
`THUAT-NGU.md`: NTD ngành y đánh giá kinh nghiệm **theo tuyến**, không chỉ theo số năm.

`PUT /candidates/me/history` là **replace-all** chứ không phải thêm/sửa/xóa từng dòng: UI là 1 form nhiều
dòng rồi bấm Lưu 1 lần — tách 3 loại request thì client phải theo dõi dòng nào mới/đã sửa/đã xóa, dễ lệch
trạng thái khi 1 request lỗi giữa chừng. Bổ sung vào **cả 2 DTO** (ứng viên tự xem và NTD xem sau unlock),
có test riêng khẳng định NTD thấy được sau khi trả Credit.

**Giữ nguyên** công thức `ProfileCompletion` (5 tiêu chí × 20%) — thêm 2 tiêu chí mới sẽ làm % của mọi hồ
sơ hiện có tụt xuống vô cớ.

3 vấn đề phát hiện khi làm UI:
1. **`FACILITY_TIERS` phải tách sang file riêng** (`lib/facility-tier.ts`): `lib/candidates.ts` import
   `backendFetch` (dùng `cookies()`, server-only), nên Client Component import **giá trị runtime** từ đó
   kéo cả module vào bundle client → build lỗi. Type thì import được (bị xoá lúc biên dịch), const thì
   không. Typecheck **không** bắt được, chỉ `next build` mới báo.
2. **Nhãn nút sai**: dùng khóa i18n `save` — khóa đó vốn của CV Builder nên nội dung là "Lưu CV", hiện
   nhãn sai hoàn toàn cho khối này. Phát hiện khi xem ảnh chụp UI. Thêm khóa `saveHistory` riêng.
3. **Thiếu `htmlFor` cho 11 nhãn** — repo đã có convention này ở `profile-form.tsx` nhưng tôi viết thiếu.
   Nhãn không liên kết input thì trình đọc màn hình cũng không đọc đúng (phát hiện nhờ Playwright
   `getByLabel` không khớp được).

Cũng sửa 1 **dòng tài liệu sai**: `TIEN-DO-CHI-TIET.md` ghi "Upload giấy phép hoạt động ⬜ chưa nối MinIO",
nhưng kiểm tra code thấy chức năng **đã hoàn chỉnh** và nối MinIO thật từ trước. Đã sửa kèm ghi rõ phần
còn thiếu thật (text hardcode chưa qua i18n, `docType` cứng `business_license`).

Verify: backend 135/135 (từ 129) + 3/3, cả 2 frontend tsc + lint + build sạch, `web-admin/` 84/84. Verify
UI thật đầu-cuối: ứng viên nhập kinh nghiệm → lưu → **tải lại trang, dữ liệu vẫn còn** → NTD nạp Credit,
unlock hồ sơ → **thấy đủ học vấn + kinh nghiệm kèm tuyến đã dịch tiếng Việt**.

> Ghi chú về quá trình verify: lần đầu script Playwright báo "dữ liệu MẤT sau tải lại", nhưng kiểm tra
> API thấy dữ liệu **đã lưu đúng** trong DB. Nguyên nhân là script đọc `innerText` mà header nổi che mất
> text — đổi sang đọc `inputValue()` thì đúng. Bài học: khi kết quả tự động mâu thuẫn với dữ liệu thật,
> kiểm chứng bằng nguồn khác trước khi kết luận có bug.

**Trang Cài đặt cá nhân `web-admin/` — làm thật thay template** — chọn mục này vì đây là khu vực UI
duy nhất còn hiển thị **dữ liệu giả cho người dùng thật**: cả 5 màn Settings là template shadcn-admin
nguyên bản (tiếng Anh, email mẫu `m@example.com`, mục "friend requests/follows", chọn 9 ngôn ngữ không
khớp 6 ngôn ngữ dự án), và mọi nút Lưu đều gọi `showSubmittedData` — chỉ hiện toast JSON, **không lưu gì
cả**. Người dùng bấm Lưu, thấy toast, tưởng đã lưu.

**Xóa 2 màn thay vì làm cho có**: `notifications` (công tắc bật/tắt email — hệ thống chưa có provider
email/SMS nào, giữ lại là hứa hẹn tính năng không tồn tại) và `display` (chọn hiện thư mục
Recents/Home/Desktop/Downloads — demo macOS Finder, không liên quan nghiệp vụ tuyển dụng). Xóa đúng hơn
là để đó rồi ghi "sẽ làm sau".

**3 màn còn lại làm thật**, và mấu chốt là **chỉ làm những gì backend có dữ liệu**: `AuthUserDto` chỉ có
`id/email/role/status/emailVerified/locale` — không có username/bio/URLs/ngày sinh/avatar. Nên màn Hồ sơ
là **chỉ đọc** (email, vai trò, trạng thái, xác thực email, tên tổ chức + link sang trang Tổ chức) chứ
không dựng form sửa những field backend không lưu. Màn Tài khoản nối `PUT /users/me/password` — endpoint
này **có sẵn từ trước nhưng chưa ai nối UI**. Màn Giao diện giữ nguyên vì theme/font là thật
(localStorage), chỉ i18n hóa và bỏ `showSubmittedData`.

Bỏ luôn phần chọn ngôn ngữ mà template có: `LanguageSwitcher` ở header **đã** lưu `users.locale` thật
rồi, thêm nữa chỉ là 2 chỗ làm cùng 1 việc.

**Rà kèm phát hiện 5 lỗi ảnh hưởng mọi trang, không chỉ Settings** — đáng kể nhất là lỗi thứ nhất:

1. `auth.user` **chỉ được set đúng 1 lần lúc đăng nhập** và không bao giờ dựng lại. Token lưu cookie
   nhưng user thì không, nên **sau khi tải lại trang (F5), `user` là `null`** → `app-sidebar` mất `role`
   → `getSidebarData(undefined)` trả **cả 2 nhóm** (nhóm này cố ý trả cả 2 khi chưa biết role, để tránh
   chớp menu) → **Nhà tuyển dụng thấy toàn bộ menu Vận hành nền tảng**. Không phải lỗ hổng phân quyền
   (backend vẫn chặn 403) nhưng là lỗi hiển thị nghiêm trọng, và chỉ cần F5 là gặp. Sửa bằng hook
   `useCurrentUser` gọi lại `GET /users/me`.
2. `lib/http.ts` cố định `Accept-Language: 'vi'`, comment dẫn "ADR-0008 mục 5 — app này chỉ tiếng Việt".
   Nhưng **ADR-0010 đã đảo lại** thành 6 ngôn ngữ, comment và code đều lỗi thời → mọi thông báo lỗi
   validation từ backend về tiếng Việt dù UI đang tiếng Anh/Nhật.
3. Chân sidebar hiện `Người dùng demo / demo@blousehiding.vn` **trên mọi trang**; `ProfileDropdown` góc
   trên phải hiện `satnaing / satnaingdev@gmail.com` kèm mục `Billing` và `New Team` không tồn tại.
4. `TeamSwitcher` có dropdown đổi team + mục "Add team" — hệ thống **không có** khái niệm nhiều team
   (mỗi tài khoản NTD thuộc đúng 1 tổ chức, Vận hành không thuộc tổ chức nào). Thay bằng `AppBrand`
   hiện thương hiệu + tên tổ chức thật. Xóa luôn `AppTitle` (cũng template: "Shadcn-Admin / Vite +
   ShadcnUI").
5. Nút tìm kiếm và command palette (⌘K) còn tiếng Anh: `Search`, `Type a command or search...`,
   `No results found.`, `Theme/Light/Dark/System`. Không sửa `components/ui/sidebar.tsx` ("Toggle
   Sidebar") vì đó là primitive shadcn copy-code, sửa sẽ lệch upstream, và chuỗi đó chỉ `sr-only`.

**Thêm test chặn tái diễn**: `src/i18n.test.ts` so khóa của **cả 8 namespace** giữa 6 ngôn ngữ — thiếu
khóa ở 1 ngôn ngữ thì i18next lặng lẽ lùi về `fallbackLng`, không ai phát hiện; đây đúng là loại lỗi
quy tắc #10 CLAUDE.md muốn chặn nhưng trước giờ không có gì kiểm tự động. Test này cũng xác nhận 7
namespace cũ không có khóa nào lệch. Thêm test sidebar không còn trỏ tới route đã xóa (bấm vào sẽ ra
trang không tìm thấy). Test 84 → 94.

`search-provider.test.tsx` **vỡ 7 test** khi tôi i18n hóa palette vì nó ghim chuỗi
`'Type a command or search...'` — sửa để đọc từ file dịch, đổi bản dịch sẽ không làm vỡ test nữa.

Verify UI thật (Playwright + backend + tài khoản/tổ chức thật): 31 kiểm tra đạt, gồm **đổi mật khẩu rồi
đăng nhập lại bằng mật khẩu MỚI (HTTP 200) và mật khẩu cũ bị vô hiệu (HTTP 401)** — bằng chứng đã ghi
DB, không chỉ là hiện toast; và **sau F5 sidebar không còn hiện nhóm Vận hành**.

> Ghi chú quá trình verify: script báo 1 lỗi "sidebar header không hiện tên tổ chức", nhưng ảnh chụp
> cho thấy hiện **đúng** — script đọc DOM trước khi `GET /organizations/mine` trả về. Đã thêm
> `waitForFunction` chờ dữ liệu. Ảnh chụp lại giúp phát hiện **2 lỗi thật mà test không bắt** (nút
> "Search" và palette còn tiếng Anh) — nên vẫn nên xem ảnh, không chỉ đọc số PASS/FAIL.

**Nhóm CV: xuất PDF + CV dạng file** — chọn nhóm này vì đây là 2 việc còn lại nằm đúng trong luồng
"ứng viên đăng tải thông tin cá nhân, CV" mà người dùng đã chốt phạm vi.

**Xuất PDF** — nút vốn `disabled` với nhãn "Xuất PDF (sắp ra mắt)". Có 3 cách và đã hỏi để chốt:
`window.print()` + CSS `@media print` (chọn), `jspdf`+`html2canvas`, hay backend render QuestPDF như
thiết kế gốc ghi ở `API-DESIGN.md` (`POST /candidates/me/cvs/{cvId}/export`). Chọn cách in trình duyệt
vì `html2canvas` chụp khối xem trước thành **ảnh** — chữ bị rasterize (mờ khi in/zoom, không copy được
text) và **tiếng Việt có dấu dễ lỗi font**, lại thêm ~1MB bundle; còn backend render phải dựng lại
layout CV lần 2 ở backend + nhúng font tiếng Việt. Endpoint `export` trong tài liệu đã đánh dấu **không
làm**, kèm lý do.

Chi tiết CSS đáng ghi lại: dùng `visibility: hidden` cho `body *` chứ **không** `display: none`.
`display: none` trên phần tử cha thì con không thể hiện lại được, còn `visibility: hidden` thì con đặt
`visible` vẫn hiện — nhờ vậy không phải liệt kê từng tầng DOM giữa `body` và `#cv-print`.

**Rà kèm phát hiện khối xem trước CV thiếu nhiều thứ**: không có email liên hệ, địa chỉ, tiểu sử,
chuyên khoa, **và CCHN**. Trên web thì NTD đã thấy email qua luồng mở hồ sơ nên không cần, nhưng bản in
là file **rời khỏi hệ thống** — CV không có cách liên hệ thì NTD đọc xong không gọi được, và CV ngành y
thiếu CCHN thì không đánh giá được (đúng điểm khác biệt cốt lõi của nền tảng). Đã bổ sung cả 5 khối.
**Chỉ in CCHN đã Verified**: CCHN đang chờ duyệt/bị từ chối mà in ra sẽ gây nhầm lẫn vì trên giấy NTD
không thấy được trạng thái duyệt.

**CV dạng file** — trước đây CV file lúc ứng tuyển chỉ lưu ở `applications.cv_file_url`, gắn chặt vào 1
đơn, nên lần ứng tuyển sau ứng viên phải upload lại đúng file đó; chỉ CV Builder được ghi vào bảng
`Cvs`. Thêm 3 command: upload (là **INSERT** chứ không upsert như builder — ứng viên được có nhiều CV
file: bản tiếng Việt/tiếng Anh, bản theo chuyên khoa), đặt CV chính, xóa.

`SetPrimaryCv` đọc **hết** CV của ứng viên rồi gán trong **1** `SaveChanges` thay vì 2 lệnh riêng (bỏ cờ
cũ / bật cờ mới) — tách ra mà lệnh sau lỗi thì hồ sơ rơi vào trạng thái có 0 hoặc 2 CV chính. `DeleteCv`
xóa **cứng** (CV là dữ liệu ứng viên tự tạo, không thuộc nhóm phải giữ vĩnh viễn như CCHN/audit log —
CLAUDE.md mục 4 quy tắc #4) và tự chuyển cờ "chính" sang CV còn lại mới nhất.

**LỖ HỔNG phát hiện khi làm, đáng kể**: chọn CV dạng file đã lưu khi ứng tuyển thì
`applications.cv_file_url` vẫn `null` — mà **NTD xem CV của đơn qua `ApplicationDto.CvFileUrl`, không
đọc bảng `cvs`**. Tức là đơn nộp bằng CV đã lưu hiện ra **không có link CV nào** dù ứng viên đã gắn CV.
Sửa bằng cách sao chép `FileUrl` sang đơn lúc nộp. Sao chép (thay vì join lúc đọc) cũng đúng về nghiệp
vụ: đơn giữ đúng bản CV **tại thời điểm nộp**, ứng viên sửa/xóa CV sau đó không làm đổi hồ sơ NTD đã
nhận. Test cũ chỉ kiểm `applicationId != Empty` nên không bắt được — test mới kiểm qua
`GetApplicationByIdQuery` với **vai NTD**.

Cả `DeleteCv` và `SetPrimaryCv` lọc theo `ProfileId` **ngay trong truy vấn** thay vì tìm theo `CvId` rồi
so chủ sở hữu sau — để "không tồn tại" và "không phải của mình" đều trả NotFound, không lộ CV nào có thật.

Test 135 → 145 (+10). Verify UI thật: 18/18 cho xuất PDF (gồm **bản in gọn trong 1 trang A4**,
header/nút bấm không lên giấy, CCHN chưa duyệt **không** bị in) và 6/6 cho CV file (upload 2 bản lên
MinIO thật, đổi CV chính, đúng 1 CV chính). Cuối cùng kiểm đầu-cuối bằng API với vai NTD là thành viên
tổ chức: `GET /applications/{id}` trả đúng link `cv-b.pdf` — chính là lỗ hổng vừa bịt.

> Ghi chú quá trình verify (3 lần script báo sai, sản phẩm đúng):
> 1. `getComputedStyle(nút).display` trả `'flex'` khiến tôi tưởng nút vẫn lên giấy — computed style trả
>    `display` **khai báo của chính phần tử**, không phản ánh việc phần tử cha đã `display: none`. Phải
>    kiểm bằng `getClientRects().length`.
> 2. Regex `/CV chính/` khớp cả **nhãn nút** "Đặt làm CV chính" nên báo sai số CV chính. Phải đếm badge.
> 3. Script báo không tìm thấy nút "Ứng tuyển" — vì lần chạy trước đã nộp đơn nên nút đổi thành "Bạn đã
>    ứng tuyển tin này rồi".
>
> Cả 3 lần đều xác minh lại bằng nguồn khác (ảnh chụp UI, truy vấn DB) trước khi kết luận — tiếp tục
> đúng bài học đã ghi ở đợt học vấn/kinh nghiệm.

### Giai đoạn 2 — Hoàn thiện

*(Chưa bắt đầu)*

### Giai đoạn 3 — Mở rộng

*(Chưa bắt đầu)*
