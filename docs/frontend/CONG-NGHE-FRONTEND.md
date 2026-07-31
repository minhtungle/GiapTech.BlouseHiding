# GiapTech.BlouseHiding — Công nghệ Frontend

> Xem tổng quan kiến trúc ở [`../kien-truc/TONG-QUAN-KIEN-TRUC.md`](../kien-truc/TONG-QUAN-KIEN-TRUC.md).
> Phong cách thiết kế & quyết định dùng mã nguồn mở ở đâu: [`THIET-KE-GIAO-DIEN.md`](./THIET-KE-GIAO-DIEN.md).
> Wireframe/style guide trực quan: [`wireframes/`](./wireframes/).

**3 khu vực site, 1 codebase** (xem quy ước tên ở [`../kien-truc/THUAT-NGU.md`](../kien-truc/THUAT-NGU.md)):
`app/(client)/...` (ứng viên/khách — bản sắc "Tin cậy lâm sàng" đầy đủ), `app/(admin)/...` (Nhà tuyển
dụng) và `app/(ops)/...` (đội Vận hành nội bộ) — **2 khu vực sau dùng chung giao diện dashboard chuẩn
shadcn/ui trung tính**, khác "ngôn ngữ hình ảnh" so với `(client)` (xem
[ADR-0007](../kien-truc/adr/0007-shadcn-chuan-cho-admin-van-hanh.md)). **Không** tách app riêng cho
`(admin)`/`(ops)` chạy thẳng shadcn-admin — chỉ tham khảo bố cục từ đó rồi tự viết lại bằng chính
component đã chọn dưới đây, để giữ 1 domain – 1 container (xem
[ADR-0004](../kien-truc/adr/0004-tech-stack-net-nextjs.md), [ADR-0005](../kien-truc/adr/0005-dat-ten-3-khu-vuc-site.md)).

---

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Framework | **Next.js** (App Router) + **React 19** + TypeScript | SSR/SSG cho SEO tin tuyển dụng (quan trọng — tin cần lên Google), API routes làm BFF nhẹ khi cần |
| Component/UI | **shadcn/ui** (dựa trên Radix UI + Tailwind) | Không phải thư viện đóng gói (copy code vào repo) → tùy biến sâu theo bộ nhận diện riêng, tránh trông giống mọi trang MUI/AntD khác |
| Styling | **Tailwind CSS** | Đồng bộ với shadcn/ui, tốc độ style nhanh |
| Icon | **Lucide** | Đi kèm mặc định shadcn/ui, nét vẽ nhất quán, mã nguồn mở |
| Minh họa | **unDraw** | Mã nguồn mở, tự đổi màu theo bảng màu riêng thay vì ảnh stock chung chung |
| Font thân | **Be Vietnam Pro** (SIL OFL, tự host) cho chữ Latin (vi/en/es); font hệ điều hành cho chữ Hán/Nhật/Hàn | Be Vietnam Pro tối ưu dấu tiếng Việt; không tự host font CJK (dung lượng rất lớn) — xem mục "Đa ngôn ngữ" |
| Quản lý state server | **TanStack Query (React Query)** | Cache, refetch, optimistic update cho dữ liệu từ API (jobs, applications...) |
| Quản lý state client nhẹ | **Zustand** | Chỉ cho state UI thuần túy (modal, wizard hồ sơ) — tránh Redux thừa cho quy mô này |
| Form & validate | **React Hook Form + Zod** | Zod schema tái dùng ý tưởng validate giống FluentValidation phía backend |
| Kéo-thả ATS Kanban | **dnd-kit** | Nhẹ, accessible, đúng nhu cầu màn hình ATS đã thiết kế |
| Bảng dữ liệu (trang Admin/Vận hành, danh sách ứng viên/tin) | **TanStack Table** (headless) | Không mang UI mặc định, tự style theo shadcn/ui table component |
| Rich text editor | **Tiptap** | Mô tả công việc, bài viết "Góc nghề y" (Giai đoạn 3) |
| Biểu đồ (dashboard Admin/Vận hành) | **Tremor** (dựng trên Recharts) | Component biểu đồ + KPI card sẵn, phối màu theo CSS variable — khớp nhanh với token thiết kế |
| Realtime client | **@microsoft/signalr** | Khớp SignalR backend |
| i18n | **next-intl** | 6 ngôn ngữ (vi/en/ja/zh/ko/es) — xem chi tiết mục "Đa ngôn ngữ" bên dưới |
| Testing | **Vitest** + **React Testing Library** (unit) + **Playwright** (e2e) | Playwright thay thế Cypress — nhanh hơn, chạy đa trình duyệt |

---

## Đa ngôn ngữ (i18n)

