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
| Runtime | **.NET 10 (LTS)** | LTS mới nhất tại thời điểm bắt đầu — hỗ trợ dài hạn phù hợp dự án chạy nhiều năm. Nếu môi trường CI/hosting chưa hỗ trợ, dùng **.NET 8 (LTS)** làm phương án lùi. |
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
| File lưu trữ | `AWSSDK.S3` hoặc `Azure.Storage.Blobs` (tùy hạ tầng chốt ở mục 5) | Upload ảnh CCHN, giấy phép, logo qua presigned URL |
| Email | **MailKit** gửi qua SMTP hoặc SDK của SendGrid/Amazon SES | Email xác thực, thông báo |
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
| Object storage | S3-compatible (AWS S3 / Cloudflare R2 / MinIO tự host) | Ảnh CCHN, giấy phép, CV PDF, logo — không lưu trong Postgres |
| Backup | `pg_dump` định kỳ + point-in-time recovery (WAL archiving) nếu managed DB hỗ trợ | Dữ liệu CCHN/doanh nghiệp là dữ liệu nhạy cảm, cần khôi phục được |

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

## 5. Hạ tầng & vận hành (DevOps)

| Thành phần | Lựa chọn | Lý do |
|---|---|---|
| Container | **Docker** + `docker-compose` cho môi trường dev (Postgres, Redis, RabbitMQ, MinIO local) | Đồng bộ môi trường giữa các dev, không cần cài từng service thủ công |
| CI/CD | **GitHub Actions** | Build, test, lint, build image, deploy theo nhánh |
| Container registry | GitHub Container Registry (ghcr.io) hoặc registry của cloud đã chọn | |
| Orchestration | Chưa cần Kubernetes ở MVP — dùng **Azure App Service / AWS ECS Fargate** (container đơn giản, tự scale) | Modular Monolith chưa cần K8s; tránh over-engineering hạ tầng theo đúng nguyên tắc đã chốt |
| Secrets | .NET **User Secrets** (dev) → **Azure Key Vault**/**AWS Secrets Manager** (staging/prod) | Không commit secret vào repo |
| Error tracking | **Sentry** (backend + frontend cùng dashboard) | Theo dõi lỗi runtime thực tế, không chỉ log |
| Uptime/monitoring | Cloud-native (Azure Monitor / CloudWatch) + Grafana nếu cần dashboard tùy biến | |

> **Còn cần chốt** (đã nêu ở mục 11 phương án thực hiện): Azure hay AWS hay self-host. Bảng dưới hỗ trợ quyết định.

### 5.1 So sánh nhanh Azure vs AWS cho stack .NET này

| Tiêu chí | Azure | AWS |
|---|---|---|
| Độ khớp với .NET | Cao nhất — App Service, Azure SQL/Postgres Flexible Server, SignalR Service quản lý sẵn | Tốt, cần tự cấu hình nhiều hơn cho .NET |
| Chi phí khởi điểm | Có gói cho startup (Azure for Startups) | Có gói tương tự (AWS Activate) |
| SignalR managed | **Azure SignalR Service** — bỏ luôn việc tự quản lý backplane Redis | Không có dịch vụ tương đương, phải tự vận hành Redis backplane |
| Elasticsearch/OpenSearch managed | Azure không có OpenSearch managed chính chủ (dùng Elastic Cloud) | **Amazon OpenSearch Service** managed sẵn |

→ Với trọng số backend .NET + SignalR realtime là ưu tiên, **nghiêng về Azure**; nếu đội ngũ đã quen AWS hoặc cần OpenSearch managed sớm, AWS vẫn hợp lý. Đây vẫn là quyết định cần bạn chốt.

---

## 6. Dịch vụ bên thứ ba đặc thù Việt Nam

| Nhu cầu | Đề xuất | Ghi chú |
|---|---|---|
| Thanh toán (gói tin, credit) | **PayOS** hoặc tích hợp trực tiếp **VNPay + Momo + ZaloPay** | PayOS là cổng tổng hợp của Việt Nam — 1 lần tích hợp, hỗ trợ nhiều phương thức, giảm công sức hơn tự nối từng cổng. Tự nối trực tiếp VNPay/Momo cho kiểm soát tốt hơn nhưng tốn công gấp 3 lần. **Cần bạn xác nhận hướng nào** (đã nêu ở mục 11 phương án thực hiện). |
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
Storage:    S3-compatible qua presigned URL
Frontend:   Next.js (App Router) + React 19 + TypeScript
UI:         shadcn/ui (Radix + Tailwind) · dnd-kit · Tiptap · Recharts
State:      TanStack Query + Zustand · React Hook Form + Zod
DevOps:     Docker + GitHub Actions · Azure App Service (đề xuất) · Sentry · OpenTelemetry
VN services: PayOS/VNPay/Momo (thanh toán) · eSMS/SpeedSMS (OTP) · Zalo OAuth
```

---

## 8. Việc cần bạn xác nhận (kế thừa mục 11 phương án thực hiện + phát sinh mới)

1. **Hạ tầng**: Azure (đề xuất) hay AWS hay self-host?
2. **Cổng thanh toán**: PayOS (tổng hợp, nhanh) hay tự nối trực tiếp từng cổng VNPay/Momo/ZaloPay?
3. **.NET 10 hay .NET 8**: nếu môi trường CI/hosting hiện có chưa hỗ trợ .NET 10, dùng .NET 8 LTS.
4. **OpenSearch vs Elasticsearch** ở Giai đoạn 2 — đã nghiêng OpenSearch, chốt nếu không có ràng buộc khác.

> Sau khi chốt các mục trên, đủ điều kiện khởi tạo solution theo Clean Architecture (Giai đoạn 0 của roadmap).
