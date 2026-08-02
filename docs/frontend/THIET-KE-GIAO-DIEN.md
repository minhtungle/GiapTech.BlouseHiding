# GiapTech.BlouseHiding — Định hướng thiết kế giao diện

> Trả lời trực tiếp 3 câu hỏi: phong cách thiết kế nào, có dùng thẳng mã nguồn mở không, và thư viện
> nào thuận tiện. Cụ thể hóa phần Frontend của [`CONG-NGHE-FRONTEND.md`](./CONG-NGHE-FRONTEND.md)
> và tiếp nối bảng màu/kiểu chữ đã dùng ở [`wireframes/man-hinh-cot-loi.html`](./wireframes/man-hinh-cot-loi.html).

---

## 1. Phong cách thiết kế: "Tin cậy lâm sàng" cho Client, chuẩn shadcn/ui trung tính cho Admin/Vận hành

> ⚠️ Đã điều chỉnh (xem [ADR-0007](../kien-truc/adr/0007-shadcn-chuan-cho-admin-van-hanh.md)): bản sắc
> "Tin cậy lâm sàng" đầy đủ ở mục này **chỉ áp dụng cho khu vực Client** (trang công khai + ứng viên).
> **Admin (NTD) và Vận hành** dùng giao diện dashboard **chuẩn shadcn/ui trung tính** — xem mục 3.2.
> Lý do: Client là bề mặt tiếp thị/khám phá việc làm (bản sắc quan trọng), còn Admin/Vận hành là công
> cụ làm việc lặp lại nhiều lần/ngày (quen thuộc & tốc độ thao tác quan trọng hơn bản sắc).

**Vấn đề cần tránh (khu vực Client):** đa số nền tảng tuyển dụng na ná nhau (xanh dương corporate như
TopCV, hoặc màu mè trẻ trung như Ybox). Với ngành y, **niềm tin là tính năng số 1** — giao diện phải
truyền tải được điều đó trước khi người dùng đọc một chữ nào.

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

## 3. Có dùng thẳng mã nguồn mở không? — Có, nhưng phân biệt rõ 2 nhóm khu vực

> ⚠️ Phân nhóm đã cập nhật theo [ADR-0007](../kien-truc/adr/0007-shadcn-chuan-cho-admin-van-hanh.md):
> **Client** dùng bản sắc branded đầy đủ; **Admin (NTD) + Vận hành** dùng chuẩn shadcn/ui trung tính
> — không còn nhóm Admin chung với Client như quyết định ban đầu.

### 3.1 Khu vực Client — trang công khai + ứng viên (bề mặt tiếp thị/khám phá việc làm)
→ **Không lấy nguyên theme/template có sẵn.** Một template tải về (kiểu ThemeForest hay một Next.js
SaaS starter) sẽ khiến trang trông giống hàng loạt sản phẩm khác, phá vỡ đúng thứ đang cố xây (niềm
tin qua bản sắc riêng). Thay vào đó:

- Dùng **shadcn/ui** — về bản chất đây *là* mã nguồn mở, nhưng ở dạng **copy code vào repo bạn sở hữu**,
  không phải package đóng gói cài qua npm rồi bị khóa theo phiên bản của người khác. Nền tảng bên dưới
  (Radix UI primitives) đã được kiểm chứng về accessibility (focus trap, ARIA, keyboard nav) — thứ khó
  và tốn thời gian nhất nếu tự viết từ đầu — trong khi phần giao diện (màu/bo góc/khoảng cách) hoàn
  toàn tùy biến theo token ở mục 5 (bản sắc "Tin cậy lâm sàng"), không bị ép theo "trông giống shadcn
  mặc định".

### 3.2 Khu vực Admin (NTD) + Vận hành — công cụ làm việc nội bộ, dùng lặp lại nhiều lần/ngày
> ⚠️ Quyết định cuối (xem [ADR-0007](../kien-truc/adr/0007-shadcn-chuan-cho-admin-van-hanh.md) cho màu
> sắc/phong cách, [ADR-0008](../kien-truc/adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md) cho
> khung kỹ thuật): **1 app riêng biệt (`web-admin/`), chạy thẳng shadcn-admin** (Vite + React Router) —
> **không** còn "chỉ tham khảo bố cục rồi tự dựng lại trong Next.js" như quyết định trước đó. shadcn-admin
> được cài đặt trực tiếp làm nền tảng, tùy biến theo token màu ở mục 5. App này tách hẳn khỏi `web/`
> (Client, Next.js) — dùng chung 1 instance cho cả Admin và Vận hành, phân biệt màn hình theo role.

