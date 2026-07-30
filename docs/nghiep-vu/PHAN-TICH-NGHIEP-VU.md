# GiapTech.BlouseHiding — Phân tích nghiệp vụ

> Nền tảng tuyển dụng **chuyên biệt cho ngành y tế** (Healthcare Recruitment Platform),
> tham khảo mô hình của các nền tảng phổ biến như **TopCV** (CV/việc làm) và **Ybox** (sự kiện/cơ hội).
>
> Tài liệu này tập trung **nghiệp vụ**: mục tiêu, actor, phạm vi tính năng, đối chiếu thị trường,
> roadmap, rủi ro. Kiến trúc & công nghệ xem [`../kien-truc/TONG-QUAN-KIEN-TRUC.md`](../kien-truc/TONG-QUAN-KIEN-TRUC.md).
> Luồng nghiệp vụ chi tiết + màn hình xem [`LUONG-NGHIEP-VU-MAN-HINH.md`](./LUONG-NGHIEP-VU-MAN-HINH.md).
> Danh sách tính năng dạng checklist xem [`DANH-SACH-TINH-NANG.md`](./DANH-SACH-TINH-NANG.md).

---

## 1. Tổng quan & mục tiêu

**Vấn đề:** Các nền tảng tuyển dụng đại trà (TopCV, VietnamWorks…) không phù hợp với đặc thù ngành y:
không xác thực **chứng chỉ hành nghề**, không phân loại theo **chuyên khoa**, không hiểu các loại hình
làm việc đặc thù (trực ca, locum, cộng tác viên khám bệnh…), và dữ liệu ứng viên y tế rất nhạy cảm.

**Mục tiêu sản phẩm:**
- Kết nối **cơ sở y tế** (bệnh viện, phòng khám, nhà thuốc, công ty dược, phòng lab…) với
  **nhân sự y tế** (bác sĩ, điều dưỡng, dược sĩ, kỹ thuật viên, hộ lý, nữ hộ sinh…).
- Xác thực năng lực hành nghề để tăng độ tin cậy.
- Đăng tin – tìm kiếm – ứng tuyển – quản lý tuyển dụng (ATS) theo đặc thù ngành y.
- Bổ sung lớp "cơ hội phát triển nghề nghiệp" kiểu Ybox: hội thảo/CME, khóa đào tạo, học bổng.

**KPI khởi điểm (gợi ý):** số hồ sơ đã xác thực, số tin tuyển dụng, tỷ lệ ứng tuyển → phỏng vấn,
thời gian tuyển trung bình (time-to-hire).

---

## 2. Đối tượng người dùng (Actors) & khu vực site

> **Quy ước đặt tên 3 khu vực site** (áp dụng xuyên suốt mọi tài liệu, xem thêm
> [`../kien-truc/THUAT-NGU.md`](../kien-truc/THUAT-NGU.md)):
> - **Client** — trang cho Ứng viên & Khách (công khai).
> - **Admin** — trang quản trị dành cho **Nhà tuyển dụng** (đăng tin, quản lý CV/ATS, mua gói, credit).
> - **Vận hành** — trang nội bộ đội ngũ vận hành nền tảng (không phải NTD): duyệt CCHN, duyệt doanh
>   nghiệp, duyệt tin, xử lý báo cáo.

| Actor | Mô tả | Nhu cầu chính | Dùng khu vực site |
|------|-------|----------------|---|
| **Ứng viên** | Nhân sự y tế đang/muốn tìm việc | Tạo hồ sơ, xác thực chứng chỉ, tìm & ứng tuyển, theo dõi trạng thái | Client |
| **Nhà tuyển dụng (NTD)** | Cơ sở y tế / HR / trưởng khoa | Đăng tin, sàng lọc, quản lý pipeline ứng viên, hẹn phỏng vấn | **Admin** |
| **Nhân viên vận hành** | Đội ngũ nội bộ vận hành nền tảng (role backend: `admin`) | Kiểm duyệt tin & doanh nghiệp, xác thực chứng chỉ, xử lý báo cáo | **Vận hành** |
| **Kiểm duyệt viên** | Đội duyệt nội dung (role backend: `moderator`) | Duyệt tin tuyển dụng, duyệt hồ sơ doanh nghiệp | **Vận hành** |
| **Khách (Guest)** | Chưa đăng nhập | Xem tin, tìm kiếm cơ bản | Client |

