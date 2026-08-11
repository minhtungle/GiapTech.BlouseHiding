# CLAUDE.md — Hướng dẫn cho Agent

> File này là **nguồn sự thật đầu tiên** mà bất kỳ AI agent nào (Claude Code hoặc công cụ khác) phải
> đọc trước khi động vào repo này. Nếu một quy tắc ở đây mâu thuẫn với suy đoán của bạn về "cách làm
> thông thường", **quy tắc ở đây thắng**. Nếu bạn không chắc, đọc tài liệu tham chiếu trước khi đoán.

---

## 1. Dự án này là gì

Nền tảng **tuyển dụng chuyên biệt cho ngành y tế** tại Việt Nam (kết nối cơ sở y tế với nhân sự y tế),
tham khảo TopCV/Ybox nhưng thêm lớp xác thực chứng chỉ hành nghề (CCHN) làm rào cản tin cậy cốt lõi.
Xem [`docs/nghiep-vu/PHAN-TICH-NGHIEP-VU.md`](docs/nghiep-vu/PHAN-TICH-NGHIEP-VU.md) để hiểu đầy đủ bối cảnh trước khi code.

**Trạng thái hiện tại (2026-08-11):** Giai đoạn 1 (MVP) **đã chốt** — 103 endpoint, 16 migration,
255 test pass, cả 2 frontend đã nối API thật cho toàn bộ luồng chính. Xem
[`docs/nghiep-vu/tien-do/2026-08-11--chot-giai-doan-1.md`](docs/nghiep-vu/tien-do/2026-08-11--chot-giai-doan-1.md).

---

## 1b. ⚠️ MỤC TIÊU NGƯỜI DÙNG ĐÃ CHỐT — đọc trước khi đề xuất bất kỳ việc gì

> Người dùng đã nhắc **nhiều lần**. Đây là thứ tự ưu tiên bất di bất dịch, **không tự ý đổi**:

**Thứ tự làm việc:**
1. **Hoàn thiện toàn bộ UI của cả hệ thống trước** — mọi màn hình của `web/` và `web-admin/`, dùng
   **dữ liệu giả** cũng được. Giao diện đủ và đẹp trước, API sau.
2. **Rồi mới tới API cho các chức năng chính**: quản lý **nộp/nhận hồ sơ ứng viên**, hồ sơ cá nhân/CV,
   tra cứu tin & đơn vị tuyển dụng, danh sách ứng viên phù hợp/đã nộp.

**NGOÀI PHẠM VI — không tự ý làm, không đề xuất, không nêu như "thứ đang chặn tiến độ":**

| Hạng mục | Ghi chú |
|---|---|
| Thanh toán / cổng thanh toán | Kể cả `manual_transfer` đã có — **không mở rộng thêm** |
| Email / SMS / provider gửi thông báo | Không nối provider thật, không coi OTP-qua-DB là vấn đề chặn |
| OAuth Google / Zalo | |
| Bộ đề y tế thật (test năng lực) | UI minh họa là đủ |
| Nhắn tin NTD ↔ ứng viên | |

**Sai lầm đã mắc, đừng lặp lại:**
- Nhiều lần nêu email/SMS/thanh toán như "thứ chặn việc mở cho người dùng thật" — người dùng **đã loại
  rồi**, nêu lại là lan man.
- Làm nghiệp vụ Vận hành (hoàn Credit, duyệt/đối soát) trong khi UI còn màn hình chưa hoàn thiện.
- Đề xuất theo "mục nào còn ⬜ trong checklist" thay vì theo **mục tiêu người dùng đã chốt**.

**Khi không chắc nên làm gì tiếp:** chọn việc thuộc nhóm 1 (hoàn thiện UI) trước; hết việc nhóm 1 thì
sang nhóm 2 (API cho luồng nộp/nhận hồ sơ). Đừng lấy checklist giai đoạn làm nguồn ưu tiên.

---

## 2. Bản đồ tài liệu — đọc đúng chỗ trước khi hỏi/đoán

