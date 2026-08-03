using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Candidates.Commands.AddProfileSpecialty;

[Authorize(Roles = Roles.Candidate)]
public record AddProfileSpecialtyCommand : ICommand<Guid>
{
    public Guid UserId { get; init; }
    public Guid SpecialtyId { get; init; }
    public SpecialtyLevel Level { get; init; }
}

public class AddProfileSpecialtyCommandValidator : AbstractValidator<AddProfileSpecialtyCommand>
{
    public AddProfileSpecialtyCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.SpecialtyId).NotEmpty();
        RuleFor(x => x.Level).IsInEnum();
    }
}

public class AddProfileSpecialtyCommandHandler : ICommandHandler<AddProfileSpecialtyCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddProfileSpecialtyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Guid> Handle(AddProfileSpecialtyCommand command, CancellationToken cancellationToken)
    {
        var profile = await _context.CandidateProfiles
            .Include(p => p.Specialties)
            .Include(p => p.Licenses)
            .FirstOrDefaultAsync(p => p.UserId == command.UserId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.CandidateProfile), command.UserId);

        var specialtyExists = await _context.Specialties.AnyAsync(s => s.Id == command.SpecialtyId, cancellationToken);
        if (!specialtyExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.Specialty), command.SpecialtyId);
        }

        if (profile.Specialties.Any(s => s.SpecialtyId == command.SpecialtyId))
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                nameof(command.SpecialtyId), "Chuyên khoa này đã được gắn vào hồ sơ.")]);
        }

        var profileSpecialty = new Domain.Entities.ProfileSpecialty
        {
            ProfileId = profile.Id,
            SpecialtyId = command.SpecialtyId,
            Level = command.Level,
        };

        profile.Specialties.Add(profileSpecialty);
        _context.ProfileSpecialties.Add(profileSpecialty);
        profile.CompletionPct = ProfileCompletion.Calculate(profile);

        await _context.SaveChangesAsync(cancellationToken);

        return profileSpecialty.Id;
    }
}
