namespace GiapTech.BlouseHiding.Domain.Entities;

public class ProfileUnlock : BaseEntity
{
    public Guid OrganizationId { get; set; }

    public Guid CandidateId { get; set; }

    public int CreditCost { get; set; }

    public Guid UnlockedBy { get; set; }

    public DateTimeOffset UnlockedAt { get; set; } = DateTimeOffset.UtcNow;
}
