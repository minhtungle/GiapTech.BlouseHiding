using GiapTech.BlouseHiding.Domain.Common;

namespace GiapTech.BlouseHiding.Domain.Entities;

public class SpecialtyTranslation : BaseEntity
{
    public Guid SpecialtyId { get; set; }

    public Specialty Specialty { get; set; } = null!;

    // en/ja/zh/ko/es — không lưu vi (đã có ở Specialty.Name).
    public string Locale { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}
