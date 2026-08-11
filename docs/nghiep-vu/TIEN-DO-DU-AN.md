# GiapTech.BlouseHiding — Tiến độ dự án & Quy trình chốt giai đoạn

> Tài liệu này khác [`DANH-SACH-TINH-NANG.md`](./DANH-SACH-TINH-NANG.md): file đó là **checklist tính
> năng** (còn gì phải làm), file này là **nhật ký tiến độ** (đã làm gì, đã verify chưa, giai đoạn đã
> đủ điều kiện chốt chưa) — cập nhật khi **kết thúc một giai đoạn**, không phải mỗi commit.

---

## 1. Quy trình chốt 1 giai đoạn

Trước khi chuyển sang giai đoạn tiếp theo, chạy qua đủ checklist này — **không tự ý bỏ qua bước nào**.
Nếu có mục không đạt, ghi rõ lý do + kế hoạch xử lý vào nhật ký (mục 3) thay vì im lặng bỏ qua.

- [ ] **Tất cả tính năng** trong phạm vi giai đoạn ở `DANH-SACH-TINH-NANG.md` đã chuyển ✅ — mục nào
      còn ⬜/🟨 phải có lý do rõ ràng (dời sang giai đoạn sau, không phải quên).
- [ ] **Build sạch** — backend (`dotnet build`, 0 warning/error) + `web/` + `web-admin/`
      (`npm run build`, 0 error).
- [ ] **Test suite chạy pass** — `dotnet test` (unit + functional/integration nếu có), `npm run lint`
      cả 2 frontend.
- [ ] **Verify chạy thật, không chỉ dựa vào build** — dev server thật + gọi API thật (curl/browser),
      không chấp nhận "chắc là chạy được" khi chưa tự tay kiểm tra.
- [ ] **Tài liệu khớp code thật** — ERD/API-DESIGN.md/DANH-SACH-TINH-NANG.md không lệch so với những gì
      vừa implement (CLAUDE.md mục 4 quy tắc 8).
- [ ] **Không còn lỗ hổng nghiệp vụ nghiêm trọng** chưa xử lý hoặc chưa ghi nhận rõ (rà theo kiểu đợt
      audit đã làm trước Giai đoạn 0.2 — xem mục 3).
- [ ] **Đã commit + push** toàn bộ, không còn thay đổi treo lơ lửng.
- [ ] **Ghi log tổng kết vào mục 3 của tài liệu này** trước khi bắt đầu công việc của giai đoạn tiếp
      theo — nhật ký phải nêu rõ: đã làm gì, verify bằng cách nào, còn thiếu gì, quyết định gì đã chốt.

---

## 2. Trạng thái tổng quan (cập nhật mới nhất lên đầu)

> ⚠️ Bảng này từng lệch rất xa thực tế (ghi `web-admin/` chưa làm, export PDF CV còn thiếu,
> Jobs/ATS/Credit chưa làm — tất cả đã xong từ lâu). Cập nhật lại 2026-08-10 sau khi rà bằng code
> thật. Khi hoàn thành 1 khối việc, sửa bảng này **cùng lúc** với nhật ký mục 3.

| Giai đoạn | Trạng thái | Ghi chú ngắn |
|---|---|---|
| 0.1 — UI Shell | ✅ Xong | Cả 2 app dựng đủ màn hình, đã dọn hết tàn dư template. Còn 1 mục dời sang sau: rà responsive/dark mode/WCAG AA thành 1 đợt riêng |
| 0.2 — Backend & hạ tầng | ✅ Xong | Clean Architecture + Docker Compose + CI/CD 3 repo + danh mục (đọc/ghi) + middleware `Accept-Language` |
| 1 — MVP | ✅ **Chốt 2026-08-11** (đủ 8 bước checklist mục 1 — xem log "Chốt Giai đoạn 1" ở mục 3) | Backend 23 endpoint group / 16 migration / 151 test. `web/` + `web-admin/` đã nối API thật toàn bộ luồng chính: Identity, hồ sơ ứng viên (CCHN/chuyên khoa/học vấn/kinh nghiệm/CV Builder/CV file/xuất PDF), tra cứu tin + tổ chức, nộp hồ sơ, ATS Kanban, Credit + unlock hồ sơ, hàng đợi duyệt Vận hành, thanh toán `manual_transfer`. **Còn**: OAuth Google/Zalo + thông báo email — cả 2 người dùng đã loại khỏi phạm vi |
| 2 — Hoàn thiện | 🟨 Chớm bắt đầu | 3 mục 🟨 làm ở mức đơn giản đã chốt (matching theo chuyên khoa, test năng lực, máy tính lương); OpenSearch/SignalR/mobile chưa bắt đầu |
| 3 — Mở rộng | ⬜ Chưa bắt đầu | |

