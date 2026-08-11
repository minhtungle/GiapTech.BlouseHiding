# 2026-08-04 — Notifications, Report/Ops, upload MinIO, quản lý việc làm, đồng bộ locale

> Mục lục mọi ngày: [`../TIEN-DO-DU-AN.md`](../TIEN-DO-DU-AN.md)

---

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
