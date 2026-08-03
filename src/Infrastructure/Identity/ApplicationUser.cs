using GiapTech.BlouseHiding.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace GiapTech.BlouseHiding.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string Role { get; set; } = Domain.Constants.Roles.Candidate;

    public UserStatus Status { get; set; } = UserStatus.Active;

    public string Locale { get; set; } = "vi";

    public DateTimeOffset? EmailVerifiedAt { get; set; }

    public DateTimeOffset? PhoneVerifiedAt { get; set; }
}
