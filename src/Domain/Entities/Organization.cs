namespace GiapTech.BlouseHiding.Domain.Entities;

public class Organization : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    public OrganizationType OrgType { get; set; }

    public string? LicenseNo { get; set; }

    public OrganizationSize? Size { get; set; }

    public string? Description { get; set; }

    public string? LogoUrl { get; set; }

    public string? CoverUrl { get; set; }

    public string? Address { get; set; }

    public Guid? LocationId { get; set; }

    public OrganizationVerifyStatus VerifyStatus { get; set; } = OrganizationVerifyStatus.Pending;

    public Guid? VerifiedBy { get; set; }

    public string? RejectReason { get; set; }

    public List<EmployerMember> Members { get; private set; } = [];

    public EmployerMember AddOwner(Guid userId)
    {
        var owner = new EmployerMember
        {
            OrganizationId = Id,
            UserId = userId,
            MemberRole = EmployerMemberRole.Owner,
            JoinedAt = DateTimeOffset.UtcNow,
        };

        Members.Add(owner);

        return owner;
    }
}
