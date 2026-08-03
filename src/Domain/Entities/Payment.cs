using GiapTech.BlouseHiding.Domain.Enums;

namespace GiapTech.BlouseHiding.Domain.Entities;

// Xem docs/database/ERD-CHI-TIET.md mục 2.5 ("payments") và ADR-0003 — MVP chỉ dùng
// provider = ManualTransfer (chuyển khoản thủ công, Vận hành đối soát qua ReferenceCode).
public class Payment : BaseAuditableEntity
{
    public Guid OrganizationId { get; set; }

    public Organization? Organization { get; set; }

    public PaymentType Type { get; set; }

    public decimal Amount { get; set; }

    public PaymentProvider Provider { get; set; } = PaymentProvider.ManualTransfer;

    // Bắt buộc với Provider = ManualTransfer — mã NTD ghi vào nội dung chuyển khoản để Vận hành
    // đối soát (ERD: UNIQUE).
    public string? ReferenceCode { get; set; }

    public string? ProviderTxnId { get; set; }

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public Guid? ConfirmedBy { get; set; }

    // Chỉ dùng khi Type = CreditTopup — số Credit sẽ cộng vào ví khi xác nhận thành công.
    public int? CreditAmount { get; set; }

    public void ConfirmSuccess(Guid confirmedBy)
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException("Chỉ xác nhận được giao dịch đang chờ.");
        }

        Status = PaymentStatus.Success;
        ConfirmedBy = confirmedBy;
    }

    public void ConfirmFailed(Guid confirmedBy)
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException("Chỉ từ chối được giao dịch đang chờ.");
        }

        Status = PaymentStatus.Failed;
        ConfirmedBy = confirmedBy;
    }
}