---

## 3. Phạm vi nghiệp vụ (Feature domains)

### 3.1 Tài khoản & định danh
- Đăng ký/đăng nhập (email, SĐT, OAuth Google/Zalo), OTP, quên mật khẩu.
- Phân quyền theo vai trò (RBAC, role backend): `candidate` / `employer` / `admin` (đội Vận hành) / `moderator`.
- Đa hồ sơ NTD dưới một tổ chức (nhiều HR cùng quản lý 1 bệnh viện).

### 3.2 Hồ sơ ứng viên (CV) — **đặc thù y tế**
- Trường chuẩn: học vấn, kinh nghiệm, kỹ năng, ngoại ngữ, mục tiêu nghề nghiệp.
- Trường đặc thù ngành y **(điểm khác biệt cốt lõi)**:
  - **Chứng chỉ hành nghề** (số CCHN, nơi cấp, phạm vi hoạt động chuyên môn, ngày cấp/hết hạn).
  - **Chuyên khoa / chuyên ngành** (Nội, Ngoại, Sản, Nhi, Gây mê hồi sức, Dược lâm sàng…).
  - **Trình độ** (BS đa khoa, BS chuyên khoa I/II, Thạc sĩ, Tiến sĩ, ĐD cao đẳng/đại học…).
  - **Kinh nghiệm lâm sàng** theo tuyến (TW, tỉnh, huyện, phòng khám tư).
  - **CME / chứng chỉ đào tạo liên tục**.
- CV Builder (mẫu dựng sẵn) + upload PDF; xuất PDF.

### 3.3 Doanh nghiệp / cơ sở y tế
- Hồ sơ tổ chức: loại hình (BV công/tư, phòng khám, nhà thuốc, cty dược…), quy mô, giấy phép hoạt động.
- Xác minh doanh nghiệp (upload giấy phép → đội Vận hành duyệt).
- Trang thương hiệu tuyển dụng (employer branding).

### 3.4 Tin tuyển dụng
- Đăng tin với các trường: vị trí, chuyên khoa, loại hình (full-time/part-time/**trực ca**/**locum**/CTV),
  mức lương, địa điểm, yêu cầu CCHN, kinh nghiệm, phúc lợi.
- Quy trình duyệt tin (draft → pending → published → expired/closed).
- **Gói đăng tin theo tier** (Eco/Pro/Max, tính theo thời gian hiển thị) — mô hình tham khảo TopCV,
  đưa vào **ngay từ MVP** vì là nguồn doanh thu chính (xem mục 4.1).
- Đăng tin miễn phí có giới hạn cho cơ sở y tế mới — chiến lược mồi tham khảo Ybox (xem mục 7 — rủi ro).

### 3.5 Tìm kiếm & gợi ý
- Tìm kiếm full-text + bộ lọc (chuyên khoa, địa điểm, lương, loại hình, tuyến).
- Gợi ý việc làm theo hồ sơ (recommendation), gợi ý ứng viên cho NTD (matching).

### 3.6 Ứng tuyển & ATS (Applicant Tracking System)
- Ứng tuyển bằng CV nền tảng hoặc CV upload.
- Pipeline trạng thái: Mới → Đang xem → Phù hợp → Hẹn PV → Offer → Trúng tuyển / Từ chối.
- NTD ghi chú, đánh giá, gắn nhãn ứng viên.
- **Hệ thống Credit** — NTD trả điểm để chủ động "mở"/liên hệ hồ sơ ứng viên phù hợp thay vì
  chỉ chờ ứng tuyển (tham khảo TopCV) — quan trọng với ngành y vì nhân sự giỏi thường bị động,
  ít tự nộp đơn, đưa vào **ngay từ MVP**.
- **Chấm điểm hồ sơ (Profile/CV Scoring)** theo tiêu chí y khoa: CCHN hợp lệ & còn hạn, khớp
  chuyên khoa, kinh nghiệm lâm sàng theo tuyến — hỗ trợ NTD sàng lọc nhanh.

