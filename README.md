# GiapTech.BlouseHiding

Nền tảng **tuyển dụng chuyên biệt cho ngành y tế** — kết nối cơ sở y tế (bệnh viện, phòng khám,
nhà thuốc, công ty dược…) với nhân sự y tế (bác sĩ, điều dưỡng, dược sĩ, kỹ thuật viên…).
Tham khảo mô hình của các nền tảng phổ biến như **TopCV** và **Ybox**, bổ sung các đặc thù ngành y:
xác thực chứng chỉ hành nghề, phân loại chuyên khoa, loại hình làm việc đặc thù (trực ca, locum…).

> Nếu bạn là AI agent, đọc [`CLAUDE.md`](./CLAUDE.md) trước — đó là nguồn quy tắc bắt buộc, file này
> chỉ là mục lục điều hướng.

## Trạng thái

Dự án đang ở giai đoạn thiết kế/lập kế hoạch, solution/code thật chưa khởi tạo.

**Đã chốt:** backend **.NET 10 (ASP.NET Core)**, Web trước với MVP đầy đủ, hạ tầng **self-host VPS**,
kiến trúc **Clean Architecture + Modular Monolith**, frontend **Next.js + shadcn/ui**, đa ngôn ngữ
**Tiếng Việt + Tiếng Anh**, thanh toán tự động **hoãn lại** (MVP dùng quy trình thủ công). Xem trạng
thái đầy đủ + việc còn cần chốt ở [`docs/kien-truc/TONG-QUAN-KIEN-TRUC.md`](docs/kien-truc/TONG-QUAN-KIEN-TRUC.md) mục 7.

## Mục lục tài liệu

- **[Tổng thuật hệ thống](docs/TONG-THUAT-HE-THONG.md)** — trình bày toàn bộ hệ thống dưới dạng văn
  bản liền mạch, viết cho người mới tiếp cận đọc một mạch để đánh giá tổng thể trước khi đọc sâu từng
  phần bên dưới.

### Kiến trúc & công nghệ chung
- [Tổng quan kiến trúc & công nghệ](docs/kien-truc/TONG-QUAN-KIEN-TRUC.md) — điểm vào chính, bản đồ công nghệ, trạng thái quyết định
- [Architecture Decision Records (ADR)](docs/kien-truc/adr/) — vì sao mỗi quyết định lớn được chọn
- [Thuật ngữ](docs/kien-truc/THUAT-NGU.md) — từ điển thuật ngữ y tế & nghiệp vụ

### Nghiệp vụ
- [Phân tích nghiệp vụ](docs/nghiep-vu/PHAN-TICH-NGHIEP-VU.md) — mục tiêu, actor, phạm vi tính năng, đối chiếu TopCV/Ybox, roadmap, rủi ro
- [Luồng nghiệp vụ & màn hình](docs/nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md) — luồng chi tiết theo actor + danh sách màn hình
- [Danh sách tính năng](docs/nghiep-vu/DANH-SACH-TINH-NANG.md) — checklist theo giai đoạn

### Backend
- [Kiến trúc & quy ước code Backend](docs/backend/KIEN-TRUC-BACKEND.md) — layer, CQRS/MediatR, quy tắc bất biến
- [Công nghệ Backend](docs/backend/CONG-NGHE-BACKEND.md) — thư viện cụ thể, auth, dịch vụ bên thứ 3
- [Thiết kế API](docs/backend/API-DESIGN.md) — endpoint REST theo từng module, phân quyền RBAC

### Database
- [ERD chi tiết](docs/database/ERD-CHI-TIET.md) — schema đầy đủ, công nghệ CSDL/lưu trữ, ràng buộc nghiệp vụ
- [Quy ước Migration](docs/database/QUY-UOC-MIGRATION.md)

### Frontend
- [Công nghệ Frontend](docs/frontend/CONG-NGHE-FRONTEND.md) — thư viện cụ thể
- [Định hướng thiết kế giao diện](docs/frontend/THIET-KE-GIAO-DIEN.md) — phong cách, quyết định dùng mã nguồn mở ở đâu
- [Wireframe màn hình cốt lõi](docs/frontend/wireframes/man-hinh-cot-loi.html) (mở bằng trình duyệt)
- [Style Guide trực quan](docs/frontend/wireframes/style-guide.html) (mở bằng trình duyệt)

### Hạ tầng triển khai
- [Hạ tầng triển khai](docs/ha-tang/HA-TANG-TRIEN-KHAI.md) — self-host VPS, kiến trúc container
- [Runbook vận hành](docs/ha-tang/VAN-HANH-RUNBOOK.md) — backup, restore, xử lý sự cố, deploy
- [Biến môi trường](docs/ha-tang/BIEN-MOI-TRUONG.md)

### Hướng dẫn sử dụng
- [Ứng viên](docs/huong-dan-su-dung/HUONG-DAN-UNG-VIEN.md)
- [Nhà tuyển dụng](docs/huong-dan-su-dung/HUONG-DAN-NHA-TUYEN-DUNG.md)
- [Trang Vận hành — Quản trị viên / Kiểm duyệt viên](docs/huong-dan-su-dung/HUONG-DAN-VAN-HANH.md)

### Vận hành dự án (root)
- [CLAUDE.md](CLAUDE.md) — hướng dẫn bắt buộc cho AI agent
- [CONTRIBUTING.md](CONTRIBUTING.md) — git flow, commit convention, quy trình PR
- [SECURITY.md](SECURITY.md) — chính sách bảo mật, tuân thủ NĐ 13/2023
- [CHANGELOG.md](CHANGELOG.md) — nhật ký thay đổi
