# ADR-0002: Hạ tầng self-host VPS thay vì cloud managed

**Trạng thái:** Đã chấp nhận

## Bối cảnh
Cần chọn nơi chạy hệ thống: cloud managed (Azure/AWS) hay tự vận hành VPS. Cloud managed giảm gánh
nặng vận hành (backup, patching, scaling) nhưng chi phí cao hơn và phụ thuộc vendor. Dữ liệu xử lý
(CCHN, hồ sơ y tế) là dữ liệu cá nhân nhạy cảm theo NĐ 13/2023.

## Quyết định
Dùng **self-host VPS** (Docker Compose + Caddy), thay các dịch vụ managed từng cân nhắc (Azure SignalR
Service, Key Vault, RDS...) bằng thành phần mã nguồn mở tự host tương đương (MinIO, Grafana/Loki/
Prometheus, Uptime Kuma). Ngoại lệ: **Sentry dùng bản cloud** (free/gói nhỏ) vì tự host cần cụm
Postgres/Redis/Clickhouse riêng, không đáng công vận hành cho team nhỏ.

## Phương án đã cân nhắc
- **Azure**: khớp .NET tốt nhất (App Service, Azure SignalR Service managed) — loại bỏ vì chi phí cao
  hơn và người dùng yêu cầu self-host.
- **AWS**: tương tự Azure, có Amazon OpenSearch Service managed — loại bỏ cùng lý do.

## Hệ quả
- (+) Chi phí thấp hơn, kiểm soát toàn quyền hạ tầng, không phụ thuộc vendor.
- (+) Dữ liệu có thể lưu trong nước (VPS Việt Nam) — thuận lợi hơn khi cần chứng minh tuân thủ NĐ 13/2023.
- (−) Team phải tự chịu trách nhiệm những gì cloud managed vốn lo sẵn: backup, patching, HA, giám sát.
  → Bắt buộc có quy trình vận hành rõ ràng, xem [`../../ha-tang/VAN-HANH-RUNBOOK.md`](../../ha-tang/VAN-HANH-RUNBOOK.md).
- (−) Không có snapshot managed của DB — bắt buộc backup + restore drill định kỳ, không phải tùy chọn.
- Chi tiết kiến trúc triển khai: [`../../ha-tang/HA-TANG-TRIEN-KHAI.md`](../../ha-tang/HA-TANG-TRIEN-KHAI.md).
