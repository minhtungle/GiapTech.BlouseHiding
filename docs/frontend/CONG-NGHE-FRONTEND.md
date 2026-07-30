# GiapTech.BlouseHiding — Công nghệ Frontend

> Xem tổng quan kiến trúc ở [`../kien-truc/TONG-QUAN-KIEN-TRUC.md`](../kien-truc/TONG-QUAN-KIEN-TRUC.md).
> Phong cách thiết kế & quyết định dùng mã nguồn mở ở đâu: [`THIET-KE-GIAO-DIEN.md`](./THIET-KE-GIAO-DIEN.md).
> Wireframe/style guide trực quan: [`wireframes/`](./wireframes/).

**3 khu vực site, 1 codebase** (xem quy ước tên ở [`../kien-truc/THUAT-NGU.md`](../kien-truc/THUAT-NGU.md)):
`app/(client)/...` (ứng viên/khách), `app/(admin)/...` (Nhà tuyển dụng), `app/(ops)/...` (đội Vận hành
nội bộ). **Không** tách app riêng cho `(ops)` chạy thẳng shadcn-admin — chỉ tham khảo bố cục từ đó rồi
tự viết lại bằng chính component đã chọn dưới đây, để giữ 1 domain – 1 container – 1 bộ thiết kế
(xem [ADR-0004](../kien-truc/adr/0004-tech-stack-net-nextjs.md), [ADR-0005](../kien-truc/adr/0005-dat-ten-3-khu-vuc-site.md)).

---

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Framework | **Next.js** (App Router) + **React 19** + TypeScript | SSR/SSG cho SEO tin tuyển dụng (quan trọng — tin cần lên Google), API routes làm BFF nhẹ khi cần |
| Component/UI | **shadcn/ui** (dựa trên Radix UI + Tailwind) | Không phải thư viện đóng gói (copy code vào repo) → tùy biến sâu theo bộ nhận diện riêng, tránh trông giống mọi trang MUI/AntD khác |
| Styling | **Tailwind CSS** | Đồng bộ với shadcn/ui, tốc độ style nhanh |
| Icon | **Lucide** | Đi kèm mặc định shadcn/ui, nét vẽ nhất quán, mã nguồn mở |
| Minh họa | **unDraw** | Mã nguồn mở, tự đổi màu theo bảng màu riêng thay vì ảnh stock chung chung |
| Font thân | **Be Vietnam Pro** (SIL OFL, tự host) | Tối ưu riêng cho dấu tiếng Việt |
| Quản lý state server | **TanStack Query (React Query)** | Cache, refetch, optimistic update cho dữ liệu từ API (jobs, applications...) |
| Quản lý state client nhẹ | **Zustand** | Chỉ cho state UI thuần túy (modal, wizard hồ sơ) — tránh Redux thừa cho quy mô này |
| Form & validate | **React Hook Form + Zod** | Zod schema tái dùng ý tưởng validate giống FluentValidation phía backend |
| Kéo-thả ATS Kanban | **dnd-kit** | Nhẹ, accessible, đúng nhu cầu màn hình ATS đã thiết kế |
| Bảng dữ liệu (trang Admin/Vận hành, danh sách ứng viên/tin) | **TanStack Table** (headless) | Không mang UI mặc định, tự style theo shadcn/ui table component |
| Rich text editor | **Tiptap** | Mô tả công việc, bài viết "Góc nghề y" (Giai đoạn 3) |
| Biểu đồ (dashboard Admin/Vận hành) | **Tremor** (dựng trên Recharts) | Component biểu đồ + KPI card sẵn, phối màu theo CSS variable — khớp nhanh với token thiết kế |
| Realtime client | **@microsoft/signalr** | Khớp SignalR backend |
| i18n | **next-intl** | Tiếng Việt + Tiếng Anh — xem chi tiết mục "Đa ngôn ngữ" bên dưới |
| Testing | **Vitest** + **React Testing Library** (unit) + **Playwright** (e2e) | Playwright thay thế Cypress — nhanh hơn, chạy đa trình duyệt |

---

## Đa ngôn ngữ (i18n)

> Quyết định & phạm vi: [ADR-0006](../kien-truc/adr/0006-da-ngon-ngu.md). Backend tương ứng:
> [`../backend/CONG-NGHE-BACKEND.md`](../backend/CONG-NGHE-BACKEND.md) mục i18n. Schema: `users.locale`
> + cột `*_en` ở danh mục, xem [`../database/ERD-CHI-TIET.md`](../database/ERD-CHI-TIET.md).

**Ngôn ngữ hỗ trợ:** Tiếng Việt (mặc định) + Tiếng Anh. **Phạm vi dịch có giới hạn có chủ đích** — chỉ
dịch giao diện (nút, nhãn, thông báo, email) và danh mục chuẩn hóa (chuyên khoa, địa điểm, loại hình
làm việc). **Không** dịch tự động nội dung do người dùng tự viết (mô tả tin tuyển dụng, tiểu sử ứng
viên) — nội dung đó hiển thị nguyên văn ngôn ngữ người viết đã nhập, kể cả khi xem ở giao diện tiếng
Anh. Đây là đánh đổi có chủ đích để giữ MVP đơn giản, không phải thiếu sót.

| Thành phần | Cách làm |
|---|---|
| Routing theo locale | `next-intl` với path prefix `/vi/...` / `/en/...` (locale mặc định `vi` không hiện prefix ở URL, theo cấu hình `localePrefix: 'as-needed'`) |
| Phát hiện ngôn ngữ lần đầu | Header `Accept-Language` trình duyệt → gợi ý, không tự ép chuyển nếu người dùng đã chọn thủ công |
| Ghi nhớ lựa chọn | Cookie + `users.locale` (khi đã đăng nhập) — đồng bộ để email/thông báo gửi đúng ngôn ngữ đã chọn dù đăng nhập từ thiết bị khác |
| File dịch | `messages/vi.json`, `messages/en.json` theo namespace từng route group (`client.json`, `admin.json`, `ops.json`) — tránh 1 file khổng lồ khó bảo trì |
| Danh mục (chuyên khoa/địa điểm/loại hình) | Component tự chọn `name` hay `name_en` từ API theo locale hiện tại của trang — dữ liệu đã có cả 2 cột sẵn từ backend, không dịch phía client |
| Font & dấu | Be Vietnam Pro đã hỗ trợ tốt cả bảng chữ Latin mở rộng (tiếng Anh không cần font riêng) |
| Quy tắc bắt buộc khi thêm text UI mới | Luôn thêm cả 2 khóa (`vi` + `en`) trong cùng PR — không hardcode chuỗi tiếng Việt trực tiếp trong component (xem `CLAUDE.md` mục 4) |

---

## Xem thêm
- Phong cách thiết kế, token màu/chữ, quyết định OSS: [`THIET-KE-GIAO-DIEN.md`](./THIET-KE-GIAO-DIEN.md)
- Wireframe 3 màn hình cốt lõi + style guide: [`wireframes/`](./wireframes/)
