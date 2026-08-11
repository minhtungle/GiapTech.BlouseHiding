# 2026-08-11 (phần 3) — responsive, dark mode, contrast WCAG AA

> Mục lục mọi ngày: [`../TIEN-DO-DU-AN.md`](../TIEN-DO-DU-AN.md)
> Cùng ngày: [phần 1 — chốt Giai đoạn 1](./2026-08-11--chot-giai-doan-1.md) ·
> [phần 2 — bộ lọc tin & ứng viên](./2026-08-11--phan-2-bo-loc-tin-tuyen-luong.md)

---

Mục ⬜ **duy nhất còn lại của Giai đoạn 0**, chưa từng làm thành 1 đợt riêng. Thuộc đúng nhóm "hoàn
thiện UI" người dùng đã chốt.

## Cách rà — đo bằng số, không nhìn bằng mắt

**Contrast**: viết script tính tỷ lệ theo công thức WCAG cho **mọi cặp màu dùng thật**, cả 2 app, cả
sáng lẫn tối (`web-admin/` dùng OKLCH nên phải chuyển đổi sang sRGB trước). Nhìn bằng mắt không phân
biệt được 4.48:1 với 4.5:1, mà một cái đạt chuẩn còn một cái không.

**Responsive**: dùng Playwright quét **12 trang × 3 kích thước** (375 / 768 / 1280), đo 2 thứ đo được
chính xác: trang có tràn ngang không, và có vùng bấm nào nhỏ hơn 24×24 không. Không đếm class
`sm:`/`md:` — "không có breakpoint" không có nghĩa là chưa responsive (form 1 cột vốn đã responsive).

## Lỗi tìm được

**1. Nặng nhất — `web/` mất điều hướng trên điện thoại.** Thanh nav để `hidden md:flex` nên dưới
768px **4 link chính (Tìm việc / Cơ sở y tế / Giới thiệu / Công cụ) biến mất hoàn toàn**, và **không
có menu thay thế**. Người dùng điện thoại chỉ còn logo + nút đăng nhập. Đã thêm
`SiteHeaderMobileNav` (Sheet trượt từ trái), tự đóng sau khi chọn link.

**2. Tràn ngang.** `web/` tràn 383px > 375px trên **mọi trang** (khối nút bên phải header rộng 236px),
`web-admin/` tràn 403px ở `/settings`. Sửa bằng `min-w-0` + `truncate` cho logo + thu gọn `gap`/`p`
ở màn hẹp.

**3. Contrast thiếu chuẩn — cùng 1 lỗi ở cả 2 app**: chữ phụ trên nền chìm chỉ đạt 4.48 (`web/`) và
4.35 (`web-admin/`), dưới ngưỡng AA 4.5. Chọn màu tối hơn **ít nhất có thể** để giữ gần thiết kế gốc:
`#5b6e6c` → `#576968` và `oklch(0.554…)` → `oklch(0.53…)`, cả hai lên 4.81.

**4. Dark mode của `web/` là code chết.** `globals.css` có đủ **44 dòng** định nghĩa màu `.dark`,
`next-themes` đã cài, `ui/sonner.tsx` gọi `useTheme()` — nhưng **không có ThemeProvider nào bọc app**,
nên không cách nào bật. Người dùng chọn làm thật thay vì xoá. Thêm provider + nút đổi ở header.

Đáng chú ý: bộ màu dark viết sẵn đó **đạt AA toàn bộ** ngay từ đầu (9/9 cặp), không phải sửa gì —
người viết đã cân màu cẩn thận, chỉ thiếu bước cuối là kích hoạt.

## Chi tiết kỹ thuật đáng ghi

Nút đổi theme: cách phổ biến trên mạng là `useEffect(() => setMounted(true))` để tránh lệch hydration.
Cách đó **bị ESLint của repo chặn** (`react-hooks/set-state-in-effect`) và thêm 1 lần render mỗi khi
mount. Thay bằng render **cả 2 icon** rồi ẩn/hiện bằng `dark:` của Tailwind — HTML hai bên giống hệt
nhau nên không lệch hydration, CSS áp đúng ngay khi next-themes gắn class.

