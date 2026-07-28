# GiapTech.BlouseHiding — Phương án thực hiện

> Nền tảng tuyển dụng **chuyên biệt cho ngành y tế** (Healthcare Recruitment Platform),
> tham khảo mô hình của các nền tảng phổ biến như **TopCV** (CV/việc làm) và **Ybox** (sự kiện/cơ hội).

Tài liệu này là bản phân tích nghiệp vụ và đề xuất kiến trúc/công nghệ/lộ trình để khởi động dự án.
Các quyết định lớn (tech stack, phạm vi MVP) được nêu rõ để chốt trước khi code.

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

## 2. Đối tượng người dùng (Actors)

| Actor | Mô tả | Nhu cầu chính |
|------|-------|----------------|
| **Ứng viên** | Nhân sự y tế đang/muốn tìm việc | Tạo hồ sơ, xác thực chứng chỉ, tìm & ứng tuyển, theo dõi trạng thái |
| **Nhà tuyển dụng (NTD)** | Cơ sở y tế / HR / trưởng khoa | Đăng tin, sàng lọc, quản lý pipeline ứng viên, hẹn phỏng vấn |
| **Quản trị viên (Admin)** | Vận hành nền tảng | Kiểm duyệt tin & doanh nghiệp, xác thực chứng chỉ, xử lý báo cáo |
| **Kiểm duyệt viên (Moderator)** | Đội duyệt nội dung | Duyệt tin tuyển dụng, duyệt hồ sơ doanh nghiệp |
| **Khách (Guest)** | Chưa đăng nhập | Xem tin, tìm kiếm cơ bản |

---

## 3. Phạm vi nghiệp vụ (Feature domains)

### 3.1 Tài khoản & định danh
- Đăng ký/đăng nhập (email, SĐT, OAuth Google/Zalo), OTP, quên mật khẩu.
- Phân quyền theo vai trò (RBAC): Candidate / Employer / Admin / Moderator.
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
- Xác minh doanh nghiệp (upload giấy phép → Admin duyệt).
- Trang thương hiệu tuyển dụng (employer branding).

### 3.4 Tin tuyển dụng
- Đăng tin với các trường: vị trí, chuyên khoa, loại hình (full-time/part-time/**trực ca**/**locum**/CTV),
  mức lương, địa điểm, yêu cầu CCHN, kinh nghiệm, phúc lợi.
- Quy trình duyệt tin (draft → pending → published → expired/closed).
- Gói tin (free / nổi bật / gấp) — cơ sở cho monetization.

### 3.5 Tìm kiếm & gợi ý
- Tìm kiếm full-text + bộ lọc (chuyên khoa, địa điểm, lương, loại hình, tuyến).
- Gợi ý việc làm theo hồ sơ (recommendation), gợi ý ứng viên cho NTD (matching).

### 3.6 Ứng tuyển & ATS (Applicant Tracking System)
- Ứng tuyển bằng CV nền tảng hoặc CV upload.
- Pipeline trạng thái: Mới → Đang xem → Phù hợp → Hẹn PV → Offer → Trúng tuyển / Từ chối.
- NTD ghi chú, đánh giá, gắn nhãn ứng viên.

### 3.7 Tương tác & thông báo
- Nhắn tin NTD ↔ ứng viên (chat).
- Thông báo real-time + email/Zalo/SMS (tin mới phù hợp, thay đổi trạng thái ứng tuyển).

### 3.8 Lớp "Ybox" — Cơ hội & phát triển nghề nghiệp
- Sự kiện/hội thảo y khoa, khóa CME, workshop, học bổng, cuộc thi chuyên môn.
- Đăng ký tham dự, nhắc lịch.

### 3.9 Quản trị & vận hành
- Kiểm duyệt tin & doanh nghiệp, xác thực CCHN, xử lý report/spam.
- Dashboard số liệu, quản lý người dùng, cấu hình danh mục (chuyên khoa, tuyến, địa điểm).

---

## 4. Đặc thù ngành y tế cần lưu ý (khác biệt so với TopCV/Ybox)

1. **Xác thực chứng chỉ hành nghề** là tính năng lõi tạo niềm tin — cần quy trình duyệt thủ công (giai đoạn đầu),
   hướng tới tích hợp/đối soát dữ liệu ngành khi khả thi.
