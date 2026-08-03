using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Candidates.Commands.UpdateLicense;

[Authorize(Roles = Roles.Candidate)]
public record UpdateLicenseCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid LicenseId { get; init; }
    public string LicenseNo { get; init; } = string.Empty;
    public string IssuedBy { get; init; } = string.Empty;
    public string? Scope { get; init; }
    public DateOnly IssuedAt { get; init; }
    public DateOnly? ExpiredAt { get; init; }
    public string DocumentUrl { get; init; } = string.Empty;
}

public class UpdateLicenseCommandValidator : AbstractValidator<UpdateLicenseCommand>
{
    public UpdateLicenseCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.LicenseId).NotEmpty();
        RuleFor(x => x.LicenseNo).NotEmpty().MaximumLength(100);
        RuleFor(x => x.IssuedBy).NotEmpty().MaximumLength(255);
        RuleFor(x => x.IssuedAt).NotEmpty();
        RuleFor(x => x.DocumentUrl).NotEmpty();
    }
}

public class UpdateLicenseCommandHandler : ICommandHandler<UpdateLicenseCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateLicenseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(UpdateLicenseCommand command, CancellationToken cancellationToken)
    {
        var license = await _context.Licenses
            .Include(l => l.Profile)
            .FirstOrDefaultAsync(l => l.Id == command.LicenseId && l.Profile!.UserId == command.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.License), command.LicenseId);

        if (!license.CanEdit)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                nameof(command.LicenseId), "Chỉ sửa được CCHN đang chờ duyệt hoặc bị từ chối.")]);
        }

        license.LicenseNo = command.LicenseNo;
        license.IssuedBy = command.IssuedBy;
        license.Scope = command.Scope;
        license.IssuedAt = command.IssuedAt;
        license.ExpiredAt = command.ExpiredAt;
        license.DocumentUrl = command.DocumentUrl;
        license.VerifyStatus = Domain.Enums.LicenseVerifyStatus.Pending;
        license.RejectReason = null;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
