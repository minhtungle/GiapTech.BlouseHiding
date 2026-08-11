# 2026-08-10 (phần 2) — hoàn thiện giao diện, CV, phân trang, học vấn/kinh nghiệm, hoàn Credit

> Mục lục mọi ngày: [`../TIEN-DO-DU-AN.md`](../TIEN-DO-DU-AN.md)
> Ngày 10/08 làm nhiều việc nên tách 2 phần: [phần 1 — backend Ops & Giai đoạn 2](./2026-08-10--phan-1-ops-va-giai-doan-2.md) · [phần 2 — hoàn thiện UI & CV](./2026-08-10--phan-2-hoan-thien-ui-va-cv.md)

---

## Trong ngày này

- Rà lại toàn bộ màn hình theo yêu cầu "ưu tiên hoàn thiện giao diện trước tính năng"
- Dọn tàn dư template shadcn-admin (`web-admin/`)
- Thêm trạng thái đang tải / lỗi cho 6 màn `web-admin/` còn thiếu
- Làm 3 màn hình còn thiếu
- Gói tin & giao dịch (`web-admin/`, `/job-packages`)
- Việc đã ứng tuyển (`web/`, `/dashboard/applications`)
- Hoàn tất kế hoạch hoàn thiện giao diện
- Sửa 2 vấn đề tồn đọng
- 1. Tài khoản Vận hành seed sẵn không đăng nhập được qua UI.
- 2. Cổng API mặc định của 2 frontend không khớp backend.
- 3. N+1 query ở `SearchCandidatesQueryHandler`
- Làm bounded context CV
- Làm bounded context "việc đã lưu"
- Còn lại 1 chỗ dùng dữ liệu giả:

---

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

**Nợ nhỏ: loại giấy tờ tổ chức + ngôn ngữ đã lưu** — 2 việc cuối trong luồng người dùng đã chốt.

**Ngôn ngữ đã lưu** — hóa ra `web/` **đã** lưu `users.locale` từ trước (dòng tài liệu ghi "không lưu"
đã lỗi thời, đã sửa). Nhưng rà kỹ thì phát hiện điều còn thiếu **thật**: lựa chọn được lưu mà
**không chỗ nào đọc lại**. `web/` định tuyến locale bằng **tiền tố URL**, nên đăng nhập ở máy khác qua
`/vi/auth/login` luôn ra tiếng Việt dù tài khoản đã chọn tiếng Anh. Sửa: `/api/auth/login` đọc `locale`
từ `GET /users/me` sau khi đặt cookie rồi trả về client, form đăng nhập `router.push` kèm locale đó.
Kiểm giá trị nằm trong `LOCALES` thay vì ép kiểu — dữ liệu từ DB, locale lạ sẽ điều hướng tới route
không tồn tại.

**Loại giấy tờ** — phạm vi thật rộng hơn ghi chú cũ ("text hardcode + `docType` cứng"): cả màn hình Hồ
sơ tổ chức (**547 dòng**) chưa i18n gì, **25 chuỗi tiếng Việt hardcode**. Kèm 2 lỗi hiển thị: dropdown
"Loại hình" và "Quy mô" hiện **giá trị enum thô** (`BenhVienCong`, `Under50`) cho người dùng thật.

`docType` hardcode `business_license` nghĩa là **nhà thuốc/công ty dược cũng phải nộp "giấy phép khám
chữa bệnh"** — sai loại giấy tờ theo nghiệp vụ. Thêm 4 loại theo `LUONG-NGHIEP-VU-MAN-HINH.md` mục 1 và
endpoint `GET /catalog/organization-document-types` để UI không hardcode lại rồi lệch với validator.

Backend trước đây nhận `doc_type` **chuỗi tự do** (chỉ `NotEmpty` + `MaximumLength(100)`) — 1 lỗi chính
tả ở client là tạo ra loại giấy tờ mới mà Vận hành không lọc/dịch được. Giữ cột `varchar(100)` thay vì
đổi sang `int` để thêm loại mới không cần migration, nhưng validator chốt danh sách.

Chi tiết UI: ô chọn file **bị chặn** tới khi chọn loại giấy tờ — nếu không thì upload lên MinIO xong mới
báo lỗi validate, file rác đã nằm trên storage.

**LỖ HỔNG BẢO MẬT phát hiện khi rà, đáng kể nhất trong đợt này**: `GetOrganizationDocumentsQuery`
**không kiểm quyền gì**, endpoint chỉ `RequireAuthorization()` — nên **bất kỳ user đăng nhập nào** (ứng
viên, NTD của tổ chức đối thủ) biết `organizationId` đều tải được **file giấy phép doanh nghiệp** của
tổ chức khác. Command *ghi* thì có kiểm `isMember`, query *đọc* thì không — đúng kiểu lỗi dễ lọt vì
người ta nhớ chặn lúc ghi mà quên lúc đọc. Vi phạm CLAUDE.md mục 4 quy tắc #9. Tài liệu
`API-DESIGN.md` thậm chí ghi endpoint này là **`Public`** — nay đã sửa cả code lẫn tài liệu.