| Cần biết gì | Đọc ở đâu |
|---|---|
| Đọc một mạch để nắm tổng thể trước khi đi sâu | [`docs/TONG-THUAT-HE-THONG.md`](docs/TONG-THUAT-HE-THONG.md) |
| Kiến trúc & công nghệ tổng quan, quyết định đã chốt | [`docs/kien-truc/TONG-QUAN-KIEN-TRUC.md`](docs/kien-truc/TONG-QUAN-KIEN-TRUC.md) |
| Vì sao 1 quyết định lớn được chọn (không phải chọn cái khác) | [`docs/kien-truc/adr/`](docs/kien-truc/adr/) — **đọc [ADR-0011](docs/kien-truc/adr/0011-tach-3-repo-git-submodule.md) trước khi commit** nếu chưa quen mô hình 3 submodule |
| Thuật ngữ y tế/nghiệp vụ (CCHN, chuyên khoa, tuyến, locum...) | [`docs/kien-truc/THUAT-NGU.md`](docs/kien-truc/THUAT-NGU.md) |
| Nghiệp vụ, actor, đối chiếu TopCV/Ybox, roadmap | [`docs/nghiep-vu/PHAN-TICH-NGHIEP-VU.md`](docs/nghiep-vu/PHAN-TICH-NGHIEP-VU.md) |
| Luồng nghiệp vụ chi tiết + danh sách màn hình | [`docs/nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md`](docs/nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md) |
| Checklist tính năng theo giai đoạn | [`docs/nghiep-vu/DANH-SACH-TINH-NANG.md`](docs/nghiep-vu/DANH-SACH-TINH-NANG.md) |
| Tra cứu chi tiết theo module: chức năng nào đã nối API thật, còn mock, hay chưa có | [`docs/nghiep-vu/TIEN-DO-CHI-TIET.md`](docs/nghiep-vu/TIEN-DO-CHI-TIET.md) |
| Quy trình chốt giai đoạn + trạng thái tổng quan + **mục lục nhật ký theo ngày** — đọc trước khi báo "xong giai đoạn X" | [`docs/nghiep-vu/TIEN-DO-DU-AN.md`](docs/nghiep-vu/TIEN-DO-DU-AN.md) (index ngắn) |
| Nhật ký chi tiết từng ngày làm việc (đã làm gì, verify ra sao, chốt gì) | [`docs/nghiep-vu/tien-do/`](docs/nghiep-vu/tien-do/) — 1 file/ngày |
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
| Cổng & tài khoản đăng nhập khi test dev cục bộ | [`docs/ha-tang/MOI-TRUONG-DEV-CUC-BO.md`](docs/ha-tang/MOI-TRUONG-DEV-CUC-BO.md) |
| Hướng dẫn sử dụng cho từng vai trò | [`docs/huong-dan-su-dung/`](docs/huong-dan-su-dung/) |
| Quy trình đóng góp code (git flow, commit, PR) | [`CONTRIBUTING.md`](CONTRIBUTING.md) |
| Chính sách bảo mật | [`SECURITY.md`](SECURITY.md) |
| Nhật ký thay đổi | [`CHANGELOG.md`](CHANGELOG.md) |

## 3. Cấu trúc repo — 1 repo tổng + 3 git submodule (xem [ADR-0011](docs/kien-truc/adr/0011-tach-3-repo-git-submodule.md))

⚠️ **Từ ADR-0011, backend/web/web-admin không còn nằm trực tiếp trong repo tổng** — mỗi thư mục dưới
đây là 1 **git submodule** trỏ tới 1 repo GitHub riêng, ghim đúng 1 commit cụ thể. Sau khi clone repo
tổng, phải chạy `git submodule update --init --recursive` (hoặc clone bằng
`git clone --recurse-submodules ...`) mới có đủ code — clone thường chỉ có `docs/` + file cấu hình
chung, 3 thư mục `api/`/`web/`/`web-admin/` sẽ trống.

