using GiapTech.BlouseHiding.Application.Common.Interfaces;

namespace GiapTech.BlouseHiding.Application.Catalog.Queries.GetJobPackages;

public record JobPackageDto
{
    public Guid Id { get; init; }
    public string Tier { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int DurationDays { get; init; }
    public decimal Price { get; init; }
    public int? MaxActiveJobs { get; init; }
    public Dictionary<string, bool> Perks { get; init; } = new();
}

public record GetJobPackagesQuery : IQuery<List<JobPackageDto>>;

public class GetJobPackagesQueryHandler : IQueryHandler<GetJobPackagesQuery, List<JobPackageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentLocale _locale;

    public GetJobPackagesQueryHandler(IApplicationDbContext context, ICurrentLocale locale)
    {
        _context = context;
        _locale = locale;
    }

    public async ValueTask<List<JobPackageDto>> Handle(GetJobPackagesQuery query, CancellationToken cancellationToken)
    {
        var locale = _locale.Value;

        return await _context.JobPackages
            .OrderBy(p => p.Price)
            .Select(p => new JobPackageDto
            {
                Id = p.Id,
                Tier = p.Tier.ToString(),
                DurationDays = p.DurationDays,
                Price = p.Price,
                MaxActiveJobs = p.MaxActiveJobs,
                Perks = p.Perks,
                Name = p.Translations.Where(t => t.Locale == locale).Select(t => t.Name).FirstOrDefault() ?? p.Name,
            })
            .ToListAsync(cancellationToken);
    }
}
