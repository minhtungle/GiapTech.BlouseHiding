using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Jobs.Queries.GetJobById;

public record GetJobByIdQuery : IQuery<JobDto>
{
    public Guid JobId { get; init; }

    // null nếu Guest (chưa đăng nhập) — chỉ xem được tin published.
    public Guid? RequestingUserId { get; init; }
}

public class GetJobByIdQueryHandler : IQueryHandler<GetJobByIdQuery, JobDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentLocale _locale;

    public GetJobByIdQueryHandler(IApplicationDbContext context, ICurrentLocale locale)
    {
        _context = context;
        _locale = locale;
    }

    public async ValueTask<JobDto> Handle(GetJobByIdQuery query, CancellationToken cancellationToken)
    {
        var locale = _locale.Value;

        var job = await _context.Jobs
            .Where(j => j.Id == query.JobId)
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
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Job), query.JobId);

        if (job.Status != JobStatus.Published.ToString())
        {
            var isMember = query.RequestingUserId != null && await _context.EmployerMembers
                .AnyAsync(m => m.OrganizationId == job.OrganizationId && m.UserId == query.RequestingUserId, cancellationToken);

            if (!isMember)
            {
                throw new NotFoundException(nameof(Domain.Entities.Job), query.JobId);
            }
        }

        return job;
    }
}
