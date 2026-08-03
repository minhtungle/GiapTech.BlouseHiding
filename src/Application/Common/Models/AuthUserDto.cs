namespace GiapTech.BlouseHiding.Application.Common.Models;

public record AuthUserDto(Guid Id, string Email, string Role, string Status, bool EmailVerified);
