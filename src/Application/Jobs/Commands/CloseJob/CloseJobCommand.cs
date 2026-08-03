using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Jobs.Commands.CloseJob;

[Authorize(Roles = Roles.Employer)]
public record CloseJobCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid JobId { get; init; }
}

public class CloseJobCommandValidator : AbstractValidator<CloseJobCommand>
{
    public CloseJobCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.JobId).NotEmpty();
    }
}

public class CloseJobCommandHandler : ICommandHandler<CloseJobCommand>
{
    private readonly IApplicationDbContext _context;

    public CloseJobCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(CloseJobCommand command, CancellationToken cancellationToken)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == command.JobId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Job), command.JobId);

        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == job.OrganizationId && m.UserId == command.UserId, cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        job.Close();

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
