# GiapTech.BlouseHiding — Môi trường dev cục bộ: cổng & tài khoản

> Tra cứu nhanh khi chạy/test dự án trên máy dev — cổng nào chạy dịch vụ gì, tài khoản đăng nhập mặc
> định là gì. **Chỉ áp dụng cho môi trường dev cục bộ** (Docker Compose ở repo root), không phải giá
> trị production. Danh sách *tên biến* cần cấu hình xem
> [`BIEN-MOI-TRUONG.md`](./BIEN-MOI-TRUONG.md); quy trình vận hành production xem
> [`VAN-HANH-RUNBOOK.md`](./VAN-HANH-RUNBOOK.md). Lệnh khởi động đầy đủ xem
> [`../../CLAUDE.md`](../../CLAUDE.md) mục 6.
>
> ⚠️ Từ [ADR-0011](../kien-truc/adr/0011-tach-3-repo-git-submodule.md), backend/`web/`/`web-admin/` là
> **git submodule** — clone phải kèm `--recurse-submodules`, xem mục 0.

---

## 0. Clone lần đầu

```bash
git clone --recurse-submodules https://github.com/minhtungle/GiapTech.BlouseHiding.git
cd GiapTech.BlouseHiding
```

Nếu đã clone thường (quên `--recurse-submodules`) — 3 thư mục `api/`, `web/`, `web-admin/` sẽ trống,
chạy thêm:
```bash
git submodule update --init --recursive
```

## 1. Ứng dụng

Toàn bộ lệnh dưới đây chạy **từ thư mục gốc repo tổng** (`GiapTech.BlouseHiding/`) — `cd` vào submodule
tương ứng trước khi chạy `dotnet run`/`npm run dev`.

| Ứng dụng | Lệnh chạy | URL | Ghi chú |
|---|---|---|---|
| Backend API (.NET) | `cd api && dotnet run --project src/Web` | http://localhost:5256 (theo `launchSettings.json`) | ⚠️ **Không khớp** biến `NEXT_PUBLIC_API_BASE_URL`/`VITE_API_BASE_URL` mặc định của 2 frontend (đang trỏ `5100`, xem mục 3) — chạy `ASPNETCORE_URLS=http://localhost:5100 dotnet run --project src/Web --no-launch-profile` để khớp, hoặc sửa `.env` của frontend trỏ đúng `5256`. Xem [`scalar`](http://localhost:5256/scalar) để có UI thử API (OpenAPI/Scalar tự bật ở Development) |
| `web/` — Client (Next.js) | `cd web && npm run dev` | http://localhost:3000 | Next.js mặc định, redirect `/vi` |
| `web-admin/` — Admin (NTD) + Vận hành (Vite) | `cd web-admin && npm run dev` | http://localhost:5173 | Vite mặc định |

Chạy song song bằng 3 cửa sổ terminal riêng (mỗi app 1 cửa sổ, không tắt cửa sổ nào giữa lúc test).

## 2. Hạ tầng (Docker Compose — `docker-compose.yml` ở repo root)

| Dịch vụ | Port | User | Password | Ghi chú |
|---|---|---|---|---|
| **Postgres** | `5432` | `blousehiding` | `blousehiding_dev` | Database: `blousehiding`. Connection string: `postgresql://blousehiding:blousehiding_dev@localhost:5432/blousehiding` |
| **Redis** | `6379` | — | — | Không bật auth ở dev |
| **RabbitMQ (AMQP)** | `5672` | `blousehiding` | `blousehiding_dev` | ⚠️ Không phải mặc định `guest`/`guest` của image gốc — đã override qua `RABBITMQ_DEFAULT_USER`/`_PASS` |
| **RabbitMQ Management UI** | http://localhost:15672 | `blousehiding` | `blousehiding_dev` | Cùng tài khoản AMQP |
| **MinIO API (S3-compatible)** | `9000` | `blousehiding` | `blousehiding_dev` | ⚠️ Không phải mặc định `minioadmin`/`minioadmin` — đã override qua `MINIO_ROOT_USER`/`_PASSWORD` |
| **MinIO Console** | http://localhost:9001 | `blousehiding` | `blousehiding_dev` | Cùng tài khoản API |

