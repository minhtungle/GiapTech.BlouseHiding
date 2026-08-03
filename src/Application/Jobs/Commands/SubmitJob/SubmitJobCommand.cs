using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Jobs.Commands.SubmitJob;

// MVP: chỉ hỗ trợ nộp qua gói Free (published không cần thanh toán). Gói trả phí (Eco/Pro/Max) và
// luồng payments/manual_transfer là bounded context riêng, chưa làm — xem TIEN-DO-DU-AN.md.
[Authorize(Roles = Roles.Employer)]
public record SubmitJobCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid JobId { get; init; }
    public Guid PackageId { get; init; }
}

public class SubmitJobCommandValidator : AbstractValidator<SubmitJobCommand>
{
    public SubmitJobCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.JobId).NotEmpty();
        RuleFor(x => x.PackageId).NotEmpty();
    }
}

public class SubmitJobCommandHandler : ICommandHandler<SubmitJobCommand>
{
    private readonly IApplicationDbContext _context;

    public SubmitJobCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(SubmitJobCommand command, CancellationToken cancellationToken)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == command.JobId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Job), command.JobId);

        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == job.OrganizationId && m.UserId == command.UserId, cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        var package = await _context.JobPackages.FirstOrDefaultAsync(p => p.Id == command.PackageId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.JobPackage), command.PackageId);

        if (package.Tier != JobPackageTier.Free)
        {
            throw new Common.Exceptions.ValidationException([new FluentValidation.Results.ValidationFailure(
                nameof(command.PackageId), "Gói trả phí chưa hỗ trợ ở MVP — chỉ nộp được qua gói Free.")]);
        }

        job.Submit(package.Tier, package.DurationDays);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
