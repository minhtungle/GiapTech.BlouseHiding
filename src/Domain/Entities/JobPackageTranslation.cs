using GiapTech.BlouseHiding.Domain.Common;

namespace GiapTech.BlouseHiding.Domain.Entities;

public class JobPackageTranslation : BaseEntity
{
    public Guid JobPackageId { get; set; }

    public JobPackage JobPackage { get; set; } = null!;

    public string Locale { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
}
