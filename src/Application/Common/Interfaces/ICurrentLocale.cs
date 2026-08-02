namespace GiapTech.BlouseHiding.Application.Common.Interfaces;

// Locale hiện tại của request — resolve từ header Accept-Language phía Web, xem
// docs/backend/CONG-NGHE-BACKEND.md mục "Đa ngôn ngữ". Luôn là 1 trong 6 giá trị hỗ trợ,
// không bao giờ null (mặc định "vi" nếu header thiếu/không khớp).
public interface ICurrentLocale
{
    string Value { get; }
}
