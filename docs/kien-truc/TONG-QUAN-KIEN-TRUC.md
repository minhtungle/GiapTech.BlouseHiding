# GiapTech.BlouseHiding — Tổng quan kiến trúc & công nghệ

> Tài liệu **kiến trúc & công nghệ chung** — điểm vào để nắm bức tranh tổng thể trước khi đọc sâu vào
> từng lớp cụ thể. Nếu bạn (người hoặc Agent) chỉ đọc 1 tài liệu kiến trúc, hãy đọc tài liệu này trước.
>
> Nghiệp vụ chi tiết: [`../nghiep-vu/PHAN-TICH-NGHIEP-VU.md`](../nghiep-vu/PHAN-TICH-NGHIEP-VU.md).
> Quy tắc dành cho Agent: [`../../CLAUDE.md`](../../CLAUDE.md).

---

## 1. Nguyên tắc kiến trúc

1. **Modular Monolith trước, tách microservice sau** — khi tải & đội ngũ đủ lớn mới tách. Tối ưu tốc
   độ ra MVP, tránh trả giá vận hành của microservice quá sớm. Xem [ADR-0001](./adr/0001-clean-architecture-modular-monolith.md).
2. **Clean Architecture + Domain-Driven Design (DDD)** — tách `Domain` / `Application` / `Infrastructure`
   / `Api`. Chi tiết layer & quy ước code: [`../backend/KIEN-TRUC-BACKEND.md`](../backend/KIEN-TRUC-BACKEND.md).
3. **API-first** — backend phục vụ đồng thời web + mobile qua REST (cân nhắc gRPC nội bộ khi thực sự
   tách service).

---

## 2. Phân rã module (bounded contexts)

```
Identity        — tài khoản, vai trò, xác thực
Profile         — hồ sơ ứng viên, CCHN, chuyên khoa
Employer        — tổ chức/cơ sở y tế, xác minh
Job             — tin tuyển dụng, danh mục, kiểm duyệt
Application     — ứng tuyển, ATS pipeline
Search          — index & truy vấn (OpenSearch, Giai đoạn 2)
Matching        — gợi ý việc/ứng viên
Messaging       — chat, thông báo
Events          — hội thảo/CME (lớp Ybox)
Ops             — kiểm duyệt, cấu hình, báo cáo (đội Vận hành nội bộ — không phải trang Admin của NTD)
```

> Quy ước tên 3 khu vực site (Client/Admin/Vận hành) xem [`THUAT-NGU.md`](./THUAT-NGU.md).

Mỗi bounded context tương ứng 1 nhóm `Features/` trong tầng `Application` — xem quy ước cụ thể ở
`KIEN-TRUC-BACKEND.md`.

---

## 3. Sơ đồ tổng quát (logic)

```
  [Client: Next.js]   [Mobile App, GĐ2]   [shadcn-admin: 1 app dùng chung
        \                     |             cho Admin (NTD) + Vận hành,
         \                    |             RBAC phân biệt màn hình — ADR-0008]
          \                   |                        /
              ┌────────────────────────┐
              │  Caddy (reverse proxy)  │
              └────────────────────────┘
                          │
              ┌────────────────────────┐
              │  Backend (Modular       │
              │  Monolith / ASP.NET     │
              │  Core .NET 10)          │
              │  Identity · Profile ·   │
              │  Job · Application ·    │
              │  Matching · Messaging   │
              └────────────────────────┘
             /        |         |        \
   [PostgreSQL] [OpenSearch, GĐ2] [Redis] [MinIO]
                                    │
                        [RabbitMQ / message bus]
                                    │
                 [Workers: email, index, matching, notif]
```

Hạ tầng triển khai thật (VPS, container, monitoring) xem [`../ha-tang/HA-TANG-TRIEN-KHAI.md`](../ha-tang/HA-TANG-TRIEN-KHAI.md).

---

## 4. Giao thức kết nối & tích hợp

| Kết nối | Giao thức | Ghi chú |
|---|---|---|
| Web/Mobile ↔ Backend | **REST/HTTPS (TLS 1.2+), JSON** | Theo [`../backend/API-DESIGN.md`](../backend/API-DESIGN.md); versioned qua `/api/v1` |
| Chat & thông báo realtime | **WebSocket qua SignalR** (fallback long-polling tự động) | Xem `../backend/CONG-NGHE-BACKEND.md` |
| Backend ↔ Message broker | **AMQP 0-9-1 qua RabbitMQ** (MassTransit abstraction) | Xử lý bất đồng bộ: gửi email, index tìm kiếm, tính điểm hồ sơ |
| Backend ↔ Cổng thanh toán | **REST + webhook**, xác thực chữ ký **HMAC-SHA256** | Tạm hoãn cổng tự động — xem [ADR-0003](./adr/0003-hoan-cong-thanh-toan-tu-dong.md) |
| Backend ↔ Object storage | **Presigned URL** (client upload thẳng lên MinIO, không qua backend) | Tránh nghẽn băng thông backend khi upload ảnh CCHN/CV lớn |
| Backend ↔ SMS/OTP | **REST API** của nhà cung cấp (eSMS/SpeedSMS) | Không có chuẩn chung, mỗi nhà cung cấp có format riêng |
| Nội bộ service-to-service (khi tách microservice, Giai đoạn 3+) | **gRPC** | Chỉ cần khi thực sự tách service — chưa dùng ở Modular Monolith hiện tại |
| Mobile push (Giai đoạn 2) | **FCM (Firebase Cloud Messaging)** | Qua `FirebaseAdmin` SDK phía backend |

