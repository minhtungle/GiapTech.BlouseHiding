# GiapTech.BlouseHiding — Định hướng thiết kế giao diện

> Trả lời trực tiếp 3 câu hỏi: phong cách thiết kế nào, có dùng thẳng mã nguồn mở không, và thư viện
> nào thuận tiện. Cụ thể hóa phần Frontend của [`TECH-STACK-CHI-TIET.md`](./TECH-STACK-CHI-TIET.md)
> và tiếp nối bảng màu/kiểu chữ đã dùng ở [`wireframes/man-hinh-cot-loi.html`](./wireframes/man-hinh-cot-loi.html).

---

## 1. Phong cách thiết kế: "Tin cậy lâm sàng" (Clinical Trust), không phải "SaaS chung chung"

**Vấn đề cần tránh:** đa số nền tảng tuyển dụng na ná nhau (xanh dương corporate như TopCV, hoặc
màu mè trẻ trung như Ybox). Với ngành y, **niềm tin là tính năng số 1** — giao diện phải truyền tải
được điều đó trước khi người dùng đọc một chữ nào.

**3 nguyên tắc chỉ đạo:**

1. **Rõ ràng hơn là "đẹp" thuần túy.** Đối tượng dùng gồm bác sĩ/điều dưỡng nhiều độ tuổi, không phải
   dân công nghệ trẻ. Contrast cao, cỡ chữ đủ lớn (tối thiểu 15px body), layout không dồn nhiều thao
   tác trên 1 màn hình, tuân thủ WCAG AA.
2. **Bản sắc gắn với "xác thực"**, không phải trang trí ngẫu nhiên. Đã dùng ở wireframe: màu **đỏ triện
   (`seal`)** cho các trạng thái xác thực/CTA quan trọng — gợi liên tưởng con dấu đỏ trên giấy tờ y tế/
   hành chính Việt Nam, và màu **xanh ngọc (`jade`)** trầm cho các yếu tố tin cậy/chuyên môn (chip
   chuyên khoa, trạng thái verified). Đây là 2 màu có **ý nghĩa nghiệp vụ**, không phải chọn ngẫu nhiên.
3. **Không rơi vào khuôn mẫu "AI SaaS"**: tránh gradient tím-xanh, nền kem be + serif display + cam đất
   (tổ hợp đang bị lạm dụng khắp nơi), tránh dùng emoji làm icon chính, tránh bo góc `rounded-lg` cho
   mọi thứ không phân biệt.

**Tiếp tục dùng làm chuẩn chính thức** (đã kiểm chứng ở wireframe, xem style guide trực quan mục 5):
- Nền trung tính hơi ngả xanh-xám lạnh (không phải trắng/kem thuần) — gợi cảm giác "clinical"
- Chữ tiêu đề: serif có tính cách (gợi văn bản chính thức/chứng chỉ) + chữ thân: sans-serif hệ thống, dễ đọc
- Badge trạng thái (verified/pending/rejected) mã hóa **cả màu lẫn hình dạng** (chấm tròn + nhãn chữ),
  không chỉ dựa vào màu — quan trọng vì một số người dùng có thể mù màu một phần.

---

## 2. Font chữ tiếng Việt — điểm dễ bị bỏ sót

| Vai trò | Đề xuất | Lý do |
|---|---|---|
| Chữ thân (UI, form, nội dung) | **Be Vietnam Pro** (mã nguồn mở, SIL OFL) | Do nhà thiết kế người Việt tạo, **tối ưu riêng cho dấu tiếng Việt** — nhiều font quốc tế phổ biến render dấu chồng/lệch dòng khi có nhiều thanh điệu (ệ, ữ, ồ...) |
| Chữ tiêu đề | Một serif có cá tính, dùng bản **Variable Font** (Lora, Source Serif 4, hoặc Fraunces) | Cần **kiểm tra thủ công bộ glyph tiếng Việt đầy đủ** trước khi chốt (không phải serif nào cũng phủ hết dấu) |
| Số liệu/mã (CCHN, tiền, ngày) | Font mono hệ thống, bật `font-variant-numeric: tabular-nums` | Căn cột số ngay ngắn trong bảng ATS/dashboard |

**Tự host font** (tải file .woff2 về server của mình, không gọi Google Fonts runtime lúc production):
nhanh hơn, không để trình duyệt người dùng gọi ra bên thứ ba mỗi lần tải trang — hợp với tinh thần
bảo vệ dữ liệu cá nhân đã nêu ở phương án thực hiện.

---

## 3. Có dùng thẳng mã nguồn mở không? — Có, nhưng phân biệt rõ 2 khu vực

### 3.1 Khu vực công khai & nghiệp vụ chính (Candidate / Employer-facing)
→ **Không lấy nguyên theme/template có sẵn.** Một template tải về (kiểu ThemeForest hay một Next.js
SaaS starter) sẽ khiến trang trông giống hàng loạt sản phẩm khác, phá vỡ đúng thứ đang cố xây (niềm
tin qua bản sắc riêng). Thay vào đó:

