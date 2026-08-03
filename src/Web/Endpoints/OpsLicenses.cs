using System.Security.Claims;
using GiapTech.BlouseHiding.Application.Ops.Commands.VerifyLicense;
using GiapTech.BlouseHiding.Application.Ops.Queries.GetPendingLicenses;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Xem docs/backend/API-DESIGN.md mục 11.
public class OpsLicenses : IEndpointGroup
{
    public static string? RoutePrefix => "/api/v1/ops/licenses";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetPending, "").RequireAuthorization();
        groupBuilder.MapPost(Verify, "{licenseId:guid}/verify").RequireAuthorization();
    }

    public static async Task<List<PendingLicenseDto>> GetPending(ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetPendingLicensesQuery(), cancellationToken);
    }

    public static async Task Verify(Guid licenseId, VerifyLicenseRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var command = new VerifyLicenseCommand
        {
            LicenseId = licenseId,
            VerifiedBy = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!),
            Approved = request.Approved,
            RejectReason = request.RejectReason,
        };

        await sender.Send(command, cancellationToken);
    }
}

public record VerifyLicenseRequest(bool Approved, string? RejectReason);
