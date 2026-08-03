# ADR-0011: Tách backend/web/web-admin thành 3 repo Git riêng (submodule)

**Trạng thái:** Đã chấp nhận

## Bối cảnh
Từ đầu dự án, toàn bộ backend (.NET), `web/` (Client), `web-admin/` (Admin+Vận hành) và `docs/` nằm
trong **1 repo Git duy nhất** (monorepo) — mọi thay đổi, dù chỉ chạm 1 phần, đều đi qua cùng 1 lịch sử
commit, 1 danh sách PR, 1 bộ quyền truy cập.

Người dùng muốn tách 3 ứng dụng thành 3 nguồn (source) độc lập — có thể do nhu cầu quản lý quyền truy
cập riêng theo team, lịch sử commit gọn hơn khi chỉ quan tâm 1 phần, hoặc chuẩn bị cho khả năng các
phần phát triển với tốc độ/team khác nhau trong tương lai — nhưng vẫn muốn giữ 1 thư mục tổng chứa
`docs/`, `CLAUDE.md`, `docker-compose.yml`, `.gitignore` chung để không phải nhân bản tài liệu kiến
trúc/nghiệp vụ ra 3 nơi.

**Không nhầm lẫn với ADR-0001**: ADR-0001 nói về kiến trúc *code* bên trong backend (Modular Monolith,
không tách microservice). Quyết định ở đây chỉ về *tổ chức repo Git* — backend vẫn là 1 Modular
Monolith duy nhất như ADR-0001 đã chốt, chỉ khác là code đó giờ nằm ở repo Git riêng
(`GiapTech.BlouseHiding.Api`) thay vì thư mục `src/`+`tests/` của repo tổng.

## Quyết định

### 1. Mô hình: git submodule (không phải 3 repo rời rạc không liên kết)
Repo tổng (`GiapTech.BlouseHiding`, giữ nguyên tên) chỉ còn:
```
CLAUDE.md, README.md, CHANGELOG.md, CONTRIBUTING.md, SECURITY.md
docs/                  — toàn bộ tài liệu kiến trúc/nghiệp vụ/backend/frontend/hạ tầng
docker-compose.yml      — hạ tầng dev (Postgres/Redis/RabbitMQ/MinIO)
.devcontainer/          — môi trường dev chung (VS Code devcontainer)
.editorconfig, .gitignore, .github/ — dùng chung/còn lại cho repo tổng
api/                    — submodule → GiapTech.BlouseHiding.Api
web/                    — submodule → GiapTech.BlouseHiding.Web
web-admin/              — submodule → GiapTech.BlouseHiding.WebAdmin
```
Chọn submodule (không phải 3 repo hoàn toàn rời rạc, mỗi người tự `git clone` riêng lẻ không liên
quan) vì:
- **Ghim đúng 1 phiên bản (commit) cụ thể** của mỗi app vào repo tổng — biết chính xác "phiên bản nào
  của backend đi cùng phiên bản nào của web/web-admin" tại 1 thời điểm, giống cách tag release hoạt
  động nhưng tự động theo git.
- `git clone --recurse-submodules` (hoặc `git submodule update --init` sau khi clone thường) lấy được
  đủ cả 3 app + docs trong 1 lệnh — không mất khả năng "clone 1 lần có tất cả" của monorepo cũ.
- Có thể `cd api && git checkout <branch-khac>` để thử 1 nhánh backend khác mà không ảnh hưởng con trỏ
  đã ghim ở repo tổng cho tới khi chủ động `git add api` commit lại — tách biệt rõ "đang thử" và "đã
  chốt".

### 2. Repo mới
| Repo | Nội dung | URL |
|---|---|---|
| `GiapTech.BlouseHiding.Api` | `src/`, `tests/`, `*.slnx`, `global.json`, `Directory.Build.props`, `Directory.Packages.props`, `.aspire/`, `.editorconfig` riêng | https://github.com/minhtungle/GiapTech.BlouseHiding.Api |
| `GiapTech.BlouseHiding.Web` | Toàn bộ `web/` cũ, `.editorconfig` riêng | https://github.com/minhtungle/GiapTech.BlouseHiding.Web |
| `GiapTech.BlouseHiding.WebAdmin` | Toàn bộ `web-admin/` cũ, `.editorconfig` riêng | https://github.com/minhtungle/GiapTech.BlouseHiding.WebAdmin |

Lịch sử commit của từng phần **được giữ nguyên** (dùng `git-filter-repo`, không squash) — mỗi repo mới
chỉ chứa các commit thực sự chạm tới phần tương ứng, tính từ đầu dự án.

