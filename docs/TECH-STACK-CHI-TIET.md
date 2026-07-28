# GiapTech.BlouseHiding — Lựa chọn công nghệ chi tiết

> Cụ thể hóa mục 7 (Đề xuất công nghệ) của [`PHUONG-AN-THUC-HIEN.md`](./PHUONG-AN-THUC-HIEN.md).
> Đi theo từng lớp: Backend → CSDL/lưu trữ → Frontend → Giao thức kết nối → Hạ tầng/vận hành →
> Dịch vụ bên thứ ba đặc thù Việt Nam. Tên thư viện cụ thể để scaffold trực tiếp ở Giai đoạn 0;
> phiên bản chính xác (patch) chốt lúc khởi tạo solution vì có thể có bản mới hơn tại thời điểm đó.

---

## 1. Backend — ASP.NET Core (.NET)

### 1.1 Nền tảng
| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Runtime | **.NET 10 (LTS)** — đã chốt | Hỗ trợ dài hạn (LTS), phù hợp dự án chạy nhiều năm; VPS self-host tự cài nên không bị giới hạn bởi hosting managed |
| Ngôn ngữ | C# (nullable reference types bật, `ImplicitUsings`) | Chuẩn của hệ sinh thái .NET hiện đại |
| Kiến trúc solution | Clean Architecture: `Domain` / `Application` / `Infrastructure` / `Api` (+ `Web` nếu BFF) | Đã chốt ở phương án thực hiện mục 6 |
| Pattern nghiệp vụ | CQRS nhẹ với **MediatR** (Command/Query + Handler) | Tách rõ luồng ghi/đọc, dễ test, dễ thêm behavior pipeline (validation, logging) mà không rải code |
| Validation | **FluentValidation**, tích hợp làm MediatR `IPipelineBehavior` | Validate tập trung trước khi vào handler, không rải `if` rải rác |
| Mapping | **Mapster** (thay vì AutoMapper) | Nhanh hơn, cấu hình đơn giản hơn cho project mới |

### 1.2 Truy cập dữ liệu
| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| ORM chính | **EF Core 10** (Npgsql provider) | Migration, tracking, phù hợp CRUD của Identity/Profile/Job/Application |
| Truy vấn đọc nặng | **Dapper** cho các query phức tạp (tìm kiếm, dashboard, báo cáo) | EF Core sinh SQL không tối ưu cho join/aggregate phức tạp — dùng Dapper song song, không thay thế hoàn toàn EF Core |
| Migration | EF Core Migrations, review SQL sinh ra trước khi apply production | Tránh migration tự động phá dữ liệu |

### 1.3 Auth & bảo mật
| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Xác thực | **JWT Bearer** (`Microsoft.AspNetCore.Authentication.JwtBearer`) + access token ngắn hạn (~15 phút) + refresh token lưu DB (hash) | Chuẩn cho API-first, dễ dùng chung cho web + mobile sau này |
| Định danh mật khẩu | **ASP.NET Core Identity** (chỉ dùng phần hashing/password policy, không dùng cookie auth mặc định) | Tận dụng hạ tầng hash/lockout đã kiểm chứng thay vì tự viết |
| OAuth ngoài | Google, Zalo — dùng thư viện `AspNet.Security.OAuth.*` hoặc gọi trực tiếp OAuth2 endpoint | Zalo không có SDK .NET chính thức — tự implement theo REST API của Zalo |
| Phân quyền | Policy-based Authorization (`[Authorize(Policy = "EmployerMember")]`) + custom `IAuthorizationHandler` kiểm tra membership tổ chức | RBAC đơn giản (role) không đủ vì cần kiểm tra "có phải member của org này" |
| Rate limiting | `Microsoft.AspNetCore.RateLimiting` (built-in .NET) | Chặn spam OTP, brute-force login |

### 1.4 Xử lý nền & realtime
| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Background job | **Hangfire** (Postgres storage) | Dashboard giám sát sẵn có, phù hợp job như hết hạn CCHN, hết hạn tin, gửi email hàng loạt |
| Message queue | **RabbitMQ** qua **MassTransit** | MassTransit trừu tượng hóa AMQP, dễ đổi broker sau này, tích hợp outbox pattern sẵn |
| Realtime | **SignalR** (Redis backplane khi scale nhiều instance) | Chat, thông báo — đã chốt ở phương án thực hiện |
| Resilience | **Polly** (retry, circuit breaker) cho gọi cổng thanh toán/SMS ngoài | Các dịch vụ bên thứ 3 (VNPay, eSMS...) không ổn định 100% |

