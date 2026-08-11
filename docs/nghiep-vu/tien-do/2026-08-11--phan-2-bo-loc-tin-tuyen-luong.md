# 2026-08-11 (phần 2) — bộ lọc tin theo tuyến & mức lương

> Mục lục mọi ngày: [`../TIEN-DO-DU-AN.md`](../TIEN-DO-DU-AN.md)
> Phần 1 cùng ngày: [chốt Giai đoạn 1](./2026-08-11--chot-giai-doan-1.md)

---

**Bắt đầu nhóm việc "hoàn thiện UI"** theo đúng ưu tiên người dùng đã chốt (CLAUDE.md mục 1b). Việc
đầu tiên là **rà toàn bộ màn hình** thay vì đoán: đối chiếu `LUONG-NGHIEP-VU-MAN-HINH.md` mục 2
(42 màn) với route thật của `web/` (21 route) và `web-admin/` (28 route).

**Kết quả rà: không thiếu màn hình nào** — 42/42 đều có route. Nhưng có **4 khoảng trống bên trong**
các màn đã có, so với mô tả trong thiết kế:

| # | Màn hình | Thiết kế | Thực tế |
|---|---|---|---|
| 1 | Tìm việc | 5 bộ lọc (thêm **mức lương**, **tuyến**) | Chỉ 3 |
| 2 | Tìm ứng viên | Lọc **kinh nghiệm** | Chưa có |
| 3 | Chi tiết ứng viên | **Lịch sử tương tác** | Chưa có |
| 4 | — | Mọi text qua i18n | 2 chuỗi hardcode |

Làm mục 1 trước vì backend đã sẵn `SalaryMin`, và đây là bộ lọc ứng viên dùng nhiều nhất.

**Người dùng chọn "làm cả 2, kèm migration"** cho bộ lọc tuyến (phương án đầy đủ theo thiết kế, thay
vì bỏ qua tuyến).

## Backend

Thêm cột `jobs.tier` (nullable, enum `FacilityTier` — **tái dùng** enum đã có cho
`experiences.tier`, không tạo enum mới). Migration `202608111000_AddTierToJobs` chỉ `ADD COLUMN`
nullable, không rủi ro dữ liệu.

Quyết định đáng ghi: **lưu tuyến ở TIN chứ không suy ra từ tổ chức**. Một bệnh viện tuyến tỉnh vẫn có
thể đăng tin cho phòng khám vệ tinh tuyến huyện; ngoài ra `organizations` cũng không có cột tuyến nên
phương án "lọc gián tiếp qua tổ chức" vẫn phải migration mà lại kém chính xác.

**Tin không khai tuyến (`null`) bị loại khi lọc theo tuyến** — nếu trả về thì ứng viên lọc "tuyến
trung ương" sẽ nhận cả tin không rõ tuyến, đúng kiểu kết quả làm mất niềm tin vào bộ lọc.

Ngược lại, **`salaryMin` giữ tin "thỏa thuận"** (`SalaryMax = null`): lọc lương mà ẩn hết tin thỏa
thuận thì ứng viên mất phần lớn tin thật của thị trường ngành y. Logic này đã có sẵn, nhưng trước đây
**không có UI nào gọi tới** — tức là filter viết rồi mà chưa từng dùng.

Test 157 → 160 (+3): lọc đúng tuyến, loại tin không khai tuyến, và lọc lương giữ tin thỏa thuận.

## Frontend

`web/` trang Tìm việc: thêm 2 bộ lọc. Mức lương dùng **mốc rời rạc** (10/15/20/30/50 triệu) thay vì ô
nhập tự do — ứng viên nghĩ theo ngưỡng ("từ 20 triệu"), và mốc cố định giữ URL chia sẻ được thay vì
sinh vô số biến thể query.

`web-admin/`: ô chọn tuyến trong form tạo **và** sửa tin, pre-fill từ tin hiện có.

## Lỗi phát hiện thêm khi làm

1. **Dropdown "Loại hình" hiện enum thô** (`FullTime`) ở **cả 3 chỗ**: bộ lọc `web/`, form tạo tin và
   form sửa tin `web-admin/`. Đã dịch cả 3.
