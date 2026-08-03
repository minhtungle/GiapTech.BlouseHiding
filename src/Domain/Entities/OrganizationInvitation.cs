namespace GiapTech.BlouseHiding.Domain.Entities;

public class OrganizationInvitation : BaseEntity
{
    public Guid OrganizationId { get; set; }

    public Organization? Organization { get; set; }

    public string Email { get; set; } = string.Empty;

    public EmployerMemberRole InvitedRole { get; set; }

    public Guid InvitedBy { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? AcceptedAt { get; set; }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;

    public bool IsAccepted => AcceptedAt is not null;
}
