using GiapTech.BlouseHiding.Domain.Enums;
using GiapTech.BlouseHiding.Domain.Common;

namespace GiapTech.BlouseHiding.Domain.Entities;

// Danh mục gói đăng tin — dữ liệu cấu hình, không phải giao dịch (xem
// docs/database/ERD-CHI-TIET.md mục "job_packages").
public class JobPackage : BaseAuditableEntity
{
    public JobPackageTier Tier { get; set; }

    public string Name { get; set; } = string.Empty;

    public int DurationDays { get; set; }

    public decimal Price { get; set; }

    public int? MaxActiveJobs { get; set; }

    public Dictionary<string, bool> Perks { get; set; } = new();

    public ICollection<JobPackageTranslation> Translations { get; set; } = new List<JobPackageTranslation>();
}
