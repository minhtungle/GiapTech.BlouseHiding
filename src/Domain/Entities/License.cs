namespace GiapTech.BlouseHiding.Domain.Entities;

public class License : BaseAuditableEntity
{
    public Guid ProfileId { get; set; }

    public CandidateProfile? Profile { get; set; }

    public string LicenseNo { get; set; } = string.Empty;

    public string IssuedBy { get; set; } = string.Empty;

    public string? Scope { get; set; }

    public DateOnly IssuedAt { get; set; }

    public DateOnly? ExpiredAt { get; set; }

    public LicenseVerifyStatus VerifyStatus { get; set; } = LicenseVerifyStatus.Pending;

    public Guid? VerifiedBy { get; set; }

    public DateTimeOffset? VerifiedAt { get; set; }

    public string? RejectReason { get; set; }

    public string DocumentUrl { get; set; } = string.Empty;

    public bool CanEdit => VerifyStatus is LicenseVerifyStatus.Pending or LicenseVerifyStatus.Rejected;

    public void Verify(Guid verifiedBy)
    {
        VerifyStatus = LicenseVerifyStatus.Verified;
        VerifiedBy = verifiedBy;
        VerifiedAt = DateTimeOffset.UtcNow;
        RejectReason = null;
    }

    public void Reject(Guid verifiedBy, string reason)
    {
        VerifyStatus = LicenseVerifyStatus.Rejected;
        VerifiedBy = verifiedBy;
        VerifiedAt = DateTimeOffset.UtcNow;
        RejectReason = reason;
    }
}
