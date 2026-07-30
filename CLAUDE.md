# CLAUDE.md — Hướng dẫn cho Agent

> File này là **nguồn sự thật đầu tiên** mà bất kỳ AI agent nào (Claude Code hoặc công cụ khác) phải
> đọc trước khi động vào repo này. Nếu một quy tắc ở đây mâu thuẫn với suy đoán của bạn về "cách làm
> thông thường", **quy tắc ở đây thắng**. Nếu bạn không chắc, đọc tài liệu tham chiếu trước khi đoán.

---

## 1. Dự án này là gì

Nền tảng **tuyển dụng chuyên biệt cho ngành y tế** tại Việt Nam (kết nối cơ sở y tế với nhân sự y tế),
tham khảo TopCV/Ybox nhưng thêm lớp xác thực chứng chỉ hành nghề (CCHN) làm rào cản tin cậy cốt lõi.
Xem [`docs/nghiep-vu/PHAN-TICH-NGHIEP-VU.md`](docs/nghiep-vu/PHAN-TICH-NGHIEP-VU.md) để hiểu đầy đủ bối cảnh trước khi code.

**Trạng thái hiện tại:** giai đoạn thiết kế, solution/code thật **chưa được khởi tạo**. Khi solution
đã tồn tại, chạy lại skill `init` để cập nhật phần "Lệnh build/test" (mục 6) của file này cho khớp
thực tế — đừng để phần đó lạc hậu so với code.

## 2. Bản đồ tài liệu — đọc đúng chỗ trước khi hỏi/đoán

| Cần biết gì | Đọc ở đâu |
|---|---|
| Kiến trúc & công nghệ tổng quan, quyết định đã chốt | [`docs/kien-truc/TONG-QUAN-KIEN-TRUC.md`](docs/kien-truc/TONG-QUAN-KIEN-TRUC.md) |
| Vì sao 1 quyết định lớn được chọn (không phải chọn cái khác) | [`docs/kien-truc/adr/`](docs/kien-truc/adr/) |
| Thuật ngữ y tế/nghiệp vụ (CCHN, chuyên khoa, tuyến, locum...) | [`docs/kien-truc/THUAT-NGU.md`](docs/kien-truc/THUAT-NGU.md) |
| Nghiệp vụ, actor, đối chiếu TopCV/Ybox, roadmap | [`docs/nghiep-vu/PHAN-TICH-NGHIEP-VU.md`](docs/nghiep-vu/PHAN-TICH-NGHIEP-VU.md) |
| Luồng nghiệp vụ chi tiết + danh sách màn hình | [`docs/nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md`](docs/nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md) |
| Checklist tính năng theo giai đoạn | [`docs/nghiep-vu/DANH-SACH-TINH-NANG.md`](docs/nghiep-vu/DANH-SACH-TINH-NANG.md) |
| Quy ước layer/CQRS backend — **đọc trước khi viết bất kỳ code backend nào** | [`docs/backend/KIEN-TRUC-BACKEND.md`](docs/backend/KIEN-TRUC-BACKEND.md) |
| Thư viện backend cụ thể | [`docs/backend/CONG-NGHE-BACKEND.md`](docs/backend/CONG-NGHE-BACKEND.md) |
| Hợp đồng API (endpoint, request/response) | [`docs/backend/API-DESIGN.md`](docs/backend/API-DESIGN.md) |
| Schema CSDL đầy đủ + ràng buộc nghiệp vụ bắt buộc | [`docs/database/ERD-CHI-TIET.md`](docs/database/ERD-CHI-TIET.md) |
| Quy ước migration | [`docs/database/QUY-UOC-MIGRATION.md`](docs/database/QUY-UOC-MIGRATION.md) |
| Thư viện frontend cụ thể | [`docs/frontend/CONG-NGHE-FRONTEND.md`](docs/frontend/CONG-NGHE-FRONTEND.md) |
| Phong cách thiết kế, token màu/chữ | [`docs/frontend/THIET-KE-GIAO-DIEN.md`](docs/frontend/THIET-KE-GIAO-DIEN.md) |
| Wireframe/style guide trực quan | [`docs/frontend/wireframes/`](docs/frontend/wireframes/) |
| Hạ tầng triển khai (self-host VPS) | [`docs/ha-tang/HA-TANG-TRIEN-KHAI.md`](docs/ha-tang/HA-TANG-TRIEN-KHAI.md) |
| Quy trình vận hành (backup/restore/sự cố/deploy) | [`docs/ha-tang/VAN-HANH-RUNBOOK.md`](docs/ha-tang/VAN-HANH-RUNBOOK.md) |
| Biến môi trường | [`docs/ha-tang/BIEN-MOI-TRUONG.md`](docs/ha-tang/BIEN-MOI-TRUONG.md) |
| Hướng dẫn sử dụng cho từng vai trò | [`docs/huong-dan-su-dung/`](docs/huong-dan-su-dung/) |
| Quy trình đóng góp code (git flow, commit, PR) | [`CONTRIBUTING.md`](CONTRIBUTING.md) |
| Chính sách bảo mật | [`SECURITY.md`](SECURITY.md) |

