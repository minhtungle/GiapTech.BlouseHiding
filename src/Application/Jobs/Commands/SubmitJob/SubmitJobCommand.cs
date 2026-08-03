using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Jobs.Commands.SubmitJob;

// Gói Free → thẳng "pending" (chờ duyệt nội dung). Gói trả phí (Eco/Pro/Max) → "pending_payment",
// NTD phải gọi tiếp POST /payments/job-package để tạo giao dịch chuyển khoản (xem
// docs/backend/API-DESIGN.md mục 5-6, ADR-0003 — MVP dùng quy trình thủ công).
[Authorize(Roles = Roles.Employer)]
public record SubmitJobCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid JobId { get; init; }
    public Guid PackageId { get; init; }
}

public class SubmitJobCommandValidator : AbstractValidator<SubmitJobCommand>
{
    public SubmitJobCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.JobId).NotEmpty();
        RuleFor(x => x.PackageId).NotEmpty();
    }
}

public class SubmitJobCommandHandler : ICommandHandler<SubmitJobCommand>
{
    private readonly IApplicationDbContext _context;

    public SubmitJobCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(SubmitJobCommand command, CancellationToken cancellationToken)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == command.JobId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Job), command.JobId);

        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == job.OrganizationId && m.UserId == command.UserId, cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        var package = await _context.JobPackages.FirstOrDefaultAsync(p => p.Id == command.PackageId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.JobPackage), command.PackageId);

        job.Submit(package.Tier, package.DurationDays);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