2. **3 chuỗi `— Tất cả` hardcode tiếng Việt** trong bộ lọc `web/` (vi phạm quy tắc #10) → chuyển sang
   khóa `allOf` có tham số.
3. **`<label>` trong bộ lọc thiếu `htmlFor`** — trình đọc màn hình không liên kết được nhãn với ô chọn.
4. **Phân trang làm mất filter mới**: `PaginationNav` chỉ truyền 4 filter cũ, sang trang 2 là mất
   tuyến/lương. Sửa trước khi kịp lọt.

## Verify

Backend 160/160 + 3/3 unit; `web-admin/` 95/95; cả 3 build + lint sạch. Playwright **17/17** với tin
thật ở 3 tuyến khác nhau.

> Ghi chú quá trình: 2 lần đầu chạy verify đều **lộ lỗi qua ẢNH CHỤP mà test không bắt**.
> - Lần 1: console báo `MISSING_MESSAGE: jobs.employmentTypeValue.Ctv` — tôi **đoán** giá trị enum
>   `EmploymentType` (viết `Contract`, `Internship`) thay vì đọc `EmploymentType.cs`, trong khi enum
>   thật là `TrucCa`, `Ctv`. Bài học: enum có giá trị đặc thù tiếng Việt, không được suy từ tên quen thuộc.
> - Lần 2: ảnh cho thấy **thẻ tin vẫn hiện `FullTime` thô và không hiện tuyến** — tôi mới sửa dropdown
>   lọc mà quên chính thẻ kết quả, tức là thêm dữ liệu tuyến mà ứng viên không nhìn thấy. Test ban đầu
>   chỉ kiểm bộ lọc nên không phát hiện; đã bổ sung 2 check cho thẻ tin.
>
> Cũng có 2 lần script báo FAIL oan (regex `/xóa lọc/` không khớp nhãn thật "Xóa bộ lọc"; mã trạng thái
> đoán 204 trong khi handler `Task` trả 200) — đều là lỗi của script, xác minh lại trước khi kết luận.

---

## Tiếp: 3 khoảng trống còn lại (mục 2, 3, 4)

**Mục 2 — lọc kinh nghiệm ở màn Tìm ứng viên.** Câu hỏi khó nhất là **cách tính số năm**. Cộng thời
lượng từng giai đoạn nghe chính xác hơn, nhưng 2 giai đoạn **chồng lấn thời gian** (làm 2 nơi cùng lúc
— rất phổ biến ở ngành y: trực thêm phòng khám ngoài giờ) sẽ bị đếm 2 lần, cho số năm **lớn hơn thực
tế**. Chọn tính theo khoảng từ `FromDate` sớm nhất tới nay: đổi lại quãng nghỉ giữa chừng cũng tính vào
thâm niên, nhưng NTD dùng bộ lọc để **khoanh vùng**, không phải đo chính xác. Ghi rõ giới hạn này ở
handler. Ứng viên chưa khai kinh nghiệm nào **bị loại** khi lọc — trả về thì NTD lọc "từ 5 năm" nhận cả
hồ sơ trắng, bộ lọc mất tác dụng.

Kèm refactor: `candidatesApi.search` đổi từ **tham số vị trí sang object** — đã có 6 tiêu chí, truyền
theo thứ tự rất dễ nhầm (`specialty`/`location` cùng kiểu string) và thêm filter mới phải sửa mọi lời gọi.

**Mục 3 — lịch sử tương tác ở chi tiết ứng viên.** Gộp 2 nguồn có sẵn: lần mở hồ sơ (`profile_unlocks`)
và đơn ứng tuyển vào tin của chính tổ chức. Điểm quan trọng: **chỉ tương tác của tổ chức đang xem** —
ứng viên ứng tuyển nơi nào, bị tổ chức nào mở hồ sơ là việc riêng của họ, để lộ là rò rỉ thông tin. Có
test riêng dựng 2 tổ chức cùng mở 1 hồ sơ để khẳng định không lộ chéo.

**Mục 4 — chuỗi hardcode.** Hoá ra không phải 2 mà **6 chuỗi**, và nằm ở **màn đăng nhập** — màn đầu
tiên người dùng thấy. `not-found.tsx` ở root giữ nguyên: file này nằm **ngoài** provider i18n (Next.js
yêu cầu có nó khi dùng segment động `[locale]`), không gọi `t()` được.

## Lệch thiết kế phát hiện thêm

Verify lộ ra form đăng nhập hiện **"Password"** dù locale mặc định là `vi`. Không phải lỗi dịch —
`web-admin/` để `detection.order: ['localStorage', 'navigator']`, nên **máy cài tiếng Anh ra giao diện
quản trị tiếng Anh ngay lần đầu**, trong khi `web/` theo ADR-0006 mặc định `vi`. Cùng 1 tài khoản mà 2
app hành xử khác nhau.

Đã hỏi và người dùng chọn **đồng bộ về tiếng Việt**: bỏ `navigator` khỏi `detection.order`. Người dùng
nước ngoài vẫn tự đổi được bằng nút chọn ngôn ngữ (lưu localStorage).

## Verify

Backend **163/163** (từ 160) + 3/3; `web-admin/` **95/95**; cả 3 build + lint sạch. Playwright **8/8**
với dữ liệu thật: lọc ≥5 năm giảm từ 20 xuống 5 ứng viên, khối lịch sử tương tác hiện đúng.

> Lại 1 lần nữa **test cũ vỡ vì i18n**: `user-auth-form.test.tsx` tìm chuỗi cứng "Mật khẩu" trong khi
> form đã chuyển sang `t()`, mà test không init i18n nên nhận khóa thô. Đúng lỗi đã gặp ở
> `search-provider.test.tsx` đợt trước — sửa cùng cách (import `@/i18n` + ép `vi` trong `beforeEach`).
> **Bài học lặp lại: i18n hóa 1 component thì phải kiểm luôn test của nó.**
