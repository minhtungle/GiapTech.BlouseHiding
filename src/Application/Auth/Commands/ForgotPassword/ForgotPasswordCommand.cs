using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Domain.Enums;

namespace GiapTech.BlouseHiding.Application.Auth.Commands.ForgotPassword;

public record ForgotPasswordCommand : ICommand
{
    public string Email { get; init; } = string.Empty;
}

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;
    private readonly IOtpSender _otpSender;

    public ForgotPasswordCommandHandler(IIdentityService identityService, IApplicationDbContext context, IOtpSender otpSender)
    {
        _identityService = identityService;
        _context = context;
        _otpSender = otpSender;
    }

    public async ValueTask<Unit> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindByEmailAsync(command.Email);

        // Không tiết lộ email có tồn tại hay không — luôn trả về thành công (chống dò email).
        if (user is null)
        {
            return Unit.Value;
        }

        var code = OtpGenerator.GenerateCode();
        _context.OtpCodes.Add(new Domain.Entities.OtpCode
        {
            UserId = user.Id,
            Purpose = OtpPurpose.ResetPassword,
            CodeHash = OtpGenerator.Hash(code),
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10),
        });
        await _context.SaveChangesAsync(cancellationToken);

        await _otpSender.SendAsync(command.Email, code, OtpPurpose.ResetPassword, cancellationToken);

        return Unit.Value;
    }
}
