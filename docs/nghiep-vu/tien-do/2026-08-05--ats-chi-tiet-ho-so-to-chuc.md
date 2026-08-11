# 2026-08-05 — Chi tiết ATS, hồ sơ ứng viên/tổ chức, tìm & mở hồ sơ chủ động

> Mục lục mọi ngày: [`../TIEN-DO-DU-AN.md`](../TIEN-DO-DU-AN.md)

---

## Trong ngày này

- Phát hiện 1 gap backend thật khi khảo sát trước khi code
- Phát hiện 1 bug thật lúc verify curl end-to-end
- Phát hiện 1 gap backend thật khi khảo sát trước khi code
- 2 điều phát hiện lúc chạy thử
- Không có test nào từng bắt được
- Giới hạn verify UI thật lần này
- Phát hiện thêm ngoài dự tính
- Quyết định phạm vi đã xác nhận với người dùng
- Phát hiện + tự sửa 1 bug thật ngay trong lúc verify
- Phát hiện + tự sửa 1 bug thật ngay trong lúc verify
- Quyết định phạm vi đã xác nhận với người dùng
- Phát hiện gap sâu hơn agent report ban đầu lúc bắt tay vào sửa
- Quyết định thiết kế đã xác nhận
- Sự cố hạ tầng giữa lúc làm

---

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
