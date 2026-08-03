using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Applications.Queries.GetApplicationHistory;

public record GetApplicationHistoryQuery : IQuery<List<ApplicationStageHistoryDto>>
{
    public Guid ApplicationId { get; init; }
    public Guid UserId { get; init; }
}

public class GetApplicationHistoryQueryHandler : IQueryHandler<GetApplicationHistoryQuery, List<ApplicationStageHistoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetApplicationHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<List<ApplicationStageHistoryDto>> Handle(GetApplicationHistoryQuery query, CancellationToken cancellationToken)
    {
        var application = await _context.Applications
            .Include(a => a.Job)
            .Include(a => a.Candidate)
            .FirstOrDefaultAsync(a => a.Id == query.ApplicationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.JobApplication), query.ApplicationId);

        var isOwner = application.Candidate!.UserId == query.UserId;
        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == application.Job!.OrganizationId && m.UserId == query.UserId, cancellationToken);

        if (!isOwner && !isMember)
        {
            throw new ForbiddenAccessException();
        }

        return await _context.ApplicationStageHistories
            .Where(h => h.ApplicationId == query.ApplicationId)
            .OrderBy(h => h.ChangedAt)
            .Select(h => new ApplicationStageHistoryDto
            {
                FromStage = h.FromStage.ToString(),
                ToStage = h.ToStage.ToString(),
                ChangedBy = h.ChangedBy,
                ChangedAt = h.ChangedAt,
                Silent = h.Silent,
            })
            .ToListAsync(cancellationToken);
    }
}
