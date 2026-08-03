using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;

namespace GiapTech.BlouseHiding.Application.Catalog.Commands.CreateLocation;

[Authorize(Roles = "admin,moderator")]
public record CreateLocationCommand : ICommand<Guid>
{
    public string Name { get; init; } = string.Empty;
    public Guid? ParentId { get; init; }
    public Dictionary<string, string> Translations { get; init; } = new();
}

public class CreateLocationCommandValidator : AbstractValidator<CreateLocationCommand>
{
    public CreateLocationCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleForEach(x => x.Translations.Keys).Must(CatalogLocales.IsSupported)
            .WithMessage("Locale không được hỗ trợ — chỉ chấp nhận en/ja/zh/ko/es.");
    }
}

public class CreateLocationCommandHandler : ICommandHandler<CreateLocationCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateLocationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Guid> Handle(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var entity = new Location
        {
            Name = command.Name,
            ParentId = command.ParentId,
            Translations = command.Translations
                .Select(kv => new LocationTranslation { Locale = kv.Key, Name = kv.Value })
                .ToList(),
        };

        _context.Locations.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
