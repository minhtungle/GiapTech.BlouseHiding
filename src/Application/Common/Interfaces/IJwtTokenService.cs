namespace GiapTech.BlouseHiding.Application.Common.Interfaces;

public record AccessTokenResult(string Token, DateTimeOffset ExpiresAt);

public record RefreshTokenResult(string Token, DateTimeOffset ExpiresAt);

public interface IJwtTokenService
{
    AccessTokenResult CreateAccessToken(Guid userId, string email, string role);

    RefreshTokenResult CreateRefreshToken();

    string HashToken(string token);
}
