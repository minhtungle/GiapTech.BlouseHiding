# GiapTech.BlouseHiding

Nền tảng **tuyển dụng chuyên biệt cho ngành y tế** — kết nối cơ sở y tế (bệnh viện, phòng khám,
nhà thuốc, công ty dược…) với nhân sự y tế (bác sĩ, điều dưỡng, dược sĩ, kỹ thuật viên…).
Tham khảo mô hình của các nền tảng phổ biến như **TopCV** và **Ybox**, bổ sung các đặc thù ngành y:
xác thực chứng chỉ hành nghề, phân loại chuyên khoa, loại hình làm việc đặc thù (trực ca, locum…).

## Tài liệu

- [Phương án thực hiện](docs/PHUONG-AN-THUC-HIEN.md) — phân tích nghiệp vụ, đối chiếu TopCV/Ybox, kiến trúc, công nghệ, lộ trình.
- [Luồng nghiệp vụ & màn hình](docs/LUONG-NGHIEP-VU-MAN-HINH.md) — luồng chi tiết theo actor + danh sách màn hình.
- [ERD chi tiết](docs/ERD-CHI-TIET.md) — schema đầy đủ: bảng, cột, khóa, ràng buộc nghiệp vụ.
- [Thiết kế API](docs/API-DESIGN.md) — endpoint REST theo từng module, phân quyền RBAC.
- [Wireframe màn hình cốt lõi](docs/wireframes/man-hinh-cot-loi.html) — bố cục 3 màn hình chính (mở bằng trình duyệt).
- [Lựa chọn công nghệ chi tiết](docs/TECH-STACK-CHI-TIET.md) — thư viện cụ thể theo từng lớp: backend, CSDL, frontend, giao thức kết nối, hạ tầng, dịch vụ VN.

## Trạng thái

Dự án đang ở giai đoạn thiết kế/lập kế hoạch.
- Đã chốt: backend .NET (ASP.NET Core), triển khai Web trước với MVP đầy đủ.
- Xem [mục 11 của phương án thực hiện](docs/PHUONG-AN-THUC-HIEN.md#11-quyết-định--việc-còn-lại) cho các quyết định còn lại.
