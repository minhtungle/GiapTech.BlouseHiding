using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GiapTech.BlouseHiding.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
        IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        _authorizationService = authorizationService;
    }

    public async Task<string?> GetUserNameAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user?.UserName;
    }

    public async Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password)
    {
        var user = new ApplicationUser
        {
            UserName = userName,
            Email = userName,
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id.ToString());
    }

    public async Task<bool> IsInRoleAsync(string userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName);

        return result.Succeeded;
    }

    public async Task<Result> DeleteUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user != null ? await DeleteUserAsync(user) : Result.Success();
    }

    // Soft-delete + anonymize (NĐ 13/2023) — không bao giờ xóa cứng, xem docs/database/ERD-CHI-TIET.md
    // mục 4 điểm 5. Giữ lại row users (audit_logs/applications tham chiếu tới vẫn còn hợp lệ).
    public async Task<Result> DeleteUserAsync(ApplicationUser user)
    {
        user.Status = Domain.Enums.UserStatus.Deleted;
        user.Email = $"deleted-{user.Id}@anonymized.local";
        user.UserName = user.Email;
        user.PhoneNumber = null;
        user.NormalizedEmail = user.Email.ToUpperInvariant();
        user.NormalizedUserName = user.Email.ToUpperInvariant();
        user.PasswordHash = null;

        var result = await _userManager.UpdateAsync(user);

        return result.ToApplicationResult();
    }

    public async Task<(Result Result, Guid UserId)> RegisterUserAsync(string email, string password, string role)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            Role = role,
        };

        var result = await _userManager.CreateAsync(user, password);

        return (result.ToApplicationResult(), user.Id);
    }

    public async Task<AuthUserDto?> FindByEmailAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user is null ? null : ToDto(user);
    }

    public async Task<AuthUserDto?> FindByIdAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        return user is null ? null : ToDto(user);
    }

    public async Task<bool> CheckPasswordAsync(Guid userId, string password)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        return user is not null && await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task MarkEmailVerifiedAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return;
        }

        user.EmailVerifiedAt = DateTimeOffset.UtcNow;
        await _userManager.UpdateAsync(user);
    }

    public async Task SetPasswordAsync(Guid userId, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user is null)
        {
            return;
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        await _userManager.ResetPasswordAsync(user, token, newPassword);
    }

    private static AuthUserDto ToDto(ApplicationUser user) =>
        new(user.Id, user.Email!, user.Role, user.Status.ToString(), user.EmailVerifiedAt is not null);
}
