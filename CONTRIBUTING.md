# Đóng góp vào GiapTech.BlouseHiding

> Áp dụng cho cả người và AI agent đóng góp code/tài liệu vào repo này. Xem thêm quy tắc kiến trúc bắt
> buộc ở [`CLAUDE.md`](./CLAUDE.md).

## Git flow

- Nhánh chính: `main` — luôn ở trạng thái deploy được.
- Nhánh tính năng: `feature/<mo-ta-ngan>` (vd `feature/verify-license-flow`).
- Nhánh sửa lỗi: `fix/<mo-ta-ngan>`.
- Không commit thẳng vào `main` — luôn qua Pull Request.

## Commit message

Theo [Conventional Commits](https://www.conventionalcommits.org/):
```
<type>(<scope>): <mô tả ngắn>

<mô tả chi tiết nếu cần — giải thích TẠI SAO, không chỉ CÁI GÌ>
```
`type`: `feat`, `fix`, `docs`, `refactor`, `test`, `chore`, `perf`.
Ví dụ: `feat(licenses): thêm luồng duyệt CCHN thủ công cho đội Vận hành`.

## Quy trình Pull Request

1. Nhánh feature/fix nhỏ, tập trung 1 việc — dễ review, dễ rollback.
2. Mô tả PR nêu rõ: vấn đề gì, giải quyết thế nào, có ảnh hưởng schema/API không.
3. Nếu đổi schema CSDL hoặc endpoint API → **PR phải kèm cập nhật tài liệu tương ứng**
   (`docs/database/ERD-CHI-TIET.md`, `docs/backend/API-DESIGN.md`) — không tách PR riêng "cập nhật tài
   liệu sau".
4. Bắt buộc có test cho use case mới (xem `docs/backend/KIEN-TRUC-BACKEND.md` mục 4).
5. Chạy skill `review` trước khi merge; chạy thêm `security-review` nếu PR động vào CCHN/thanh toán/dữ
   liệu cá nhân.
6. Ít nhất 1 người khác (hoặc agent độc lập) review trước khi merge — không tự merge PR của chính mình
   khi có người khác trong team.

## Coding convention

- Backend: theo [`docs/backend/KIEN-TRUC-BACKEND.md`](docs/backend/KIEN-TRUC-BACKEND.md) — không thỏa hiệp
  Dependency Rule của Clean Architecture để "cho nhanh".
- Frontend: dùng shadcn/ui + Tailwind theo token thiết kế ở
  [`docs/frontend/THIET-KE-GIAO-DIEN.md`](docs/frontend/THIET-KE-GIAO-DIEN.md) — không tự chế màu/font
  ngoài token đã định nghĩa.
- Đặt tên bảng/cột/endpoint theo đúng những gì đã có trong ERD/API design — không tự sáng tạo tên khác
  cho cùng khái niệm.

## Cập nhật CHANGELOG

Mỗi PR gộp vào `main` làm thay đổi có ý nghĩa với người dùng (tính năng mới, sửa lỗi quan trọng) →
thêm 1 dòng vào [`CHANGELOG.md`](./CHANGELOG.md) mục `[Unreleased]`.
