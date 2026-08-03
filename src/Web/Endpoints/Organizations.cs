using System.Security.Claims;
using GiapTech.BlouseHiding.Application.Employers.Commands.CreateOrganization;
using GiapTech.BlouseHiding.Application.Jobs;
using GiapTech.BlouseHiding.Application.Jobs.Queries.GetOrganizationJobs;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Xem docs/backend/API-DESIGN.md mục 4, 5 (GET .../jobs).
public class Organizations : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateOrganization, "").RequireAuthorization();
        groupBuilder.MapGet(GetJobs, "{organizationId:guid}/jobs").RequireAuthorization();
    }

    public static async Task<List<JobDto>> GetJobs(Guid organizationId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await sender.Send(new GetOrganizationJobsQuery { OrganizationId = organizationId, UserId = userId }, cancellationToken);
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
}

public record CreateOrganizationRequest(
    string Name,
    Domain.Enums.OrganizationType OrgType,
    string? LicenseNo,
    Domain.Enums.OrganizationSize? Size);
