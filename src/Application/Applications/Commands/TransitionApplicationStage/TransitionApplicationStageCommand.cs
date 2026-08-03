using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Applications.Commands.TransitionApplicationStage;

// Chuyển stage phải ghi application_stage_history cùng transaction — không cho phép update stage
// mà thiếu lịch sử (CLAUDE.md mục 4 quy tắc bất di bất dịch #3).
[Authorize(Roles = Roles.Employer)]
public record TransitionApplicationStageCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid ApplicationId { get; init; }
    public Domain.Enums.ApplicationStage Stage { get; init; }
    public bool Silent { get; init; }
    public string? RejectedReason { get; init; }
}

public class TransitionApplicationStageCommandValidator : AbstractValidator<TransitionApplicationStageCommand>
{
    public TransitionApplicationStageCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.Stage).IsInEnum();
    }
}

public class TransitionApplicationStageCommandHandler : ICommandHandler<TransitionApplicationStageCommand>
{
    private readonly IApplicationDbContext _context;

    public TransitionApplicationStageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(TransitionApplicationStageCommand command, CancellationToken cancellationToken)
    {
        var application = await _context.Applications
            .Include(a => a.Job)
            .FirstOrDefaultAsync(a => a.Id == command.ApplicationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.JobApplication), command.ApplicationId);

        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == application.Job!.OrganizationId && m.UserId == command.UserId, cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        application.TransitionStage(command.Stage, command.UserId, command.Silent);

        if (command.Stage == Domain.Enums.ApplicationStage.Rejected)
        {
            application.RejectedReason = command.RejectedReason;
        }

        _context.ApplicationStageHistories.AddRange(application.StageHistory);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