2. **Phân loại chuyên khoa chuẩn hóa** — dùng danh mục (taxonomy) thống nhất để tìm kiếm/khớp lệnh chính xác.
3. **Loại hình làm việc đặc thù**: trực ca, locum (làm thay theo buổi), cộng tác viên khám bệnh.
4. **Bảo mật dữ liệu cá nhân nhạy cảm** — tuân thủ **Nghị định 13/2023/NĐ-CP** về bảo vệ dữ liệu cá nhân:
   mã hóa, phân quyền chặt, audit log, cơ chế đồng ý (consent) và xóa dữ liệu.
5. **Tuân thủ quảng cáo/tuyển dụng ngành y** theo quy định Bộ Y tế.

---

## 5. Kiến trúc hệ thống đề xuất

### 5.1 Nguyên tắc
- **Modular Monolith trước, tách microservice sau** khi tải & đội ngũ đủ lớn — tối ưu tốc độ ra MVP.
- **Clean Architecture + Domain-Driven Design (DDD)**: tách Domain / Application / Infrastructure / API.
- API-first: backend phục vụ đồng thời web + mobile qua REST (cân nhắc gRPC nội bộ).

### 5.2 Phân rã module (bounded contexts)
```
Identity        — tài khoản, vai trò, xác thực
Profile         — hồ sơ ứng viên, CCHN, chuyên khoa
Employer        — tổ chức/cơ sở y tế, xác minh
Job             — tin tuyển dụng, danh mục, kiểm duyệt
Application     — ứng tuyển, ATS pipeline
Search          — index & truy vấn (Elasticsearch)
Matching        — gợi ý việc/ứng viên
Messaging       — chat, thông báo
Events          — hội thảo/CME (lớp Ybox)
Admin           — kiểm duyệt, cấu hình, báo cáo
```

### 5.3 Sơ đồ tổng quát (logic)
```
   [Web App]        [Mobile App]        [Admin Portal]
        \                |                   /
         \               |                  /
              ┌────────────────────────┐
              │      API Gateway        │
              └────────────────────────┘
                          │
              ┌────────────────────────┐
              │  Backend (Modular       │
              │  Monolith / .NET)       │
              │  Identity · Profile ·   │
              │  Job · Application ·     │
              │  Matching · Messaging   │
              └────────────────────────┘
             /        |         |        \
   [PostgreSQL] [Elasticsearch] [Redis] [Blob/S3]
                                    │
                        [RabbitMQ / message bus]
                                    │
                 [Workers: email, index, matching, notif]
```

---

## 6. Đề xuất công nghệ (tech stack)

> Tên project theo quy ước `GiapTech.*` (namespace .NET) ⇒ **khuyến nghị stack .NET** cho backend.

| Lớp | Đề xuất chính | Phương án thay thế |
|-----|---------------|--------------------|
| Backend | **ASP.NET Core (.NET 8/9), C#** | Node.js (NestJS), Java (Spring Boot) |
| Kiến trúc | Clean Architecture + DDD, EF Core | Dapper cho truy vấn nặng |
| CSDL | **PostgreSQL** | SQL Server |
| Tìm kiếm | **Elasticsearch / OpenSearch** | Postgres full-text (giai đoạn MVP) |
| Cache | **Redis** | — |
| Hàng đợi | **RabbitMQ** (hoặc Azure Service Bus) | Kafka (khi scale lớn) |
| Realtime | **SignalR** (chat/thông báo) | WebSocket thuần |
| Lưu trữ file | **S3 / Azure Blob** | MinIO (self-host) |
| Web frontend | **Next.js (React, TypeScript)** | Blazor (nếu muốn full-.NET) |
| Mobile | **Flutter** hoặc **React Native** | — |
| Auth | JWT + refresh token, OAuth2 | IdentityServer / Keycloak |
| Hạ tầng | Docker + CI/CD (GitHub Actions), Azure/AWS | Kubernetes khi cần scale |
| Quan trắc | Serilog + OpenTelemetry, Prometheus/Grafana, Sentry | — |

**Lưu ý:** nếu đội ngũ mạnh về JS/TS hơn .NET, có thể đổi backend sang NestJS mà không thay đổi kiến trúc tổng thể.
Đây là quyết định cần chốt trước khi khởi tạo solution.

---

