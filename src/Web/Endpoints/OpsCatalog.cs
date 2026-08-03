using GiapTech.BlouseHiding.Application.Catalog.Commands.CreateLocation;
using GiapTech.BlouseHiding.Application.Catalog.Commands.CreateSpecialty;
using GiapTech.BlouseHiding.Application.Catalog.Commands.UpdateLocation;
using GiapTech.BlouseHiding.Application.Catalog.Commands.UpdateSpecialty;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Quản lý danh mục — Vận hành (role backend admin/moderator) — xem docs/backend/API-DESIGN.md mục 11.
public class OpsCatalog : IEndpointGroup
{
    public static string? RoutePrefix => "/api/v1/ops/catalog";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateSpecialty, "specialties").RequireAuthorization();
        groupBuilder.MapPut(UpdateSpecialty, "specialties/{id}").RequireAuthorization();
        groupBuilder.MapPost(CreateLocation, "locations").RequireAuthorization();
        groupBuilder.MapPut(UpdateLocation, "locations/{id}").RequireAuthorization();
    }

    public static async Task<Guid> CreateSpecialty(ISender sender, CreateSpecialtyCommand command, CancellationToken cancellationToken)
    {
        return await sender.Send(command, cancellationToken);
    }

    public static async Task UpdateSpecialty(ISender sender, Guid id, UpdateSpecialtyCommand command, CancellationToken cancellationToken)
    {
        await sender.Send(command with { Id = id }, cancellationToken);
    }

    public static async Task<Guid> CreateLocation(ISender sender, CreateLocationCommand command, CancellationToken cancellationToken)
    {
        return await sender.Send(command, cancellationToken);
    }

    public static async Task UpdateLocation(ISender sender, Guid id, UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        await sender.Send(command with { Id = id }, cancellationToken);
    }
}
