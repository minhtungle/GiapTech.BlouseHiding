using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Employers.Commands.RemoveMember;

[Authorize(Roles = Roles.Employer)]
public record RemoveMemberCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid MemberId { get; init; }
}

public class RemoveMemberCommandValidator : AbstractValidator<RemoveMemberCommand>
{
    public RemoveMemberCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.MemberId).NotEmpty();
    }
}

public class RemoveMemberCommandHandler : ICommandHandler<RemoveMemberCommand>
{
    private readonly IApplicationDbContext _context;

    public RemoveMemberCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(RemoveMemberCommand command, CancellationToken cancellationToken)
    {
        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == command.OrganizationId && m.UserId == command.UserId, cancellationToken);
        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        var target = await _context.EmployerMembers
            .FirstOrDefaultAsync(m => m.Id == command.MemberId && m.OrganizationId == command.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.EmployerMember), command.MemberId);

        if (target.MemberRole == EmployerMemberRole.Owner)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(nameof(command.MemberId), "Không thể xoá chủ tổ chức (owner).")]);
        }

        _context.EmployerMembers.Remove(target);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
