using GiapTech.BlouseHiding.Application.Common.Models;

namespace GiapTech.BlouseHiding.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId);

    Task<bool> IsInRoleAsync(string userId, string role);

    Task<bool> AuthorizeAsync(string userId, string policyName);

    Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

    Task<Result> DeleteUserAsync(string userId);

    Task<(Result Result, Guid UserId)> RegisterUserAsync(string email, string password, string role);

    Task<AuthUserDto?> FindByEmailAsync(string email);

    Task<AuthUserDto?> FindByIdAsync(Guid userId);

    Task<bool> CheckPasswordAsync(Guid userId, string password);

    Task MarkEmailVerifiedAsync(Guid userId);

    Task SetPasswordAsync(Guid userId, string newPassword);
}
