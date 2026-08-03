namespace GiapTech.BlouseHiding.Domain.Entities;

public class CreditWallet : BaseEntity
{
    public Guid OrganizationId { get; set; }

    public int Balance { get; set; }

    public List<CreditTransaction> Transactions { get; private set; } = [];

    public CreditTransaction Credit(int amount, CreditTransactionReason reason, Guid? referenceId, Guid? createdBy)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Số tiền nạp phải dương.");
        }

        Balance += amount;

        var transaction = new CreditTransaction
        {
            WalletId = Id,
            Amount = amount,
            Reason = reason,
            ReferenceId = referenceId,
            CreatedBy = createdBy,
        };

        Transactions.Add(transaction);

        return transaction;
    }

    public CreditTransaction Debit(int amount, CreditTransactionReason reason, Guid? referenceId, Guid? createdBy)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Số tiền trừ phải dương.");
        }

        if (Balance < amount)
        {
            throw new InvalidOperationException("Số dư Credit không đủ.");
        }

        Balance -= amount;

        var transaction = new CreditTransaction
        {
            WalletId = Id,
            Amount = -amount,
            Reason = reason,
            ReferenceId = referenceId,
            CreatedBy = createdBy,
        };

        Transactions.Add(transaction);

        return transaction;
    }
}
