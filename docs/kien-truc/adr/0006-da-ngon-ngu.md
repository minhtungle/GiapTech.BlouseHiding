# ADR-0006: Hỗ trợ đa ngôn ngữ — Tiếng Việt + Tiếng Anh, giới hạn phạm vi dịch

**Trạng thái:** Đã chấp nhận

## Bối cảnh
Ứng viên/tổ chức y tế nước ngoài (bệnh viện có vốn đầu tư nước ngoài, NGO y tế, chuyên gia expat) là
một phần thực tế của thị trường tuyển dụng y tế Việt Nam. Nền tảng cần phục vụ được nhóm này mà không
làm phức tạp hóa quá mức luồng nghiệp vụ chính (vốn đã nhắm tới người dùng Việt Nam là chủ đạo).

## Quyết định
1. **Hai ngôn ngữ: Tiếng Việt (mặc định) + Tiếng Anh.** Không thiết kế hệ thống dịch tổng quát cho
   n-ngôn ngữ — với đúng 2 ngôn ngữ, dùng cột song song (`name` / `name_en`) thay vì bảng dịch
   (translation table) tổng quát, đơn giản hơn và đủ dùng.
2. **Phạm vi dịch giới hạn**: chỉ dịch (a) giao diện — nút, nhãn, thông báo, email/SMS, và (b) danh
   mục chuẩn hóa do nền tảng quản lý — chuyên khoa, địa điểm, loại hình làm việc, tên gói dịch vụ.
3. **Không dịch nội dung do người dùng tự viết** — mô tả tin tuyển dụng, tiểu sử ứng viên, mô tả tổ
   chức hiển thị **nguyên văn ngôn ngữ tác giả đã nhập**, bất kể người xem đang dùng giao diện ngôn ngữ
   nào. Không dịch máy tự động (rủi ro dịch sai thuật ngữ y khoa nhạy cảm), không bắt buộc NTD nhập
   song ngữ (tăng gánh nặng nhập liệu không cần thiết cho MVP).

## Phương án đã cân nhắc
- **Bảng dịch tổng quát (`translations` key-value theo locale, áp dụng mọi bảng)**: loại bỏ vì
  over-engineering cho đúng 2 ngôn ngữ cố định — đúng tinh thần tránh thiết kế thừa đã áp dụng xuyên
  suốt dự án (vd lựa chọn Modular Monolith thay vì microservice).
- **Dịch máy tự động nội dung người dùng viết** (mô tả tin, tiểu sử): loại bỏ — thuật ngữ y khoa dịch
  sai có thể gây hiểu nhầm nghiêm trọng (vd tên chuyên khoa, yêu cầu chứng chỉ), rủi ro cao hơn lợi ích
  ở MVP.
- **Bắt buộc NTD nhập song ngữ cho tin tuyển dụng**: loại bỏ vì tăng ma sát khi đăng tin — đa số NTD
  trong nước không cần, chỉ số ít tổ chức có nhu cầu tuyển ứng viên nước ngoài thực sự cần, có thể tự
  viết tin bằng tiếng Anh trực tiếp nếu muốn (nội dung tự do, không ép khuôn 2 ô nhập).

## Hệ quả
- (+) Đơn giản hóa đáng kể so với hệ thống dịch tổng quát — chỉ 2 cột thay vì 1 bảng quan hệ riêng.
- (+) Không phát sinh rủi ro dịch sai thuật ngữ y khoa nhạy cảm.
- (−) Ứng viên/NTD dùng giao diện tiếng Anh vẫn có thể gặp mô tả tin bằng tiếng Việt thuần túy nếu tác
  giả không tự viết tiếng Anh — chấp nhận được vì đây không phải nhóm người dùng chính của MVP.
- (−) Nếu tương lai cần thêm ngôn ngữ thứ 3, cách làm cột song song (`name_en`) sẽ phải đổi sang bảng
  dịch tổng quát — đây là chi phí kỹ thuật chấp nhận trước, không phải thiết kế sai.
- Chi tiết kỹ thuật: [`../../frontend/CONG-NGHE-FRONTEND.md`](../../frontend/CONG-NGHE-FRONTEND.md)
  mục "Đa ngôn ngữ", [`../../backend/CONG-NGHE-BACKEND.md`](../../backend/CONG-NGHE-BACKEND.md),
  schema ở [`../../database/ERD-CHI-TIET.md`](../../database/ERD-CHI-TIET.md).
