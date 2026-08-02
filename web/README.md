# BlouseHiding — Client (web/)

App Next.js (App Router) cho khu vực **Client** (ứng viên & khách chưa đăng nhập) — xem
[`../CLAUDE.md`](../CLAUDE.md) và [`../docs/frontend/`](../docs/frontend/) trước khi sửa.

```bash
npm install
npm run dev     # http://localhost:3000 (redirect sang /vi mặc định)
npm run build
npm run lint
```

- Đa ngôn ngữ: `next-intl`, 6 locale (`vi/en/ja/zh/ko/es`), cấu hình ở `i18n/`, bản dịch ở `messages/`.
- Token thiết kế "Tin cậy lâm sàng": `app/globals.css` — xem
  [`../docs/frontend/THIET-KE-GIAO-DIEN.md`](../docs/frontend/THIET-KE-GIAO-DIEN.md).
- Đang ở Giai đoạn 0.1 (UI Shell) — dữ liệu hiển thị lấy từ `lib/mock-data.ts`, chưa nối API thật.