### 3.7 Tương tác & thông báo
- Nhắn tin NTD ↔ ứng viên (chat).
- Thông báo real-time + email/Zalo/SMS (tin mới phù hợp, thay đổi trạng thái ứng tuyển).

### 3.8 Lớp "Ybox" — Cơ hội & phát triển nghề nghiệp
- Sự kiện/hội thảo y khoa, khóa CME, workshop, học bổng, cuộc thi chuyên môn.
- Đăng ký tham dự, nhắc lịch.
- Chuyên mục nội dung dạng kênh thông tin (tin tức ngành y, kỹ năng nghề nghiệp) để thu hút
  organic traffic — tham khảo cấu trúc chuyên mục của Ybox (`tin-tuc`/`ky-nang`/`su-kien`).

### 3.9 Công cụ tiện ích cho ứng viên (tham khảo TopCV)
- Tính phụ cấp trực đêm/độc hại/thâm niên nghề y, thuế TNCN, BHXH — điểm khác biệt hoá & SEO tốt.
- Test đánh giá năng lực chuyên môn theo vị trí (dược lâm sàng, điều dưỡng, cấp cứu…) — Giai đoạn 2.
- Đánh giá cơ sở y tế (review) — cần kiểm duyệt chặt do tính nhạy cảm ngành y.

### 3.10 Quản trị & vận hành
- Kiểm duyệt tin & doanh nghiệp, xác thực CCHN, xử lý report/spam.
- Dashboard số liệu, quản lý người dùng, cấu hình danh mục (chuyên khoa, tuyến, địa điểm).

### 3.11 Đa ngôn ngữ
- Giao diện **6 ngôn ngữ**: Tiếng Việt (mặc định), Tiếng Anh, Tiếng Nhật, Tiếng Trung, Tiếng Hàn, Tiếng
  Tây Ban Nha — phục vụ ứng viên/tổ chức nước ngoài (BV vốn đầu tư nước ngoài, NGO y tế, chuyên gia
  expat, đặc biệt từ Nhật/Hàn/Trung — các quốc gia đầu tư y tế/dược lớn tại VN) bên cạnh người dùng
  trong nước là chủ đạo.
- **Nhận diện ngôn ngữ qua tiền tố URL** (`/vi/...`, `/en/...`...), tự động gợi ý theo ngôn ngữ trình
  duyệt khi truy cập lần đầu, có **nút chọn ngôn ngữ** trên giao diện (header, kiểu bố trí phổ biến ở
  các trang lớn) để người dùng tự đổi bất kỳ lúc nào.
- **Chỉ dịch giao diện + danh mục chuẩn** (chuyên khoa, địa điểm, loại hình làm việc, tên gói dịch vụ).
  **Không** dịch nội dung tự viết (mô tả tin, tiểu sử ứng viên) — hiển thị nguyên văn ngôn ngữ tác giả
  đã nhập, không dịch máy tự động (rủi ro sai thuật ngữ y khoa). Quyết định & lý do đầy đủ:
  [ADR-0006](../kien-truc/adr/0006-da-ngon-ngu.md).

---

## 4. Phân tích đối chiếu TopCV & Ybox

### 4.1 TopCV — marketplace 2 chiều (ứng viên ↔ nhà tuyển dụng)

**Ứng viên:** tìm việc + gợi ý cá nhân hóa, việc làm gần bạn, công ty nổi bật; **CV Builder** (50+ mẫu);
**CV Scoring** (chấm điểm/sàng lọc); test năng lực & trắc nghiệm tính cách (MBTI); **Top Connect**
(chat trực tiếp NTD); công cụ tính lương Gross-Net/thuế TNCN/BHXH; đánh giá công ty; blog kỹ năng.

**Nhà tuyển dụng:** đăng tin theo **gói tier trả phí theo thời gian** (Eco/Pro/Max, ~2 tuần/gói — vị trí
hiển thị khác nhau); bộ lọc CV theo vị trí/kỹ năng/bằng cấp; ATS cơ bản (pipeline, phát hiện trùng lặp);
**hệ thống Credit** để chủ động "mở" hồ sơ ứng viên và liên hệ (không chỉ chờ ứng tuyển); tích hợp
TestCenter.vn; báo cáo xem CV/gửi offer.

