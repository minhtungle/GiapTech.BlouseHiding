# ADR-0005: Đặt tên 3 khu vực site — Client / Admin (NTD) / Vận hành (nội bộ)

**Trạng thái:** Đã chấp nhận

## Bối cảnh
Hệ thống có 3 nhóm người dùng dùng 3 giao diện khác nhau: ứng viên/khách (công khai), nhà tuyển dụng
(đăng tin, quản lý CV/ATS), và đội nội bộ vận hành nền tảng (duyệt CCHN, duyệt doanh nghiệp, duyệt tin,
xử lý report). Ban đầu tài liệu gọi khu vực thứ 3 là **"Admin"** — nhưng người dùng dự án dùng từ
**"Admin"** để chỉ **trang quản trị của Nhà tuyển dụng**, gây trùng tên giữa 2 khu vực khác nhau.

## Quyết định
Chốt tên gọi chính thức cho 3 khu vực site, dùng nhất quán trong mọi tài liệu và code:

| Tên site | Dành cho | Role backend |
|---|---|---|
| **Client** | Ứng viên & khách chưa đăng nhập | `candidate`, guest |
| **Admin** | Nhà tuyển dụng (đăng tin, quản lý CV/ATS, mua gói, credit) | `employer` |
| **Vận hành** | Đội nội bộ vận hành nền tảng (duyệt CCHN/doanh nghiệp/tin, xử lý report) | `admin`, `moderator` |

Role backend `admin` **giữ nguyên tên** (không đổi DB enum) vì đây là định danh kỹ thuật nội bộ, không
phải tên site hiển thị — chỉ tên **site/khu vực UI** được chuẩn hóa lại. API endpoint dành cho đội Vận
hành đổi prefix từ `/admin/*` sang **`/ops/*`** để tránh gây hiểu lầm là "trang Admin" của NTD (xem
[`../../backend/API-DESIGN.md`](../../backend/API-DESIGN.md) mục 11).

## Phương án đã cân nhắc
- **Giữ role backend + site đều tên "Admin" cho platform ops, thêm tên khác cho trang NTD**: loại bỏ vì
  người dùng dự án đã dùng "Admin" quen để chỉ trang NTD — đi ngược lại gây khó hiểu hơn là đổi tên
  khu vực còn lại.
- **Đổi luôn role backend `admin` → `platform_admin`**: loại bỏ vì không cần thiết — đây là định danh
  kỹ thuật, đổi tên tốn công migrate mà không tăng thêm rõ ràng cho người dùng cuối.

## Hệ quả
- (+) Không còn nhập nhằng giữa "trang Admin" (NTD) và "đội vận hành" khi đọc tài liệu/trao đổi với team.
- (+) API path `/ops/*` rõ ràng ngay từ tên endpoint là dành cho nội bộ nền tảng, không phải NTD.
- (−) Phải rà soát & cập nhật lại toàn bộ tài liệu đã viết trước đó có nhắc "Admin" với nghĩa cũ (đã
  thực hiện trong cùng đợt thay đổi này) — cái giá một lần cho rõ ràng lâu dài.
