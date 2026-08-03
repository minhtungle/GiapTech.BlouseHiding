using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Employers.Commands.AcceptInvitation;
using GiapTech.BlouseHiding.Application.Employers.Commands.CreateOrganization;
using GiapTech.BlouseHiding.Application.Employers.Commands.InviteMember;
using GiapTech.BlouseHiding.Application.Employers.Commands.RemoveMember;
using GiapTech.BlouseHiding.Application.Employers.Queries.GetOrganizationMembers;
using GiapTech.BlouseHiding.Application.FunctionalTests.Infrastructure;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Entities;
using GiapTech.BlouseHiding.Domain.Enums;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.FunctionalTests.Employers;

public class OrganizationMembersTests : TestBase
{
    private async Task<Guid> CreateVerifiedOwnerOrgAsync(string ownerEmail)
    {
        var ownerId = await TestApp.RunAsUserAsync(ownerEmail, "Testing1234!", [Roles.Employer]);

        return await TestApp.SendAsync<Guid>(new CreateOrganizationCommand
        {
            OwnerUserId = ownerId,
            Name = "Bệnh viện Test Members",
            OrgType = OrganizationType.BenhVienTu,
        });
    }

    [Test]
    public async Task InviteMember_Then_Accept_Should_Create_Member_And_Mark_Invitation_Accepted()
    {
        var ownerEmail = $"owner-{Guid.NewGuid():N}@test.local";
        var orgId = await CreateVerifiedOwnerOrgAsync(ownerEmail);
        var ownerId = TestApp.GetUserId()!.Value;

        var inviteeEmail = $"invitee-{Guid.NewGuid():N}@test.local";

        var invitationId = await TestApp.SendAsync<Guid>(new InviteMemberCommand
        {
            UserId = ownerId,
            OrganizationId = orgId,
            Email = inviteeEmail,
            InvitedRole = EmployerMemberRole.HrMember,
        });

        invitationId.ShouldNotBe(Guid.Empty);
        var token = CapturingInvitationSender.LastToken;
        token.ShouldNotBeNull();

        var inviteeId = await TestApp.RunAsUserAsync(inviteeEmail, "Testing1234!", [Roles.Employer]);

        await TestApp.SendAsync(new AcceptInvitationCommand
        {
            UserId = inviteeId,
            UserEmail = inviteeEmail,
            Token = token!,
        });

        var members = await TestApp.SendAsync<OrganizationMembersDto>(new GetOrganizationMembersQuery
        {
            UserId = ownerId,
            OrganizationId = orgId,
        });

        members.Members.ShouldContain(m => m.UserId == inviteeId && m.MemberRole == "HrMember");
        members.PendingInvitations.ShouldBeEmpty();
    }

    [Test]
    public async Task AcceptInvitation_With_Wrong_Email_Should_Throw_ValidationException()
    {
        var ownerEmail = $"owner2-{Guid.NewGuid():N}@test.local";
        var orgId = await CreateVerifiedOwnerOrgAsync(ownerEmail);
        var ownerId = TestApp.GetUserId()!.Value;

        var inviteeEmail = $"invitee2-{Guid.NewGuid():N}@test.local";

        await TestApp.SendAsync<Guid>(new InviteMemberCommand
        {
            UserId = ownerId,
            OrganizationId = orgId,
            Email = inviteeEmail,
            InvitedRole = EmployerMemberRole.HrMember,
        });

        var token = CapturingInvitationSender.LastToken!;

        var wrongUserEmail = $"someone-else-{Guid.NewGuid():N}@test.local";
        var wrongUserId = await TestApp.RunAsUserAsync(wrongUserEmail, "Testing1234!", [Roles.Employer]);

        await Should.ThrowAsync<ValidationException>(() => TestApp.SendAsync(new AcceptInvitationCommand
        {
            UserId = wrongUserId,
            UserEmail = wrongUserEmail,
            Token = token,
        }));
    }

    [Test]
    public async Task InviteMember_Should_Throw_Forbidden_When_Caller_Is_Not_Member()
    {
        var ownerEmail = $"owner3-{Guid.NewGuid():N}@test.local";
        var orgId = await CreateVerifiedOwnerOrgAsync(ownerEmail);

        var outsiderId = await TestApp.RunAsUserAsync($"outsider-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Employer]);

        await Should.ThrowAsync<ForbiddenAccessException>(() => TestApp.SendAsync(new InviteMemberCommand
        {
            UserId = outsiderId,
            OrganizationId = orgId,
            Email = "someone@test.local",
            InvitedRole = EmployerMemberRole.HrMember,
        }));
    }

    [Test]
    public async Task RemoveMember_Should_Remove_HrMember_But_Not_Owner()
    {
        var ownerEmail = $"owner4-{Guid.NewGuid():N}@test.local";
        var orgId = await CreateVerifiedOwnerOrgAsync(ownerEmail);
        var ownerId = TestApp.GetUserId()!.Value;

        var inviteeEmail = $"invitee4-{Guid.NewGuid():N}@test.local";
        await TestApp.SendAsync<Guid>(new InviteMemberCommand
        {
            UserId = ownerId,
            OrganizationId = orgId,
            Email = inviteeEmail,
            InvitedRole = EmployerMemberRole.HrMember,
        });
        var token = CapturingInvitationSender.LastToken!;

        var inviteeId = await TestApp.RunAsUserAsync(inviteeEmail, "Testing1234!", [Roles.Employer]);
        await TestApp.SendAsync(new AcceptInvitationCommand { UserId = inviteeId, UserEmail = inviteeEmail, Token = token });

        TestApp.SetCurrentUser(ownerId, [Roles.Employer]);

        var members = await TestApp.SendAsync<OrganizationMembersDto>(new GetOrganizationMembersQuery { UserId = ownerId, OrganizationId = orgId });
        var inviteeMemberId = members.Members.Single(m => m.UserId == inviteeId).Id;
        var ownerMemberId = members.Members.Single(m => m.UserId == ownerId).Id;

        await TestApp.SendAsync(new RemoveMemberCommand { UserId = ownerId, OrganizationId = orgId, MemberId = inviteeMemberId });

        var remaining = await TestApp.SendAsync<OrganizationMembersDto>(new GetOrganizationMembersQuery { UserId = ownerId, OrganizationId = orgId });
        remaining.Members.ShouldNotContain(m => m.UserId == inviteeId);

        await Should.ThrowAsync<ValidationException>(() => TestApp.SendAsync(new RemoveMemberCommand
        {
            UserId = ownerId,
            OrganizationId = orgId,
            MemberId = ownerMemberId,
        }));
    }
}
