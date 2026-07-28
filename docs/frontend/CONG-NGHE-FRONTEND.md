# GiapTech.BlouseHiding — Công nghệ Frontend

> Xem tổng quan kiến trúc ở [`../kien-truc/TONG-QUAN-KIEN-TRUC.md`](../kien-truc/TONG-QUAN-KIEN-TRUC.md).
> Phong cách thiết kế & quyết định dùng mã nguồn mở ở đâu: [`THIET-KE-GIAO-DIEN.md`](./THIET-KE-GIAO-DIEN.md).
> Wireframe/style guide trực quan: [`wireframes/`](./wireframes/).

**Khu vực Admin:** cùng 1 codebase Next.js (route group `app/(admin)/...`), **không** tách app riêng
chạy thẳng shadcn-admin — chỉ tham khảo bố cục từ đó rồi tự viết lại bằng chính component đã chọn dưới
đây, để giữ 1 domain – 1 container – 1 bộ thiết kế (xem [ADR-0004](../kien-truc/adr/0004-tech-stack-net-nextjs.md)).

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
| Bảng dữ liệu (Admin, danh sách ứng viên/tin) | **TanStack Table** (headless) | Không mang UI mặc định, tự style theo shadcn/ui table component |
| Rich text editor | **Tiptap** | Mô tả công việc, bài viết "Góc nghề y" (Giai đoạn 3) |
| Biểu đồ (dashboard Admin) | **Tremor** (dựng trên Recharts) | Component biểu đồ + KPI card sẵn, phối màu theo CSS variable — khớp nhanh với token thiết kế |
| Realtime client | **@microsoft/signalr** | Khớp SignalR backend |
| i18n | **next-intl** | Tiếng Việt chính, để ngỏ tiếng Anh cho ứng viên/tổ chức nước ngoài sau này |
| Testing | **Vitest** + **React Testing Library** (unit) + **Playwright** (e2e) | Playwright thay thế Cypress — nhanh hơn, chạy đa trình duyệt |

---

## Xem thêm
- Phong cách thiết kế, token màu/chữ, quyết định OSS: [`THIET-KE-GIAO-DIEN.md`](./THIET-KE-GIAO-DIEN.md)
- Wireframe 3 màn hình cốt lõi + style guide: [`wireframes/`](./wireframes/)
