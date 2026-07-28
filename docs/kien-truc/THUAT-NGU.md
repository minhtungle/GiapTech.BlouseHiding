# GiapTech.BlouseHiding — Thuật ngữ

> Từ điển thuật ngữ nghiệp vụ & y tế dùng xuyên suốt hệ thống — để người mới (hoặc Agent) không phải
> đoán nghĩa hoặc hỏi lại nhiều lần khi đọc tài liệu/code. Tên trường/bảng trong ngoặc `code` khớp với
> [`../database/ERD-CHI-TIET.md`](../database/ERD-CHI-TIET.md).

## Quy ước tên 3 khu vực site (đọc trước — hay gây nhầm lẫn nhất)

| Tên site | Dành cho | Role backend tương ứng |
|---|---|---|
| **Client** | Ứng viên & khách chưa đăng nhập (công khai) | `candidate`, guest |
| **Admin** | **Nhà tuyển dụng** — đăng tin, quản lý CV/ATS, mua gói, credit | `employer` |
| **Vận hành** | Đội nội bộ vận hành nền tảng — duyệt CCHN/doanh nghiệp/tin, xử lý report | `admin`, `moderator` |

> ⚠️ **Điểm dễ nhầm**: role backend tên là `admin` nhưng đó là đội **Vận hành**, không phải trang
> **Admin** (trang đó dành cho Nhà tuyển dụng — role `employer`). Hai chữ "Admin" chỉ trùng tên, không
> liên quan đến nhau. Luôn ghi rõ "trang Admin (NTD)" hoặc "đội Vận hành" khi có thể nhầm lẫn.

## Thuật ngữ ngành y

| Thuật ngữ | Giải thích |
|---|---|
| **CCHN** | Chứng chỉ hành nghề — giấy phép do Sở/Bộ Y tế cấp cho cá nhân đủ điều kiện hành nghề y. Bảng `licenses`. |
| **Chuyên khoa** | Lĩnh vực chuyên môn y tế (Nội, Ngoại, Sản, Nhi, Gây mê hồi sức, Dược lâm sàng...). Bảng `specialties`. |
| **Tuyến** | Cấp bậc cơ sở y tế theo hệ thống VN: trung ương → tỉnh → huyện → tư nhân. Trường `experiences.tier`. |
| **Locum** | Hình thức làm thay theo buổi/ca, không cố định — phổ biến với bác sĩ/điều dưỡng làm thêm ở nhiều cơ sở. |
| **Trực ca** | Hình thức làm việc theo ca trực (thường ca đêm), đặc thù ngành y không có ở tuyển dụng đại trà. |
| **CME** | Continuing Medical Education — chứng chỉ đào tạo y khoa liên tục, bắt buộc để duy trì/gia hạn CCHN ở nhiều trường hợp. Bảng `continuing_certificates`. |
| **BS đa khoa / BS chuyên khoa I/II** | Bậc trình độ đào tạo bác sĩ tại Việt Nam, cao hơn cử nhân nhưng khác hệ với Thạc sĩ/Tiến sĩ nghiên cứu. |

## Thuật ngữ nghiệp vụ nền tảng

| Thuật ngữ | Giải thích |
|---|---|
| **NTD** | Nhà tuyển dụng — đại diện cơ sở y tế (HR, trưởng khoa) đăng tin/tuyển dụng. |
| **ATS** | Applicant Tracking System — hệ thống quản lý pipeline ứng viên (`applications.stage`). |
| **CV Scoring** | Chấm điểm hồ sơ tự động/thủ công theo tiêu chí (CCHN hợp lệ, khớp chuyên khoa, kinh nghiệm). Trường `applications.score`. |
| **Credit** | Đơn vị trả phí để NTD chủ động "mở" hồ sơ ứng viên (liên hệ trực tiếp thay vì chờ ứng tuyển). Bảng `credit_wallets`, `credit_transactions`, `profile_unlocks`. |
| **Gói tin (Job Package)** | Gói trả phí Eco/Pro/Max quyết định thời gian hiển thị & vị trí ưu tiên của tin tuyển dụng. Bảng `job_packages`, `job_purchases`. |
| **Profile Unlock** | Hành động NTD trừ Credit để xem thông tin liên hệ đầy đủ của 1 ứng viên. |
| **Verify status** | Trạng thái xác thực (CCHN hoặc doanh nghiệp): `pending` (chờ duyệt) → `verified` (đã duyệt) / `rejected` (từ chối, kèm lý do) / `expired` (hết hạn). |

## Quy ước viết tắt trong code/tài liệu

| Viết tắt | Ý nghĩa |
|---|---|
| **RBAC** | Role-Based Access Control — phân quyền theo vai trò (`candidate`/`employer`/`admin`/`moderator`). |
| **BFF** | Backend For Frontend — tầng trung gian mỏng giữa frontend và backend chính (Next.js API routes nếu cần). |
| **ADR** | Architecture Decision Record — bản ghi quyết định kiến trúc, xem [`adr/`](./adr/). |

---

Thêm thuật ngữ mới vào bảng phù hợp khi phát sinh trong quá trình phát triển — không để thuật ngữ chỉ
tồn tại trong đầu người viết code.
