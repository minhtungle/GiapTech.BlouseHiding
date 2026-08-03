using System.Security.Claims;
using GiapTech.BlouseHiding.Application.Employers.Commands.CreateOrganization;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Xem docs/backend/API-DESIGN.md mục 4.
public class Organizations : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateOrganization, "").RequireAuthorization();
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
