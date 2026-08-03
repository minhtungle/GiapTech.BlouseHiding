using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Applications.Commands.ScoreApplication;

[Authorize(Roles = Roles.Employer)]
public record ScoreApplicationCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid ApplicationId { get; init; }
    public int Score { get; init; }
}

public class ScoreApplicationCommandValidator : AbstractValidator<ScoreApplicationCommand>
{
    public ScoreApplicationCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.Score).InclusiveBetween(0, 100);
    }
}

public class ScoreApplicationCommandHandler : ICommandHandler<ScoreApplicationCommand>
{
    private readonly IApplicationDbContext _context;

    public ScoreApplicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(ScoreApplicationCommand command, CancellationToken cancellationToken)
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

        application.Score = command.Score;
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
