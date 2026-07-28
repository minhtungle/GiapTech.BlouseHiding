# GiapTech.BlouseHiding — Biến môi trường

> Danh sách biến môi trường cần thiết theo từng thành phần. Không commit giá trị thật vào repo — chỉ
> commit file `.env.example` với giá trị mẫu/rỗng. Xem quy tắc bảo mật ở [`../../SECURITY.md`](../../SECURITY.md).

---

## Backend (ASP.NET Core)

| Biến | Mô tả | Ví dụ |
|---|---|---|
| `ConnectionStrings__Postgres` | Chuỗi kết nối PostgreSQL | `Host=postgres;Database=blousehiding;Username=...;Password=...` |
| `ConnectionStrings__Redis` | Chuỗi kết nối Redis | `redis:6379` |
| `RabbitMq__Host` / `__Username` / `__Password` | Kết nối RabbitMQ | |
| `Minio__Endpoint` / `__AccessKey` / `__SecretKey` / `__Bucket` | Kết nối MinIO (object storage) | |
| `Jwt__SigningKey` | Khóa ký JWT (bí mật, xoay định kỳ) | |
| `Jwt__AccessTokenMinutes` | Thời hạn access token | `15` |
| `Sms__Provider` / `__ApiKey` | eSMS/SpeedSMS gửi OTP | |
| `Email__SmtpHost` / `__SmtpUser` / `__SmtpPassword` | Gửi email qua SMTP relay | |
| `OAuth__Google__ClientId` / `__ClientSecret` | Đăng nhập Google | |
| `OAuth__Zalo__AppId` / `__AppSecret` | Đăng nhập Zalo | |
| `Sentry__Dsn` | Error tracking | |
| `OpenTelemetry__OtlpEndpoint` | Export traces/metrics tới Prometheus/Grafana | |

## Frontend (Next.js)

| Biến | Mô tả |
|---|---|
| `NEXT_PUBLIC_API_BASE_URL` | Base URL gọi backend (`/api/v1`) |
| `NEXT_PUBLIC_SIGNALR_HUB_URL` | URL SignalR hub cho chat/thông báo realtime |
| `NEXT_PUBLIC_SENTRY_DSN` | Error tracking phía frontend |

## Hạ tầng (Docker Compose / VPS)

| Biến | Mô tả |
|---|---|
| `POSTGRES_PASSWORD` | Mật khẩu Postgres container |
| `MINIO_ROOT_USER` / `MINIO_ROOT_PASSWORD` | Tài khoản gốc MinIO |
| `CADDY_DOMAIN` | Domain chính để Caddy tự cấp HTTPS |
| `GHCR_TOKEN` | Token pull image từ GitHub Container Registry (dùng trong CI/CD, không lưu trên VPS lâu dài) |

---

## Quy tắc

1. **Không bao giờ** commit `.env` thật vào repo — chỉ `.env.example` với giá trị placeholder.
2. Secret production chỉ tồn tại trên VPS (`chmod 600`), không gửi qua Slack/email/chat thường.
3. Khi rotate secret (vd `Jwt__SigningKey`), có kế hoạch cho session đang hoạt động (JWT cũ hết hạn tự
   nhiên trong ≤15 phút — không cần revoke thủ công nhờ access token ngắn hạn).
4. Đủ team lớn hơn → chuyển sang **Infisical** (self-host, mã nguồn mở) để quản lý secret có audit log
   thay vì file `.env` thủ công (xem `HA-TANG-TRIEN-KHAI.md` mục 2).