## Không sửa — có lý do

- Link trong dòng chữ ("Xem tất cả →", "Quên mật khẩu?") cao < 24px: **WCAG 2.5.8 miễn trừ** link
  nằm trong đoạn văn.
- `Router16 items` / `state9 items`: TanStack devtools, đã kiểm `import.meta.env.MODE === 'development'`
  nên không lên production.
- Thanh kéo sidebar 16×1024: hẹp nhưng cao hết màn hình.

## Verify

Quét lại sau khi sửa: **tràn ngang hết sạch** (26 → 16 vấn đề, 16 còn lại đều thuộc nhóm "không sửa"
ở trên). Playwright **15/15**: menu mobile mở/điều hướng/tự đóng, dark mode đổi màu nền thật
(`rgb(242,244,241)` → `rgb(16,25,24)`), giữ sau khi tải lại trang, chữ vẫn sáng trên nền tối. Cả 3
repo build + lint sạch.

> ESLint bắt được lỗi `setState` trong effect ngay lần chạy đầu — đáng ghi vì đây là lỗi tôi định
> viết theo thói quen, chỉ có lint mới chặn lại.

---

## Rà tiếp: dark mode trên TOÀN BỘ màn hình

Đợt trên mới verify trang chủ. Nay quét **26 màn hình × 2 chế độ** (13 trang `web/` gồm cả trang cần
đăng nhập, 13 trang `web-admin/`), đo contrast **thật trên DOM đã render** — không chỉ tính trên bảng
màu, vì bảng màu không biết chỗ nào thực sự đặt chữ gì lên nền gì.

**Lỗi thật tìm được — `text-white` cứng trên badge.** Nền accent ở chế độ tối **sáng hơn hẳn**, nên
chữ trắng mất tương phản:

| Nền | Chữ trắng (sáng) | Chữ trắng (tối) |
|---|---|---|
| `accent-jade` | 5.95 ✅ | **2.72 ❌** |
| `accent-seal` | 5.62 ✅ | **3.47 ❌** |
| `amber-pending` | **3.97 ❌** | **2.12 ❌** |

Badge "Chờ duyệt" **không đạt chuẩn ở CẢ 2 chế độ** — lỗi có sẵn từ trước, chế độ tối chỉ làm lộ ra.

Sửa gốc bằng token `--on-accent` tự đảo theo chế độ (`#ffffff` ↔ `#101918`) thay vì sửa từng chỗ, áp
cho **32 vị trí** (9 ở `web/`, 23 ở `web-admin/`). Kèm tối lại `--amber-pending` chế độ sáng
`#a9761b` → `#9c6c18`.

Sau khi sửa: **0 lỗi** trên 26 màn × 2 chế độ. 26 mục còn báo đều là nhãn "TanStack Router" của
devtools — đã kiểm `import.meta.env.MODE === 'development'` nên không lên production.

> **Bẫy khi viết script đo contrast**: `getComputedStyle` có thể trả `oklab()` với giá trị 0..1;
> parse bằng regex số sẽ ra `rgb(0.99, 0.00004, 0.00002)` và tính contrast **sai hoàn toàn** — lần
> chạy đầu báo **15 lỗi giả** ở chế độ sáng, gồm cả tiêu đề "BlouseHiding" 1.34:1 (thực tế 14:1).
> Cách đúng: vẽ màu lên canvas 1×1 rồi đọc pixel, để trình duyệt tự quy về sRGB.
>
> Đây là lần thứ 3 trong ngày công cụ đo báo sai còn sản phẩm đúng. Vẫn giữ nguyên tắc: kết quả tự
> động mâu thuẫn trực giác thì kiểm chứng bằng nguồn khác trước khi sửa code.
