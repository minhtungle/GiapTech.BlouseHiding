using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Candidates.Commands.AddLicense;

[Authorize(Roles = Roles.Candidate)]
public record AddLicenseCommand : ICommand<Guid>
{
    public Guid UserId { get; init; }
    public string LicenseNo { get; init; } = string.Empty;
    public string IssuedBy { get; init; } = string.Empty;
    public string? Scope { get; init; }
    public DateOnly IssuedAt { get; init; }
    public DateOnly? ExpiredAt { get; init; }
    public string DocumentUrl { get; init; } = string.Empty;
}

public class AddLicenseCommandValidator : AbstractValidator<AddLicenseCommand>
{
    public AddLicenseCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.LicenseNo).NotEmpty().MaximumLength(100);
        RuleFor(x => x.IssuedBy).NotEmpty().MaximumLength(255);
        RuleFor(x => x.IssuedAt).NotEmpty();
        RuleFor(x => x.DocumentUrl).NotEmpty();
    }
}

public class AddLicenseCommandHandler : ICommandHandler<AddLicenseCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddLicenseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Guid> Handle(AddLicenseCommand command, CancellationToken cancellationToken)
    {
        var profile = await _context.CandidateProfiles
            .Include(p => p.Licenses)
            .FirstOrDefaultAsync(p => p.UserId == command.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.CandidateProfile), command.UserId);

        var license = profile.AddLicense(
            command.LicenseNo,
            command.IssuedBy,
            command.IssuedAt,
            command.ExpiredAt,
            command.DocumentUrl,
            command.Scope);

        _context.Licenses.Add(license);
        profile.CompletionPct = ProfileCompletion.Calculate(profile);

        await _context.SaveChangesAsync(cancellationToken);

        return license.Id;
    }
}
