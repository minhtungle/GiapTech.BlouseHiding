using GiapTech.BlouseHiding.Application.Common.Interfaces;

namespace GiapTech.BlouseHiding.Application.Catalog.Queries.GetLocations;

public record LocationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid? ParentId { get; init; }
}

public record GetLocationsQuery : IQuery<List<LocationDto>>;

public class GetLocationsQueryHandler : IQueryHandler<GetLocationsQuery, List<LocationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentLocale _locale;

    public GetLocationsQueryHandler(IApplicationDbContext context, ICurrentLocale locale)
    {
        _context = context;
        _locale = locale;
    }

    public async ValueTask<List<LocationDto>> Handle(GetLocationsQuery query, CancellationToken cancellationToken)
    {
        var locale = _locale.Value;

        return await _context.Locations
            .OrderBy(l => l.Name)
            .Select(l => new LocationDto
            {
                Id = l.Id,
                ParentId = l.ParentId,
                Name = l.Translations.Where(t => t.Locale == locale).Select(t => t.Name).FirstOrDefault() ?? l.Name,
            })
            .ToListAsync(cancellationToken);
    }
}
