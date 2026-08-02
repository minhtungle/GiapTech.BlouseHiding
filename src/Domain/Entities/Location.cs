using GiapTech.BlouseHiding.Domain.Common;

namespace GiapTech.BlouseHiding.Domain.Entities;

// Cây tỉnh/thành -> quận/huyện qua ParentId — xem docs/database/ERD-CHI-TIET.md mục "locations".
public class Location : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public Guid? ParentId { get; set; }

    public Location? Parent { get; set; }

    public ICollection<Location> Children { get; set; } = new List<Location>();

    public ICollection<LocationTranslation> Translations { get; set; } = new List<LocationTranslation>();
}