Cũng sửa **1 comment lỗi thời** ở đầu `web-admin/src/lib/api.ts`: "App này chỉ tiếng Việt (ADR-0008 mục
5)" — ADR-0010 đã đảo lại thành 6 ngôn ngữ. Tôi đã sửa `http.ts` ở đợt Cài đặt nhưng **bỏ sót file
này**; đúng bài học đã ghi trước đó là khi sửa 1 giá trị/nhận định lặp lại thì phải grep cả cây, không
chỉ file đang mở.

Test backend 145 → 148, `web-admin/` 94 → 95 (test i18n tự thêm case cho namespace mới, xác nhận đủ
khóa 6 ngôn ngữ). Verify UI thật 11/11: nhà thuốc chọn "Giấy chứng nhận đủ điều kiện kinh doanh dược" →
**DB lưu đúng `pharmacy_license`**; đổi sang tiếng Anh thì nhãn dịch đúng; và tài khoản có
`users.locale = "en"` đăng nhập từ `/vi/auth/login` → chuyển đúng sang `/en/dashboard`.

> Ghi nhận thêm 1 việc chưa làm (thấy trên ảnh chụp, không thuộc phạm vi đợt này): ô chọn file dùng
> `<input type="file">` thuần nên nút hiện chữ `Choose File / No file chosen` theo ngôn ngữ **trình
> duyệt**, không dịch được qua i18n. Muốn dịch phải bọc lại bằng nút custom + input ẩn (cách đã dùng ở
> `web/` `AvatarSection`).

**Hoàn Credit thủ công khi tranh chấp** — mục cuối của Giai đoạn 1 mà người dùng chưa loại khỏi phạm
vi. ERD mục 2.7 thiết kế `reason=refund` từ đầu và `API-DESIGN.md` đã liệt kê endpoint, nhưng **chưa có
đường nào tạo được giao dịch refund** — Vận hành gặp tranh chấp (NTD trả 15 Credit mở hồ sơ nhưng hồ sơ
trùng/liên hệ không dùng được) thì không có cách xử lý ngoài cộng bù bằng `credit-bonus`, mà như vậy
không truy được đã hoàn cho lần mở nào.

Thiết kế khác `credit-bonus` ở 3 điểm, đều là để **kiểm được**: hoàn gắn với **đúng 1 giao dịch
`unlock_profile`** có thật; **hoàn đúng số đã trừ** (`Math.Abs` của `Amount` âm, không cho nhập tay); và
**mỗi giao dịch hoàn 1 lần** — giao dịch hoàn trỏ `ReferenceId` về giao dịch gốc, kiểm bằng truy vấn
thay vì tin vào UI. Để nhập số tự do như bonus thì không có cách nào biết hoàn đúng hay hoàn trùng.

Lọc theo `WalletId` **ngay trong truy vấn** thay vì tìm theo `TransactionId` rồi so ví sau: Vận hành
nhập nhầm cặp (tổ chức A, giao dịch của tổ chức B) sẽ cộng Credit vào **sai ví**.

Thêm `GET .../refundable-unlocks` là query **riêng cho Vận hành** — `GetCreditTransactionsQuery` phía
NTD chặn người không phải thành viên tổ chức, Vận hành không dùng được. Trả kèm cờ `isRefunded` để UI
vô hiệu hoá nút thay vì để bấm rồi mới nhận lỗi.

**LỖI phát hiện khi rà**: ô "Lý do (ghi chú nội bộ)" ở form cộng Credit **tồn tại từ trước** nhưng
`CreditBonusRequest` không có field đó — Vận hành nhập lý do rồi tưởng đã lưu, thực tế mất. Chú thích
dưới ô đó thậm chí thừa nhận "hệ thống hiện chưa lưu lại lý do" (trung thực nhưng nghĩa là ô nhập vô
dụng). Đã thêm `Reason` bắt buộc + **ghi audit log cho cả `credit_bonus` và `credit_refund`** — cả hai
là thao tác tiền, phải truy được ai làm, bao nhiêu, vì sao.

Test 148 → 156 (+8): hoàn đúng số, chặn hoàn 2 lần, chặn hoàn giao dịch `bonus`, chặn nhầm tổ chức,
bắt buộc lý do, và kiểm audit log thật ghi được lý do.

Verify UI thật 12/12 + đối chiếu DB: ví 100 → trừ 15 (unlock) → hoàn 15 → **về đúng 100**; giao dịch
hoàn có `ReferenceId`; audit log ghi đủ lý do cho cả 2 loại thao tác.

> Ghi chú: 9 test `Applications` fail giữa đợt này **không phải do code** — Docker daemon tự tắt nên
> Testcontainers không khởi động được (`Container runtime 'docker' ... unhealthy`). Bật lại Docker +
> `docker compose up -d` là pass hết. Đáng ghi lại vì thông báo lỗi trông như lỗi test.