---

## 5. Bản đồ công nghệ theo lớp (tham chiếu nhanh, chi tiết ở tài liệu con)

```
Backend:    .NET 10 (LTS) · ASP.NET Core Web API · MediatR · FluentValidation · Mapster
Data:       EF Core 10 (Npgsql) + Dapper (đọc nặng) · PostgreSQL 16+ · Redis 7+
Search:     pg_trgm/tsvector (MVP) → OpenSearch (GĐ2)
Queue/RT:   RabbitMQ + MassTransit · SignalR (Redis backplane) · Hangfire
Storage:    MinIO (self-host, S3-compatible) qua presigned URL
Frontend:   Client = Next.js (App Router) + React 19 + TypeScript
            Admin/Vận hành = shadcn-admin (Vite + React Router + TypeScript), 1 app riêng — ADR-0008
UI:         shadcn/ui (Radix + Tailwind) · dnd-kit · Tiptap · Recharts — dùng chung nền tảng ở cả 2 app
State:      TanStack Query + Zustand · React Hook Form + Zod
i18n:       next-intl (vi/en/ja/zh/ko/es, vi mặc định) · routing tiền tố URL — CHỈ áp dụng cho Client;
            Admin/Vận hành chỉ tiếng Việt (ADR-0008). Giao diện + danh mục dịch qua bảng
            `*_translations`, nội dung tự viết không dịch
Hạ tầng:    Self-host VPS · Docker Compose + Caddy · GitHub Actions ·
            Grafana/Loki/Prometheus + Uptime Kuma (self-host) · Sentry (cloud)
VN services: Thanh toán thủ công tạm thời (chưa chọn cổng) · eSMS/SpeedSMS (OTP) · Zalo OAuth
```

| Lớp | Tài liệu chi tiết |
|---|---|
| Backend (thư viện, pattern, auth) | [`../backend/CONG-NGHE-BACKEND.md`](../backend/CONG-NGHE-BACKEND.md), [`../backend/KIEN-TRUC-BACKEND.md`](../backend/KIEN-TRUC-BACKEND.md) |
| API | [`../backend/API-DESIGN.md`](../backend/API-DESIGN.md) |
| Database (schema, CSDL/lưu trữ) | [`../database/ERD-CHI-TIET.md`](../database/ERD-CHI-TIET.md), [`../database/QUY-UOC-MIGRATION.md`](../database/QUY-UOC-MIGRATION.md) |
| Frontend (thư viện, thiết kế) | [`../frontend/CONG-NGHE-FRONTEND.md`](../frontend/CONG-NGHE-FRONTEND.md), [`../frontend/THIET-KE-GIAO-DIEN.md`](../frontend/THIET-KE-GIAO-DIEN.md) |
| Hạ tầng triển khai | [`../ha-tang/HA-TANG-TRIEN-KHAI.md`](../ha-tang/HA-TANG-TRIEN-KHAI.md) |

---

## 6. Quyết định kiến trúc (Architecture Decision Records)

Các quyết định lớn, khó đảo ngược được ghi lại dạng ADR (lý do, phương án đã cân nhắc, hệ quả) —
tránh tình trạng "sao lại chọn cái này" phải hỏi lại khi bàn giao:

| ADR | Quyết định |
|---|---|
| [0001](./adr/0001-clean-architecture-modular-monolith.md) | Clean Architecture + Modular Monolith (không microservice ngay) |
| [0002](./adr/0002-self-host-vps.md) | Hạ tầng self-host VPS thay vì cloud managed (Azure/AWS) |
| [0003](./adr/0003-hoan-cong-thanh-toan-tu-dong.md) | Hoãn chọn cổng thanh toán tự động, dùng quy trình thủ công ở MVP |
| [0004](./adr/0004-tech-stack-net-nextjs.md) | Chọn .NET (backend) + Next.js/shadcn-ui (frontend) thay vì Node.js full-stack |
| [0005](./adr/0005-dat-ten-3-khu-vuc-site.md) | Đặt tên 3 khu vực site: Client / Admin (NTD) / Vận hành (nội bộ) |
| [0006](./adr/0006-da-ngon-ngu.md) | Đa ngôn ngữ: 6 ngôn ngữ (vi/en/ja/zh/ko/es), dịch qua bảng translation + routing tiền tố URL |
| [0007](./adr/0007-shadcn-chuan-cho-admin-van-hanh.md) | Admin (NTD) chuyển sang giao diện chuẩn shadcn/ui trung tính, cùng nhóm Vận hành |
| [0008](./adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md) | Admin (NTD) + Vận hành tách thành 1 app riêng, chạy thẳng shadcn-admin (đảo ngược 1 phần ADR-0004) |