---

## 3. Nhật ký tiến độ — mỗi ngày 1 file

> **Không viết nhật ký vào file này.** Mỗi ngày làm việc là **1 file riêng** trong
> [`tien-do/`](./tien-do/), đặt tên `YYYY-MM-DD--mo-ta-ngan.md`. File này chỉ giữ **quy trình chốt
> giai đoạn (mục 1)**, **trạng thái tổng quan (mục 2)** và **mục lục dưới đây** — luôn ngắn để đọc hết
> được trong 1 lần.
>
> **Vì sao tách:** trước 2026-08-11 toàn bộ nhật ký nằm chung 1 file **2512 dòng / 238 KB**. Hệ quả
> thật: yêu cầu "ưu tiên hoàn thiện UI trước" của người dùng *có* được ghi lại, nhưng nằm ở dòng
> ~1247 nên không lần nào được đọc tới — dẫn tới lặp lại việc người dùng đã yêu cầu bỏ qua **8 lần**.
> Tài liệu quá dài thì không ai đọc, kể cả agent.

| Ngày | Nội dung | File |
|---|---|---|
| 2026-08-02 | Giai đoạn 0 — UI Shell + Backend & hạ tầng | [`2026-08-02--giai-doan-0-khoi-tao.md`](./tien-do/2026-08-02--giai-doan-0-khoi-tao.md) |
| 2026-08-03 | Identity thật, danh mục, hồ sơ ứng viên, tin tuyển dụng, ATS, Credit, nối 2 frontend | [`2026-08-03--giai-doan-1-nen-tang.md`](./tien-do/2026-08-03--giai-doan-1-nen-tang.md) |
| 2026-08-04 | Notifications, Report/Ops, upload MinIO, quản lý việc làm, đồng bộ locale | [`2026-08-04--thong-bao-upload-payments.md`](./tien-do/2026-08-04--thong-bao-upload-payments.md) |
| 2026-08-05 | Chi tiết ATS, hồ sơ ứng viên/tổ chức, tìm & mở hồ sơ chủ động | [`2026-08-05--ats-chi-tiet-ho-so-to-chuc.md`](./tien-do/2026-08-05--ats-chi-tiet-ho-so-to-chuc.md) |
| 2026-08-10 (p1) | 4 bounded context Ops, đánh giá cơ sở y tế, Giai đoạn 2 | [`2026-08-10--phan-1-ops-va-giai-doan-2.md`](./tien-do/2026-08-10--phan-1-ops-va-giai-doan-2.md) |
| 2026-08-10 (p2) | Hoàn thiện giao diện, CV, phân trang, học vấn/kinh nghiệm, hoàn Credit | [`2026-08-10--phan-2-hoan-thien-ui-va-cv.md`](./tien-do/2026-08-10--phan-2-hoan-thien-ui-va-cv.md) |
| 2026-08-11 | **Chốt Giai đoạn 1 (MVP)** — đủ 8 bước checklist | [`2026-08-11--chot-giai-doan-1.md`](./tien-do/2026-08-11--chot-giai-doan-1.md) |

### Cách ghi nhật ký ngày mới

1. Tạo `tien-do/YYYY-MM-DD--mo-ta-ngan.md` (dùng ngày thật, không đoán).
2. Mở đầu bằng `# YYYY-MM-DD — <mô tả>` + dòng trỏ về mục lục này.
3. Ghi: **đã làm gì · verify bằng cách nào · còn thiếu gì · quyết định gì đã chốt**.
4. Thêm 1 dòng vào bảng mục lục trên, và cập nhật **mục 2 (trạng thái tổng quan)** nếu trạng thái
   giai đoạn đổi.
5. **Nếu 1 file ngày vượt ~400 dòng** — tách tiếp theo buổi/chủ đề (`YYYY-MM-DD--phan-2-....md`).
