using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;

namespace GiapTech.BlouseHiding.Application.Catalog.Commands.CreateJobPackage;

[Authorize(Roles = "admin,moderator")]
public record CreateJobPackageCommand : ICommand<Guid>
{
    public JobPackageTier Tier { get; init; }
    public string Name { get; init; } = string.Empty;
    public int DurationDays { get; init; }
    public decimal Price { get; init; }
    public int? MaxActiveJobs { get; init; }
    public Dictionary<string, bool> Perks { get; init; } = new();
    public Dictionary<string, string> Translations { get; init; } = new();
}

public class CreateJobPackageCommandValidator : AbstractValidator<CreateJobPackageCommand>
{
    public CreateJobPackageCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DurationDays).GreaterThan(0);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleForEach(x => x.Translations.Keys).Must(CatalogLocales.IsSupported)
            .WithMessage("Locale không được hỗ trợ — chỉ chấp nhận en/ja/zh/ko/es.");
    }
}

public class CreateJobPackageCommandHandler : ICommandHandler<CreateJobPackageCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateJobPackageCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Guid> Handle(CreateJobPackageCommand command, CancellationToken cancellationToken)
    {
        var entity = new JobPackage
        {
            Tier = command.Tier,
            Name = command.Name,
            DurationDays = command.DurationDays,
            Price = command.Price,
            MaxActiveJobs = command.MaxActiveJobs,
            Perks = command.Perks,
            Translations = command.Translations
                .Select(kv => new JobPackageTranslation { Locale = kv.Key, Name = kv.Value })
                .ToList(),
        };

        _context.JobPackages.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
