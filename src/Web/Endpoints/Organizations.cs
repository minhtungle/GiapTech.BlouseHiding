using System.Security.Claims;
using GiapTech.BlouseHiding.Application.Employers.Commands.CreateOrganization;
using GiapTech.BlouseHiding.Application.Employers.Commands.InviteMember;
using GiapTech.BlouseHiding.Application.Employers.Commands.RemoveMember;
using GiapTech.BlouseHiding.Application.Employers.Queries.GetCreditTransactions;
using GiapTech.BlouseHiding.Application.Employers.Queries.GetCreditWallet;
using GiapTech.BlouseHiding.Application.Employers.Queries.GetMyOrganizations;
using GiapTech.BlouseHiding.Application.Employers.Queries.GetOrganizationById;
using GiapTech.BlouseHiding.Application.Employers.Queries.GetOrganizationMembers;
using GiapTech.BlouseHiding.Application.Jobs;
using GiapTech.BlouseHiding.Application.Jobs.Queries.GetOrganizationJobs;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Xem docs/backend/API-DESIGN.md mục 4, 5 (GET .../jobs), 7 (credit-wallet/credit-transactions).
// "mine" không nằm trong thiết kế gốc — bổ sung khi nối web-admin/ (employer cần biết tổ chức của
// chính mình để dùng cho mọi thao tác khác, xem docs/nghiep-vu/TIEN-DO-DU-AN.md). Tương tự
// members/invite, members/{id} (DELETE), members (GET) — bổ sung khi thay features/users/ ở
// web-admin/ (đang dùng faker) bằng danh sách thành viên tổ chức thật.
public class Organizations : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateOrganization, "").RequireAuthorization();
        groupBuilder.MapGet(GetMyOrganizations, "mine").RequireAuthorization();
        groupBuilder.MapGet(GetOrganizationById, "{organizationId:guid}");
        groupBuilder.MapGet(GetJobs, "{organizationId:guid}/jobs").RequireAuthorization();
        groupBuilder.MapGet(GetCreditWallet, "{organizationId:guid}/credit-wallet").RequireAuthorization();
        groupBuilder.MapGet(GetCreditTransactions, "{organizationId:guid}/credit-transactions").RequireAuthorization();
        groupBuilder.MapGet(GetMembers, "{organizationId:guid}/members").RequireAuthorization();
        groupBuilder.MapPost(InviteMember, "{organizationId:guid}/members/invite").RequireAuthorization();
        groupBuilder.MapDelete(RemoveMember, "{organizationId:guid}/members/{memberId:guid}").RequireAuthorization();
    }

    public static async Task<List<MyOrganizationDto>> GetMyOrganizations(ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await sender.Send(new GetMyOrganizationsQuery { UserId = userId }, cancellationToken);
    }

    public static async Task<OrganizationDto> GetOrganizationById(Guid organizationId, ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetOrganizationByIdQuery { OrganizationId = organizationId }, cancellationToken);
    }

    public static async Task<List<JobDto>> GetJobs(Guid organizationId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await sender.Send(new GetOrganizationJobsQuery { OrganizationId = organizationId, UserId = userId }, cancellationToken);
    }

    public static async Task<CreditWalletDto> GetCreditWallet(Guid organizationId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await sender.Send(new GetCreditWalletQuery { OrganizationId = organizationId, UserId = userId }, cancellationToken);
    }

    public static async Task<List<CreditTransactionDto>> GetCreditTransactions(Guid organizationId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await sender.Send(new GetCreditTransactionsQuery { OrganizationId = organizationId, UserId = userId }, cancellationToken);
    }

    public static async Task<Guid> CreateOrganization(CreateOrganizationRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new CreateOrganizationCommand
        {
            OwnerUserId = userId,
            Name = request.Name,
            OrgType = request.OrgType,
            LicenseNo = request.LicenseNo,
            Size = request.Size,
        };

        return await sender.Send(command, cancellationToken);
    }

    public static async Task<OrganizationMembersDto> GetMembers(Guid organizationId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await sender.Send(new GetOrganizationMembersQuery { OrganizationId = organizationId, UserId = userId }, cancellationToken);
    }

    public static async Task<Guid> InviteMember(Guid organizationId, InviteMemberRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new InviteMemberCommand
        {
            UserId = userId,
            OrganizationId = organizationId,
            Email = request.Email,
            InvitedRole = request.InvitedRole,
        };

        return await sender.Send(command, cancellationToken);
    }

    public static async Task RemoveMember(Guid organizationId, Guid memberId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new RemoveMemberCommand
        {
            UserId = userId,
            OrganizationId = organizationId,
            MemberId = memberId,
        };

        await sender.Send(command, cancellationToken);
    }
}

public record InviteMemberRequest(string Email, Domain.Enums.EmployerMemberRole InvitedRole);

public record CreateOrganizationRequest(
    string Name,
    Domain.Enums.OrganizationType OrgType,
    string? LicenseNo,
    Domain.Enums.OrganizationSize? Size);
