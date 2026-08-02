using GiapTech.BlouseHiding.Application.Catalog.Queries.GetEmploymentTypes;
using GiapTech.BlouseHiding.Application.Catalog.Queries.GetJobPackages;
using GiapTech.BlouseHiding.Application.Catalog.Queries.GetLocations;
using GiapTech.BlouseHiding.Application.Catalog.Queries.GetSpecialties;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Danh mục dùng chung, đọc công khai — xem docs/backend/API-DESIGN.md mục 10.
public class Catalog : IEndpointGroup
{
    public static string? RoutePrefix => "/api/v1/catalog";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetSpecialties, "specialties");
        groupBuilder.MapGet(GetLocations, "locations");
        groupBuilder.MapGet(GetJobPackages, "job-packages");
        groupBuilder.MapGet(GetEmploymentTypes, "employment-types");
    }

    public static async Task<List<SpecialtyDto>> GetSpecialties(ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetSpecialtiesQuery(), cancellationToken);
    }

    public static async Task<List<LocationDto>> GetLocations(ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetLocationsQuery(), cancellationToken);
    }

    public static async Task<List<JobPackageDto>> GetJobPackages(ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetJobPackagesQuery(), cancellationToken);
    }

    public static async Task<List<string>> GetEmploymentTypes(ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetEmploymentTypesQuery(), cancellationToken);
    }
}
