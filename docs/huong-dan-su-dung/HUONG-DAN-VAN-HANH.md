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
3. **Rút xác thực** (khi phát hiện gian lận hoặc qua báo cáo, mục 5): chuyển trạng thái tổ chức sang
   "Đã rút xác thực" — **hệ thống tự động ẩn toàn bộ tin đang công khai** của tổ chức đó ngay lập tức,
   không phải tự tay ẩn từng tin. Khi tổ chức được xác thực lại, phải **duyệt lại từng tin thủ công**
   trước khi tin hiện công khai trở lại (không tự động hiện lại).

## 3. Duyệt tin tuyển dụng
1. Vào **Hàng đợi duyệt tin**, kiểm tra nội dung: minh bạch mức lương, không vi phạm quy định quảng cáo
   tuyển dụng ngành y, không chứa nội dung phân biệt đối xử.
2. Duyệt để tin lên công khai, hoặc từ chối kèm lý do.
3. Hàng đợi này **chỉ hiển thị tin đã thanh toán xong** (mục 4) — tin đang chờ thanh toán nằm ở hàng đợi
   riêng, không lẫn vào đây.

## 4. Đối soát thanh toán thủ công (tạm thời)
> Áp dụng cho đến khi có cổng thanh toán tự động — xem [ADR-0003](../kien-truc/adr/0003-hoan-cong-thanh-toan-tu-dong.md).

1. Vào **Đối soát thanh toán thủ công**, xem danh sách giao dịch đang chờ (`pending`) kèm **mã tham
   chiếu** và số tiền cần khớp.
2. Kiểm tra sao kê ngân hàng theo mã tham chiếu nhà tuyển dụng cung cấp khi chuyển khoản.
3. **Khớp đúng** → bấm Xác nhận: hệ thống tự động cộng Credit hoặc kích hoạt gói tin tương ứng, tin
   chuyển sang hàng đợi duyệt nội dung (mục 3) — không phải tự tay vào từng hồ sơ tổ chức để cộng thủ công.
4. **Không khớp/sai số tiền** → bấm Từ chối kèm lý do: tin quay lại "Nháp" để NTD tự sửa và nộp lại.
5. Mỗi giao dịch chỉ xác nhận **1 lần** (hệ thống chặn xác nhận trùng cùng 1 giao dịch).

## 5. Xử lý báo cáo vi phạm
1. Vào **Danh sách báo cáo**, xem nội dung report (tin/tổ chức/hồ sơ/tin nhắn bị báo cáo).
2. Xử lý: cảnh cáo, gỡ nội dung, hoặc khóa tài khoản tùy mức độ vi phạm.
3. Chọn "Gỡ nội dung" sẽ **tự động** thực hiện đúng hành động tương ứng (đóng tin / rút xác thực tổ
   chức theo mục 2.3) trong cùng thao tác — không phải xử lý report xong rồi tự nhớ đi ẩn tin riêng.

## 6. Hoàn Credit (khi có tranh chấp)
- Trường hợp mở nhầm hồ sơ, lỗi hệ thống trừ credit sai, hoặc tranh chấp khác — vào hồ sơ tổ chức liên
  quan, chọn **Hoàn Credit**, nhập lý do. Đây là thao tác **thủ công có chủ đích**, không có luồng hoàn
  tự động.

## 7. Quản lý danh mục & người dùng
- **Danh mục**: cập nhật danh sách chuyên khoa, tuyến, địa điểm, loại hình làm việc — đây là dữ liệu
  chuẩn hóa dùng cho tìm kiếm/lọc, thay đổi cần cân nhắc ảnh hưởng tới dữ liệu đã có.
- **Người dùng**: tìm kiếm, khóa/mở khóa tài khoản vi phạm.
- **Gói dịch vụ**: cấu hình giá/thời hạn gói Eco/Pro/Max, giá Credit.

## 8. Nhật ký kiểm toán (Audit log)
- Mọi thao tác nhạy cảm (duyệt CCHN, xóa tài khoản, thay đổi gói dịch vụ) được ghi lại trong Audit log
  — dùng để tra cứu khi có tranh chấp hoặc yêu cầu tuân thủ NĐ 13/2023.

---

⚠️ **Sắp có (Giai đoạn 2):** kiểm duyệt đánh giá cơ sở y tế, dashboard số liệu nâng cao.
