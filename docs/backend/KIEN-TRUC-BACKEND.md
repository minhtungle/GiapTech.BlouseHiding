# GiapTech.BlouseHiding — Kiến trúc & quy ước code Backend

> Tài liệu này là **luật chơi bắt buộc** khi viết code backend — cả người và Agent đều phải tuân theo,
> không tùy ý phá vỡ layer để "cho nhanh". Đây là tài liệu mà [`CLAUDE.md`](../../CLAUDE.md) trỏ tới
> khi Agent cần biết đặt code ở đâu.

---

## 1. Cấu trúc solution (theo khung Jason Taylor Clean Architecture)

```
src/
  Domain/            — Entity, Enum, ValueObject, DomainEvent, Exception nghiệp vụ.
                        KHÔNG được reference bất kỳ project nào khác. Không NuGet package ngoài
                        (trừ thứ thuần domain như Ardalis.SmartEnum nếu cần).

  Application/        — Use case (CQRS qua MediatR), interface hướng ra ngoài (IJobRepository,
                        IEmailSender...), validator, mapping. CHỈ reference Domain.
    Common/
      Interfaces/      — interface mà Infrastructure phải implement (Dependency Inversion)
      Behaviours/       — MediatR pipeline: ValidationBehaviour, LoggingBehaviour, ...
      Exceptions/
    Candidates/         — 1 thư mục / 1 bounded context (khớp mục 2 của TONG-QUAN-KIEN-TRUC.md)
      Commands/
        VerifyLicense/
          VerifyLicenseCommand.cs
          VerifyLicenseCommandHandler.cs
          VerifyLicenseCommandValidator.cs
      Queries/
        GetCandidateProfile/
          GetCandidateProfileQuery.cs
          GetCandidateProfileQueryHandler.cs
    Jobs/ · Applications/ · Employers/ · Messaging/ · Admin/ · Events/   (tương tự Candidates/)

  Infrastructure/     — EF Core DbContext + Migrations, implement interface của Application
                        (EmailSender, S3/MinIO storage, SMS OTP client...), ASP.NET Core Identity setup.
                        Reference Application + Domain.

  Web/                — Controllers (hoặc Minimal API endpoints theo `API-DESIGN.md`), DI composition
                        root, appsettings, middleware. Reference Application + Infrastructure.
```

**Quy tắc phụ thuộc (Dependency Rule) — không được vi phạm:**
```
Domain  ←  Application  ←  Infrastructure
                ↑                ↑
                └──────  Web  ───┘
```
Domain không biết gì về Application/Infrastructure/Web. Application định nghĩa interface, Infrastructure
implement — Web chỉ nối dây (composition root), không chứa business logic.

---

## 2. Quy ước CQRS/MediatR

- Mỗi thao tác **ghi** → 1 `{Verb}{Noun}Command` (vd `CreateJobCommand`, `VerifyLicenseCommand`,
  `TransitionApplicationStageCommand`) + `Handler` + `Validator` cùng thư mục.
- Mỗi thao tác **đọc** → 1 `{Verb}{Noun}Query` (vd `GetJobByIdQuery`, `SearchCandidatesQuery`) + `Handler`.
- **Mọi Command bắt buộc có FluentValidation validator** — không kiểm tra input rải rác trong handler.
- Business rule bất biến (invariant) đặt trong **Domain entity** (vd `License.MarkVerified()` tự kiểm
  tra trạng thái hợp lệ trước khi chuyển), không đặt trong Command Handler.
- Handler chỉ điều phối (load aggregate → gọi method domain → lưu lại), không chứa logic nghiệp vụ trực tiếp.
- Truy vấn phức tạp (tìm kiếm, dashboard) dùng Dapper trực tiếp trong Query Handler, không ép qua EF Core
  nếu sinh SQL không tối ưu (xem `CONG-NGHE-BACKEND.md` mục 2).

## 3. Quy tắc bất biến bắt buộc thực thi (ánh xạ từ ERD)

Xem đầy đủ ở [`../database/ERD-CHI-TIET.md`](../database/ERD-CHI-TIET.md) mục 4 — nhắc lại các điểm
Agent **hay quên nhất** khi sinh code mới:

1. `jobs.status` chỉ chuyển `published` khi `organizations.verify_status = verified` — kiểm tra trong
   `Application` layer trước khi gọi `Job.Publish()`, không phó mặc validate phía frontend.
2. Trừ Credit (`ProfileUnlock`) phải nằm trong 1 transaction DB, lock dòng `credit_wallets` — không bao
   giờ trừ credit và tạo `profile_unlocks` ở 2 lệnh SQL tách rời không transaction.
3. Đổi `applications.stage` phải ghi `application_stage_history` cùng transaction — không cho phép
   update `stage` mà thiếu lịch sử.
4. Xóa tài khoản là soft-delete + anonymize, không bao giờ `DELETE` cứng dữ liệu liên quan CCHN/audit log
   (tuân thủ NĐ 13/2023).

## 4. Testing bắt buộc

- Mỗi Command/Query Handler mới → có test tương ứng trong `tests/Application.UnitTests`.
- Luồng động vào tiền/credit/xác thực CCHN → bắt buộc có **integration test** dùng Testcontainers
  (Postgres thật), không chỉ mock repository.
- Không merge PR nếu thiếu test cho use case mới — xem [`../../CONTRIBUTING.md`](../../CONTRIBUTING.md).

---

## Xem thêm
- Thư viện cụ thể: [`CONG-NGHE-BACKEND.md`](./CONG-NGHE-BACKEND.md)
- Hợp đồng API: [`API-DESIGN.md`](./API-DESIGN.md)
- Quy tắc dành riêng cho Agent: [`../../CLAUDE.md`](../../CLAUDE.md)
