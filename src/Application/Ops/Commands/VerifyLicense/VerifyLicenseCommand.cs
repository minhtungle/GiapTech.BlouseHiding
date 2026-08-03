using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Ops.Commands.VerifyLicense;

[Authorize(Roles = "admin,moderator")]
public record VerifyLicenseCommand : ICommand
{
    public Guid LicenseId { get; init; }
    public Guid VerifiedBy { get; init; }
    public bool Approved { get; init; }
    public string? RejectReason { get; init; }
}

public class VerifyLicenseCommandValidator : AbstractValidator<VerifyLicenseCommand>
{
    public VerifyLicenseCommandValidator()
    {
        RuleFor(x => x.LicenseId).NotEmpty();
        RuleFor(x => x.VerifiedBy).NotEmpty();
        RuleFor(x => x.RejectReason).NotEmpty().When(x => !x.Approved)
            .WithMessage("Cần nêu lý do khi từ chối CCHN.");
    }
}

public class VerifyLicenseCommandHandler : ICommandHandler<VerifyLicenseCommand>
{
    private readonly IApplicationDbContext _context;

    public VerifyLicenseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(VerifyLicenseCommand command, CancellationToken cancellationToken)
    {
        var license = await _context.Licenses
            .FirstOrDefaultAsync(l => l.Id == command.LicenseId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.License), command.LicenseId);

        if (command.Approved)
        {
            license.Verify(command.VerifiedBy);
        }
        else
        {
            license.Reject(command.VerifiedBy, command.RejectReason!);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
