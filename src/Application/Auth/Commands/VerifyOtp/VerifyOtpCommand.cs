using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Auth.Commands.VerifyOtp;

public record VerifyOtpCommand : ICommand<bool>
{
    public string Email { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
}

public class VerifyOtpCommandValidator : AbstractValidator<VerifyOtpCommand>
{
    public VerifyOtpCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Code).NotEmpty().Length(6);
    }
}

public class VerifyOtpCommandHandler : ICommandHandler<VerifyOtpCommand, bool>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;

    public VerifyOtpCommandHandler(IIdentityService identityService, IApplicationDbContext context)
    {
        _identityService = identityService;
        _context = context;
    }

    public async ValueTask<bool> Handle(VerifyOtpCommand command, CancellationToken cancellationToken)
    {
        var user = await _identityService.FindByEmailAsync(command.Email)
            ?? throw new NotFoundException(nameof(command.Email), command.Email);

        var codeHash = OtpGenerator.Hash(command.Code);

        var otp = await _context.OtpCodes
            .Where(o => o.UserId == user.Id && o.Purpose == OtpPurpose.Register && o.CodeHash == codeHash)
            .OrderByDescending(o => o.ExpiresAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp is null || !otp.IsValid)
        {
            return false;
        }

        otp.ConsumedAt = DateTimeOffset.UtcNow;
        await _identityService.MarkEmailVerifiedAsync(user.Id);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
