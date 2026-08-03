using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Applications.Queries.GetApplicationById;

public record GetApplicationByIdQuery : IQuery<ApplicationDto>
{
    public Guid ApplicationId { get; init; }
    public Guid UserId { get; init; }
}

public class GetApplicationByIdQueryHandler : IQueryHandler<GetApplicationByIdQuery, ApplicationDto>
{
    private readonly IApplicationDbContext _context;

    public GetApplicationByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<ApplicationDto> Handle(GetApplicationByIdQuery query, CancellationToken cancellationToken)
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

        return new ApplicationDto
        {
            Id = application.Id,
            JobId = application.JobId,
            JobTitle = application.Job!.Title,
            CandidateId = application.CandidateId,
            CandidateFullName = application.Candidate!.FullName,
            CoverLetter = application.CoverLetter,
            Stage = application.Stage.ToString(),
            Score = application.Score,
            RejectedReason = application.RejectedReason,
            AppliedAt = application.AppliedAt,
        };
    }
}
