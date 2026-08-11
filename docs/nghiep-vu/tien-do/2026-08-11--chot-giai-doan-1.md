# 2026-08-11 — Chốt Giai đoạn 1 (MVP) — đủ 8 bước checklist

> Mục lục mọi ngày: [`../TIEN-DO-DU-AN.md`](../TIEN-DO-DU-AN.md)

---

## CHỐT GIAI ĐOẠN 1 — MVP (2026-08-11)

Chạy đủ **8 bước** checklist ở mục 1, không bỏ bước nào. Kết quả từng bước:

**1. Tính năng trong phạm vi** — 36/41 ✅. 5 mục còn lại, mỗi mục có lý do đã chốt, **không phải quên**:
| Mục | Lý do dời |
|---|---|
| OAuth Google, OAuth Zalo | Người dùng loại khỏi phạm vi |
| Thông báo email | Người dùng loại khỏi phạm vi; hạ tầng `INotificationEmailSender` đã có, thiếu provider thật |
| Cổng thanh toán tự động | Hoãn theo [ADR-0003](../../kien-truc/adr/0003-hoan-cong-thanh-toan-tu-dong.md) — dùng `manual_transfer` |
| Full-text search (`pg_trgm`/`tsvector`) | LINQ/EF Core đủ cho lượng dữ liệu MVP, tối ưu khi có traffic thật |

**2. Build sạch** — backend `dotnet build`: **0 warning / 0 error**; `web/` `next build`: 136 trang tĩnh
sinh xong; `web-admin/` `vite build`: xong. Cả 3 `tsc --noEmit` sạch.

**3. Test + lint** — backend **157 functional + 3 unit**; `web-admin/` **95**; tổng **255 test pass**.
`npm run lint` sạch ở cả 2 frontend.

**4. Verify chạy thật** — viết `scripts/smoke-test.sh` (giữ lại trong repo để chốt các giai đoạn sau),
chạy **45 kiểm tra** qua HTTP thật trên **dữ liệu mới hoàn toàn** (email/tổ chức gắn timestamp, không
dùng lại dữ liệu seed từ các đợt trước): ứng viên đăng ký → hồ sơ → học vấn/kinh nghiệm → CCHN → CV
Builder + CV file → tra cứu tin/tổ chức → nộp hồ sơ bằng CV đã lưu → nhận thông báo; NTD đăng ký → tổ
chức → giấy tờ pháp lý → tin tuyển dụng → ATS → chuyển giai đoạn; Vận hành duyệt tổ chức/CCHN/tin →
cộng Credit → hoàn Credit tranh chấp; **6 kiểm tra RBAC** chặn vượt quyền. **45/45 đạt.**

> Lần chạy đầu 12 kiểm tra "fail" — **tất cả do script tôi viết sai**, không phải sản phẩm: mọi handler
> khai kiểu `Task` (không `IResult`) trả **HTTP 200 chứ không 204**, và `VerifyLicense`/`ModerateJob`
> nhận `{approved: bool}` chứ không `{action}`. Nhận ra vì "hoàn Credit" báo fail nhưng dòng ngay sau
> "số dư về lại 50" lại đạt — tức là hoàn đã thành công thật. Đã ghi lưu ý này vào header script.

**5. Tài liệu khớp code** — đối chiếu tự động: **103 endpoint trong code = 103 endpoint trong
`API-DESIGN.md`**. Sửa 2 chỗ lệch thật: (a) `DELETE /applications/{id}` (**ứng viên rút đơn**) đã
implement nhưng **thiếu hẳn** trong tài liệu; (b) dòng `/ops/catalog` ghi gộp "CRUD" nhưng code chỉ có
POST/PUT — **không có DELETE** (xóa danh mục đang được tham chiếu sẽ làm mồ côi dữ liệu).

**6. Rà lỗ hổng** — cách rà: liệt kê mọi `*Query.cs` **không** có `[Authorize]` và **không** kiểm
`isMember`/`UserId`. Ra 9 query; 8 là dữ liệu công khai đúng thiết kế (danh mục, tin `Published`, tổ
chức `Verified`, đánh giá đã duyệt). Còn 1 là **lỗ hổng thật, đã bịt**:

> `GET /organizations/{id}` là endpoint **Public** (trang công khai cơ sở y tế mà ứng viên xem) nhưng
> trả cả `rejectReason` — **ghi chú nội bộ của Vận hành** khi từ chối xác thực, kiểu "nghi giấy phép
> làm giả". Bất kỳ ai biết `organizationId` đều đọc được nhận xét nội bộ về tổ chức đó. Tài liệu ghi
> đúng *mục đích* field (để NTD biết lý do bị từ chối) nhưng bỏ sót việc nó **lộ ra công khai**. Sửa:
> chỉ trả cho thành viên tổ chức; `UserId` nullable vì endpoint vẫn Public. Không bỏ hẳn field vì NTD
> thật sự cần nó để biết phải sửa gì.

Đây là **lỗ hổng thứ 3 cùng một dạng** trong dự án (trước đó: `GET /organizations/{id}/documents` cho
người ngoài tải giấy phép; `SearchCandidates` từng lộ email chưa unlock). Dạng chung: **command *ghi*
được kiểm quyền cẩn thận, query *đọc* thì bỏ sót**. Rút thành quy tắc rà cho các giai đoạn sau.

**7. Commit + push** — 3 submodule + repo tổng đã push, không còn thay đổi treo.

**8. Log này.**

---

### Trạng thái khi chốt

| Hạng mục | Số liệu |
|---|---|
| Endpoint | 103 (23 endpoint group) |
| Migration | 16 |
| Test | 255 (157 functional + 3 unit backend, 95 web-admin) |
| File code | 324 `.cs` (src) · 122 `web/` · 207 `web-admin/` |
| Ngôn ngữ | 6 (vi/en dịch thật; ja/zh/ko/es placeholder chờ dịch — ADR-0010) |

**Còn chặn việc mở cho người dùng thật** (không phải thiếu code, mà thiếu **quyết định/nhà cung cấp**):
1. **Email/SMS thật** — chưa có provider, nên OTP đăng ký hiện phải lấy từ DB. Đây là thứ chặn nặng nhất.
2. **Cổng thanh toán** — đang `manual_transfer`, Vận hành đối soát tay (đúng ADR-0003, nhưng không mở rộng được).
3. **Rà responsive + dark mode + WCAG AA** — mục ⬜ duy nhất còn lại của Giai đoạn 0, chưa làm thành đợt riêng.

### Giai đoạn 2 — Hoàn thiện

*(Chưa bắt đầu)*

### Giai đoạn 3 — Mở rộng

*(Chưa bắt đầu)*
