# Hướng dẫn sử dụng — Nhà tuyển dụng

> ⚠️ Tài liệu mô tả trải nghiệm **dự kiến** dựa trên thiết kế luồng nghiệp vụ
> ([`../nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md`](../nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md)) — cần cập
> nhật lại theo giao diện thật khi sản phẩm hoàn thiện. Đây là hướng dẫn cho **trang Admin** (khu vực
> dành riêng cho Nhà tuyển dụng — không phải trang nội bộ đội Vận hành, xem
> [`../kien-truc/THUAT-NGU.md`](../kien-truc/THUAT-NGU.md)).

---

## 1. Đăng ký & xác minh tổ chức
1. Đăng ký tài khoản, chọn vai trò **Nhà tuyển dụng**.
2. Tạo **Hồ sơ tổ chức**: tên, loại hình (bệnh viện công/tư, phòng khám, nhà thuốc, công ty dược...),
   quy mô, mô tả.
3. Upload **giấy phép hoạt động** — đội ngũ kiểm duyệt sẽ xác minh trước khi tổ chức được phép đăng tin.
4. Mời thêm thành viên HR khác vào tổ chức nếu cần (mỗi thành viên có vai trò owner/quản lý/thành viên).

## 2. Đăng tin tuyển dụng
1. Vào **Danh sách tin tuyển dụng** → **Tạo tin mới**.
2. Điền đầy đủ: vị trí, chuyên khoa, loại hình (toàn thời gian/trực ca/locum/CTV), mức lương, yêu cầu
   CCHN, kinh nghiệm, phúc lợi.
3. Chọn **gói đăng tin**: Eco/Pro/Max — khác nhau về thời gian hiển thị và vị trí ưu tiên trong kết quả
   tìm kiếm.
4. **Thanh toán (hiện tại là quy trình thủ công):** chuyển khoản theo thông tin hiển thị kèm **mã tham
   chiếu**, sau đó chờ đội Vận hành đối soát và duyệt gói (thường trong ngày làm việc). ⚠️ Thanh toán tự động sẽ
   được bổ sung sau, xem [ADR-0003](../kien-truc/adr/0003-hoan-cong-thanh-toan-tu-dong.md).
5. Sau khi thanh toán được duyệt, tin vào hàng đợi kiểm duyệt nội dung trước khi hiển thị công khai.

## 3. Quản lý ứng viên (ATS)
1. Vào tin đã đăng → tab **Ứng viên** để xem bảng Kanban theo 6 cột trạng thái: Mới → Đang xem → Phù
   hợp → Hẹn phỏng vấn → Offer → Trúng tuyển (kéo-thả để chuyển trạng thái).
2. Xem chi tiết từng ứng viên, ghi chú nội bộ, xem điểm chấm hồ sơ (CV Scoring) để sàng lọc nhanh.
3. Nhắn tin trực tiếp với ứng viên qua mục **Tin nhắn**.

## 4. Tìm & liên hệ ứng viên chủ động (Credit)
1. Vào **Tìm kiếm ứng viên**, lọc theo chuyên khoa/kinh nghiệm/khu vực — kết quả hiển thị hồ sơ rút
   gọn, **ẩn thông tin liên hệ**.
2. Bấm **Mở hồ sơ** để trừ Credit và xem đầy đủ thông tin liên hệ + lịch sử.
3. Nạp thêm Credit tại **Ví Credit** nếu số dư không đủ.
4. Hữu ích khi cần tuyển gấp vị trí hiếm (vd chuyên khoa Gây mê hồi sức) mà ít ứng viên chủ động nộp đơn.

## 5. Quản lý tài khoản tổ chức
- **Ví Credit**: xem số dư, lịch sử giao dịch.
- **Thành viên HR**: mời/xóa thành viên, phân quyền.

---

⚠️ **Sắp có (Giai đoạn 2):** cổng thanh toán tự động, gợi ý ứng viên phù hợp (matching), đánh giá cơ
sở y tế từ ứng viên.