- Dùng **shadcn/ui** — về bản chất đây *là* mã nguồn mở, nhưng ở dạng **copy code vào repo bạn sở hữu**,
  không phải package đóng gói cài qua npm rồi bị khóa theo phiên bản của người khác. Nền tảng bên dưới
  (Radix UI primitives) đã được kiểm chứng về accessibility (focus trap, ARIA, keyboard nav) — thứ khó
  và tốn thời gian nhất nếu tự viết từ đầu — trong khi phần giao diện (màu/bo góc/khoảng cách) hoàn
  toàn tùy biến theo token ở mục 5, không bị ép theo "trông giống shadcn mặc định".

### 3.2 Khu vực quản trị nội bộ (Admin/Moderator)
→ **Nên dùng thẳng một admin dashboard mã nguồn mở** làm nền, vì khu vực này:
- Rất ít người dùng (chỉ nội bộ), không cần truyền tải bản sắc thương hiệu
- Ưu tiên tốc độ ra màn hình hơn là thiết kế riêng
- Đề xuất: **shadcn-admin** (dự án cộng đồng, MIT, xây sẵn trên đúng shadcn/ui + Tailwind) — vì dùng
  chung hệ component với khu vực chính, không phải học/维护 2 hệ thống UI song song.

**Tóm lại — trả lời thẳng câu hỏi:** dùng mã nguồn mở ở tầng **nền tảng/hạ tầng component** (shadcn/ui,
Radix, icon, font) chứ không dùng ở tầng **giao diện cuối cùng** cho khu vực đối ngoại. Ngược lại, khu
vực nội bộ (Admin) có thể dùng thẳng cả giao diện có sẵn vì không ảnh hưởng thương hiệu.

---

## 4. Thư viện cụ thể theo từng nhu cầu (thuận tiện nhất cho stack đã chọn)

| Nhu cầu | Thư viện | Vì sao thuận tiện |
|---|---|---|
| Component nền + theming | **shadcn/ui** (Radix + Tailwind) | Copy-code, tùy biến 100%, theming bằng CSS variable — khớp thẳng với token ở mục 5 |
| Icon | **Lucide** | Đi kèm mặc định với shadcn/ui, nét vẽ nhất quán, mã nguồn mở, đủ icon y tế cơ bản (stethoscope, syringe, hospital...) |
| Minh họa (empty state, onboarding) | **unDraw** | Mã nguồn mở, **tự đổi màu SVG theo bảng màu riêng** (khác hẳn ảnh stock chung chung) |
| Admin dashboard nền | **shadcn-admin** | Xây trên shadcn/ui — đồng bộ component, không phải học hệ UI thứ 2 |
| Kanban kéo-thả (ATS) | **dnd-kit** | Nhẹ, accessible, đã chọn ở tech stack |
| Rich text (mô tả tin, bài viết) | **Tiptap** | Headless, style theo token riêng, không mang theo CSS mặc định xấu |
| Biểu đồ dashboard | **Tremor** (dựng trên Recharts) | Component biểu đồ + KPI card sẵn, phối màu theo CSS variable — khớp nhanh với theme |
| Form | **React Hook Form + Zod** | Đã chọn ở tech stack — validate đồng nhất với backend |
| Bảng dữ liệu (danh sách ứng viên, tin, người dùng ở Admin) | **TanStack Table** (headless) | Không mang UI mặc định, tự style theo shadcn/ui table component |

---

## 5. Hệ thống token thiết kế (formal hóa từ wireframe)

Bảng màu và type đã dùng ở `wireframes/man-hinh-cot-loi.html` trở thành **token chính thức** — ánh xạ
thẳng sang biến CSS mà shadcn/ui dùng để theming (`--background`, `--foreground`, `--primary`...), nên
khi scaffold dự án thật, chỉ cần copy giá trị, không phải thiết kế lại từ đầu.

```css
/* Light */
--paper: #f2f4f1;          /* nền trang — trung tính lạnh, không phải trắng/kem thuần */
--paper-raised: #ffffff;   /* nền card/surface nổi */
--ink: #16262a;            /* chữ chính */
--ink-muted: #5b6e6c;      /* chữ phụ */
--accent-seal: #ba3b21;    /* đỏ triện — CTA chính, badge "verified/rejected" */
--accent-jade: #2f6e62;    /* xanh ngọc trầm — chuyên khoa, liên kết, badge phụ */
--amber-pending: #a9761b;  /* vàng hổ phách — trạng thái "pending" */
--line: #d9ded9;           /* viền/hairline */

/* Dark — không đảo ngược máy móc, giữ độ tương phản và vai trò từng màu */
--paper: #101918;
--paper-raised: #16211f;
--ink: #e8ede9;
--ink-muted: #93a6a2;
--accent-seal: #e1633f;    /* sáng hơn để đủ tương phản trên nền tối */
--accent-jade: #59ab99;
--amber-pending: #d9ab52;
--line: #263230;
```

**Type scale:** display serif (tiêu đề) dùng tiết chế — chỉ H1/H2 trang chính, không lạm dụng; phần
còn lại (H3 trở xuống, toàn bộ UI/form) dùng Be Vietnam Pro để tối ưu tốc độ đọc & dấu tiếng Việt.

---

## 6. Bước tiếp theo

Đã dựng thêm [`wireframes/style-guide.html`](./wireframes/style-guide.html) — trang tham chiếu trực
quan (color swatch, type scale, button/badge/card states, cả 2 theme) để soi khi code component thật,
tránh mỗi màn hình tự suy diễn lại giá trị. Artifact xem trực tiếp trong hội thoại.