```
api/                  — submodule → github.com/minhtungle/GiapTech.BlouseHiding.Api (.NET 10 Clean
                        Architecture — xem docs/backend/KIEN-TRUC-BACKEND.md mục 1 cho chi tiết layer
                        bên trong: src/Domain, src/Application, src/Infrastructure, src/Web,
                        src/ServiceDefaults, src/Shared, tests/*.UnitTests, tests/*.FunctionalTests,
                        tests/*.IntegrationTests, tests/TestAppHost)
web/                  — submodule → github.com/minhtungle/GiapTech.BlouseHiding.Web
                        Next.js (App Router) — CHỈ khu vực Client (candidate/guest), bản sắc
                        "Tin cậy lâm sàng" đầy đủ, cần SEO/SSR cho tin tuyển dụng
web-admin/            — submodule → github.com/minhtungle/GiapTech.BlouseHiding.WebAdmin
                        shadcn-admin (Vite + TanStack Router + TS) — 1 app riêng dùng chung cho
                        Admin (Nhà tuyển dụng) + Vận hành (nội bộ nền tảng), phân biệt màn hình theo
                        role đăng nhập, RBAC chặn thật ở backend (xem ADR-0008)
                        (xem docs/kien-truc/THUAT-NGU.md — role backend `admin` ≠ site "Admin")
docker-compose.yml    — Postgres/Redis/RabbitMQ/MinIO cho dev cục bộ (self-host, xem ADR-0002)
.devcontainer/        — môi trường dev chung cho cả workspace (không riêng app nào, giữ ở repo tổng)
docs/                 — toàn bộ tài liệu (bản đồ ở mục 2)
```

**Cách commit thay đổi trong 1 app (2 tầng, khác monorepo cũ):**
1. `cd api/` (hoặc `web/`/`web-admin/`) → sửa code → `git add` + `git commit` + `git push` **ngay
   trong submodule đó** — repo con có CI riêng, PR ở đó tự chạy build/test.
2. Về repo tổng: `git add api` (hoặc `web`/`web-admin`) + `git commit` — ghim con trỏ submodule sang
   commit mới. **Dễ quên bước này** — nếu quên, repo tổng vẫn ghim commit cũ dù submodule đã có code
   mới, người khác clone lại sẽ không thấy thay đổi.
3. Nếu 1 thay đổi đụng cả backend + 1 frontend (vd thêm API mới + nối UI trong 1 lượt làm việc) — đây
   giờ là **2 PR ở 2 repo khác nhau**, không còn gộp 1 PR như monorepo cũ.

## 4. Quy tắc bất di bất dịch — không tự ý phá vỡ

1. **Dependency Rule của Clean Architecture**: `Domain` không được reference `Application`/
   `Infrastructure`/`Web`. Nếu thấy mình cần import ngược, dừng lại — thiết kế sai chỗ, không phải
   quy tắc sai.
2. **Mọi Command (Mediator) bắt buộc có FluentValidation validator.** Không validate rải rác trong
   handler hay controller.
3. **Business invariant nằm trong Domain entity**, không nằm trong Command Handler hay Controller.
4. **4 ràng buộc dữ liệu bắt buộc** (chi tiết ở `docs/database/ERD-CHI-TIET.md` mục 4 và
   `docs/backend/KIEN-TRUC-BACKEND.md` mục 3):
   - `jobs.status → published` chỉ khi `organizations.verify_status = verified`.
   - Trừ Credit (`profile_unlocks`) phải trong 1 transaction, lock `credit_wallets` — không tách 2 lệnh.
   - Đổi `applications.stage` phải ghi `application_stage_history` cùng transaction.
   - Xóa tài khoản là soft-delete + anonymize, không bao giờ xóa cứng dữ liệu liên quan CCHN/audit log.
