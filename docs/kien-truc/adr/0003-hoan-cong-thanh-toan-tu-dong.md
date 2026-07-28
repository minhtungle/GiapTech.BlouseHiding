# ADR-0003: Hoãn chọn cổng thanh toán tự động, dùng quy trình thủ công ở MVP

**Trạng thái:** Đã chấp nhận (tạm thời — sẽ có ADR bổ sung khi chọn cổng cụ thể)

## Bối cảnh
Hệ thống cần thu tiền gói đăng tin (Eco/Pro/Max) và nạp Credit cho nhà tuyển dụng. Có nhiều lựa chọn
cổng thanh toán Việt Nam (VNPay, Momo, ZaloPay, hoặc cổng tổng hợp như PayOS) nhưng quyết định chọn cổng
cụ thể chưa được đưa ra — không nên chặn tiến độ phát triển MVP để chờ quyết định này.

## Quyết định
- **MVP không tích hợp cổng thanh toán tự động.** Quy trình thay thế: nhà tuyển dụng chuyển khoản ngân
  hàng theo thông tin hiển thị kèm mã tham chiếu; đội **Vận hành** đối soát thủ công và duyệt gói
  tin/nạp credit qua endpoint `/ops/*` (xem [ADR-0005](./0005-dat-ten-3-khu-vuc-site.md)).
- Schema (`payments`, `job_purchases`, `credit_transactions` trong ERD) và endpoint (`POST /payments/*`
  trong API design) **giữ nguyên như đã thiết kế cho cổng tự động** — trường `provider` trong `payments`
  đơn giản chưa có giá trị thật nào ngoài xử lý thủ công, để khi chọn cổng thật chỉ cần cắm thêm
  provider mới, không phải đổi model dữ liệu.

## Phương án đã cân nhắc
- **PayOS** (cổng tổng hợp VN, 1 lần tích hợp nhiều phương thức): vẫn là lựa chọn khả thi nhất khi
  quyết định — ghi lại ở đây để không phải nghiên cứu lại từ đầu.
- **Tự nối trực tiếp VNPay + Momo + ZaloPay**: kiểm soát tốt hơn nhưng tốn công gấp nhiều lần, chỉ nên
  làm nếu có lý do kinh doanh cụ thể cần từng cổng riêng.

## Hệ quả
- (+) Không chặn tiến độ khởi tạo/phát triển MVP.
- (+) Khi chọn cổng thật, chỉ thêm implementation, không đổi schema/API đã có.
- (−) Vận hành thủ công (đối soát chuyển khoản) tốn công đội Vận hành, không scale tốt khi số lượng
  giao dịch tăng — cần chốt cổng tự động trước khi tăng trưởng đáng kể.
