# ADR-0010: Đảo ngược ADR-0008 mục 5 — thêm đa ngôn ngữ cho `web-admin/`

**Trạng thái:** Đã chấp nhận (đảo ngược 1 phần [ADR-0008](./0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md)
mục 5 — chỉ phần "chỉ tiếng Việt", không đảo ngược phần còn lại của ADR-0008: `web-admin/` vẫn là 1 app
riêng chạy shadcn-admin, vẫn dùng chung 1 instance cho cả Admin (NTD) và Vận hành)

## Bối cảnh
ADR-0008 (mục 5) chốt `web-admin/` chỉ tiếng Việt, với lý do: đây là công cụ nội bộ dùng bởi NTD tại
Việt Nam và đội Vận hành nội bộ, không phải bề mặt hướng ứng viên quốc tế — dùng `next-intl` cho 1 SPA
nội bộ "không đáng công sức".

Thực tế phát sinh không khớp giả định đó: có **nhà tuyển dụng/nhân sự phía tổ chức y tế không nói được
tiếng Việt** (cơ sở y tế có vốn đầu tư nước ngoài, quản lý là người nước ngoài hoặc gốc Việt không đọc
được tiếng Việt) cần tự vào `web-admin/` (khu vực Admin) để đăng tin, quản lý ứng viên, quản lý tổ
chức — không thể chỉ giao dịch qua người trung gian nói tiếng Việt. Giả định "toàn bộ người dùng
`web-admin/` đọc được tiếng Việt" không còn đúng, nên quyết định gốc ở ADR-0008 mục 5 cần đảo lại.

**Lưu ý phạm vi:** nhu cầu này xuất phát từ phía Admin (NTD), không phải từ đội Vận hành nội bộ (nhân
viên công ty vận hành nền tảng vẫn giả định nói tiếng Việt). Vì `web-admin/` dùng chung 1 instance cho
cả 2 vai trò (ADR-0008 mục 2), không tách riêng theo vai trò được — quyết định áp dụng đa ngôn ngữ cho
toàn bộ `web-admin/`, không chỉ phần Admin.

## Quyết định

### 1. Ngôn ngữ hỗ trợ
Áp dụng đúng 6 ngôn ngữ đã chọn cho `web/` ở [ADR-0006](./0006-da-ngon-ngu.md) mục 1 — không rút gọn,
không mở rộng thêm: **Tiếng Việt (`vi`, mặc định)**, **Tiếng Anh (`en`)**, **Tiếng Nhật (`ja`)**,
**Tiếng Trung giản thể (`zh`)**, **Tiếng Hàn (`ko`)**, **Tiếng Tây Ban Nha (`es`)**. Giữ cùng bộ để
không phải quản lý 2 danh sách ngôn ngữ khác nhau trong 1 hệ thống, và để tái dùng được các bảng dịch
danh mục (`specialty_translations`, `location_translations`, `job_package_translations`) đã có sẵn ở
backend — các bảng này không phụ thuộc app frontend nào hiển thị (ADR-0006 đã nêu).

### 2. Thư viện: `react-i18next`, không phải `next-intl`
`next-intl` gắn với Next.js App Router (Server Components, `middleware.ts`, `generateStaticParams`) —
không cài được cho Vite SPA thuần. Chọn **`react-i18next`** (kèm `i18next-browser-languagedetector`)
vì:
- Thư viện i18n phổ biến nhất cho React SPA nói chung, không ràng buộc framework — phù hợp trực tiếp
  với stack Vite + React 19 + TanStack Router của `web-admin/`.
- Hỗ trợ namespace theo file JSON (giống cách tổ chức `messages/{locale}/{namespace}.json` của `web/`)
  — giữ được cùng tư duy tổ chức bản dịch giữa 2 app, dễ giao vendor dịch thuê ngoài theo từng gói
  ngôn ngữ như đã áp dụng ở `web/`.
- Không cần SSR/SSG (khác `next-intl` được thiết kế quanh Server Components) — khớp đúng bản chất SPA
  của `web-admin/`, không thêm phụ thuộc không dùng tới.

### 3. Cấu trúc thư mục bản dịch
Đặt tại `web-admin/src/messages/{locale}/{namespace}.json` — cùng tên thư mục `messages/` và cùng
cấu trúc `{locale}/{namespace}.json` như `web/messages/`, để nhất quán trực giác giữa 2 app dù dùng 2
thư viện khác nhau (đây chính là câu hỏi khởi nguồn ADR này — "web-admin cũng cần tương tự"). Namespace
chia theo khu vực màn hình, không theo 1-1 với `web/` (nghiệp vụ khác nhau): `common` (nav/sidebar/nút
chung), `auth` (đăng nhập), `jobs` (đăng tin/danh sách/ATS), `candidates` (tìm ứng viên), `credit`
(ví Credit/thanh toán), `ops` (3 hàng đợi duyệt + đối soát + danh mục), `members` (thành viên tổ chức).

### 4. Chọn ngôn ngữ trên giao diện
Thêm 1 dropdown chọn ngôn ngữ trong `Header` (cạnh `ThemeSwitch`/`ProfileDropdown` hiện có) — cùng vị
trí quen thuộc với `web/` (góc phải header). Lưu lựa chọn vào `localStorage` (không phải cookie —
`web-admin/` là SPA thuần, không có SSR cần đọc cookie phía server). Không đồng bộ 2 chiều với
`users.locale` ở backend trong phạm vi ADR này — mục 5 nêu rõ đây là việc riêng, chưa làm ngay.

