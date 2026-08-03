using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;

namespace GiapTech.BlouseHiding.Application.Catalog.Commands.UpdateJobPackage;

[Authorize(Roles = "admin,moderator")]
public record UpdateJobPackageCommand : ICommand
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int DurationDays { get; init; }
    public decimal Price { get; init; }
    public int? MaxActiveJobs { get; init; }
    public Dictionary<string, bool> Perks { get; init; } = new();
    public Dictionary<string, string> Translations { get; init; } = new();
}

public class UpdateJobPackageCommandValidator : AbstractValidator<UpdateJobPackageCommand>
{
    public UpdateJobPackageCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DurationDays).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleForEach(x => x.Translations.Keys).Must(CatalogLocales.IsSupported)
            .WithMessage("Locale không được hỗ trợ — chỉ chấp nhận en/ja/zh/ko/es.");
    }
}

public class UpdateJobPackageCommandHandler : ICommandHandler<UpdateJobPackageCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateJobPackageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(UpdateJobPackageCommand command, CancellationToken cancellationToken)
    {
        var entity = await _context.JobPackages
            .Include(p => p.Translations)
            .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(JobPackage), command.Id.ToString());

        entity.Name = command.Name;
        entity.DurationDays = command.DurationDays;
        entity.Price = command.Price;
        entity.MaxActiveJobs = command.MaxActiveJobs;
        entity.Perks = command.Perks;

        foreach (var (locale, name) in command.Translations)
        {
            var translation = entity.Translations.FirstOrDefault(t => t.Locale == locale);
            if (translation is null)
            {
                entity.Translations.Add(new JobPackageTranslation { JobPackageId = entity.Id, Locale = locale, Name = name });
            }
            else
            {
                translation.Name = name;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
