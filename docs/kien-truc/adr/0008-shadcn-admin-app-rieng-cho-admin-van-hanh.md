# ADR-0008: Admin (NTD) + Vận hành tách thành 1 app riêng, chạy thẳng shadcn-admin

**Trạng thái:** Đã chấp nhận (đảo ngược 1 phần [ADR-0004](./0004-tech-stack-net-nextjs.md) và khung
kỹ thuật nêu trong [ADR-0007](./0007-shadcn-chuan-cho-admin-van-hanh.md) — phần "cùng 1 codebase
Next.js", **không** đảo ngược phần quyết định màu sắc/phong cách của ADR-0007)

## Bối cảnh
ADR-0004 từng loại bỏ phương án "shadcn-admin chạy như app riêng" vì lo ngại 2 hệ sinh thái frontend,
2 lần cấu hình theme, thêm domain/container/pipeline — ưu tiên 1 codebase Next.js dùng route group,
chỉ tham khảo bố cục shadcn-admin rồi tự dựng lại. Sau khi dựng mockup thật
(`wireframes/trang-chu-3-vai-tro.html`) và cân nhắc lại tốc độ triển khai Admin/Vận hành (2 khu vực
không cần SEO/SSR, không cần chia sẻ route với trang công khai), quyết định **dùng thẳng shadcn-admin**
làm nền tảng thật cho 2 khu vực này thay vì tự dựng lại từ đầu bên trong Next.js.

## Quyết định
1. **Admin (NTD) + Vận hành tách khỏi Next.js**, chuyển thành **1 ứng dụng riêng** dựng trên
   **shadcn-admin** (Vite + TanStack Router + TypeScript, MIT license) — cài đặt trực tiếp làm nền tảng
   app, không còn chỉ "tham khảo bố cục".
   > 📝 Đính chính lúc scaffold thật (Giai đoạn 0.1): bản ghi ban đầu của ADR này ghi router là "React
   > Router" — kiểm chứng lại repo gốc [`satnaing/shadcn-admin`](https://github.com/satnaing/shadcn-admin)
   > thì router thật là **TanStack Router**, không phải React Router. Đã sửa xuyên suốt tài liệu liên
   > quan (`CLAUDE.md`, `CONG-NGHE-FRONTEND.md`, `THIET-KE-GIAO-DIEN.md`, `TONG-QUAN-KIEN-TRUC.md`).
   > Không đổi bản chất quyết định (vẫn Vite SPA, không phải Next.js) — chỉ đính chính chi tiết kỹ thuật.
2. **Dùng chung 1 instance duy nhất** cho cả Admin (NTD) và Vận hành (không tách 2 app riêng biệt) —
   phân biệt màn hình/nav theo role đăng nhập (`employer_*` thấy menu Admin; `admin`/`moderator` thấy
   menu Vận hành), chặn thật sự bằng RBAC phía backend (đã có sẵn, xem
   [`../../backend/CONG-NGHE-BACKEND.md`](../../backend/CONG-NGHE-BACKEND.md) mục 3), phía frontend chỉ
   ẩn/hiện nav + route guard cho trải nghiệm, không phải lớp bảo mật chính.
3. **Client giữ nguyên Next.js** (App Router) — không đổi, vẫn cần SSR/SSG cho SEO tin tuyển dụng.
4. **Xác thực xuyên 2 app**: đã dùng JWT Bearer (không phải cookie session, xem
   [`../../backend/CONG-NGHE-BACKEND.md`](../../backend/CONG-NGHE-BACKEND.md) mục 3) nên không phát
   sinh vấn đề cookie cross-domain — chỉ cần thêm origin của app Admin/Vận hành vào whitelist CORS của
   backend.
5. **Đa ngôn ngữ**: app Admin/Vận hành **không áp dụng 6 ngôn ngữ** — chỉ tiếng Việt. Đây là công cụ nội
   bộ dùng bởi NTD tại Việt Nam và đội Vận hành nội bộ, không phải bề mặt hướng ứng viên quốc tế; dùng
   `next-intl` cho 1 SPA nội bộ không đáng công sức. Phạm vi 6 ngôn ngữ ở
   [ADR-0006](./0006-da-ngon-ngu.md) thu hẹp lại **chỉ áp dụng cho Client**.

## Phương án đã cân nhắc
- **Giữ nguyên route group trong Next.js, chỉ tham khảo bố cục (quyết định cũ ở ADR-0004)**: loại bỏ —
  tốn công tự dựng lại toàn bộ pattern (sidebar, bảng, KPI card, form dialog...) mà shadcn-admin đã có
  sẵn, trong khi Admin/Vận hành không cần bất kỳ lợi ích nào của Next.js (không cần SEO, không cần SSR,
  không chia sẻ URL công khai với Client).
- **2 instance shadcn-admin riêng biệt cho Admin và Vận hành**: loại bỏ — 2 vai trò dùng phần lớn cùng
  1 bộ pattern UI (bảng, card, dialog), tách riêng nhân đôi domain/container/pipeline không cần thiết;
  RBAC nội bộ 1 app đã đủ để phân biệt màn hình theo role mà không lộ dữ liệu (chặn thật ở backend).
- **Dùng shadcn-admin cho cả Client luôn (bỏ Next.js)**: loại bỏ — Client bắt buộc cần SSR/SSG cho SEO
  tin tuyển dụng (đã nêu ở tiêu chí chọn Next.js từ đầu), shadcn-admin dựng trên Vite (SPA thuần, không
  SSR) không đáp ứng được.

## Hệ quả
- (+) Tận dụng ngay bộ màn hình/pattern có sẵn của shadcn-admin (sidebar, bảng dữ liệu, form dialog,
  command palette...) — đi nhanh hơn đáng kể so với tự dựng lại trong Next.js.
- (+) Ranh giới rõ ràng hơn theo đúng nhu cầu kỹ thuật: Client (cần SEO/SSR) dùng Next.js, Admin/Vận
  hành (công cụ nội bộ, không cần SEO) dùng SPA — không còn ép 1 codebase phải phục vụ 2 nhu cầu khác
  nhau.
- (+) **Hạ tầng đơn giản hơn dự kiến, không phức tạp hơn**: shadcn-admin build ra static assets (Vite
  SPA) — Caddy phục vụ thẳng như static site, **không cần thêm container/Node runtime riêng** như lo
  ngại ban đầu ở ADR-0004. Cập nhật sơ đồ triển khai ở
  [`../../ha-tang/HA-TANG-TRIEN-KHAI.md`](../../ha-tang/HA-TANG-TRIEN-KHAI.md).
- (+) Auth JWT Bearer có sẵn từ đầu nên không phát sinh vấn đề cookie cross-domain khi tách app — chỉ
  cần thêm whitelist CORS.
- (−) 2 hệ sinh thái frontend (Next.js cho Client, Vite/TanStack Router cho Admin/Vận hành) — 2 lần cấu
  hình Tailwind/theme token (dù cùng giá trị màu ở
  [`../../frontend/THIET-KE-GIAO-DIEN.md`](../../frontend/THIET-KE-GIAO-DIEN.md) mục 5), 2 lần setup
  test (Vitest+RTL vẫn dùng chung được, nhưng Playwright e2e phải trỏ 2 base URL khác nhau).
  Không dùng chung component React giữa 2 app (khác build tool) — chấp nhận trùng lặp nhỏ (vd Badge
  trạng thái) đổi lấy tốc độ dựng Admin/Vận hành.
- (−) Phạm vi đa ngôn ngữ thu hẹp — Admin/Vận hành chỉ tiếng Việt (xem điểm 5 ở Quyết định); nếu sau
  này cần đa ngôn ngữ cho Admin (vd NTD nước ngoài), phải đánh giá lại — chưa có nhu cầu hiện tại.
- Cập nhật liên quan: [`../../../CLAUDE.md`](../../../CLAUDE.md) mục 3 & 6,
  [`../../frontend/CONG-NGHE-FRONTEND.md`](../../frontend/CONG-NGHE-FRONTEND.md),
  [`../../frontend/THIET-KE-GIAO-DIEN.md`](../../frontend/THIET-KE-GIAO-DIEN.md) mục 3.2 & 4,
  [`../TONG-QUAN-KIEN-TRUC.md`](../TONG-QUAN-KIEN-TRUC.md) mục 3/5/6/7,
  [`../../ha-tang/HA-TANG-TRIEN-KHAI.md`](../../ha-tang/HA-TANG-TRIEN-KHAI.md) mục 1,
  wireframe [`../../frontend/wireframes/trang-chu-3-vai-tro.html`](../../frontend/wireframes/trang-chu-3-vai-tro.html).
