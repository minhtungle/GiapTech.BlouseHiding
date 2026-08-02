using GiapTech.BlouseHiding.Application.Common.Interfaces;

namespace GiapTech.BlouseHiding.Application.Catalog.Queries.GetSpecialties;

public record SpecialtyDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Guid? ParentId { get; init; }
}

public record GetSpecialtiesQuery : IQuery<List<SpecialtyDto>>;

public class GetSpecialtiesQueryHandler : IQueryHandler<GetSpecialtiesQuery, List<SpecialtyDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentLocale _locale;

    public GetSpecialtiesQueryHandler(IApplicationDbContext context, ICurrentLocale locale)
    {
        _context = context;
        _locale = locale;
    }

    public async ValueTask<List<SpecialtyDto>> Handle(GetSpecialtiesQuery query, CancellationToken cancellationToken)
    {
        var locale = _locale.Value;

        return await _context.Specialties
            .OrderBy(s => s.Name)
            .Select(s => new SpecialtyDto
            {
                Id = s.Id,
                Code = s.Code,
                ParentId = s.ParentId,
                Name = s.Translations.Where(t => t.Locale == locale).Select(t => t.Name).FirstOrDefault() ?? s.Name,
            })
            .ToListAsync(cancellationToken);
    }
}
