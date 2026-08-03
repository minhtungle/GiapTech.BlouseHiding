namespace GiapTech.BlouseHiding.Domain.Entities;

public class EmployerMember : BaseEntity
{
    public Guid OrganizationId { get; set; }

    public Organization? Organization { get; set; }

    public Guid UserId { get; set; }

    public EmployerMemberRole MemberRole { get; set; }

    public Guid? InvitedBy { get; set; }

    public DateTimeOffset? JoinedAt { get; set; }
}
