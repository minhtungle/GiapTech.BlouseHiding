# GiapTech.BlouseHiding — Công nghệ Frontend

> Xem tổng quan kiến trúc ở [`../kien-truc/TONG-QUAN-KIEN-TRUC.md`](../kien-truc/TONG-QUAN-KIEN-TRUC.md).
> Phong cách thiết kế & quyết định dùng mã nguồn mở ở đâu: [`THIET-KE-GIAO-DIEN.md`](./THIET-KE-GIAO-DIEN.md).
> Wireframe/style guide trực quan: [`wireframes/`](./wireframes/).

**2 ứng dụng frontend riêng biệt** (xem quy ước tên khu vực ở [`../kien-truc/THUAT-NGU.md`](../kien-truc/THUAT-NGU.md),
quyết định tách app ở [ADR-0008](../kien-truc/adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md)):

- **`web/`** — Next.js, chỉ khu vực **Client** (ứng viên/khách), bản sắc "Tin cậy lâm sàng" đầy đủ, cần
  SSR/SSG cho SEO tin tuyển dụng, đủ 6 ngôn ngữ.
- **`web-admin/`** — **shadcn-admin** (Vite + React Router + TypeScript), dùng chung cho **Admin (NTD)**
  và **Vận hành** (nội bộ) — phân biệt màn hình theo role đăng nhập, RBAC chặn thật ở backend. Dùng
  giao diện dashboard chuẩn shadcn/ui trung tính (xem [ADR-0007](../kien-truc/adr/0007-shadcn-chuan-cho-admin-van-hanh.md)),
  chỉ tiếng Việt (không dùng next-intl).

---

### `web/` — Client (Next.js)

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
| Rich text editor | **Tiptap** | Mô tả công việc, bài viết "Góc nghề y" (Giai đoạn 3) |
| Realtime client | **@microsoft/signalr** | Khớp SignalR backend |
| i18n | **next-intl** | 6 ngôn ngữ (vi/en/ja/zh/ko/es) — xem chi tiết mục "Đa ngôn ngữ" bên dưới |
| Testing | **Vitest** + **React Testing Library** (unit) + **Playwright** (e2e) | Playwright thay thế Cypress — nhanh hơn, chạy đa trình duyệt |

### `web-admin/` — Admin (NTD) + Vận hành (shadcn-admin)

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Framework | **shadcn-admin** (Vite + **React Router** + React 19 + TypeScript, MIT) | Cài đặt trực tiếp làm nền tảng app — tận dụng pattern sidebar/bảng/dialog có sẵn, xem [ADR-0008](../kien-truc/adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md) |
| Component/UI | **shadcn/ui** (Radix + Tailwind) | Cùng nền tảng component với Client — theme trung tính riêng (xem `THIET-KE-GIAO-DIEN.md` mục 3.2) |
| Quản lý state server | **TanStack Query** | Giống Client — cache/refetch dữ liệu API |
| Quản lý state client nhẹ | **Zustand** | Modal, wizard nội bộ (đăng tin, duyệt hồ sơ) |
| Form & validate | **React Hook Form + Zod** | Đồng nhất với Client |
| Kéo-thả ATS Kanban | **dnd-kit** | Nhẹ, accessible, đúng nhu cầu màn hình ATS đã thiết kế |
| Bảng dữ liệu (danh sách ứng viên/tin/người dùng) | **TanStack Table** (headless) | Không mang UI mặc định, tự style theo shadcn/ui table component |
| Biểu đồ dashboard | **Tremor** (dựng trên Recharts) | Component biểu đồ + KPI card sẵn, phối màu theo CSS variable — khớp nhanh với token thiết kế |
| Realtime client | **@microsoft/signalr** | Thông báo/report mới cho Vận hành |
| i18n | **Không dùng** — chỉ tiếng Việt | Công cụ nội bộ, không hướng đối tượng đa quốc gia — xem [ADR-0008](../kien-truc/adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md) mục 5 |
| Testing | **Vitest** + **React Testing Library** (unit) + **Playwright** (e2e, base URL riêng) | Cùng bộ công cụ với Client, cấu hình riêng theo Vite |

---

## Đa ngôn ngữ (i18n)

> ⚠️ Toàn bộ mục này chỉ áp dụng cho **`web/` (Client)**. `web-admin/` (Admin/Vận hành) không dùng
> next-intl, chỉ tiếng Việt — xem [ADR-0008](../kien-truc/adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md) mục 5.
>
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
| File dịch giao diện | `messages/{locale}/{namespace}.json` — 1 thư mục/ngôn ngữ (`messages/vi/`, `messages/en/`...) trong `web/`, mỗi thư mục chia theo namespace (`common.json`, `jobs.json`, `profile.json`...). Giao đúng 1 thư mục cho dịch giả/vendor thuê ngoài, không đụng ngôn ngữ khác |
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
