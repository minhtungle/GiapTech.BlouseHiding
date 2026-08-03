using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;

namespace GiapTech.BlouseHiding.Application.Candidates.Commands.UpdateMyProfile;

[Authorize(Roles = Roles.Candidate)]
public record UpdateMyProfileCommand : ICommand<Guid>
{
    public Guid UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? Headline { get; init; }
    public string? Summary { get; init; }
    public DateOnly? Dob { get; init; }
    public Domain.Enums.Gender? Gender { get; init; }
    public string? Address { get; init; }
}

public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(255);
    }
}

public class UpdateMyProfileCommandHandler : ICommandHandler<UpdateMyProfileCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public UpdateMyProfileCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Guid> Handle(UpdateMyProfileCommand command, CancellationToken cancellationToken)
    {
        var profile = await _context.CandidateProfiles
            .FirstOrDefaultAsync(p => p.UserId == command.UserId, cancellationToken);

        if (profile is null)
        {
            profile = new Domain.Entities.CandidateProfile { UserId = command.UserId };
            _context.CandidateProfiles.Add(profile);
        }

        profile.FullName = command.FullName;
        profile.Headline = command.Headline;
        profile.Summary = command.Summary;
        profile.Dob = command.Dob;
        profile.Gender = command.Gender;
        profile.Address = command.Address;
        profile.CompletionPct = ProfileCompletion.Calculate(profile);

        await _context.SaveChangesAsync(cancellationToken);

        return profile.Id;
    }
}
