# ADR-0004: Backend .NET + Frontend Next.js/shadcn-ui, không lấy nguyên template SaaS mã nguồn mở

**Trạng thái:** Đã chấp nhận

## Bối cảnh
Cần chọn stack backend/frontend và quyết định mức độ dùng lại mã nguồn mở có sẵn thay vì tự viết từ
đầu, đồng thời tránh mất bản sắc thiết kế riêng ("Tin cậy lâm sàng" — xem
[`../../frontend/THIET-KE-GIAO-DIEN.md`](../../frontend/THIET-KE-GIAO-DIEN.md)).

## Quyết định
1. **Backend**: ASP.NET Core (.NET 10), khởi tạo từ **Jason Taylor's Clean Architecture Template**
   (MIT license) thay vì tự dựng khung Clean Architecture từ đầu.
2. **Frontend**: Next.js (App Router) scaffold tối giản (`create-next-app`) + **shadcn/ui** CLI —
   **không** dùng một "Next.js SaaS Starter" đóng gói sẵn.
3. **Trang Vận hành** (đội nội bộ nền tảng — xem [ADR-0005](./0005-dat-ten-3-khu-vuc-site.md)): cùng
   1 codebase Next.js (route group), tham khảo bố cục từ **shadcn-admin** (mã nguồn mở) nhưng không
   chạy nó như 1 app riêng.

## Phương án đã cân nhắc
- **Ardalis Clean Architecture Template**: cũng tốt nhưng kèm thư viện riêng của tác giả
  (Ardalis.Specification, Ardalis.Result) lệch khỏi stack MediatR/FluentValidation thuần đã chọn —
  không chọn để giảm số lượng quy ước phải học.
- **Next.js SaaS Starter mã nguồn mở** (đa số bundle Prisma + Next API routes làm backend riêng): loại
  bỏ vì xung đột với việc đã có backend .NET riêng — dùng sẽ phải gỡ bỏ nhiều hơn là tận dụng.
- **shadcn-admin chạy như app riêng cho trang Vận hành** (Vite, tách biệt): loại bỏ vì tạo 2 hệ sinh
  thái frontend, 2 lần cấu hình bảng màu/theme, thêm 1 domain/container/pipeline — đi ngược tiêu chí
  "dễ kiểm soát, tránh lộn xộn, dễ bàn giao".

## Hệ quả
- (+) Tận dụng khung Clean Architecture đã kiểm chứng rộng rãi, giảm thời gian dựng khung ban đầu.
- (+) Frontend 1 codebase duy nhất, 1 bộ thiết kế nhất quán cho cả Client/Admin (NTD)/Vận hành.
- (−) Không có sẵn màn hình Vận hành đầy đủ ngay lập tức như khi dùng thẳng shadcn-admin — phải tự
  build lại theo tham khảo, tốn công hơn một chút ở giai đoạn đầu.
- Thư viện cụ thể: [`../../backend/CONG-NGHE-BACKEND.md`](../../backend/CONG-NGHE-BACKEND.md),
  [`../../frontend/CONG-NGHE-FRONTEND.md`](../../frontend/CONG-NGHE-FRONTEND.md).