### 1.5 Hạ tầng phụ trợ trong code
| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Logging | **Serilog** → sink Console (dev) + Seq (dev/staging) + Application Insights/CloudWatch (prod) | Structured logging, dễ truy vết theo `correlationId` |
| Observability | **OpenTelemetry** (traces + metrics), export sang Prometheus/Grafana hoặc APM có sẵn của cloud | Chuẩn mở, không khóa vào 1 vendor |
| API docs | **Swashbuckle** (Swagger/OpenAPI) | Tự sinh tài liệu từ code, dùng luôn để test thủ công |
| File lưu trữ | `AWSSDK.S3` trỏ endpoint vào **MinIO tự host** (API tương thích S3) | Upload ảnh CCHN, giấy phép, logo qua presigned URL — không cần đổi SDK dù đổi hạ tầng lưu trữ sau này |
| Email | **MailKit** gửi qua SMTP (Amazon SES hoặc SMTP relay như Mailgun/Brevo — không cần VPS tự host SMTP vì dễ bị đánh spam) | Email xác thực, thông báo |
| Testing | **xUnit** + **FluentAssertions** + **Testcontainers** (Postgres/Redis thật trong test) + **Bogus** (sinh dữ liệu giả) | Test tích hợp sát thực tế thay vì mock DB |
| Contract test bên ngoài | **WireMock.Net** | Giả lập VNPay/eSMS khi test, không gọi thật |

---

## 2. Cơ sở dữ liệu & lưu trữ

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| CSDL chính | **PostgreSQL 16+** | Đã chốt — mạnh về JSONB (lưu `data_json` của CV Builder, `payload` thông báo), full-text search sẵn có cho MVP |
| Full-text MVP | Extension **`pg_trgm`** + `tsvector` trên `jobs`, `candidate_profiles` | Đủ dùng cho tìm kiếm ở Giai đoạn 1, tránh vận hành Elasticsearch quá sớm |
| Search nâng cao (GĐ2) | **OpenSearch** (thay vì Elasticsearch) | License Apache 2.0 rõ ràng hơn Elastic License, tương thích API Elasticsearch cũ |
| Cache | **Redis 7+** qua `StackExchange.Redis` | Cache kết quả tìm kiếm, session SignalR backplane, rate-limit counter |
| Object storage | **MinIO** tự host (API tương thích S3) | Ảnh CCHN, giấy phép, CV PDF, logo — không lưu trong Postgres; MinIO chạy tốt trên VPS, API giống hệt S3 nên dễ chuyển sang cloud object storage sau này nếu cần |
| Backup | `pg_dump`/`pg_basebackup` định kỳ (cron) + WAL archiving, đẩy bản backup ra **ngoài VPS chính** (VPS phụ hoặc Backblaze B2/S3-compatible) | Tự host thì không có snapshot managed của cloud — **bắt buộc** có bản sao ngoài máy chủ chính + diễn tập khôi phục (restore drill) định kỳ, vì dữ liệu CCHN/doanh nghiệp rất nhạy cảm |

---

