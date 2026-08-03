namespace GiapTech.BlouseHiding.Domain.Entities;

public class CreditTransaction : BaseEntity
{
    public Guid WalletId { get; set; }

    public int Amount { get; set; }

    public CreditTransactionReason Reason { get; set; }

    public Guid? ReferenceId { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
