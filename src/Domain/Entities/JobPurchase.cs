namespace GiapTech.BlouseHiding.Domain.Entities;

public class JobPurchase : BaseEntity
{
    public Guid JobId { get; set; }

    public Guid PackageId { get; set; }

    public Guid OrganizationId { get; set; }

    // Null nếu gói free (không qua Payment) — xem docs/database/ERD-CHI-TIET.md mục 2.5.
    public Guid? PaymentId { get; set; }

    public DateTimeOffset PurchasedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset ExpiresAt { get; set; }
}