## 3. Frontend (Web) — đã chốt Web trước

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Framework | **Next.js** (App Router) + **React 19** + TypeScript | SSR/SSG cho SEO tin tuyển dụng (quan trọng — tin cần lên Google), API routes làm BFF nhẹ khi cần |
| Component/UI | **shadcn/ui** (dựa trên Radix UI + Tailwind) | Không phải thư viện đóng gói (copy code vào repo) → tùy biến sâu theo bộ nhận diện riêng, tránh trông giống mọi trang MUI/AntD khác |
| Styling | **Tailwind CSS** | Đồng bộ với shadcn/ui, tốc độ style nhanh |
| Quản lý state server | **TanStack Query (React Query)** | Cache, refetch, optimistic update cho dữ liệu từ API (jobs, applications...) |
| Quản lý state client nhẹ | **Zustand** | Chỉ cho state UI thuần túy (modal, wizard hồ sơ) — tránh Redux thừa cho quy mô này |
| Form & validate | **React Hook Form + Zod** | Zod schema có thể tái dùng ý tưởng validate giống FluentValidation phía backend (đồng bộ rule 2 phía) |
| Kéo-thả ATS Kanban | **dnd-kit** | Nhẹ, accessible, đúng nhu cầu màn hình ATS đã thiết kế |
| Rich text editor | **Tiptap** | Mô tả công việc, bài viết "Góc nghề y" (Giai đoạn 3) |
| Biểu đồ (dashboard Admin) | **Recharts** hoặc **Tremor** | Đủ dùng cho dashboard số liệu, không cần D3 thuần |
| Realtime client | **@microsoft/signalr** | Khớp SignalR backend |
| i18n | **next-intl** | Tiếng Việt chính, để ngỏ tiếng Anh cho ứng viên/tổ chức nước ngoài sau này |
| Testing | **Vitest** + **React Testing Library** (unit) + **Playwright** (e2e) | Playwright thay thế Cypress — nhanh hơn, chạy đa trình duyệt |

---

## 4. Giao thức kết nối & tích hợp

| Kết nối | Giao thức | Ghi chú |
|---|---|---|
| Web/Mobile ↔ Backend | **REST/HTTPS (TLS 1.2+), JSON** | Theo `API-DESIGN.md`; versioned qua `/api/v1` |
| Chat & thông báo realtime | **WebSocket qua SignalR** (fallback long-polling tự động) | Đã chốt ở mục 3.7 phương án thực hiện |
| Backend ↔ Message broker | **AMQP 0-9-1 qua RabbitMQ** (MassTransit abstraction) | Xử lý bất đồng bộ: gửi email, index tìm kiếm, tính điểm hồ sơ |
| Backend ↔ Cổng thanh toán | **REST + webhook**, xác thực chữ ký **HMAC-SHA256** (theo chuẩn từng cổng: VNPay/Momo/ZaloPay) | Webhook endpoint (`/payments/webhook/{provider}`) phải verify signature trước khi tin, chống giả mạo callback |
| Backend ↔ Object storage | **Presigned URL** (client upload thẳng lên S3, không qua backend) | Tránh nghẽn băng thông backend khi upload ảnh CCHN/CV lớn |
| Backend ↔ SMS/OTP | **REST API** của nhà cung cấp (mục 6) | Không có chuẩn chung, mỗi nhà cung cấp có format riêng |
| Nội bộ service-to-service (khi tách microservice, Giai đoạn 3+) | **gRPC** | Chỉ cần khi thực sự tách service — chưa dùng ở Modular Monolith hiện tại |
| Mobile push (Giai đoạn 2, khi có app) | **FCM (Firebase Cloud Messaging)** | Chuẩn chung Android/iOS, tích hợp tốt với backend .NET qua `FirebaseAdmin` SDK |

---

## 5. Hạ tầng & vận hành — Self-host VPS (đã chốt)

Toàn bộ hạ tầng tự vận hành trên VPS, không dùng dịch vụ managed của cloud lớn. Hệ quả trực tiếp:
mọi thứ Azure/AWS managed từng đề xuất (SignalR Service, Key Vault, RDS...) được thay bằng thành phần
**mã nguồn mở tự host tương đương** — đúng tinh thần "ưu tiên mã nguồn mở" đã thống nhất ở phần frontend.

### 5.1 Kiến trúc triển khai (single VPS cho MVP)

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
 │ (web)       │  │ Core API    │  │ (nếu tách)  │    cùng 1 docker network nội bộ)
 └─────────────┘  └──────┬─────┘  └─────────────┘
                        │
   ┌───────────┬────────┼────────┬────────────┐
┌──▼───┐   ┌────▼───┐ ┌──▼───┐ ┌──▼────┐  ┌────▼────┐
│Postgres│  │ Redis  │ │RabbitMQ│ │MinIO │  │Hangfire │  ← tất cả container riêng,
└────────┘  └────────┘ └────────┘ └──────┘  │dashboard│    KHÔNG expose port ra ngoài
                                              └─────────┘    (chỉ Caddy expose 443)