### 3. Tên thư mục submodule ở repo tổng
Đặt `api/` (không phải `src-api/` hay giữ `src/`+`tests/` tách rời) để 1 submodule = 1 app rõ ràng;
`web/` và `web-admin/` giữ đúng tên cũ để không phải sửa lại mọi đường dẫn tham chiếu trong `docs/`
(rất nhiều file `docs/*.md` đã trỏ `web/`, `web-admin/` theo tên này).

### 4. `.editorconfig`
Copy nguyên bản vào cả 3 repo con (không chỉ giữ ở repo tổng) — để mở riêng từng repo trong editor vẫn
có đúng style rule, không phụ thuộc việc có mở kèm repo tổng hay không.

### 5. `.aspire/`, `Directory.Build.props`, `Directory.Packages.props` → theo backend
Cả 3 chỉ .NET dùng, không liên quan `docs/` hay 2 frontend — chuyển hẳn vào `GiapTech.BlouseHiding.Api`.

### 6. `.devcontainer/` → giữ ở repo tổng
Mô tả môi trường dev chung cho cả workspace (mở cả 3 app cùng lúc qua VS Code) — không thuộc riêng 1
app nào, giữ ở repo tổng hợp lý hơn nhân bản vào 3 nơi.

### 7. CI/CD: mỗi repo con tự có workflow riêng
`backend.yml`/`web.yml`/`web-admin.yml` **di chuyển vào chính repo con** tương ứng (không giữ ở repo
tổng) — để PR mở trực tiếp ở `GiapTech.BlouseHiding.Api`/`.Web`/`.WebAdmin` tự chạy build/test ngay,
không phải đẩy commit lên repo tổng (cập nhật con trỏ submodule) rồi mới thấy kết quả CI. Repo tổng từ
nay **không còn workflow build/test app nào** — chỉ còn tài liệu, không có gì để build ở đó.

## Phương án đã cân nhắc
- **3 repo hoàn toàn rời rạc, không submodule**: loại bỏ — mất khả năng biết "bộ 3 phiên bản nào đi
  cùng nhau", và người mới phải tự nhớ clone đủ 3 nơi thay vì 1 lệnh.
- **Giữ CI ở repo tổng, sửa checkout thêm `submodules: true`**: loại bỏ — nghĩa là PR ở repo con không
  tự test được, phải đợi ai đó cập nhật con trỏ submodule ở repo tổng rồi CI mới chạy — làm chậm hẳn
  luồng làm việc thường ngày so với trước khi tách.
- **Squash lịch sử commit khi tách** (mỗi repo con bắt đầu từ 1 commit "initial import"): loại bỏ —
  mất hẳn lịch sử/lý do các quyết định trước đó (nhiều commit message giải thích rất chi tiết "vì sao"
  của từng thay đổi, xem cách viết message xuyên suốt `TIEN-DO-DU-AN.md`) — quá đắt để đánh đổi chỉ để
  gọn hơn.

## Hệ quả
- (+) Mỗi app có lịch sử commit riêng, gọn hơn khi chỉ quan tâm 1 phần.
- (+) PR/quyền truy cập có thể phân theo repo nếu cần (vd hạn chế ai được duyệt PR backend khác ai
  được duyệt PR frontend) — chưa cấu hình ngay, nhưng cấu trúc đã sẵn sàng cho việc đó.
- (+) CI mỗi repo chạy nhanh hơn (chỉ build đúng 1 app, không path-filter phức tạp như trước).
- (−) **Thao tác hàng ngày phức tạp hơn đáng kể**: sau khi sửa code trong `api/`/`web/`/`web-admin/`,
  phải `commit` + `push` **ở đúng repo con đó**, sau đó về repo tổng `git add api` (hoặc `web`/
  `web-admin`) + `commit` để ghim con trỏ mới — 1 thay đổi giờ cần ít nhất 2 commit ở 2 repo khác nhau
  thay vì 1 commit duy nhất như monorepo cũ. Dễ quên bước cập nhật con trỏ ở repo tổng, dẫn tới repo
  tổng "chậm" hơn code thật 1-2 commit nếu không cẩn thận.
- (−) 1 thay đổi cross-cutting (đụng cả backend + 1 frontend cùng lúc, ví dụ thêm API mới + nối vào
  UI trong 1 lượt làm việc) giờ tách thành 2 PR ở 2 repo riêng, mất khả năng review "1 PR thấy hết thay
  đổi liên quan" như trước.
- (−) Người mới clone lần đầu cần biết cả lệnh `git submodule update --init --recursive` (hoặc
  `--recurse-submodules` lúc clone) — thêm 1 bước so với `git clone` đơn giản trước đây; cần cập nhật
  hướng dẫn onboarding.
- Cập nhật liên quan: `CLAUDE.md` mục 3 (cấu trúc repo) và mục 6 (lệnh build/test — thêm bước
  `git submodule update --init` và lưu ý cách commit 2 tầng).