→ **Doanh thu chính**: gói đăng tin theo tier + credit mở CV.

### 4.2 Ybox — cộng đồng/kênh thông tin (không phải ATS chuyên sâu)

Tổ chức nội dung theo chuyên mục rõ ràng: `tuyen-dung` (việc làm/CTV/thực tập), `ky-nang` (bài viết),
`tin-tuc`, `su-kien`. Đăng tin **miễn phí** là chủ đạo để thu hút tổ chức/CLB mồi ban đầu → hiệu ứng
mạng lưới. Tăng trưởng qua mô hình **cộng tác viên (CTV)** — cộng đồng tự sản xuất/lan truyền nội dung.
Doanh thu từ bảo trợ truyền thông trả phí, không phải từ CV Builder/ATS.

→ Ybox mạnh về **community + content**, yếu về **công cụ tuyển dụng chuyên sâu**.

### 4.3 Áp dụng vào thiết kế (đã đưa vào các mục liên quan ở trên)

| Học từ | Áp dụng cho ngành y | Vào mục |
|---|---|---|
| CV Scoring | Chấm điểm hồ sơ theo CCHN + chuyên khoa + kinh nghiệm lâm sàng | 3.6 |
| Credit mở CV | NTD chủ động tìm & liên hệ ứng viên (nhân sự y tế khan hiếm, thụ động) | 3.6 |
| Gói đăng tin theo tier | Eco/Pro/Max cho cơ sở y tế — doanh thu chính, đưa vào MVP | 3.4 |
| Công cụ tiện ích | Tính phụ cấp trực/độc hại/thâm niên nghề y, thuế TNCN | 3.9 |
| Đánh giá công ty | Đánh giá cơ sở y tế — kiểm duyệt chặt do nhạy cảm ngành | 3.9 |
| Chuyên mục nội dung | "Góc nghề y": CME, hội thảo, học bổng, cuộc thi | 3.8, Giai đoạn 3 |
| Đăng tin miễn phí mồi | Giải bài toán "con gà–quả trứng" cho cơ sở y tế mới | 3.4, mục 7 (rủi ro) |
| Mô hình CTV/cộng đồng | Cộng đồng sinh viên y khoa/hội chuyên khoa — không phải core MVP | Giai đoạn 3 |

**Khác biệt cố ý (không copy):** TopCV/Ybox không cần xác thực năng lực hành nghề — đây vẫn là **rào cản
tin cậy phải giữ làm lõi** của hệ thống này, không đánh đổi lấy tốc độ ra mắt như hai nền tảng trên.

---

## 5. Đặc thù ngành y tế cần lưu ý (khác biệt so với TopCV/Ybox)

1. **Xác thực chứng chỉ hành nghề** là tính năng lõi tạo niềm tin — cần quy trình duyệt thủ công (giai đoạn đầu),
   hướng tới tích hợp/đối soát dữ liệu ngành khi khả thi.
2. **Phân loại chuyên khoa chuẩn hóa** — dùng danh mục (taxonomy) thống nhất để tìm kiếm/khớp lệnh chính xác.
3. **Loại hình làm việc đặc thù**: trực ca, locum (làm thay theo buổi), cộng tác viên khám bệnh.
4. **Bảo mật dữ liệu cá nhân nhạy cảm** — tuân thủ **Nghị định 13/2023/NĐ-CP** về bảo vệ dữ liệu cá nhân:
   mã hóa, phân quyền chặt, audit log, cơ chế đồng ý (consent) và xóa dữ liệu.
5. **Tuân thủ quảng cáo/tuyển dụng ngành y** theo quy định Bộ Y tế.

---

## 6. Lộ trình triển khai (Roadmap)

### Giai đoạn 0 — Khởi tạo (1–2 tuần)
- Khởi tạo solution (Clean Architecture), CI/CD, môi trường dev/staging, Docker Compose.
- Thiết kế schema & danh mục chuyên khoa/tuyến chuẩn.