## 3. Cấu trúc repo (khi solution đã khởi tạo — xem `docs/backend/KIEN-TRUC-BACKEND.md` mục 1 để biết chi tiết)

```
src/
  Domain/            — entity, business invariant, KHÔNG reference project khác
  Application/        — use case (CQRS/MediatR), 1 thư mục / 1 bounded context
  Infrastructure/     — EF Core, implement interface của Application
  Web/                — Controllers/API, composition root
web/                  — Next.js (App Router) — 3 route group: (client)/(admin)/(ops), cùng 1 codebase
                        Client=candidate/guest · Admin=Nhà tuyển dụng · Ops="Vận hành" nội bộ nền tảng
                        (xem docs/kien-truc/THUAT-NGU.md — role backend `admin` ≠ site "Admin")
docs/                 — toàn bộ tài liệu (bản đồ ở mục 2)
```

## 4. Quy tắc bất di bất dịch — không tự ý phá vỡ

1. **Dependency Rule của Clean Architecture**: `Domain` không được reference `Application`/
   `Infrastructure`/`Web`. Nếu thấy mình cần import ngược, dừng lại — thiết kế sai chỗ, không phải
   quy tắc sai.
2. **Mọi Command (MediatR) bắt buộc có FluentValidation validator.** Không validate rải rác trong
   handler hay controller.
3. **Business invariant nằm trong Domain entity**, không nằm trong Command Handler hay Controller.
4. **4 ràng buộc dữ liệu bắt buộc** (chi tiết ở `docs/database/ERD-CHI-TIET.md` mục 4 và
   `docs/backend/KIEN-TRUC-BACKEND.md` mục 3):
   - `jobs.status → published` chỉ khi `organizations.verify_status = verified`.
   - Trừ Credit (`profile_unlocks`) phải trong 1 transaction, lock `credit_wallets` — không tách 2 lệnh.
   - Đổi `applications.stage` phải ghi `application_stage_history` cùng transaction.
   - Xóa tài khoản là soft-delete + anonymize, không bao giờ xóa cứng dữ liệu liên quan CCHN/audit log.
5. **Không tự ý đổi/xóa migration đã merge** — tạo migration mới để sửa, xem `docs/database/QUY-UOC-MIGRATION.md`.
6. **Không lấy nguyên một Next.js SaaS template hay chạy shadcn-admin như 1 app riêng** cho bất kỳ khu
   vực nào (Client/Admin/Vận hành) — lý do ở [ADR-0004](docs/kien-truc/adr/0004-tech-stack-net-nextjs.md)
   và [ADR-0005](docs/kien-truc/adr/0005-dat-ten-3-khu-vuc-site.md). Component nền tảng dùng shadcn/ui
   (copy-code vào repo), không cài package UI đóng gói khác thay thế.
7. **Quyết định kiến trúc lớn/khó đảo ngược → viết ADR mới** trong `docs/kien-truc/adr/`, đánh số tiếp
   theo, theo đúng format các ADR hiện có (Bối cảnh/Quyết định/Phương án đã cân nhắc/Hệ quả). Đừng chỉ
   sửa trực tiếp tài liệu tổng quan mà không để lại dấu vết quyết định.