---

## 7. Trạng thái quyết định (tổng hợp)

**Đã chốt:**
1. Tech stack backend: **.NET 10 (ASP.NET Core, LTS)**.
2. Nền tảng đầu tiên: **Web trước, MVP đầy đủ** (không cắt bớt hạng mục Giai đoạn 1).
3. Hạ tầng: **Self-host VPS**.
4. Cổng thanh toán tự động: **hoãn lại** — MVP dùng quy trình thủ công.
5. Kiến trúc: **Clean Architecture + Modular Monolith**.
6. Tên 3 khu vực site: **Client** (ứng viên/khách) · **Admin** (Nhà tuyển dụng) · **Vận hành** (đội nội
   bộ nền tảng). Xem [ADR-0005](./adr/0005-dat-ten-3-khu-vuc-site.md) và [`THUAT-NGU.md`](./THUAT-NGU.md).
   **Client** dùng Next.js (`web/`, riêng 1 app). **Admin + Vận hành dùng chung 1 app shadcn-admin**
   (`web-admin/`, Vite + React Router), tách khỏi Next.js — phân biệt màn hình theo role, RBAC chặn
   thật ở backend. Xem [ADR-0008](./adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md).
7. Đa ngôn ngữ: **6 ngôn ngữ** — Tiếng Việt (mặc định), Anh, Nhật, Trung, Hàn, Tây Ban Nha — **chỉ áp
   dụng cho Client** (`web/`); Admin/Vận hành (`web-admin/`) chỉ tiếng Việt (xem
   [ADR-0008](./adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md) mục 5). Routing theo tiền tố URL
   (`/vi/`, `/en/`...), tự nhận diện qua `Accept-Language` lần đầu, có bộ chọn ngôn ngữ trên giao diện.
   Chỉ dịch giao diện + danh mục chuẩn (qua bảng `*_translations`, không phải cột song song), **không**
   dịch nội dung tự viết (mô tả tin, tiểu sử ứng viên). Xem [ADR-0006](./adr/0006-da-ngon-ngu.md).
8. Phong cách giao diện theo khu vực: **Client** bản sắc "Tin cậy lâm sàng" đầy đủ; **Admin (NTD) +
   Vận hành** dùng chung giao diện dashboard **chuẩn shadcn/ui trung tính** (không serif, không mảng
   màu trang trí rộng), chỉ giữ màu thương hiệu cho badge trạng thái + 1 nút CTA chính mỗi màn hình.
   Xem [ADR-0007](./adr/0007-shadcn-chuan-cho-admin-van-hanh.md) (màu sắc/phong cách) và
   [ADR-0008](./adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md) (2 app riêng biệt).
9. Phạm vi "CV Scoring" ở Giai đoạn 1: **chỉ match-score dựa trên trường có cấu trúc sẵn** (chuyên khoa,
   số năm kinh nghiệm, địa điểm so với yêu cầu tin) — **không** phân tích văn bản CV tự do (không NLP/AI).
   Tham khảo thực tế ATS lớn (Greenhouse/Lever không có AI-score gốc, recruiter chủ yếu search/filter
   theo tiêu chí) — `score` chỉ là gợi ý sắp xếp phụ, HR vẫn quyết định qua Kanban. Chấm điểm CV bằng
   AI/NLP đầy đủ dời sang module `Matching` ở Giai đoạn 2. Xem
   [`../database/ERD-CHI-TIET.md`](../database/ERD-CHI-TIET.md) mục 4 điểm 8.

**Còn cần chốt (không chặn tiến độ):**
10. Nhà cung cấp VPS cụ thể (VN hay quốc tế) — xem `../ha-tang/HA-TANG-TRIEN-KHAI.md` mục so sánh.
11. Cổng thanh toán tự động cụ thể khi cần triển khai thật.
12. OpenSearch vs Elasticsearch ở Giai đoạn 2 — đã nghiêng OpenSearch.

> Đủ điều kiện khởi tạo solution theo Clean Architecture (Giai đoạn 0 của roadmap) — các mục còn lại
> không chặn tiến độ, có thể chốt song song trong lúc phát triển Giai đoạn 0–1.