### Giai đoạn 1 — MVP (8–10 tuần, Web) — *đủ để chạy thử thị trường*
- Identity + RBAC (Candidate/Employer/Vận hành).
- Hồ sơ ứng viên (gồm CCHN & chuyên khoa) + xác thực CCHN thủ công qua đội Vận hành.
- Hồ sơ & xác minh doanh nghiệp.
- Đăng tin + kiểm duyệt + đăng tin công khai + **gói đăng tin theo tier (Eco/Pro/Max)**.
- Tìm kiếm & lọc (giai đoạn đầu dùng Postgres full-text).
- Ứng tuyển + ATS pipeline cơ bản + chấm điểm hồ sơ (CV Scoring).
- **Hệ thống Credit** để NTD chủ động mở hồ sơ ứng viên.
- Thông báo email + trong ứng dụng.

> MVP giữ **đầy đủ hạng mục** theo quyết định đã chốt — không cắt bớt; monetization (gói tin + credit)
> được đưa vào ngay từ MVP thay vì để Giai đoạn 2, vì đây là nguồn doanh thu chính (xem mục 4.1, 4.3).
> **Thanh toán tự động (cổng VNPay/Momo/...) chưa chốt** — MVP xử lý gói tin/credit bằng quy trình
> thủ công (chuyển khoản + đội Vận hành đối soát), không chặn tiến độ; schema/API giữ nguyên để cắm cổng tự
> động sau mà không phải đổi model (xem [ADR-0003](../kien-truc/adr/0003-hoan-cong-thanh-toan-tu-dong.md)).

### Giai đoạn 2 — Hoàn thiện
- OpenSearch cho tìm kiếm nâng cao.
- Gợi ý việc / matching ứng viên.
- Chat realtime (SignalR).
- Mobile app (Flutter/React Native).
- Test đánh giá năng lực theo vị trí, công cụ tính phụ cấp/thuế, đánh giá cơ sở y tế.

### Giai đoạn 3 — Mở rộng
- Lớp sự kiện/CME kiểu Ybox + chuyên mục nội dung (tin tức/kỹ năng nghề y).
- Cộng đồng sinh viên y khoa/hội chuyên khoa (mô hình CTV tham khảo Ybox).
- Phân tích/BI, employer branding, tối ưu matching bằng ML.
- Tích hợp/đối soát xác thực CCHN theo dữ liệu ngành (khi khả thi).

---

## 7. Rủi ro & giải pháp

| Rủi ro | Giải pháp |
|--------|-----------|
| Xác thực CCHN khó tự động | Bắt đầu duyệt thủ công + hàng đợi kiểm duyệt; tự động hóa dần |
| Dữ liệu cá nhân nhạy cảm | Mã hóa, RBAC chặt, audit log, tuân thủ NĐ 13/2023 |
| "Con gà–quả trứng" (thiếu cả tin & ứng viên) | Đăng tin miễn phí có giới hạn cho cơ sở y tế mới (tham khảo Ybox), seed dữ liệu, ưu tiên 1 chuyên khoa/khu vực trước |
| Tìm kiếm không chính xác do dữ liệu tự do | Chuẩn hóa danh mục chuyên khoa/tuyến ngay từ đầu |
| Over-engineering sớm | Modular monolith trước, chỉ tách service khi thực sự cần |

---

## 8. Xem thêm

- Quyết định kiến trúc/công nghệ & trạng thái: [`../kien-truc/TONG-QUAN-KIEN-TRUC.md`](../kien-truc/TONG-QUAN-KIEN-TRUC.md)
- Luồng nghiệp vụ chi tiết + danh sách màn hình: [`LUONG-NGHIEP-VU-MAN-HINH.md`](./LUONG-NGHIEP-VU-MAN-HINH.md)
- Danh sách tính năng dạng checklist: [`DANH-SACH-TINH-NANG.md`](./DANH-SACH-TINH-NANG.md)
- Thuật ngữ nghiệp vụ/y tế: [`../kien-truc/THUAT-NGU.md`](../kien-truc/THUAT-NGU.md)