> Quyết định & phương án đã cân nhắc: [ADR-0006](../kien-truc/adr/0006-da-ngon-ngu.md). Backend tương
> ứng: [`../backend/CONG-NGHE-BACKEND.md`](../backend/CONG-NGHE-BACKEND.md) mục i18n. Schema: bảng
> `*_translations` + `users.locale`, xem [`../database/ERD-CHI-TIET.md`](../database/ERD-CHI-TIET.md).
> Tham khảo cách tổ chức routing/i18n: bài viết [Next.js i18n với next-intl](https://viblo.asia/p/neu-bat-dau-xay-dung-website-phai-biet-ve-da-ngon-ngu-nextjs-15-i18n-next-intl-W13VMrOD4Y7).

**6 ngôn ngữ hỗ trợ:** Tiếng Việt `vi` (mặc định), Tiếng Anh `en`, Tiếng Nhật `ja`, Tiếng Trung giản
thể `zh`, Tiếng Hàn `ko`, Tiếng Tây Ban Nha `es`. **Phạm vi dịch giới hạn có chủ đích** — chỉ dịch giao
diện (nút, nhãn, thông báo, email) và danh mục chuẩn hóa (chuyên khoa, địa điểm, loại hình, tên gói).
**Không** dịch tự động nội dung do người dùng tự viết (mô tả tin, tiểu sử) — hiển thị nguyên văn ngôn
ngữ tác giả đã nhập, kể cả khi xem ở giao diện ngôn ngữ khác.

### Routing theo tiền tố URL

```
/vi/viec-lam/dieu-duong-icu       (mặc định — vẫn có tiền tố, không ẩn như nhiều site khác)
/en/jobs/icu-nurse
/ja/... · /zh/... · /ko/... · /es/...
```
Cấu hình `next-intl`: `locales: ['vi','en','ja','zh','ko','es']`, `defaultLocale: 'vi'`,
`localePrefix: 'always'` — tiền tố luôn hiện kể cả ngôn ngữ mặc định (khác kiểu Apple ẩn tiền tố cho
thị trường gốc), để URL nhất quán, dễ chia sẻ link kèm ngôn ngữ cụ thể, dễ khai báo `hreflang` cho SEO.

**Tự động nhận diện lần đầu** (chưa có cookie): middleware kiểm tra `Accept-Language` của trình duyệt,
khớp với 1 trong 6 ngôn ngữ hỗ trợ → redirect sang tiền tố tương ứng; không khớp được thì mặc định
`vi`. Không dùng geo-IP (dò theo địa chỉ IP) — không cần thêm dịch vụ ngoài cho nhu cầu hiện tại.

### Bộ chọn ngôn ngữ trên giao diện

Nút chọn đặt ở **header**, góc phải cạnh khu vực tài khoản (mẫu phổ biến kiểu Notion/GitHub/Airbnb) —
icon globe + mã ngôn ngữ hiện tại (vd "VI"), bấm mở danh sách 6 ngôn ngữ hiển thị bằng **tên bản ngữ**
(English · 日本語 · 中文 · 한국어 · Tiếng Việt · Español), không dịch tên ngôn ngữ khác theo ngôn ngữ đang
xem. Chọn xong giữ nguyên trang, chỉ đổi tiền tố URL + ghi cookie. Thêm 1 link phụ ở **footer** trỏ
cùng bộ chọn (tham khảo bố cục apple.com/vn/) làm điểm truy cập dự phòng.

### Lưu trữ & tổ chức bản dịch

| Thành phần | Cách làm |
|---|---|
| File dịch giao diện | `messages/{locale}/{namespace}.json` — 1 thư mục/ngôn ngữ (`messages/vi/`, `messages/en/`...), mỗi thư mục chia theo namespace route group (`common.json`, `client.json`, `admin.json`, `ops.json`). Giao đúng 1 thư mục cho dịch giả/vendor thuê ngoài, không đụng ngôn ngữ khác |
| Danh mục (chuyên khoa/địa điểm/tên gói) | API trả kèm bản dịch từ bảng `*_translations` (JOIN theo `locale` đang active) — component chỉ hiển thị field nhận được, không tự chọn cột phía client |
| Ghi nhớ lựa chọn | Cookie + `users.locale` khi đã đăng nhập — đồng bộ để email/thông báo gửi đúng ngôn ngữ đã chọn dù đăng nhập từ thiết bị khác |
| Ngày/giờ/số | `Intl.DateTimeFormat`/`Intl.NumberFormat` theo locale qua tiện ích có sẵn của `next-intl` — định dạng ngày tháng khác nhau giữa các ngôn ngữ (vd `ja` dùng 年/月/日), tiền tệ VND giữ nguyên ký hiệu bất kể locale |
| Font & chữ CJK | Be Vietnam Pro tự host cho `vi`/`en`/`es` (Latin mở rộng); chữ Hán/Nhật/Hàn **không tự host** (font CJK nặng hàng chục MB) — dùng font-stack dự phòng theo hệ điều hành: `'Be Vietnam Pro', -apple-system, 'PingFang SC', 'Hiragino Sans', 'Malgun Gothic', sans-serif` |
| SEO đa ngôn ngữ | Mỗi trang phát `<link rel="alternate" hreflang="{locale}">` cho cả 6 phiên bản + `hreflang="x-default"` trỏ `vi`, quan trọng vì tin tuyển dụng cần SSR/SSG lên Google (đã nêu ở bảng trên) |
| Quy tắc bắt buộc khi thêm text UI mới | Luôn thêm đủ **6 khóa dịch** trong cùng PR — không hardcode chuỗi tiếng Việt trực tiếp trong component (xem `CLAUDE.md` mục 4) |

---

## Xem thêm
- Phong cách thiết kế, token màu/chữ, quyết định OSS: [`THIET-KE-GIAO-DIEN.md`](./THIET-KE-GIAO-DIEN.md)
- Wireframe 3 màn hình cốt lõi + style guide: [`wireframes/`](./wireframes/)