Cả **Admin** và **Vận hành** giờ dùng chung 1 kiểu: giao diện dashboard **chuẩn shadcn/ui, trung
tính** — không dùng serif tiêu đề, không mảng màu trang trí rộng, sidebar/card/bảng theo đúng convention
mặc định của shadcn (nền trắng/xám nhạt, viền hairline, active state là pill xám nhạt chứ không phải
viền màu thương hiệu). Lý do:
- Đây là công cụ NTD/Vận hành mở ra hàng chục lần mỗi ngày để xử lý công việc (đăng tin, ATS, duyệt
  hồ sơ) — quen thuộc và tốc độ thao tác quan trọng hơn gây ấn tượng thương hiệu.
- Dùng thẳng **shadcn-admin** (dự án cộng đồng, MIT, xây sẵn trên shadcn/ui + Tailwind) làm nền tảng
  app thật — không chỉ tham khảo — để đi nhanh hơn đáng kể, tận dụng pattern sidebar/bảng/dialog có sẵn.

**Không bỏ hoàn toàn màu thương hiệu** — `accent-seal`/`accent-jade`/`amber-pending` vẫn giữ nguyên
cho 2 việc duy nhất: **badge trạng thái** (verified/pending/rejected — ý nghĩa xuyên suốt cả 3 khu
vực, không đổi theo từng nơi) và **nút CTA chính duy nhất mỗi màn hình** (vd "+ Đăng tin mới"). Đây là
điểm nhấn màu duy nhất được phép, còn lại khung/chrome xung quanh giữ trung tính hoàn toàn.

**Tóm lại — trả lời thẳng câu hỏi:** dùng mã nguồn mở ở tầng **nền tảng/hạ tầng component** (shadcn/ui,
Radix, icon, font) cho cả 3 khu vực; **Client** tùy biến theo bản sắc riêng, **Admin/Vận hành** bám sát
quy ước hình ảnh mặc định của shadcn/ui — khác nhau ở mức độ tùy biến, không phải khác nhau ở việc có
dùng shadcn/ui hay không.

---

## 4. Thư viện cụ thể theo từng nhu cầu (thuận tiện nhất cho stack đã chọn)

| Nhu cầu | Thư viện | Vì sao thuận tiện |
|---|---|---|
| Component nền + theming | **shadcn/ui** (Radix + Tailwind) | Copy-code, tùy biến 100%, theming bằng CSS variable — khớp thẳng với token ở mục 5 |
| Icon | **Lucide** | Đi kèm mặc định với shadcn/ui, nét vẽ nhất quán, mã nguồn mở, đủ icon y tế cơ bản (stethoscope, syringe, hospital...) |
| Minh họa (empty state, onboarding) | **unDraw** | Mã nguồn mở, **tự đổi màu SVG theo bảng màu riêng** (khác hẳn ảnh stock chung chung) |
| Nền tảng app Admin + Vận hành (`web-admin/`) | **shadcn-admin** (cài đặt trực tiếp làm nền tảng app, Vite + React Router) | Xây trên shadcn/ui — bố cục sidebar/bảng/KPI card có sẵn, tùy biến theo token màu ở mục 5 thay vì tự dựng lại (xem [ADR-0008](../kien-truc/adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md)) |
| Kanban kéo-thả (ATS) | **dnd-kit** | Nhẹ, accessible, đã chọn ở tech stack |
| Rich text (mô tả tin, bài viết) | **Tiptap** | Headless, style theo token riêng, không mang theo CSS mặc định xấu |
| Biểu đồ dashboard | **Tremor** (dựng trên Recharts) | Component biểu đồ + KPI card sẵn, phối màu theo CSS variable — khớp nhanh với theme |
| Form | **React Hook Form + Zod** | Đã chọn ở tech stack — validate đồng nhất với backend |
| Bảng dữ liệu (danh sách ứng viên, tin, người dùng ở trang Admin/Vận hành) | **TanStack Table** (headless) | Không mang UI mặc định, tự style theo shadcn/ui table component |

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