### 5. Không đổi
- **Không** đổi `web-admin/` từ Vite SPA sang Next.js hay bất kỳ thay đổi kiến trúc nào khác của
  ADR-0008 — chỉ đảo phần "chỉ tiếng Việt" ở mục 5.
- **Không** dịch nội dung do NTD/ứng viên tự viết hiển thị trong `web-admin/` (mô tả tin tuyển dụng,
  ghi chú ATS, lý do từ chối...) — giữ nguyên nguyên tắc "không dịch nội dung tự do người dùng viết"
  đã chốt ở ADR-0006 mục 5, áp dụng chung cho cả 2 app.
- **Không** đổi phạm vi ngôn ngữ của đội Vận hành nội bộ trong tài liệu vận hành/runbook — tài liệu nội
  bộ (`docs/ha-tang/VAN-HANH-RUNBOOK.md` và tương tự) vẫn viết tiếng Việt, đây chỉ là đổi ngôn ngữ hiển
  thị *giao diện phần mềm*, không phải ngôn ngữ làm việc của đội Vận hành.

## Phương án đã cân nhắc
- **Giữ ADR-0008 mục 5 như cũ, chỉ dịch tay 1 lần cho khách nước ngoài cụ thể khi phát sinh (không xây
  hạ tầng i18n)**: loại bỏ — không scale, mỗi khách mới lại phải dịch tay lại, và không nhất quán với
  hướng đa ngôn ngữ đã đầu tư ở `web/`.
- **Tách riêng 1 app Admin dành cho NTD nước ngoài, giữ `web-admin/` hiện tại chỉ tiếng Việt cho Vận
  hành**: loại bỏ — vi phạm thẳng lý do ADR-0008 mục 2 chọn dùng chung 1 instance (tránh nhân đôi
  domain/container/pipeline); Admin và Vận hành dùng chung phần lớn pattern UI, tách riêng không cần
  thiết chỉ vì khác ngôn ngữ.
- **Dùng `@lingui/react` thay `react-i18next`**: cả 2 đều khả thi cho Vite SPA; chọn `react-i18next` vì
  phổ biến hơn, ít yêu cầu build-step riêng (Lingui cần compile message catalog qua CLI), và mô hình
  namespace JSON gần với cách `web/` đã tổ chức hơn.

## Hệ quả
- (+) Admin/NTD không nói tiếng Việt dùng được `web-admin/` — mở khả năng phục vụ cơ sở y tế có vốn/
  quản lý nước ngoài, đúng nhu cầu thực tế phát sinh.
- (+) Cùng tư duy tổ chức bản dịch (`messages/{locale}/{namespace}.json`) với `web/` — dễ giao vendor
  dịch cả 2 app cùng lúc, giảm chi phí học lại cấu trúc.
- (−) Toàn bộ chuỗi UI hiện đang hardcode tiếng Việt trực tiếp trong component (`features/**/*.tsx`)
  phải rút ra thành key dịch — đây là công việc dọn dẹp đáng kể, không chỉ thêm thư viện. Xem
  [`docs/nghiep-vu/TIEN-DO-CHI-TIET.md`](../../nghiep-vu/TIEN-DO-CHI-TIET.md) để theo dõi tiến độ rút
  chuỗi theo từng feature.
- (−) Thêm 1 thư viện + cấu hình build (namespace loader, language detector) vào `web-admin/` — tăng
  nhẹ bundle size và độ phức tạp so với hiện trạng (chuỗi tĩnh, không có tầng dịch).
- (−) Đội Vận hành nội bộ (vốn chỉ cần tiếng Việt) giờ nhìn thấy dropdown chọn ngôn ngữ không phục vụ
  nhu cầu của họ — chấp nhận vì `web-admin/` dùng chung 1 instance cho cả 2 vai trò (ADR-0008 mục 2),
  không tách riêng UI theo vai trò cho việc này.
- Cập nhật liên quan: [ADR-0006](./0006-da-ngon-ngu.md) (ghi chú phạm vi mở lại cho `web-admin/`),
  [ADR-0008](./0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md) mục 5 (đánh dấu đã đảo ngược, trỏ
  sang ADR này), [`../../frontend/CONG-NGHE-FRONTEND.md`](../../frontend/CONG-NGHE-FRONTEND.md),
  `CLAUDE.md` mục 4 quy tắc #10 (mở rộng áp dụng cho `web-admin/`).

## Việc cần làm tiếp (không thuộc phạm vi quyết định kiến trúc này, ghi chú để không quên)
1. Cài `react-i18next` + `i18next-browser-languagedetector` vào `web-admin/`.
2. Dựng khung: `i18n.ts` config, `src/messages/{locale}/{namespace}.json` cho 6 locale × 7 namespace
   (khởi tạo bằng tiếng Việt trước, dịch 5 ngôn ngữ còn lại sau — không chặn việc dựng khung).
3. Rút toàn bộ chuỗi hardcode trong `features/**/*.tsx` sang `useTranslation(namespace)` — làm theo
   từng feature, cập nhật `docs/nghiep-vu/TIEN-DO-CHI-TIET.md` khi xong mỗi feature.
4. Thêm dropdown chọn ngôn ngữ vào `Header`.
5. Đánh giá sau: có cần đồng bộ lựa chọn ngôn ngữ `web-admin/` với `users.locale` ở backend không (để
   khi NTD đăng nhập `web/` và `web-admin/` cùng 1 tài khoản thấy cùng ngôn ngữ) — chưa quyết định,
   để ngỏ cho lúc triển khai.
