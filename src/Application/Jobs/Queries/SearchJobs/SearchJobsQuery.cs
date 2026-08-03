using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Domain.Enums;

namespace GiapTech.BlouseHiding.Application.Jobs.Queries.SearchJobs;

public record SearchJobsQuery : IQuery<List<JobDto>>
{
    public Guid? SpecialtyId { get; init; }
    public Guid? LocationId { get; init; }
    public EmploymentType? EmploymentType { get; init; }
    public int? SalaryMin { get; init; }
    public string? Keyword { get; init; }
}

public class SearchJobsQueryHandler : IQueryHandler<SearchJobsQuery, List<JobDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentLocale _locale;

    public SearchJobsQueryHandler(IApplicationDbContext context, ICurrentLocale locale)
    {
        _context = context;
        _locale = locale;
    }

    public async ValueTask<List<JobDto>> Handle(SearchJobsQuery query, CancellationToken cancellationToken)
    {
        var locale = _locale.Value;

        var jobs = _context.Jobs.Where(j => j.Status == JobStatus.Published);

        if (query.SpecialtyId is not null)
        {
            jobs = jobs.Where(j => j.SpecialtyId == query.SpecialtyId);
        }

        if (query.LocationId is not null)
        {
            jobs = jobs.Where(j => j.LocationId == query.LocationId);
        }

        if (query.EmploymentType is not null)
        {
            jobs = jobs.Where(j => j.EmploymentType == query.EmploymentType);
        }

        if (query.SalaryMin is not null)
        {
            jobs = jobs.Where(j => j.SalaryMax == null || j.SalaryMax >= query.SalaryMin);
        }

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            jobs = jobs.Where(j => j.Title.ToLower().Contains(query.Keyword.ToLower()));
        }

        return await jobs
            .OrderByDescending(j => j.PublishedAt)
            .Select(j => new JobDto
            {
                Id = j.Id,
                OrganizationId = j.OrganizationId,
                OrganizationName = j.Organization != null ? j.Organization.Name : string.Empty,
                Title = j.Title,
                SpecialtyName = j.Specialty!.Translations.Where(t => t.Locale == locale).Select(t => t.Name).FirstOrDefault() ?? j.Specialty!.Name,
                LocationName = j.Location!.Translations.Where(t => t.Locale == locale).Select(t => t.Name).FirstOrDefault() ?? j.Location!.Name,
                EmploymentType = j.EmploymentType.ToString(),
                SalaryMin = j.SalaryMin,
                SalaryMax = j.SalaryMax,
                SalaryNegotiable = j.SalaryNegotiable,
                RequiredLicense = j.RequiredLicense,
                MinExperienceYears = j.MinExperienceYears,
                Description = j.Description,
                Requirements = j.Requirements,
                Benefits = j.Benefits,
                Status = j.Status.ToString(),
                PublishedAt = j.PublishedAt,
                ExpiresAt = j.ExpiresAt,
            })
            .ToListAsync(cancellationToken);
    }
}
