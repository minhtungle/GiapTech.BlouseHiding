# ADR-0007: Admin (NTD) chuyển sang giao diện chuẩn shadcn/ui trung tính, cùng nhóm với Vận hành

**Trạng thái:** Đã chấp nhận (sửa lại phân nhóm ở [ADR-0004](./0004-tech-stack-net-nextjs.md) và
`THIET-KE-GIAO-DIEN.md` mục 3.1)

## Bối cảnh
Quyết định ban đầu nhóm **Client** và **Admin (NTD)** chung 1 nhóm — cả hai đều dùng đầy đủ bản sắc
"Tin cậy lâm sàng" (serif tiêu đề, mảng màu đỏ triện/xanh ngọc rộng), chỉ riêng **Vận hành** được phép
tham khảo bố cục shadcn-admin vì là công cụ nội bộ ít người dùng. Sau khi dựng mockup thử nghiệm
(`wireframes/trang-chu-3-vai-tro.html`), nhận thấy phong cách branded đầy đủ **không phù hợp với Admin**
— đây là công cụ NTD dùng lặp lại nhiều lần/ngày (đăng tin, ATS, ví Credit), không phải bề mặt tiếp thị
cần gây ấn tượng thương hiệu như trang tìm việc công khai.

## Quyết định
**Admin (NTD) chuyển sang cùng nhóm với Vận hành** — cả hai dùng giao diện dashboard **chuẩn shadcn/ui,
trung tính** (nền/card/sidebar/bảng theo đúng convention mặc định của shadcn — không dùng serif tiêu
đề, không mảng màu trang trí lớn), thay vì bản sắc "Tin cậy lâm sàng" đầy đủ.

Phân nhóm mới:
- **Client**: giữ nguyên toàn bộ bản sắc "Tin cậy lâm sàng" (serif tiêu đề, màu đỏ triện/xanh ngọc dùng
  rộng) — đây là bề mặt tiếp thị/khám phá việc làm, bản sắc thương hiệu vẫn quan trọng nhất ở đây.
- **Admin + Vận hành**: giao diện dashboard chuẩn shadcn/ui, trung tính, tối ưu cho thao tác lặp lại
  nhanh — **không bỏ hoàn toàn màu thương hiệu**: `accent-seal`/`accent-jade`/`amber-pending` vẫn giữ
  nguyên cho **badge trạng thái** (verified/pending/rejected — đã là quy ước xuyên suốt hệ thống, xem
  style-guide.html) và cho **nút CTA chính duy nhất** mỗi màn hình (vd "+ Đăng tin mới") — 1 điểm nhấn
  màu, còn lại trung tính, đúng nguyên tắc "chỉ tô đậm 1 chỗ, phần còn lại giữ yên".

## Phương án đã cân nhắc
- **Giữ Admin branded như Client (quyết định cũ)**: loại bỏ sau khi thấy mockup thực tế — cảm giác
  "trình diễn" không phù hợp với màn hình NTD mở ra hàng chục lần/ngày để xử lý công việc.
- **Bỏ hết màu thương hiệu ở Admin/Vận hành, dùng đúng theme mặc định shadcn (zinc/slate thuần)**:
  loại bỏ — badge trạng thái vẫn cần giữ ý nghĩa màu đã xác lập (đỏ triện = xác thực/CTA, xanh ngọc =
  chuyên môn) để nhất quán với Client, tránh 1 hệ thống có 2 bộ mã màu trạng thái khác nhau.

## Hệ quả
- (+) Admin/Vận hành code nhanh hơn — bám sát pattern/convention mặc định của shadcn/ui, ít phải tùy
  biến layout, cũng là lý do gốc đã chọn tham khảo shadcn-admin cho Vận hành.
- (+) Người dùng NTD/Vận hành có cảm giác "công cụ làm việc" quen thuộc thay vì màn hình trình diễn.
- (+) Badge trạng thái vẫn nhất quán ý nghĩa màu xuyên suốt cả 3 khu vực (chỉ khác mật độ dùng màu ở
  phần khung/chrome xung quanh).
- (−) Sản phẩm có 2 "ngôn ngữ hình ảnh" khác nhau (Client branded vs Admin/Vận hành trung tính) — ranh
  giới phải rõ ràng ở route group (`app/(client)` khác hẳn `app/(admin)`/`app/(ops)`) để không lẫn lộn
  giữa 2 style khi phát triển.
- Cập nhật liên quan: [`../../frontend/THIET-KE-GIAO-DIEN.md`](../../frontend/THIET-KE-GIAO-DIEN.md)
  mục 3, wireframe [`../../frontend/wireframes/trang-chu-3-vai-tro.html`](../../frontend/wireframes/trang-chu-3-vai-tro.html).
