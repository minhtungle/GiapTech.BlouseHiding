using GiapTech.BlouseHiding.Application.Common.Interfaces;

namespace GiapTech.BlouseHiding.Application.Auth.Commands.RefreshToken;

public record RefreshTokenResponse(string AccessToken, DateTimeOffset AccessTokenExpiresAt, string RefreshToken);

public record RefreshTokenCommand : ICommand<RefreshTokenResponse>
{
    public string RefreshToken { get; init; } = string.Empty;
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(IApplicationDbContext context, IJwtTokenService jwtTokenService, IIdentityService identityService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _identityService = identityService;
    }

    public async ValueTask<RefreshTokenResponse> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var tokenHash = _jwtTokenService.HashToken(command.RefreshToken);

        var existing = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, cancellationToken);

        if (existing is null || !existing.IsActive)
        {
            throw new UnauthorizedAccessException("Refresh token không hợp lệ hoặc đã hết hạn.");
        }

        var user = await _identityService.FindByIdAsync(existing.UserId)
            ?? throw new UnauthorizedAccessException("Tài khoản không tồn tại.");

        // Xoay vòng refresh token (rotation) — thu hồi token cũ, phát token mới, chống replay.
        existing.RevokedAt = DateTimeOffset.UtcNow;

        var accessToken = _jwtTokenService.CreateAccessToken(user.Id, user.Email, user.Role);
        var newRefreshToken = _jwtTokenService.CreateRefreshToken();

        _context.RefreshTokens.Add(new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            TokenHash = _jwtTokenService.HashToken(newRefreshToken.Token),
            ExpiresAt = newRefreshToken.ExpiresAt,
        });
        await _context.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(accessToken.Token, accessToken.ExpiresAt, newRefreshToken.Token);
    }
}
