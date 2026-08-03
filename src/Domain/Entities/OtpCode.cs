namespace GiapTech.BlouseHiding.Domain.Entities;

public class OtpCode : BaseEntity
{
    public Guid UserId { get; set; }

    public OtpPurpose Purpose { get; set; }

    public string CodeHash { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? ConsumedAt { get; set; }

    public bool IsValid => ConsumedAt is null && DateTimeOffset.UtcNow < ExpiresAt;
}
