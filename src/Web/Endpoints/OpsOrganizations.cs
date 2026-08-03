using System.Security.Claims;
using GiapTech.BlouseHiding.Application.Ops.Commands.VerifyOrganization;
using GiapTech.BlouseHiding.Application.Ops.Queries.GetPendingOrganizations;
using GiapTech.BlouseHiding.Domain.Enums;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Xem docs/backend/API-DESIGN.md mục 11.
public class OpsOrganizations : IEndpointGroup
{
    public static string? RoutePrefix => "/api/v1/ops/organizations";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetPendingOrganizations, "").RequireAuthorization();
        groupBuilder.MapPost(VerifyOrganization, "{organizationId:guid}/verify").RequireAuthorization();
    }

    public static async Task<List<PendingOrganizationDto>> GetPendingOrganizations(ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetPendingOrganizationsQuery(), cancellationToken);
    }

    public static async Task VerifyOrganization(Guid organizationId, VerifyOrganizationRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var command = new VerifyOrganizationCommand
        {
            OrganizationId = organizationId,
            VerifiedBy = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!),
            Action = request.Action,
            RejectReason = request.RejectReason,
        };

        await sender.Send(command, cancellationToken);
    }
}

public record VerifyOrganizationRequest(OrganizationVerifyAction Action, string? RejectReason);
