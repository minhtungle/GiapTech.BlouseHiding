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

    public List<OrganizationInvitation> Invitations { get; private set; } = [];

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

    // Không mời thêm owner qua đây (ERD mục 2.3) — chỉ hr_manager/hr_member.
    public OrganizationInvitation InviteMember(string email, EmployerMemberRole invitedRole, Guid invitedBy, string tokenHash)
    {
        var invitation = new OrganizationInvitation
        {
            OrganizationId = Id,
            Email = email,
            InvitedRole = invitedRole,
            InvitedBy = invitedBy,
            TokenHash = tokenHash,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(7),
        };

        Invitations.Add(invitation);

        return invitation;
    }

    public EmployerMember AcceptInvitation(OrganizationInvitation invitation, Guid userId)
    {
        invitation.AcceptedAt = DateTimeOffset.UtcNow;

        var member = new EmployerMember
        {
            OrganizationId = Id,
            UserId = userId,
            MemberRole = invitation.InvitedRole,
            InvitedBy = invitation.InvitedBy,
            JoinedAt = DateTimeOffset.UtcNow,
        };

        Members.Add(member);

        return member;
    }

    public void Verify(Guid verifiedBy)
    {
        VerifyStatus = OrganizationVerifyStatus.Verified;
        VerifiedBy = verifiedBy;
        RejectReason = null;
    }

    public void Reject(Guid verifiedBy, string reason)
    {
        VerifyStatus = OrganizationVerifyStatus.Rejected;
        VerifiedBy = verifiedBy;
        RejectReason = reason;
    }

    // Rút xác thực (verified → suspended) — luôn kéo theo tự động ẩn mọi tin published của tổ chức
    // trong cùng transaction (ERD mục 4.6), không phải thao tác thủ công riêng dễ quên.
    public void Suspend(Guid verifiedBy)
    {
        VerifyStatus = OrganizationVerifyStatus.Suspended;
        VerifiedBy = verifiedBy;
    }
}
