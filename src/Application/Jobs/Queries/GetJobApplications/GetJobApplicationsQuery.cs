using GiapTech.BlouseHiding.Application.Applications;
using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Jobs.Queries.GetJobApplications;

// ATS Kanban theo tin — khả dụng bất kể jobs.status (tin hết hạn/đóng/suspended vẫn thao tác được,
// xem ERD mục 4.9).
public record GetJobApplicationsQuery : IQuery<List<ApplicationDto>>
{
    public Guid JobId { get; init; }
    public Guid UserId { get; init; }
}

public class GetJobApplicationsQueryHandler : IQueryHandler<GetJobApplicationsQuery, List<ApplicationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetJobApplicationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<List<ApplicationDto>> Handle(GetJobApplicationsQuery query, CancellationToken cancellationToken)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == query.JobId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Job), query.JobId);

        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == job.OrganizationId && m.UserId == query.UserId, cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        return await _context.Applications
            .Where(a => a.JobId == query.JobId)
            .OrderBy(a => a.AppliedAt)
            .Select(a => new ApplicationDto
            {
                Id = a.Id,
                JobId = a.JobId,
                JobTitle = job.Title,
                CandidateId = a.CandidateId,
                CandidateFullName = a.Candidate!.FullName,
                CoverLetter = a.CoverLetter,
                Stage = a.Stage.ToString(),
                Score = a.Score,
                RejectedReason = a.RejectedReason,
                AppliedAt = a.AppliedAt,
            })
            .ToListAsync(cancellationToken);
    }
}
