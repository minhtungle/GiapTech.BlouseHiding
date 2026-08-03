using GiapTech.BlouseHiding.Application.Common.Interfaces;

namespace GiapTech.BlouseHiding.Application.Applications.Queries.GetMyApplications;

public record GetMyApplicationsQuery : IQuery<List<ApplicationDto>>
{
    public Guid UserId { get; init; }
}

public class GetMyApplicationsQueryHandler : IQueryHandler<GetMyApplicationsQuery, List<ApplicationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMyApplicationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<List<ApplicationDto>> Handle(GetMyApplicationsQuery query, CancellationToken cancellationToken)
    {
        return await _context.Applications
            .Where(a => a.Candidate!.UserId == query.UserId)
            .OrderByDescending(a => a.AppliedAt)
            .Select(a => new ApplicationDto
            {
                Id = a.Id,
                JobId = a.JobId,
                JobTitle = a.Job!.Title,
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
