using GiapTech.BlouseHiding.Application.Common.Interfaces;

namespace GiapTech.BlouseHiding.Application.Auth.Commands.Login;

public record LoginResult(string AccessToken, DateTimeOffset AccessTokenExpiresAt, string RefreshToken);

public record LoginCommand : ICommand<LoginResult>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResult>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IApplicationDbContext _context;

    public LoginCommandHandler(IIdentityService identityService, IJwtTokenService jwtTokenService, IApplicationDbContext context)
    {
        _identityService = identityService;
        _jwtTokenService = jwtTokenService;
        _context = context;
    }

    public async ValueTask<LoginResult> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindByEmailAsync(command.Email);

        if (user is null || user.Status != Domain.Enums.UserStatus.Active.ToString())
        {
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");
        }

        var passwordOk = await _identityService.CheckPasswordAsync(user.Id, command.Password);
        if (!passwordOk)
        {
            throw new UnauthorizedAccessException("Email hoặc mật khẩu không đúng.");
        }

        var accessToken = _jwtTokenService.CreateAccessToken(user.Id, user.Email, user.Role);
        var refreshToken = _jwtTokenService.CreateRefreshToken();

        _context.RefreshTokens.Add(new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            TokenHash = _jwtTokenService.HashToken(refreshToken.Token),
            ExpiresAt = refreshToken.ExpiresAt,
        });
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResult(accessToken.Token, accessToken.ExpiresAt, refreshToken.Token);
    }
}
