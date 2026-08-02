using GiapTech.BlouseHiding.Domain.Common;

namespace GiapTech.BlouseHiding.Domain.Entities;

public class LocationTranslation : BaseEntity
{
    public Guid LocationId { get; set; }

    public Location Location { get; set; } = null!;

    public string Locale { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}
