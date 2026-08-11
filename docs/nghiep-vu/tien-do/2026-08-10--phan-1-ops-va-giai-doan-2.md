# 2026-08-10 (phần 1) — 4 bounded context Ops, đánh giá cơ sở y tế, Giai đoạn 2

> Mục lục mọi ngày: [`../TIEN-DO-DU-AN.md`](../TIEN-DO-DU-AN.md)
> Ngày 10/08 làm nhiều việc nên tách 2 phần: [phần 1 — backend Ops & Giai đoạn 2](./2026-08-10--phan-1-ops-va-giai-doan-2.md) · [phần 2 — hoàn thiện UI & CV](./2026-08-10--phan-2-hoan-thien-ui-va-cv.md)

---

## Trong ngày này

- không gửi lên backend
- (1) Form tìm kiếm
- (2) Trang chủ dùng data thật thay `MOCK_ORGANIZATIONS`
- (3) CV Builder
- (4) Lưu tin quan tâm (Saved Jobs)
- (5) OAuth Google/Zalo
- (1) 2 link 404 trên `web/`
- (2) Help Center `web-admin/`
- #1 — `apiGet()` (web/lib/api.ts) nuốt mọi lỗi backend thành "không có dữ liệu"
- #3-7 — Loading/empty/error state thiếu phân biệt
- #8 — Global error handler (web-admin `main.tsx`) chỉ xử lý 401/500, bỏ sót 403 + network
- #9 — Invite member dialog không validate email client-side
- #10 — Apply button (web/) không giới hạn kích thước file CV client-side
- #11 — Jobs new form (web-admin) không có loading state cho 4 catalog dropdown

---

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
