# ADR-0006: Hỗ trợ đa ngôn ngữ — 6 ngôn ngữ, dịch qua bảng translation, routing theo tiền tố URL

**Trạng thái:** Đã chấp nhận (thay thế bản v1 chỉ dự tính VI+EN). ⚠️ Sau
[ADR-0008](./0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md), phạm vi **routing/UI 6 ngôn ngữ ở mục
2–4 chỉ áp dụng cho Client (`web/`, Next.js)** — app Admin/Vận hành (`web-admin/`, shadcn-admin) chỉ
tiếng Việt, không dùng next-intl. Danh mục dịch qua bảng `*_translations` (mục 4) và `users.locale`
vẫn dùng chung cho toàn hệ thống vì đây là dữ liệu backend, không phụ thuộc app frontend nào hiển thị.

## Bối cảnh
Ứng viên/tổ chức y tế nước ngoài (bệnh viện có vốn đầu tư nước ngoài, NGO y tế, chuyên gia expat, đặc
biệt từ Nhật/Hàn/Trung — các quốc gia đầu tư y tế/dược lớn tại Việt Nam) là một phần thực tế của thị
trường tuyển dụng y tế Việt Nam. Yêu cầu ban đầu chỉ tính VI+EN; sau khi làm rõ, phạm vi mở rộng thành
6 ngôn ngữ phổ biến, cùng với yêu cầu cụ thể về routing URL và bộ chọn ngôn ngữ trên giao diện.

## Quyết định

### 1. Ngôn ngữ hỗ trợ
**Tiếng Việt (`vi`, mặc định)**, **Tiếng Anh (`en`)**, **Tiếng Nhật (`ja`)**, **Tiếng Trung giản thể
(`zh`)**, **Tiếng Hàn (`ko`)**, **Tiếng Tây Ban Nha (`es`)**.

### 2. Routing theo tiền tố URL, tự động nhận diện
Mỗi ngôn ngữ có tiền tố URL riêng, luôn hiển thị kể cả ngôn ngữ mặc định (`localePrefix: 'always'`):
```
blousehiding.vn/vi/viec-lam/...
blousehiding.vn/en/jobs/...
blousehiding.vn/ja/...
blousehiding.vn/zh/...
blousehiding.vn/ko/...
blousehiding.vn/es/...
```
Lần đầu truy cập không có tiền tố (`blousehiding.vn/viec-lam/...`), middleware xác định ngôn ngữ theo
thứ tự ưu tiên: **(1)** cookie đã lưu lựa chọn trước đó → **(2)** header `Accept-Language` trình duyệt
khớp với 1 trong 6 ngôn ngữ hỗ trợ → **(3)** mặc định `vi`. Sau đó redirect 1 lần sang URL có tiền tố
tương ứng. Không dùng geo-IP (dò quốc gia theo địa chỉ IP) — thêm dịch vụ ngoài, tốn chi phí, trong khi
`Accept-Language` đã đủ chính xác cho nhu cầu hiện tại và không cần hạ tầng thêm.

### 3. Bộ chọn ngôn ngữ trên giao diện
Nút chọn ngôn ngữ đặt ở **header** (góc phải, cạnh khu vực tài khoản/đăng nhập — mẫu phổ biến ở
Notion, GitHub, Airbnb) — biểu tượng globe + mã ngôn ngữ hiện tại, bấm vào mở danh sách 6 ngôn ngữ
theo **tên bản ngữ** (English, 日本語, 中文, 한국어, Tiếng Việt, Español), không dịch tên ngôn ngữ khác
sang ngôn ngữ đang xem. Chọn xong giữ nguyên trang đang xem, chỉ đổi tiền tố URL + lưu cookie. Thêm 1
link phụ ở **footer** trỏ tới cùng bộ chọn (mẫu tham khảo từ apple.com/vn/) — vị trí dự phòng cho người
dùng quen tìm ở footer.

### 4. Cách lưu bản dịch
- **Chuỗi giao diện (UI strings)**: file JSON theo `next-intl`, tổ chức `messages/{locale}/{namespace}.json`
  trong `web/` (namespace theo tính năng: `common`, `jobs`, `profile`...) — mỗi ngôn ngữ 1 thư mục
  riêng, dễ giao cho dịch giả/vendor dịch thuê ngoài theo từng gói ngôn ngữ, không phải sửa 1 file
  khổng lồ dùng chung. Đây là cách tổ chức phổ biến của các dự án i18n mã nguồn mở lớn dùng next-intl.
  Không áp dụng cho `web-admin/` — xem [ADR-0008](./0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md) mục 5.
