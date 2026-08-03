using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Domain.Enums;

namespace GiapTech.BlouseHiding.Application.Auth.Commands.ResetPassword;

public record ResetPasswordCommand : ICommand
{
    public string Email { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string NewPassword { get; init; } = string.Empty;
}

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Code).NotEmpty().Length(6);
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
    }
}

public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public ResetPasswordCommandHandler(IIdentityService identityService, IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async ValueTask<Unit> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindByEmailAsync(command.Email)
            ?? throw new UnauthorizedAccessException("Mã xác thực không hợp lệ.");

        var codeHash = OtpGenerator.Hash(command.Code);

        var otp = await _context.OtpCodes
            .Where(o => o.UserId == user.Id && o.Purpose == OtpPurpose.ResetPassword && o.CodeHash == codeHash)
            .OrderByDescending(o => o.ExpiresAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp is null || !otp.IsValid)
        {
            throw new UnauthorizedAccessException("Mã xác thực không hợp lệ hoặc đã hết hạn.");
        }

        otp.ConsumedAt = DateTimeOffset.UtcNow;
        await _identityService.SetPasswordAsync(user.Id, command.NewPassword);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
