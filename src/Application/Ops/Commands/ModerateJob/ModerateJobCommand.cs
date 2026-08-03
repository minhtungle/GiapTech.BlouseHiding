using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Ops.Commands.ModerateJob;

[Authorize(Roles = "admin,moderator")]
public record ModerateJobCommand : ICommand
{
    public Guid JobId { get; init; }
    public bool Approved { get; init; }
    public string? RejectReason { get; init; }
}

public class ModerateJobCommandValidator : AbstractValidator<ModerateJobCommand>
{
    public ModerateJobCommandValidator()
    {
        RuleFor(x => x.JobId).NotEmpty();
        RuleFor(x => x.RejectReason).NotEmpty().When(x => !x.Approved)
            .WithMessage("Cần nêu lý do khi từ chối tin.");
    }
}

public class ModerateJobCommandHandler : ICommandHandler<ModerateJobCommand>
{
    private readonly IApplicationDbContext _context;

    public ModerateJobCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(ModerateJobCommand command, CancellationToken cancellationToken)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == command.JobId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Job), command.JobId);

        var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == job.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Organization), job.OrganizationId);

        if (command.Approved && organization.VerifyStatus != Domain.Enums.OrganizationVerifyStatus.Verified)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                nameof(command.JobId), "Chỉ publish tin khi tổ chức đã được xác thực.")]);
        }

        job.Moderate(command.Approved, command.RejectReason, organization.VerifyStatus == Domain.Enums.OrganizationVerifyStatus.Verified);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