- **Danh mục do nền tảng quản lý** (chuyên khoa, địa điểm, loại hình, tên gói dịch vụ): **không** dùng
  cột song song (`name_en`, `name_ja`...) như bản ADR v1 — với 6 ngôn ngữ, cách này phình cột và khó
  quản lý bản dịch thiếu/thừa. Thay bằng **bảng dịch riêng theo mẫu chuẩn** (tương tự cách các hệ CMS/
  i18n framework lớn tổ chức nội dung đa ngôn ngữ):
  ```
  specialty_translations(specialty_id, locale, name)      UNIQUE(specialty_id, locale)
  location_translations(location_id, locale, name)        UNIQUE(location_id, locale)
  job_package_translations(job_package_id, locale, name)   UNIQUE(job_package_id, locale)
  ```
  Bảng gốc (`specialties`, `locations`, `job_packages`) vẫn giữ cột `name` làm giá trị mặc định/dự
  phòng khi thiếu bản dịch cho 1 locale nào đó — không tự động dịch máy để lấp chỗ trống.
- **Ngôn ngữ đã chọn của người dùng**: `users.locale` — enum 6 giá trị, dùng để gửi email/thông báo
  đúng ngôn ngữ đã lưu, độc lập với header của request nào đó kích hoạt gửi.

### 5. Phạm vi dịch — giữ nguyên như bản v1
Chỉ dịch giao diện + danh mục chuẩn. **Không dịch nội dung do NTD/ứng viên tự viết** (mô tả tin tuyển
dụng, tiểu sử, mô tả tổ chức) — hiển thị nguyên văn ngôn ngữ tác giả đã nhập bất kể giao diện đang ở
ngôn ngữ nào. Không dịch máy tự động nội dung này (rủi ro dịch sai thuật ngữ y khoa).

## Phương án đã cân nhắc
- **Cột song song `name_en`/`name_ja`/... trên từng bảng danh mục**: dùng được với 2 ngôn ngữ (bản ADR
  v1) nhưng không hợp lý với 6 — mỗi bảng thêm 5 cột, thêm ngôn ngữ sau phải `ALTER TABLE`. Loại bỏ,
  chuyển sang bảng dịch riêng.
- **Geo-IP để tự nhận diện quốc gia** (giống một số site lớn dùng CDN có sẵn khả năng này): loại bỏ ở
  giai đoạn này — thêm phụ thuộc dịch vụ ngoài không cần thiết khi `Accept-Language` đã đủ dùng; có
  thể bổ sung sau nếu có nhu cầu thực tế phân biệt theo quốc gia (không chỉ ngôn ngữ).
- **Dịch máy tự động cho nội dung người dùng tự viết**: loại bỏ, giữ nguyên lý do từ bản v1 (rủi ro
  thuật ngữ y khoa).

## Hệ quả
- (+) Thêm ngôn ngữ thứ 7 trở đi chỉ cần thêm 1 dòng cấu hình locale + nhờ dịch nội dung, không đổi
  schema (bảng dịch đã tổng quát sẵn).
- (+) Vendor dịch thuê ngoài nhận đúng 1 thư mục/locale, không đụng vào code hay các ngôn ngữ khác.
- (−) Mọi truy vấn danh mục giờ cần JOIN thêm bảng dịch (thay vì đọc thẳng 1 cột) — cân nhắc cache ở
  tầng Application/Redis cho danh mục ít thay đổi để tránh JOIN lặp lại mỗi request.
- (−) Font hiển thị cần phủ thêm chữ Hán/Nhật/Hàn (CJK) — không tự host font CJK (dung lượng rất lớn),
  dựa vào font hệ điều hành sẵn có qua font-stack dự phòng (xem `../../frontend/CONG-NGHE-FRONTEND.md`).
- Chi tiết kỹ thuật: [`../../frontend/CONG-NGHE-FRONTEND.md`](../../frontend/CONG-NGHE-FRONTEND.md)
  mục "Đa ngôn ngữ", [`../../backend/CONG-NGHE-BACKEND.md`](../../backend/CONG-NGHE-BACKEND.md),
  schema ở [`../../database/ERD-CHI-TIET.md`](../../database/ERD-CHI-TIET.md).
