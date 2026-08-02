namespace GiapTech.BlouseHiding.Application.Common.Models;

// DTO rút gọn cho danh mục/lookup (specialties, locations...). Đăng ký mapping Mapster
// (TypeAdapterConfig / IRegister) ngay tại entity cần dùng khi bounded context đó được thêm.
public class LookupDto
{
    public Guid Id { get; init; }

    public string? Title { get; init; }
}
