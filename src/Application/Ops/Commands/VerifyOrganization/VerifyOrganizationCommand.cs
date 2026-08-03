using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Ops.Commands.VerifyOrganization;

[Authorize(Roles = "admin,moderator")]
public record VerifyOrganizationCommand : ICommand
{
    public Guid OrganizationId { get; init; }
    public Guid VerifiedBy { get; init; }
    public OrganizationVerifyAction Action { get; init; }
    public string? RejectReason { get; init; }
}

public class VerifyOrganizationCommandValidator : AbstractValidator<VerifyOrganizationCommand>
{
    public VerifyOrganizationCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.VerifiedBy).NotEmpty();
        RuleFor(x => x.Action).IsInEnum();
        RuleFor(x => x.RejectReason).NotEmpty().When(x => x.Action == OrganizationVerifyAction.Reject)
            .WithMessage("Cần nêu lý do khi từ chối tổ chức.");
    }
}

public class VerifyOrganizationCommandHandler : ICommandHandler<VerifyOrganizationCommand>
{
    private readonly IApplicationDbContext _context;

    public VerifyOrganizationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(VerifyOrganizationCommand command, CancellationToken cancellationToken)
    {
        var organization = await _context.Organizations.FirstOrDefaultAsync(o => o.Id == command.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Organization), command.OrganizationId);

        switch (command.Action)
        {
            case OrganizationVerifyAction.Verify:
                if (organization.VerifyStatus != OrganizationVerifyStatus.Pending)
                {
                    throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                        nameof(command.Action), "Chỉ duyệt được tổ chức đang chờ xác thực.")]);
                }
                organization.Verify(command.VerifiedBy);
                break;

            case OrganizationVerifyAction.Reject:
                if (organization.VerifyStatus != OrganizationVerifyStatus.Pending)
                {
                    throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                        nameof(command.Action), "Chỉ từ chối được tổ chức đang chờ xác thực.")]);
                }
                organization.Reject(command.VerifiedBy, command.RejectReason!);
                break;

            case OrganizationVerifyAction.Suspend:
                if (organization.VerifyStatus != OrganizationVerifyStatus.Verified)
                {
                    throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                        nameof(command.Action), "Chỉ rút xác thực được tổ chức đang verified.")]);
                }
                organization.Suspend(command.VerifiedBy);

                // ERD mục 4.6 — rút xác thực TỰ ĐỘNG suspend mọi tin published của tổ chức, cùng
                // transaction với việc đổi verify_status, không phải bước thủ công riêng dễ quên.
                var publishedJobs = await _context.Jobs
                    .Where(j => j.OrganizationId == organization.Id && j.Status == JobStatus.Published)
                    .ToListAsync(cancellationToken);

                foreach (var job in publishedJobs)
                {
                    job.Suspend();
                }
                break;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
