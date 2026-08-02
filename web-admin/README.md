# BlouseHiding — Admin & Vận hành (web-admin/)

App Vite + TanStack Router cho **Admin (Nhà tuyển dụng)** và **Vận hành** (nội bộ nền tảng), dùng chung
1 instance, phân biệt màn hình theo role — xem [ADR-0008](../docs/kien-truc/adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md)
và [`../CLAUDE.md`](../CLAUDE.md) trước khi sửa.

Vendor từ [`satnaing/shadcn-admin`](https://github.com/satnaing/shadcn-admin) (MIT — xem `LICENSE`),
đã gỡ Clerk (auth demo không dùng — dự án tự có JWT Bearer riêng), đổi nav sang màn hình thật của dự án,
áp token màu trung tính + seal-red cho CTA/badge theo [ADR-0007](../docs/kien-truc/adr/0007-shadcn-chuan-cho-admin-van-hanh.md).

```bash
npm install
npm run dev      # http://localhost:5173 (mặc định Vite)
npm run build
npm run lint
```

- Không dùng đa ngôn ngữ (chỉ tiếng Việt) — xem ADR-0008 mục 5.
- Font tự host (`@fontsource-variable/inter`, `@fontsource-variable/manrope`) — không gọi runtime tới
  Google Fonts.
- Đang ở Giai đoạn 0.1 (UI Shell) — phần lớn màn hình theo
  [`../docs/nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md`](../docs/nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md)
  mục 2.3/2.4 **chưa được dựng**, nav sidebar hiện trỏ tới route placeholder.
