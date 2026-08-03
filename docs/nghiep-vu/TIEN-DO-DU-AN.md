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
| 1 — MVP | 🟨 Đang làm | Identity thật + tạo tổ chức + hồ sơ ứng viên (core: profile/CCHN/chuyên khoa) xong; CV Builder/Jobs/ATS/Credit chưa làm |
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

Còn thiếu (chặn việc chốt giai đoạn):
- OAuth Google/Zalo — chưa làm, quyết định hoãn sang sau khi Identity cốt lõi ổn định (đã xác nhận với
  người dùng).
- `POST /organizations/{id}/members/invite` + luồng chấp nhận lời mời (`organization_invitations`) —
  chưa làm, mới có tạo tổ chức lần đầu.
- Hồ sơ ứng viên: học vấn/kinh nghiệm (`experiences`/`educations`), CME (`continuing_certificates`), CV
  Builder + export PDF — chưa làm, quyết định tách khỏi vòng "core" (profile+CCHN+chuyên khoa) đã xác
  nhận với người dùng. Upload document CCHN hiện giả định URL có sẵn, chưa nối
  `POST /uploads/presigned-url`/MinIO thật.
- Jobs, Applications/ATS, Credit/Payment — chưa bắt đầu bounded context nào trong số này.
- Chưa nối `web/`/`web-admin/` tới API Identity/Hồ sơ ứng viên thật (màn hình đăng ký/đăng nhập/hồ sơ
  vẫn dùng mock/form tĩnh) — ưu tiên tiếp theo sau khi có thêm bounded context để có gì nối.

### Giai đoạn 2 — Hoàn thiện

*(Chưa bắt đầu)*

### Giai đoạn 3 — Mở rộng

*(Chưa bắt đầu)*
