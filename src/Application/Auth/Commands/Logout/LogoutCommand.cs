using GiapTech.BlouseHiding.Application.Common.Interfaces;

namespace GiapTech.BlouseHiding.Application.Auth.Commands.Logout;

public record LogoutCommand : ICommand
{
    public string RefreshToken { get; init; } = string.Empty;
}

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public LogoutCommandHandler(IApplicationDbContext context, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async ValueTask<Unit> Handle(LogoutCommand command, CancellationToken cancellationToken)
    {
        var tokenHash = _jwtTokenService.HashToken(command.RefreshToken);

        var existing = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (existing is not null && existing.RevokedAt is null)
        {
            existing.RevokedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
