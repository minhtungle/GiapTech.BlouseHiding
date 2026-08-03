using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Employers.Commands.InviteMember;

// Không mời thêm owner qua đây (ERD mục 2.3) — chỉ hr_manager/hr_member.
[Authorize(Roles = Roles.Employer)]
public record InviteMemberCommand : ICommand<Guid>
{
    public Guid UserId { get; init; }
    public Guid OrganizationId { get; init; }
    public string Email { get; init; } = string.Empty;
    public EmployerMemberRole InvitedRole { get; init; }
}

public class InviteMemberCommandValidator : AbstractValidator<InviteMemberCommand>
{
    public InviteMemberCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.InvitedRole).Must(r => r is EmployerMemberRole.HrManager or EmployerMemberRole.HrMember)
            .WithMessage("Chỉ mời vai trò hr_manager hoặc hr_member.");
    }
}

public class InviteMemberCommandHandler : ICommandHandler<InviteMemberCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;
    private readonly IInvitationSender _invitationSender;

    public InviteMemberCommandHandler(IApplicationDbContext context, IIdentityService identityService, IInvitationSender invitationSender)
    {
        _context = context;
        _identityService = identityService;
        _invitationSender = invitationSender;
    }

    public async ValueTask<Guid> Handle(InviteMemberCommand command, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations
            .Include(o => o.Members)
            .Include(o => o.Invitations)
            .FirstOrDefaultAsync(o => o.Id == command.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Organization), command.OrganizationId);

        var isMember = organization.Members.Any(m => m.UserId == command.UserId);
        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        var existingUser = await _identityService.FindByEmailAsync(command.Email);
        var alreadyMemberByEmail = existingUser is not null && organization.Members.Any(m => m.UserId == existingUser.Id);
        if (alreadyMemberByEmail)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(nameof(command.Email), "Email này đã là thành viên tổ chức.")]);
        }

        var hasPendingInvitation = organization.Invitations.Any(i => i.Email == command.Email && !i.IsAccepted && !i.IsExpired);
        if (hasPendingInvitation)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(nameof(command.Email), "Email này đang có lời mời chờ chấp nhận.")]);
        }

        var token = InvitationTokenGenerator.GenerateToken();
        var invitation = organization.InviteMember(command.Email, command.InvitedRole, command.UserId, InvitationTokenGenerator.Hash(token));

        _context.OrganizationInvitations.Add(invitation);
        await _context.SaveChangesAsync(cancellationToken);

        await _invitationSender.SendAsync(command.Email, organization.Name, token, cancellationToken);

        return invitation.Id;
    }
}
