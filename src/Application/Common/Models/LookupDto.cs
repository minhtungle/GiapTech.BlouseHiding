namespace GiapTech.BlouseHiding.Application.Common.Models;

// DTO rút gọn cho danh mục/lookup (specialties, locations...). Đăng ký AutoMapper
// `CreateMap<TEntity, LookupDto>()` ngay tại entity cần dùng khi bounded context đó được thêm.
public class LookupDto
{
    public int Id { get; init; }

    public string? Title { get; init; }
}
