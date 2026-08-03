using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Enums;

namespace GiapTech.BlouseHiding.Application.Ops.Queries.GetPendingJobs;

public record PendingJobDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string OrganizationName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}

[Authorize(Roles = "admin,moderator")]
public record GetPendingJobsQuery : IQuery<List<PendingJobDto>>;

public class GetPendingJobsQueryHandler : IQueryHandler<GetPendingJobsQuery, List<PendingJobDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingJobsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<List<PendingJobDto>> Handle(GetPendingJobsQuery query, CancellationToken cancellationToken)
    {
        return await _context.Jobs
            .Where(j => j.Status == JobStatus.Pending)
            .OrderBy(j => j.Created)
            .Select(j => new PendingJobDto
            {
                Id = j.Id,
                Title = j.Title,
                OrganizationName = j.Organization != null ? j.Organization.Name : string.Empty,
                Description = j.Description,
            })
            .ToListAsync(cancellationToken);
    }
}
