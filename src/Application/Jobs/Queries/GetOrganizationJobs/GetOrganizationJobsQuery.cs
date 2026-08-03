using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;

namespace GiapTech.BlouseHiding.Application.Jobs.Queries.GetOrganizationJobs;

public record GetOrganizationJobsQuery : IQuery<List<JobDto>>
{
    public Guid OrganizationId { get; init; }
    public Guid UserId { get; init; }
}

public class GetOrganizationJobsQueryHandler : IQueryHandler<GetOrganizationJobsQuery, List<JobDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentLocale _locale;

    public GetOrganizationJobsQueryHandler(IApplicationDbContext context, ICurrentLocale locale)
    {
        _context = context;
        _locale = locale;
    }

    public async ValueTask<List<JobDto>> Handle(GetOrganizationJobsQuery query, CancellationToken cancellationToken)
    {
        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == query.OrganizationId && m.UserId == query.UserId, cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        var locale = _locale.Value;

        return await _context.Jobs
            .Where(j => j.OrganizationId == query.OrganizationId)
            .OrderByDescending(j => j.Created)
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
                RejectReason = j.RejectReason,
                PublishedAt = j.PublishedAt,
                ExpiresAt = j.ExpiresAt,
            })
            .ToListAsync(cancellationToken);
    }
}
