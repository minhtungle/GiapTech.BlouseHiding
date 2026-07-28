# Hướng dẫn sử dụng — Trang Vận hành (Quản trị viên / Kiểm duyệt viên)

> ⚠️ Tài liệu mô tả trải nghiệm **dự kiến** dựa trên thiết kế luồng nghiệp vụ
> ([`../nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md`](../nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md)) — cần cập
> nhật lại theo giao diện thật khi sản phẩm hoàn thiện. Đây là trang **Vận hành** — nội bộ đội ngũ nền
> tảng, **không phải** trang Admin dành cho Nhà tuyển dụng (xem
> [`../kien-truc/THUAT-NGU.md`](../kien-truc/THUAT-NGU.md)).

---

## 1. Duyệt chứng chỉ hành nghề (CCHN)
1. Vào **Hàng đợi duyệt CCHN**, danh sách hồ sơ đang ở trạng thái "chờ duyệt".
2. Đối chiếu ảnh/scan chứng chỉ với thông tin ứng viên nhập (số CCHN, nơi cấp, phạm vi hoạt động).
3. **Duyệt** nếu hợp lệ, hoặc **Từ chối kèm lý do cụ thể** (vd "ảnh mờ, chụp lại phần số CCHN") để ứng
   viên biết cách bổ sung — không từ chối chung chung.

## 2. Duyệt hồ sơ tổ chức (cơ sở y tế)
1. Vào **Hàng đợi duyệt doanh nghiệp**, đối chiếu giấy phép hoạt động đã upload.
2. Duyệt/từ chối tương tự mục 1 — tổ chức chỉ được đăng tin sau khi xác minh thành công.

## 3. Duyệt tin tuyển dụng
1. Vào **Hàng đợi duyệt tin**, kiểm tra nội dung: minh bạch mức lương, không vi phạm quy định quảng cáo
   tuyển dụng ngành y, không chứa nội dung phân biệt đối xử.
2. Duyệt để tin lên công khai, hoặc từ chối kèm lý do.

## 4. Đối soát thanh toán thủ công (tạm thời)
> Áp dụng cho đến khi có cổng thanh toán tự động — xem [ADR-0003](../kien-truc/adr/0003-hoan-cong-thanh-toan-tu-dong.md).

1. Kiểm tra sao kê ngân hàng theo **mã tham chiếu** nhà tuyển dụng cung cấp khi chuyển khoản.
2. Vào hồ sơ tổ chức tương ứng → duyệt gói tin đã chọn hoặc nạp Credit thủ công.
3. Ghi chú lại giao dịch để có dấu vết đối soát (tránh duyệt trùng hoặc thiếu).

## 5. Xử lý báo cáo vi phạm
1. Vào **Danh sách báo cáo**, xem nội dung report (tin/tổ chức/hồ sơ/tin nhắn bị báo cáo).
2. Xử lý: cảnh cáo, gỡ nội dung, hoặc khóa tài khoản tùy mức độ vi phạm.

## 6. Quản lý danh mục & người dùng
- **Danh mục**: cập nhật danh sách chuyên khoa, tuyến, địa điểm, loại hình làm việc — đây là dữ liệu
  chuẩn hóa dùng cho tìm kiếm/lọc, thay đổi cần cân nhắc ảnh hưởng tới dữ liệu đã có.
- **Người dùng**: tìm kiếm, khóa/mở khóa tài khoản vi phạm.
- **Gói dịch vụ**: cấu hình giá/thời hạn gói Eco/Pro/Max, giá Credit.

## 7. Nhật ký kiểm toán (Audit log)
- Mọi thao tác nhạy cảm (duyệt CCHN, xóa tài khoản, thay đổi gói dịch vụ) được ghi lại trong Audit log
  — dùng để tra cứu khi có tranh chấp hoặc yêu cầu tuân thủ NĐ 13/2023.

---

⚠️ **Sắp có (Giai đoạn 2):** kiểm duyệt đánh giá cơ sở y tế, dashboard số liệu nâng cao.