## 7. Mô hình dữ liệu cốt lõi (rút gọn)

```
User(id, email, phone, passwordHash, role, status, createdAt)
CandidateProfile(id, userId, fullName, dob, headline, summary, ...)
  ├─ License(id, profileId, licenseNo, issuedBy, scope, issuedAt, expiredAt, verifyStatus)
  ├─ Specialty(id, profileId, specialtyCode, level)
  ├─ Experience(id, profileId, org, position, tier, from, to)
  └─ Education / Certificate (CME) ...
Organization(id, name, type, licenseNo, size, verifyStatus, ...)
EmployerMember(id, orgId, userId, role)          // nhiều HR / 1 tổ chức
Job(id, orgId, title, specialtyCode, employmentType, salaryMin/Max,
    location, requiredLicense, experienceYears, status, publishedAt, expiredAt)
Application(id, jobId, candidateId, cvSnapshot, stage, note, createdAt)
Conversation / Message(...)                       // chat
Event(id, type, title, startAt, location, ...)    // lớp Ybox
Notification(id, userId, type, payload, readAt)
Category(specialty / tier / location) — danh mục chuẩn hóa
AuditLog(...) — phục vụ tuân thủ NĐ 13/2023
```

---

## 8. Lộ trình triển khai (Roadmap)

### Giai đoạn 0 — Khởi tạo (1–2 tuần)
- Chốt tech stack & phạm vi MVP.
- Khởi tạo solution (Clean Architecture), CI/CD, môi trường dev/staging, Docker Compose.
- Thiết kế schema & danh mục chuyên khoa/tuyến chuẩn.

### Giai đoạn 1 — MVP (6–8 tuần) — *đủ để chạy thử thị trường*
- Identity + RBAC (Candidate/Employer/Admin).
- Hồ sơ ứng viên (gồm CCHN & chuyên khoa) + xác thực CCHN thủ công qua Admin.
- Hồ sơ & xác minh doanh nghiệp.
- Đăng tin + kiểm duyệt + đăng tin công khai.
- Tìm kiếm & lọc (giai đoạn đầu dùng Postgres full-text).
- Ứng tuyển + ATS pipeline cơ bản.
- Thông báo email + trong ứng dụng.

### Giai đoạn 2 — Hoàn thiện (6–8 tuần)
- Elasticsearch cho tìm kiếm nâng cao.
- Gợi ý việc / matching ứng viên.
- Chat realtime (SignalR).
- Mobile app.
- Gói tin trả phí (monetization) + thanh toán.

### Giai đoạn 3 — Mở rộng
- Lớp sự kiện/CME kiểu Ybox.
- Phân tích/BI, employer branding, tối ưu matching bằng ML.
- Tích hợp/đối soát xác thực CCHN theo dữ liệu ngành (khi khả thi).

---

## 9. Rủi ro & giải pháp

| Rủi ro | Giải pháp |
|--------|-----------|
| Xác thực CCHN khó tự động | Bắt đầu duyệt thủ công + hàng đợi kiểm duyệt; tự động hóa dần |
| Dữ liệu cá nhân nhạy cảm | Mã hóa, RBAC chặt, audit log, tuân thủ NĐ 13/2023 |
| "Con gà–quả trứng" (thiếu cả tin & ứng viên) | Seed dữ liệu, hợp tác cơ sở y tế mồi, ưu tiên 1 chuyên khoa/khu vực trước |
| Tìm kiếm không chính xác do dữ liệu tự do | Chuẩn hóa danh mục chuyên khoa/tuyến ngay từ đầu |
| Over-engineering sớm | Modular monolith trước, chỉ tách service khi thực sự cần |

---

## 10. Việc cần chốt trước khi bắt tay code

1. **Tech stack backend**: .NET (khuyến nghị theo tên project) hay Node/khác?
2. **Phạm vi MVP**: có cắt bớt hạng mục nào ở Giai đoạn 1 không?
3. **Nền tảng đầu tiên**: Web trước hay Web + Mobile song song?
4. **Hạ tầng**: Azure / AWS / self-host?
5. **Mô hình doanh thu**: tính phí NTD theo gói tin ngay từ đầu hay sau?

> Sau khi chốt các mục trên, bước tiếp theo là khởi tạo solution theo Clean Architecture và dựng khung module.
