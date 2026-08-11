# 2026-08-03 — Identity thật, danh mục, hồ sơ ứng viên, tin tuyển dụng, ATS, Credit, nối 2 frontend

> Mục lục mọi ngày: [`../TIEN-DO-DU-AN.md`](../TIEN-DO-DU-AN.md)

---

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

Bổ sung tiếp — **[ADR-0010](../../kien-truc/adr/0010-da-ngon-ngu-cho-web-admin.md): đảo ngược ADR-0008
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