5. **Không tự ý đổi/xóa migration đã merge** — tạo migration mới để sửa, xem `docs/database/QUY-UOC-MIGRATION.md`.
6. **Không lấy nguyên một Next.js SaaS template cho Client.** Client (`web/`) không được scaffold từ
   1 SaaS starter đóng gói — lý do ở [ADR-0004](docs/kien-truc/adr/0004-tech-stack-net-nextjs.md). Ngược
   lại, **Admin (NTD) + Vận hành (`web-admin/`) chạy thẳng shadcn-admin** làm nền tảng app thật (1
   instance dùng chung, phân quyền theo role) — xem [ADR-0008](docs/kien-truc/adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md).
   Không tự dựng lại pattern shadcn-admin đã có sẵn bên trong `web/` Next.js — 2 khu vực này **không**
   còn chung codebase. Component nền tảng của cả 2 app đều dùng shadcn/ui (copy-code vào repo), không
   cài package UI đóng gói khác thay thế.
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
10. **Đa ngôn ngữ (6 ngôn ngữ: vi/en/ja/zh/ko/es) — áp dụng cho cả `web/` (Client) và `web-admin/`
    (Admin/Vận hành)**. `web-admin/` từng chỉ tiếng Việt (ADR-0008 mục 5 bản gốc) nhưng đã đảo lại ở
    [ADR-0010](docs/kien-truc/adr/0010-da-ngon-ngu-cho-web-admin.md) do phát sinh nhu cầu NTD/nhân sự
    không nói tiếng Việt. **Khác thư viện theo app**: `web/` dùng `next-intl`
    (`messages/{locale}/{namespace}.json`); `web-admin/` dùng `react-i18next`
    (`web-admin/src/messages/{locale}/{namespace}.json`, thư mục riêng, không dùng chung với `web/`)
    vì là Vite SPA, không phải Next.js. Ở cả 2 app: không hardcode chuỗi tiếng Việt trực tiếp trong
    component/response lỗi. Mọi text UI mới thêm phải có đủ 6 khóa dịch trong cùng PR. Danh mục
    (chuyên khoa, địa điểm, tên gói) dịch qua bảng `*_translations` riêng (không phải cột song song) —
    thêm danh mục mới phải thêm luôn bản dịch cho cả 5 ngôn ngữ không phải `vi`, không để trống rồi
    tính sau. Nội dung tự do người dùng viết (mô tả tin, tiểu sử, ghi chú ATS) **không dịch**, hiển thị
    nguyên văn — xem [ADR-0006](docs/kien-truc/adr/0006-da-ngon-ngu.md).

## 5. Khi thêm tính năng mới — thứ tự làm việc

1. Đọc `docs/nghiep-vu/LUONG-NGHIEP-VU-MAN-HINH.md` xem tính năng có nằm trong luồng đã thiết kế không.
   Nếu không có, cập nhật tài liệu nghiệp vụ trước, đừng code trước rồi tài liệu chạy theo sau.
2. Nếu động vào schema → cập nhật ERD trước, tạo migration theo `docs/database/QUY-UOC-MIGRATION.md`.
3. Viết Command/Query + Handler + Validator theo `docs/backend/KIEN-TRUC-BACKEND.md`.
4. Cập nhật `docs/backend/API-DESIGN.md` nếu có endpoint mới.
5. Cập nhật `docs/nghiep-vu/DANH-SACH-TINH-NANG.md` (đổi trạng thái ⬜/🟨/✅).
6. Viết test tương ứng (xem `docs/backend/KIEN-TRUC-BACKEND.md` mục 4) — không merge thiếu test cho
   use case mới.

**Khi xong toàn bộ 1 giai đoạn (không phải 1 tính năng lẻ)** — trước khi báo "xong Giai đoạn X" hay bắt
đầu Giai đoạn tiếp theo, chạy đủ checklist chốt giai đoạn ở
[`docs/nghiep-vu/TIEN-DO-DU-AN.md`](docs/nghiep-vu/TIEN-DO-DU-AN.md) mục 1, rồi ghi nhật ký
vào **file ngày mới** trong [`docs/nghiep-vu/tien-do/`](docs/nghiep-vu/tien-do/) và thêm 1 dòng vào
bảng mục lục (xem mục 9 bên dưới). Không tự ý coi 1 giai đoạn là xong chỉ vì code compile — phải verify chạy thật.

## 6. Lệnh build/test/dev