8. **Đổi schema CSDL → cập nhật `docs/database/ERD-CHI-TIET.md` trong cùng PR.** Thêm/sửa endpoint →
   cập nhật `docs/backend/API-DESIGN.md` trong cùng PR. Tài liệu lệch khỏi code là nợ kỹ thuật, không
   phải việc "làm sau".
9. **Dữ liệu nhạy cảm** (CCHN, giấy phép doanh nghiệp, thông tin liên hệ ứng viên) — không log ra
   plaintext, không trả về API response nhiều hơn mức cần thiết cho từng role (xem RBAC ở
   `docs/backend/CONG-NGHE-BACKEND.md` mục 3). Trước khi merge thay đổi động vào các luồng này, chạy
   skill `security-review`.
10. **Đa ngôn ngữ (6 ngôn ngữ: vi/en/ja/zh/ko/es)** — không hardcode chuỗi tiếng Việt trực tiếp trong
    component/response lỗi. Mọi text UI mới thêm phải có đủ 6 khóa dịch trong cùng PR (file
    `messages/{locale}/{namespace}.json`). Danh mục (chuyên khoa, địa điểm, tên gói) dịch qua bảng
    `*_translations` riêng (không phải cột song song) — thêm danh mục mới phải thêm luôn bản dịch cho
    cả 5 ngôn ngữ không phải `vi`, không để trống rồi tính sau. Nội dung tự do người dùng viết (mô tả
    tin, tiểu sử) **không dịch**, hiển thị nguyên văn — xem [ADR-0006](docs/kien-truc/adr/0006-da-ngon-ngu.md).

## 5. Khi thêm tính năng mới — thứ tự làm việc

1. Đọc `docs/nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md` xem tính năng có nằm trong luồng đã thiết kế không.
   Nếu không có, cập nhật tài liệu nghiệp vụ trước, đừng code trước rồi tài liệu chạy theo sau.
2. Nếu động vào schema → cập nhật ERD trước, tạo migration theo `docs/database/QUY-UOC-MIGRATION.md`.
3. Viết Command/Query + Handler + Validator theo `docs/backend/KIEN-TRUC-BACKEND.md`.
4. Cập nhật `docs/backend/API-DESIGN.md` nếu có endpoint mới.
5. Cập nhật `docs/nghiep-vu/DANH-SACH-TINH-NANG.md` (đổi trạng thái ⬜/🟨/✅).
6. Viết test tương ứng (xem `docs/backend/KIEN-TRUC-BACKEND.md` mục 4) — không merge thiếu test cho
   use case mới.

## 6. Lệnh build/test/dev

> ⚠️ Solution chưa được khởi tạo — phần này là **placeholder theo kế hoạch**. Sau khi chạy
> `dotnet new install Clean.Architecture.Solution.Template` và scaffold Next.js, chạy lại skill `init`
> để thay các lệnh dưới đây bằng lệnh thật đã xác minh chạy được.

```bash
# Backend (dự kiến)
dotnet restore
dotnet build
dotnet test

# Frontend (dự kiến)
cd web && npm install
npm run dev
npm run build
npm run test        # Vitest
npm run test:e2e     # Playwright

# Hạ tầng dev (dự kiến)
docker compose up -d   # Postgres, Redis, RabbitMQ, MinIO
```

## 7. Skill nên dùng trong dự án này

| Skill | Khi nào |
|---|---|
| `init` | Sau khi scaffold solution thật — cập nhật mục 6 ở trên |
| `session-start-hook` | Đảm bảo session Claude Code mới tự restore/install môi trường |
| `update-config` | Sau khi có lệnh build/test quen thuộc, thêm allowlist giảm hỏi permission |
| `security-review` | Trước khi merge PR động vào CCHN/thanh toán/dữ liệu cá nhân |
| `review` | Mỗi PR trước khi merge |

## 8. Ngôn ngữ & phong cách viết tài liệu

- Tài liệu nghiệp vụ/kiến trúc viết **tiếng Việt** (đối tượng đọc chính là người Việt).
- Code, tên biến, tên bảng/cột, tên API viết **tiếng Anh** theo chuẩn ngành (đã áp dụng xuyên suốt ERD/API).
- Không thêm comment giải thích code hiển nhiên — chỉ comment khi có invariant/lý do không hiển nhiên
  (vd lý do 1 transaction phải lock bảng nào).
