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
