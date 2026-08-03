using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Enums;

namespace GiapTech.BlouseHiding.Application.Auth.Commands.Register;

public record RegisterCommand : ICommand<Guid>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;

    // "candidate" hoặc "employer" — Public tự chọn vai trò lúc đăng ký, xem API-DESIGN.md mục 2.
    public string Role { get; init; } = Roles.Candidate;
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x => x.Role).Must(r => r is Roles.Candidate or Roles.Employer)
            .WithMessage("Role đăng ký chỉ được là candidate hoặc employer.");
    }
}

public class RegisterCommandHandler : ICommandHandler<RegisterCommand, Guid>
{
    private readonly IIdentityService _identityService;
    private readonly IApplicationDbContext _context;
    private readonly IOtpSender _otpSender;

    public RegisterCommandHandler(IIdentityService identityService, IApplicationDbContext context, IOtpSender otpSender)
    {
        _identityService = identityService;
        _context = context;
        _otpSender = otpSender;
    }

    public async ValueTask<Guid> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var existing = await _identityService.FindByEmailAsync(command.Email);
        if (existing is not null)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(nameof(command.Email), "Email đã được đăng ký.")]);
        }

        var (result, userId) = await _identityService.RegisterUserAsync(command.Email, command.Password, command.Role);

        if (!result.Succeeded)
        {
            throw new ValidationException(result.Errors.Select(e => new FluentValidation.Results.ValidationFailure(nameof(command.Email), e)));
        }

        var code = OtpGenerator.GenerateCode();
        _context.OtpCodes.Add(new OtpCode
        {
            UserId = userId,
            Purpose = OtpPurpose.Register,
            CodeHash = OtpGenerator.Hash(code),
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10),
        });
        await _context.SaveChangesAsync(cancellationToken);

        await _otpSender.SendAsync(command.Email, code, OtpPurpose.Register, cancellationToken);

        return userId;
    }
}
