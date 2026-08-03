using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;

namespace GiapTech.BlouseHiding.Application.Catalog.Commands.UpdateLocation;

[Authorize(Roles = "admin,moderator")]
public record UpdateLocationCommand : ICommand
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public Guid? ParentId { get; init; }
    public Dictionary<string, string> Translations { get; init; } = new();
}

public class UpdateLocationCommandValidator : AbstractValidator<UpdateLocationCommand>
{
    public UpdateLocationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleForEach(x => x.Translations.Keys).Must(CatalogLocales.IsSupported)
            .WithMessage("Locale không được hỗ trợ — chỉ chấp nhận en/ja/zh/ko/es.");
    }
}

public class UpdateLocationCommandHandler : ICommandHandler<UpdateLocationCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateLocationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        var entity = await _context.Locations
            .Include(l => l.Translations)
            .FirstOrDefaultAsync(l => l.Id == command.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Location), command.Id.ToString());

        entity.Name = command.Name;
        entity.ParentId = command.ParentId;

        foreach (var (locale, name) in command.Translations)
        {
            var translation = entity.Translations.FirstOrDefault(t => t.Locale == locale);
            if (translation is null)
            {
                entity.Translations.Add(new LocationTranslation { LocationId = entity.Id, Locale = locale, Name = name });
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
