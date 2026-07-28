# GiapTech.BlouseHiding — Runbook vận hành

> Vì hạ tầng là **self-host VPS** (không có managed DB/snapshot của cloud lo sẵn), quy trình vận hành
> thủ công dưới đây là **bắt buộc**, không phải "nice to have". Xem kiến trúc hạ tầng ở
> [`HA-TANG-TRIEN-KHAI.md`](./HA-TANG-TRIEN-KHAI.md).

---

## 1. Backup

**Tần suất:** `pg_dump` toàn bộ database mỗi ngày (cron), giữ bản sao **ít nhất 14 ngày**; WAL archiving
liên tục để có thể point-in-time recovery trong ngày xảy ra sự cố.

**Vị trí lưu:** đẩy ra **ngoài VPS chính** ngay sau khi backup xong — VPS phụ hoặc object storage
ngoài (Backblaze B2/S3-compatible). Backup nằm cùng VPS với DB gốc = không phải backup thật.

**Diễn tập khôi phục (restore drill):** tối thiểu **mỗi quý** — restore bản backup gần nhất vào môi
trường staging, xác nhận ứng dụng chạy được với dữ liệu đó. Không đợi đến khi sự cố thật mới biết
backup có dùng được hay không.

## 2. Quy trình xử lý sự cố (Incident Response)

1. **Phát hiện**: Uptime Kuma cảnh báo qua Telegram/email khi service down.
2. **Phân loại mức độ**:
   - **Nghiêm trọng**: mất dữ liệu, không đăng nhập được, không xác thực CCHN được → xử lý ngay.
   - **Trung bình**: 1 tính năng phụ lỗi (chat, thông báo) → xử lý trong ngày.
   - **Nhẹ**: lỗi hiển thị không ảnh hưởng luồng chính → xử lý theo lịch thường.
3. **Xử lý**: xem log tập trung (Grafana Loki) theo `correlationId`, kiểm tra Sentry để thấy stack
   trace thực tế, không đoán mò.
4. **Khôi phục**: nếu do lỗi deploy → rollback container về image trước (`docker compose` tag trước
   đó). Nếu do lỗi dữ liệu → restore theo mục 1.
5. **Ghi nhận sau sự cố**: mỗi sự cố nghiêm trọng viết 1 đoạn ngắn "chuyện gì xảy ra – vì sao – đã sửa
   thế nào – làm gì để tránh lặp lại" trong `CHANGELOG.md` hoặc issue tracker — không để kiến thức mất
   theo trí nhớ người xử lý.

## 3. Quy trình deploy

1. PR merge vào nhánh `main` → GitHub Actions build image → push `ghcr.io`.
2. SSH vào VPS, `docker compose pull && docker compose up -d` (hoặc tự động hóa qua GitHub Actions SSH step).
3. Chạy migration EF Core **trước khi** container mới nhận traffic thật (xem
   [`../database/QUY-UOC-MIGRATION.md`](../database/QUY-UOC-MIGRATION.md)).
4. Theo dõi log 5–10 phút sau deploy trước khi coi là ổn định.

## 4. Rollback

- Container: `docker compose` giữ tag image cũ, chỉ cần đổi lại tag rồi `up -d`.
- Database: migration `down` nếu an toàn; nếu không (đã xóa cột/dữ liệu), restore từ backup gần nhất
  theo mục 1 — chấp nhận mất dữ liệu phát sinh giữa 2 lần backup, đây là lý do backup phải đủ tần suất.

## 5. Bảo trì định kỳ

| Việc | Tần suất |
|---|---|
| Kiểm tra dung lượng đĩa VPS | Hằng tuần (cảnh báo qua Uptime Kuma nếu > 80%) |
| Vá bảo mật hệ điều hành | Tự động (`unattended-upgrades`), kiểm tra log hằng tháng |
| Diễn tập restore backup | Hằng quý |
| Xoay vòng log cũ (log rotation) | Tự động qua cấu hình Loki retention |
| Rà soát chứng chỉ CCHN sắp hết hạn cần cảnh báo ứng viên | Job nền Hangfire chạy hằng ngày |

---

## Xem thêm
- Kiến trúc hạ tầng: [`HA-TANG-TRIEN-KHAI.md`](./HA-TANG-TRIEN-KHAI.md)
- Biến môi trường cần cấu hình: [`BIEN-MOI-TRUONG.md`](./BIEN-MOI-TRUONG.md)
