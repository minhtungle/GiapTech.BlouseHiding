# GiapTech.BlouseHiding — Quy ước Migration

> Schema đầy đủ xem [`ERD-CHI-TIET.md`](./ERD-CHI-TIET.md). Tài liệu này quy định **cách thay đổi**
> schema đó một cách an toàn, có kiểm soát — quan trọng vì hạ tầng self-host không có snapshot managed
> để cứu khi migration lỗi (xem [`../ha-tang/HA-TANG-TRIEN-KHAI.md`](../ha-tang/HA-TANG-TRIEN-KHAI.md)).

---

## 1. Công cụ

**EF Core Migrations** là nguồn sự thật duy nhất cho schema. Không sửa schema production bằng tay
(qua psql/GUI) — mọi thay đổi phải đi qua migration có commit trong repo.

## 2. Quy ước đặt tên

```
dotnet ef migrations add <YyyyMMddHHmm>_<MoTaNganBangTiengAnh>
```
Ví dụ: `202607281030_AddLicenseVerifyStatus`, `202607281200_CreateJobPackagesTable`.
Tiền tố thời gian đảm bảo thứ tự áp dụng đúng khi nhiều người cùng tạo migration song song.

## 3. Bắt buộc trước khi merge migration

1. **Đọc SQL sinh ra** (`dotnet ef migrations script`) — không merge nếu chưa xem qua, đặc biệt các
   lệnh `DROP COLUMN`/`DROP TABLE`/đổi kiểu dữ liệu có khả năng mất dữ liệu.
2. Migration làm thay đổi **cột đã có dữ liệu** (đổi kiểu, thêm NOT NULL) phải kèm bước backfill dữ
   liệu an toàn (2 bước: thêm cột nullable → backfill → đổi NOT NULL ở migration sau), không làm 1 bước.
3. Test migration chạy được trên bản sao dữ liệu staging trước khi áp dụng production (không test chay
   trên schema rỗng rồi coi là xong).
4. Bảng chứa dữ liệu nhạy cảm (`licenses`, `organizations`, `payments`) — thay đổi schema phải ghi chú
   trong PR lý do, vì ảnh hưởng trực tiếp tới luồng xác thực CCHN/thanh toán.

## 4. Quy trình áp dụng production (self-host VPS, không có managed DB)

1. Backup thủ công trước khi chạy migration (xem `../ha-tang/VAN-HANH-RUNBOOK.md` mục backup).
2. Chạy migration trong cửa sổ bảo trì đã thông báo trước (không chạy migration đổi cấu trúc lớn vào
   giờ cao điểm ứng viên/NTD đang dùng).
3. Có kế hoạch rollback: giữ migration "down" hoạt động được, hoặc restore từ backup nếu down script
   không an toàn (một số thay đổi — như xóa cột — không thể rollback bằng down script).

## 5. Danh mục dữ liệu (seed data)

Danh mục chuẩn hóa (`specialties`, `locations`, `job_packages`) seed qua migration riêng hoặc script
idempotent chạy sau migration — không hardcode trong code nghiệp vụ, không tạo tay qua GUI production.
