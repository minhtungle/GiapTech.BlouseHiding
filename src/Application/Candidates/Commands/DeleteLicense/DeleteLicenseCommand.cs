using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Candidates.Commands.DeleteLicense;

[Authorize(Roles = Roles.Candidate)]
public record DeleteLicenseCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid LicenseId { get; init; }
}

public class DeleteLicenseCommandValidator : AbstractValidator<DeleteLicenseCommand>
{
    public DeleteLicenseCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.LicenseId).NotEmpty();
    }
}

public class DeleteLicenseCommandHandler : ICommandHandler<DeleteLicenseCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteLicenseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(DeleteLicenseCommand command, CancellationToken cancellationToken)
    {
        var license = await _context.Licenses
            .Include(l => l.Profile)
            .FirstOrDefaultAsync(l => l.Id == command.LicenseId && l.Profile!.UserId == command.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.License), command.LicenseId);

        if (license.VerifyStatus == Domain.Enums.LicenseVerifyStatus.Verified)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                nameof(command.LicenseId), "Không thể xóa CCHN đã được duyệt.")]);
        }

        _context.Licenses.Remove(license);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
