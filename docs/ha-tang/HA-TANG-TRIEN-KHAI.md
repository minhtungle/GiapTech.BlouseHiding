# GiapTech.BlouseHiding — Hạ tầng triển khai (Self-host VPS)

> Xem tổng quan kiến trúc ở [`../kien-truc/TONG-QUAN-KIEN-TRUC.md`](../kien-truc/TONG-QUAN-KIEN-TRUC.md).
> Lý do chọn self-host VPS thay vì cloud managed: [ADR-0002](../kien-truc/adr/0002-self-host-vps.md).
> Quy trình vận hành (backup/restore/xử lý sự cố): [`VAN-HANH-RUNBOOK.md`](./VAN-HANH-RUNBOOK.md).
> Danh sách biến môi trường: [`BIEN-MOI-TRUONG.md`](./BIEN-MOI-TRUONG.md).

Toàn bộ hạ tầng tự vận hành trên VPS, không dùng dịch vụ managed của cloud lớn. Hệ quả trực tiếp: mọi
thứ Azure/AWS managed từng cân nhắc (SignalR Service, Key Vault, RDS...) được thay bằng thành phần
**mã nguồn mở tự host tương đương** — đúng tinh thần "ưu tiên mã nguồn mở" xuyên suốt dự án.

---

## 1. Kiến trúc triển khai (single VPS cho MVP)

```
                    Internet
                        │
                 ┌──────▼───────┐
                 │  Caddy        │  reverse proxy + auto HTTPS (Let's Encrypt)
                 └──────┬───────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
 ┌──────▼─────┐  ┌──────▼─────┐  ┌──────▼─────┐
 │ Next.js     │  │ ASP.NET     │  │ Admin       │   (docker compose services,
 │ (web+admin) │  │ Core API    │  │ (route      │    cùng 1 docker network nội bộ)
 │             │  │             │  │  group)     │
 └─────────────┘  └──────┬─────┘  └─────────────┘
                        │
   ┌───────────┬────────┼────────┬────────────┐
┌──▼───┐   ┌────▼───┐ ┌──▼───┐ ┌──▼────┐  ┌────▼────┐
│Postgres│  │ Redis  │ │RabbitMQ│ │MinIO │  │Hangfire │  ← tất cả container riêng,
└────────┘  └────────┘ └────────┘ └──────┘  │dashboard│    KHÔNG expose port ra ngoài
                                              └─────────┘    (chỉ Caddy expose 443)
```

## 2. Bảng thành phần

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Hệ điều hành | **Ubuntu Server 24.04 LTS** | Phổ biến nhất, tài liệu/cộng đồng nhiều, hỗ trợ dài hạn |
| Reverse proxy + TLS | **Caddy** | Tự động cấp & gia hạn HTTPS (Let's Encrypt) — ít cấu hình hơn Nginx + Certbot đáng kể, phù hợp team nhỏ tự vận hành |
| Container | **Docker + Docker Compose** | Đủ dùng cho quy mô Modular Monolith 1–2 VPS; chưa cần Kubernetes |
| Container registry | **GitHub Container Registry (ghcr.io)** | Miễn phí cho repo, tích hợp thẳng GitHub Actions |
| CI/CD deploy | **GitHub Actions** build image → push ghcr.io → SSH vào VPS chạy `docker compose pull && up -d` | Không cần runner tự host, đơn giản cho quy mô hiện tại |
| Object storage | **MinIO** (self-host, container riêng) | Thay S3/Azure Blob — API tương thích hoàn toàn |
| Secrets | File `.env` **không commit**, quyền đọc giới hạn (`chmod 600`); cân nhắc **Infisical** (self-host, mã nguồn mở) khi team lớn hơn | VPS không có Key Vault managed — cần kỷ luật vận hành thay thế |
| Log tập trung | **Grafana Loki** + **Promtail** (self-host) | Bộ ba Grafana/Loki/Prometheus phổ biến nhất cho self-host |
| Metrics/dashboard | **Prometheus + Grafana** (self-host), nhận dữ liệu qua OpenTelemetry exporter | Đồng bộ với OpenTelemetry ở `../backend/CONG-NGHE-BACKEND.md` |
| Uptime/alerting | **Uptime Kuma** (self-host, mã nguồn mở) | Nhẹ, dễ cài qua Docker, cảnh báo Telegram/email khi service down |
| Error tracking | **Sentry (bản cloud, gói free/nhỏ)** dù hạ tầng còn lại self-host | Tự host Sentry cần cụm Postgres/Redis/Clickhouse riêng — không đáng cho team nhỏ; ngoại lệ hợp lý |
| Bảo mật VPS | `ufw` (firewall, chỉ mở 22/80/443), SSH key-only, `fail2ban`, `unattended-upgrades` | VPS tự quản lý phải tự chịu trách nhiệm phần cloud vốn lo sẵn |

## 3. Chọn nhà cung cấp VPS — còn cần bạn chốt

| Hướng | Ưu điểm | Nhược điểm |
|---|---|---|
| VPS Việt Nam (Vietnix, VNG Cloud, Viettel IDC, FPT Cloud) | Độ trễ thấp cho người dùng VN; dữ liệu lưu trong nước — thuận lợi khi cần chứng minh tuân thủ NĐ 13/2023 | Giá/hiệu năng thường kém hơn quốc tế cùng tầm giá |
| VPS quốc tế (Hetzner, DigitalOcean, Vultr — datacenter Singapore) | Giá/hiệu năng tốt, Singapore cho độ trễ chấp nhận được | Dữ liệu cá nhân người Việt lưu ngoài lãnh thổ |

→ Nghiêng nhẹ về VPS trong nước hoặc Singapore để giảm rủi ro pháp lý — vẫn cần bạn xác nhận nhà cung
cấp cụ thể.

## 4. Lộ trình scale khi cần

1. **MVP**: 1 VPS chạy toàn bộ (app + DB + cache + queue) qua Docker Compose.
2. **Khi tải tăng**: tách VPS thứ 2 riêng cho PostgreSQL.
3. **Khi cần HA**: thêm VPS thứ 3 chạy replica Postgres + load balancer trước 2 VPS app.
4. Chỉ quay lại managed cloud/Kubernetes nếu vượt quá khả năng vận hành thủ công của team.

---

## Xem thêm
- Quy trình vận hành hằng ngày (backup, restore, xử lý sự cố, deploy): [`VAN-HANH-RUNBOOK.md`](./VAN-HANH-RUNBOOK.md)
- Danh sách biến môi trường: [`BIEN-MOI-TRUONG.md`](./BIEN-MOI-TRUONG.md)
- Lý do chọn self-host: [ADR-0002](../kien-truc/adr/0002-self-host-vps.md)
