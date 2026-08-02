using GiapTech.BlouseHiding.Domain.Common;

namespace GiapTech.BlouseHiding.Domain.Entities;

// Danh mục chuyên khoa, cây phân cấp qua ParentId — xem docs/database/ERD-CHI-TIET.md mục "specialties".
public class Specialty : BaseAuditableEntity
{
    public string Code { get; set; } = string.Empty;

    // Tên tiếng Việt — giá trị dự phòng khi thiếu bản dịch (xem ADR-0006).
    public string Name { get; set; } = string.Empty;

    public Guid? ParentId { get; set; }

    public Specialty? Parent { get; set; }

    public ICollection<Specialty> Children { get; set; } = new List<Specialty>();

    public ICollection<SpecialtyTranslation> Translations { get; set; } = new List<SpecialtyTranslation>();
}
