using GiapTech.BlouseHiding.Application.Common.Interfaces;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Employers.Commands.AcceptInvitation;

// Người được mời phải đăng nhập trước (đăng ký nếu chưa có tài khoản, email khớp lời mời) rồi mới
// chấp nhận — không tạo employer_members với user_id rỗng ở bất kỳ bước nào (ERD mục 2.3).
public record AcceptInvitationCommand : ICommand
{
    public Guid UserId { get; init; }
    public string UserEmail { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
}

public class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand>
{
    public AcceptInvitationCommandValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
    }
}

public class AcceptInvitationCommandHandler : ICommandHandler<AcceptInvitationCommand>
{
    private readonly IApplicationDbContext _context;

    public AcceptInvitationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(AcceptInvitationCommand command, CancellationToken cancellationToken)
    {
        var tokenHash = Employers.InvitationTokenGenerator.Hash(command.Token);

        var invitation = await _context.OrganizationInvitations
            .Include(i => i.Organization)
            .FirstOrDefaultAsync(i => i.TokenHash == tokenHash, cancellationToken);

        if (invitation is null || invitation.IsExpired || invitation.IsAccepted)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(nameof(command.Token), "Lời mời không hợp lệ hoặc đã hết hạn.")]);
        }

        if (!string.Equals(invitation.Email, command.UserEmail, StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(nameof(command.UserEmail), "Email đăng nhập không khớp email được mời.")]);
        }

        var member = invitation.Organization!.AcceptInvitation(invitation, command.UserId);

        _context.EmployerMembers.Add(member);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