> Đã xác minh chạy được (Giai đoạn 0). Cần **.NET 10 SDK** (`global.json` pin `10.0.201`,
> `rollForward: latestFeature` — cài qua [dotnet-install script](https://dot.net/v1/dotnet-install.sh)
> nếu package manager hệ thống chưa có bản 10). Cần Docker chạy sẵn cho Postgres/Redis/RabbitMQ/MinIO.
> ⚠️ Từ [ADR-0011](docs/kien-truc/adr/0011-tach-3-repo-git-submodule.md), `api/`/`web/`/`web-admin/` là
> git submodule — **bước 0 bắt buộc trước mọi lệnh dưới đây** nếu vừa clone hoặc pull repo tổng.

```bash
# Bước 0 — chỉ cần sau khi clone/pull repo tổng (đảm bảo 3 submodule có code, đúng commit đã ghim)
git submodule update --init --recursive

# Hạ tầng dev — chạy trước tiên
docker compose up -d   # Postgres, Redis, RabbitMQ, MinIO — xem docker-compose.yml

# Backend (.NET 10 Clean Architecture — submodule api/, chứa src/, tests/)
cd api
dotnet restore GiapTech.BlouseHiding.slnx
dotnet build GiapTech.BlouseHiding.slnx
dotnet test GiapTech.BlouseHiding.slnx
# Migration (dotnet-ef cần cài: dotnet tool install -g dotnet-ef)
dotnet ef migrations add <Ten> --project src/Infrastructure --startup-project src/Web --output-dir Data/Migrations
dotnet ef database update --project src/Infrastructure --startup-project src/Web
# Chạy API (mặc định đọc ConnectionStrings:Postgres từ appsettings.json, đã khớp docker-compose.yml)
dotnet run --project src/Web
cd ..

# Frontend Client (web/ — Next.js, submodule)
cd web && npm install
npm run dev     # http://localhost:3000, redirect /vi mặc định
npm run build
npm run lint
cd ..

# Frontend Admin/Vận hành (web-admin/ — shadcn-admin, Vite + TanStack Router, submodule)
cd web-admin && npm install
npm run dev     # http://localhost:5173
npm run build   # build ra static assets, Caddy phục vụ thẳng — xem ADR-0008
npm run lint
cd ..
```

**Sau khi sửa code trong 1 submodule** (`api/`/`web/`/`web-admin/`): commit + push **ngay trong
submodule đó** trước, rồi về repo tổng `git add api` (hay `web`/`web-admin`) + commit để ghim con trỏ
— xem chi tiết quy trình 2 tầng ở mục 3.

**Lưu ý quan trọng khi sinh code backend:**
- Dùng **Mediator** (namespace `Mediator`) và **Mapster**, KHÔNG phải MediatR/AutoMapper — xem
  [ADR-0009](docs/kien-truc/adr/0009-mediator-mapster-thay-mediatr-automapper.md). `IPipelineBehavior<TMessage,TResponse>.Handle`
  trả `ValueTask<TResponse>`, nhận `MessageHandlerDelegate` (không phải `Task`/`RequestHandlerDelegate`
  như MediatR).
- `Domain.csproj` không có PackageReference nào — domain event (`BaseEvent`) không implement
  `INotification` trực tiếp, Infrastructure tự bọc lại lúc dispatch.
- Solution không dùng Aspire AppHost (đã gỡ — xung đột ADR-0002 self-host). Dev cục bộ dùng
  `docker-compose.yml` ở repo root, không phải `dotnet run --project src/AppHost`.

## 7. Skill nên dùng trong dự án này

| Skill | Khi nào |
|---|---|
| `init` | Sau khi scaffold solution thật — cập nhật mục 6 ở trên |
| `session-start-hook` | Đảm bảo session Claude Code mới tự restore/install môi trường |
| `update-config` | Sau khi có lệnh build/test quen thuộc, thêm allowlist giảm hỏi permission |
| `security-review` | Trước khi merge PR động vào CCHN/thanh toán/dữ liệu cá nhân |
| `review` | Mỗi PR trước khi merge |
| `taste-skill` (vendor tại [`.claude/skills/taste-skill/`](.claude/skills/taste-skill/), MIT — [tasteskill.dev](https://www.tasteskill.dev/)) | Khi code/thiết kế UI thật cho **`web/` (Client)** — landing, trang tin tuyển dụng, hồ sơ ứng viên, các trang marketing/khám phá việc làm — để tránh giao diện "AI slop" chung chung, đúng tinh thần mục 1 nguyên tắc số 3 ở [`docs/frontend/THIET-KE-GIAO-DIEN.md`](docs/frontend/THIET-KE-GIAO-DIEN.md). **Không** áp dụng cho `web-admin/` (Admin/Vận hành là dashboard nội bộ, ngoài phạm vi skill này tự khai báo — xem [ADR-0007](docs/kien-truc/adr/0007-shadcn-chuan-cho-admin-van-hanh.md)/[ADR-0008](docs/kien-truc/adr/0008-shadcn-admin-app-rieng-cho-admin-van-hanh.md)) |

## 8. Ngôn ngữ & phong cách viết tài liệu

- Tài liệu nghiệp vụ/kiến trúc viết **tiếng Việt** (đối tượng đọc chính là người Việt).
- Code, tên biến, tên bảng/cột, tên API viết **tiếng Anh** theo chuẩn ngành (đã áp dụng xuyên suốt ERD/API).
- Không thêm comment giải thích code hiển nhiên — chỉ comment khi có invariant/lý do không hiển nhiên
  (vd lý do 1 transaction phải lock bảng nào).

---

## 9. Quy ước tổ chức tài liệu — chống "file phình to không ai đọc"

> **Bài học có thật (2026-08-11):** `TIEN-DO-DU-AN.md` phình tới **2512 dòng / 238 KB** vì mọi nhật ký
> dồn vào 1 file. Hệ quả: yêu cầu "ưu tiên hoàn thiện UI trước" của người dùng **có** được ghi lại
> (dòng ~1247) nhưng **không lần nào được đọc tới** — dẫn tới lặp lại việc người dùng đã yêu cầu bỏ
> qua **8 lần**. Tài liệu quá dài thì không ai đọc, kể cả agent.

### 9.1 Phân loại tài liệu — quyết định cách tổ chức

| Loại | Đặc điểm | Cách tổ chức | Ví dụ |
|---|---|---|---|
| **Nhật ký / cộng dồn theo thời gian** | Chỉ thêm, không sửa cái cũ; càng ngày càng dài | **1 file / 1 ngày** trong thư mục riêng + 1 file **index ngắn** | [`docs/nghiep-vu/tien-do/`](docs/nghiep-vu/tien-do/) |
| **Tra cứu theo chủ đề** | Sửa tại chỗ khi code đổi; độ dài ổn định | **Giữ 1 file**, thêm **Mục lục** khi > 300 dòng | `ERD-CHI-TIET.md`, `API-DESIGN.md` |
| **Quyết định (ADR)** | Bất biến sau khi chốt | **1 file / 1 quyết định**, đánh số tăng dần | `docs/kien-truc/adr/` |
| **Chỉ dẫn/quy tắc** | Agent đọc mỗi phiên | Phải **ngắn**, ưu tiên đặt lên đầu | File này |

### 9.2 Ngưỡng cứng

- **> 400 dòng** → bắt buộc có **Mục lục** ở đầu file.
- **> 800 dòng** với tài liệu **cộng dồn** → bắt buộc **tách theo ngày/chủ đề**, giữ lại file index.
- **> 800 dòng** với tài liệu **tra cứu** → xem có tách được theo nhóm chủ đề không (vd tách ERD theo
  bounded context); nếu bản chất phải liền mạch thì giữ nguyên nhưng **mục lục phải đủ chi tiết** để
  nhảy thẳng tới mục cần đọc.
- File **index** (như `TIEN-DO-DU-AN.md`) phải **luôn dưới 150 dòng** — đọc hết được trong 1 lần.

### 9.3 Ghi nhật ký tiến độ

Xem hướng dẫn ở cuối [`docs/nghiep-vu/TIEN-DO-DU-AN.md`](docs/nghiep-vu/TIEN-DO-DU-AN.md).
Tóm tắt: **không viết nhật ký vào file index** — tạo `docs/nghiep-vu/tien-do/YYYY-MM-DD--mo-ta.md`,
rồi thêm 1 dòng vào bảng mục lục.

### 9.4 Yêu cầu/ràng buộc từ người dùng — ghi ngay, đúng chỗ

Khi người dùng nêu **ưu tiên, phạm vi, hoặc thứ tự làm việc**: ghi vào **CLAUDE.md mục 1b ngay lượt
đó**, không đợi. Ngữ cảnh hội thoại **sẽ mất** (phiên này đã cạn ngữ cảnh 7 lần); chỉ file trong repo
là sống sót. Ghi vào nhật ký ngày là **chưa đủ** — nhật ký không được đọc mỗi phiên.
