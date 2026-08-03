using GiapTech.BlouseHiding.Application.Catalog.Commands.CreateJobPackage;
using GiapTech.BlouseHiding.Application.Catalog.Commands.UpdateJobPackage;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Cấu hình gói tin/giá — Vận hành — xem docs/backend/API-DESIGN.md mục 11.
public class OpsJobPackages : IEndpointGroup
{
    public static string? RoutePrefix => "/api/v1/ops/job-packages";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(Create, "").RequireAuthorization();
        groupBuilder.MapPut(Update, "{id}").RequireAuthorization();
    }

    public static async Task<Guid> Create(ISender sender, CreateJobPackageCommand command, CancellationToken cancellationToken)
    {
        return await sender.Send(command, cancellationToken);
    }

    public static async Task Update(ISender sender, Guid id, UpdateJobPackageCommand command, CancellationToken cancellationToken)
    {
        await sender.Send(command with { Id = id }, cancellationToken);
    }
}