Khởi động: `docker compose up -d` ở repo root. Kiểm tra container khỏe mạnh:
`docker compose ps` (cột `STATUS` phải là `healthy`).

## 3. Cổng backend mà 2 frontend đang trỏ tới

| File | Biến | Giá trị hiện tại |
|---|---|---|
| `web/.env` (hoặc `.env.local`) | `NEXT_PUBLIC_API_BASE_URL` | `http://localhost:5100/api/v1` |
| `web-admin/.env` | `VITE_API_BASE_URL` | `http://localhost:5100/api/v1` |

Cả 2 đều trỏ **`5100`**, trong khi `dotnet run --project src/Web` (chạy từ trong `api/`) mặc định chạy
ở **`5256`** (theo `api/src/Web/Properties/launchSettings.json`). Đây là gap có sẵn trong repo, chưa
được thống nhất — chọn 1 trong 2 cách khi chạy backend cho khớp:
- Ép backend chạy đúng `5100` (từ trong `api/`): `ASPNETCORE_URLS=http://localhost:5100 dotnet run --project src/Web --no-launch-profile`
  (cờ `--no-launch-profile` bỏ qua `applicationUrl` trong `launchSettings.json`, nhưng cũng bỏ qua
  `ASPNETCORE_ENVIRONMENT=Development` — log vẫn hiện OTP giả lập bình thường, không ảnh hưởng test).
- Hoặc sửa `.env`/`.env.local` của 2 frontend trỏ về `5256` rồi chạy backend bình thường qua
  `dotnet run --project src/Web` (từ trong `api/`).

## 4. Tài khoản đăng nhập

**Không có tài khoản seed sẵn nào trong migration hoặc fixture** — mọi tài khoản (candidate/employer/
admin/moderator) đều phải tự tạo qua `POST /api/v1/auth/register` rồi xác thực OTP. OTP dùng driver
**giả lập nội bộ** (không gửi email/SMS thật) — mã 6 số được ghi ra log console/terminal của backend,
tìm theo dòng `OTP giả lập (...) cho <email>: <mã>`.

Tạo tài khoản test nhanh qua curl:
```bash
# 1. Đăng ký (role: candidate hoặc employer)
curl -X POST http://localhost:5100/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"demo@test.local","password":"Testing1234!","role":"employer"}'

# 2. Lấy mã OTP từ log backend (terminal đang chạy dotnet run, hoặc file log nếu redirect ra file)
#    dòng dạng: "OTP giả lập (Register) cho demo@test.local: 123456"

# 3. Xác thực OTP để kích hoạt tài khoản
curl -X POST http://localhost:5100/api/v1/auth/verify-otp \
  -H "Content-Type: application/json" \
  -d '{"email":"demo@test.local","code":"123456"}'
```

Sau khi xác thực, đăng nhập bình thường qua UI (`web/`: `/auth/login`; `web-admin/`: `/sign-in`) bằng
email + password vừa tạo.

**Role `admin`/`moderator`** (để test màn hình Vận hành) không tự gán được qua UI đăng ký công khai
(chỉ chọn được `candidate`/`employer`) — phải gán trực tiếp trong DB sau khi tài khoản đã tồn tại:
```sql
-- Xem AspNetRoles để lấy đúng Id role cần gán
SELECT * FROM "AspNetRoles";
-- Gán role cho user (thay UserId/RoleId thật)
INSERT INTO "AspNetUserRoles" ("UserId", "RoleId") VALUES ('<user-id>', '<role-id>');
```

**Employer cần tạo tổ chức (organization) trước khi thấy dữ liệu** — tài khoản `employer` mới đăng ký
xong chưa có tổ chức nào, cần tạo qua luồng onboarding trong UI hoặc `POST /api/v1/organizations`
trước khi các trang Dashboard/Jobs/Credit/Members ở `web-admin/` có dữ liệu để hiển thị.

## 5. Kiểm tra nhanh mọi thứ đã chạy

```bash
docker compose ps                                                    # 4 container "healthy"
curl -s http://localhost:5100/api/v1/catalog/specialties -o /dev/null -w "%{http_code}\n"  # 200
curl -s http://localhost:3000 -o /dev/null -w "%{http_code}\n"        # 200 (web/)
curl -s http://localhost:5173 -o /dev/null -w "%{http_code}\n"        # 200 (web-admin/)
```
