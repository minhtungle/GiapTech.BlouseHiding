using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;

namespace GiapTech.BlouseHiding.Application.Catalog.Commands.CreateSpecialty;

[Authorize(Roles = "admin,moderator")]
public record CreateSpecialtyCommand : ICommand<Guid>
{
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Guid? ParentId { get; init; }

    // key = locale (en/ja/zh/ko/es)
    public Dictionary<string, string> Translations { get; init; } = new();
}

public class CreateSpecialtyCommandValidator : AbstractValidator<CreateSpecialtyCommand>
{
    public CreateSpecialtyCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleForEach(x => x.Translations.Keys).Must(CatalogLocales.IsSupported)
            .WithMessage("Locale không được hỗ trợ — chỉ chấp nhận en/ja/zh/ko/es.");
    }
}

public class CreateSpecialtyCommandHandler : ICommandHandler<CreateSpecialtyCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateSpecialtyCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Guid> Handle(CreateSpecialtyCommand command, CancellationToken cancellationToken)
    {
        var entity = new Specialty
        {
            Code = command.Code,
            Name = command.Name,
            ParentId = command.ParentId,
            Translations = command.Translations
                .Select(kv => new SpecialtyTranslation { Locale = kv.Key, Name = kv.Value })
                .ToList(),
        };

        _context.Specialties.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
