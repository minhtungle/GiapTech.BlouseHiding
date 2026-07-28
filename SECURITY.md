# Chính sách bảo mật

## Phạm vi dữ liệu nhạy cảm

Hệ thống xử lý dữ liệu cá nhân nhạy cảm theo **Nghị định 13/2023/NĐ-CP** (bảo vệ dữ liệu cá nhân):
- Chứng chỉ hành nghề (CCHN) — số hiệu, ảnh scan, phạm vi hoạt động chuyên môn.
- Thông tin liên hệ ứng viên (email, số điện thoại, địa chỉ).
- Giấy phép hoạt động của cơ sở y tế.
- Lịch sử giao dịch (thanh toán/credit).

## Nguyên tắc bắt buộc

1. **Mã hóa dữ liệu nhạy cảm khi lưu trữ** (at-rest) và **khi truyền tải** (TLS bắt buộc mọi kết nối,
   không có ngoại lệ HTTP thuần ngay cả nội bộ).
2. **RBAC nghiêm ngặt** — mỗi endpoint chỉ trả về dữ liệu cần thiết cho role gọi (xem
   [`docs/backend/CONG-NGHE-BACKEND.md`](docs/backend/CONG-NGHE-BACKEND.md) mục 3). Hồ sơ ứng viên đầy
   đủ chỉ hiển thị cho NTD sau khi `profile_unlocks` (đã trả Credit) — không lộ qua endpoint tìm kiếm
   thông thường.
3. **Audit log bất biến** cho mọi thao tác nhạy cảm (duyệt/từ chối CCHN, xóa tài khoản, thay đổi giao
   dịch) — xem bảng `audit_logs` trong [`docs/database/ERD-CHI-TIET.md`](docs/database/ERD-CHI-TIET.md).
4. **Xóa tài khoản = soft-delete + anonymize**, không xóa cứng dữ liệu liên quan CCHN/audit log — đảm
   bảo vừa tôn trọng quyền xóa dữ liệu của người dùng vừa giữ tính toàn vẹn số liệu cho bên thứ ba
   (NTD đã tương tác).
5. **Secret không commit vào repo** — xem [`docs/ha-tang/BIEN-MOI-TRUONG.md`](docs/ha-tang/BIEN-MOI-TRUONG.md).
6. **Rate limiting** trên các endpoint OTP/đăng nhập để chống brute-force và spam SMS.
7. Trước khi merge bất kỳ thay đổi nào động vào luồng CCHN, thanh toán, hoặc dữ liệu cá nhân — chạy
   skill `security-review`.

## Báo cáo lỗ hổng bảo mật

Nếu phát hiện lỗ hổng bảo mật, **không** tạo issue công khai trên GitHub. Liên hệ trực tiếp người phụ
trách dự án qua kênh riêng để được xử lý kín đáo trước khi công bố (nếu cần).

## Tuân thủ

- Nghị định 13/2023/NĐ-CP về bảo vệ dữ liệu cá nhân.
- Quy định Bộ Y tế về quảng cáo/tuyển dụng ngành y (xem
  [`docs/nghiep-vu/PHAN-TICH-NGHIEP-VU.md`](docs/nghiep-vu/PHAN-TICH-NGHIEP-VU.md) mục 5).