```

### 5.2 Bảng thành phần

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Hệ điều hành | **Ubuntu Server 24.04 LTS** | Phổ biến nhất, tài liệu/cộng đồng nhiều, hỗ trợ dài hạn |
| Reverse proxy + TLS | **Caddy** | Tự động cấp & gia hạn HTTPS (Let's Encrypt) — ít cấu hình hơn Nginx + Certbot đáng kể, phù hợp team nhỏ tự vận hành |
| Container | **Docker + Docker Compose** | Đủ dùng cho quy mô Modular Monolith 1–2 VPS; chưa cần Kubernetes (đúng nguyên tắc tránh over-engineering đã chốt) |
| Container registry | **GitHub Container Registry (ghcr.io)** | Miễn phí cho repo, tích hợp thẳng GitHub Actions |
| CI/CD deploy | **GitHub Actions** build image → push ghcr.io → SSH vào VPS chạy `docker compose pull && up -d` | Không cần runner tự host, đơn giản cho quy mô hiện tại |
| Object storage | **MinIO** (self-host, container riêng) | Thay S3/Azure Blob — API tương thích hoàn toàn, SDK phía backend không đổi (mục 1.5) |
| Secrets | File `.env` **không commit**, quyền đọc giới hạn (`chmod 600`) trên VPS; cân nhắc **Infisical** (self-host, mã nguồn mở) khi team lớn hơn | VPS không có Key Vault managed — cần kỷ luật vận hành thay thế |
| Log tập trung | **Grafana Loki** + **Promtail** (self-host) | Bộ ba mã nguồn mở Grafana/Loki/Prometheus phổ biến nhất cho self-host, không phụ thuộc cloud |
| Metrics/dashboard | **Prometheus + Grafana** (self-host), nhận dữ liệu qua OpenTelemetry exporter | Đồng bộ với lựa chọn OpenTelemetry ở mục 1.5 |
| Uptime/alerting | **Uptime Kuma** (self-host, mã nguồn mở, có UI đẹp) | Nhẹ, dễ cài qua Docker, cảnh báo Telegram/email khi service down |
| Error tracking | **Sentry (bản cloud, gói free/nhỏ)** dù hạ tầng còn lại self-host | Tự host Sentry cần cụm Postgres/Redis/Clickhouse riêng — chi phí vận hành không đáng cho team nhỏ; đây là ngoại lệ hợp lý, không phải mâu thuẫn với chủ trương self-host |
| Bảo mật VPS | `ufw` (firewall, chỉ mở 22/80/443), SSH key-only (tắt password login), `fail2ban`, `unattended-upgrades` (vá bảo mật tự động) | VPS tự quản lý phải tự chịu trách nhiệm phần cloud vốn lo sẵn (patching, network ACL) |

### 5.3 Chọn nhà cung cấp VPS — còn cần bạn chốt

| Hướng | Ưu điểm | Nhược điểm |
|---|---|---|
| VPS Việt Nam (Vietnix, VNG Cloud, Viettel IDC, FPT Cloud) | Độ trễ thấp cho người dùng VN; dữ liệu lưu trong nước — thuận lợi hơn khi cần chứng minh tuân thủ NĐ 13/2023 | Giá/hiệu năng thường kém hơn quốc tế cùng tầm giá |
| VPS quốc tế (Hetzner, DigitalOcean, Vultr — có datacenter Singapore) | Giá/hiệu năng tốt, Singapore cho độ trễ chấp nhận được | Dữ liệu cá nhân người Việt lưu ngoài lãnh thổ — cần đánh giá lại khi làm rõ yêu cầu tuân thủ chi tiết |

→ Với dữ liệu CCHN/hồ sơ y tế là dữ liệu cá nhân nhạy cảm, **nghiêng nhẹ về VPS trong nước hoặc ít nhất Singapore** để giảm rủi ro pháp lý — nhưng đây vẫn là lựa chọn nhà cung cấp cụ thể cần bạn xác nhận.

### 5.4 Lộ trình scale khi cần (tránh phải quyết định lại từ đầu)

1. **MVP**: 1 VPS chạy toàn bộ (app + DB + cache + queue) qua Docker Compose.
2. **Khi tải tăng**: tách VPS thứ 2 riêng cho PostgreSQL (giảm cạnh tranh CPU/IO với app).
3. **Khi cần HA**: thêm VPS thứ 3 chạy replica Postgres (streaming replication) + load balancer đứng trước 2 VPS app.
4. Chỉ cân nhắc quay lại managed cloud/Kubernetes nếu vượt quá khả năng vận hành thủ công của team — chưa phải lo ở giai đoạn này.

---

## 6. Dịch vụ bên thứ ba đặc thù Việt Nam

| Nhu cầu | Đề xuất | Ghi chú |
|---|---|---|
| Thanh toán (gói tin, credit) | **Chưa quyết — để sau** | Quyết định đã hoãn theo yêu cầu. MVP xử lý tạm bằng **quy trình thủ công**: NTD chuyển khoản ngân hàng theo thông tin hiển thị kèm mã tham chiếu, Admin đối soát và duyệt gói/nạp credit qua `POST /admin/...` thủ công (xem mục 8). `payments` (ERD) và `POST /payments/*` (API) giữ nguyên schema/endpoint đã thiết kế — khi chọn cổng tự động sau này chỉ cần cắm thêm `provider` mới, không phải đổi model dữ liệu. |
| SMS OTP | **eSMS.vn** hoặc **SpeedSMS** | Giá tốt hơn Twilio cho số điện thoại Việt Nam, độ trễ thấp trong nước |
| OAuth đăng nhập | Google (chuẩn), **Zalo** (phổ biến tại VN, tự implement REST vì không có SDK .NET chính thức) | |
| Bản đồ/địa chỉ (hiển thị vị trí cơ sở y tế) | **Goong Maps** (bản đồ Việt hóa, giá tốt hơn Google Maps cho use case cơ bản) hoặc Google Maps nếu cần độ phủ toàn cầu | Không cấp thiết ở MVP |

---

## 7. Bảng tổng hợp nhanh (tham chiếu khi scaffold)

```
Backend:    .NET 10 (LTS) · ASP.NET Core Web API · MediatR · FluentValidation · Mapster
Data:       EF Core 10 (Npgsql) + Dapper (đọc nặng) · PostgreSQL 16+ · Redis 7+
Search:     pg_trgm/tsvector (MVP) → OpenSearch (GĐ2)
Queue/RT:   RabbitMQ + MassTransit · SignalR (Redis backplane) · Hangfire
Storage:    MinIO (self-host, S3-compatible) qua presigned URL
Frontend:   Next.js (App Router) + React 19 + TypeScript
UI:         shadcn/ui (Radix + Tailwind) · dnd-kit · Tiptap · Recharts
State:      TanStack Query + Zustand · React Hook Form + Zod
Hạ tầng:    Self-host VPS · Docker Compose + Caddy · GitHub Actions ·
            Grafana/Loki/Prometheus + Uptime Kuma (self-host) · Sentry (cloud)
VN services: Thanh toán thủ công tạm thời (chưa chọn cổng) · eSMS/SpeedSMS (OTP) · Zalo OAuth
```

---

## 8. Việc cần bạn xác nhận (còn lại)

**Đã chốt:** Hạ tầng self-host VPS · .NET 10 · Thanh toán tự động hoãn lại (dùng quy trình thủ công ở MVP).

1. **Nhà cung cấp VPS cụ thể**: VPS Việt Nam (Vietnix/VNG Cloud/Viettel IDC/FPT Cloud) hay quốc tế
   (Hetzner/DigitalOcean/Vultr, ưu tiên datacenter Singapore)? Xem phân tích mục 5.3.
2. **OpenSearch vs Elasticsearch** ở Giai đoạn 2 — đã nghiêng OpenSearch, chốt nếu không có ràng buộc khác.
3. **Cổng thanh toán tự động** — cần chốt trước khi triển khai tính năng gói tin/credit thật (không
   chặn việc khởi tạo solution, vì MVP đã có phương án thủ công tạm thời ở mục 6).

> Đủ điều kiện khởi tạo solution theo Clean Architecture (Giai đoạn 0 của roadmap) — các mục còn lại
> không chặn tiến độ, có thể chốt song song trong lúc phát triển Giai đoạn 0–1.
