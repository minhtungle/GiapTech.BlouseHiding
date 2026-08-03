using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Entities;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Payments.Commands.CreateJobPackagePayment;

public record PaymentInstructionsDto
{
    public Guid PaymentId { get; init; }
    public decimal Amount { get; init; }
    public string ReferenceCode { get; init; } = string.Empty;
}

// Gọi ngay sau POST /jobs/{id}/submit khi job đã ở pending_payment (xem API-DESIGN.md mục 5-6).
// MVP chỉ có provider = ManualTransfer — trả thông tin chuyển khoản, không trả URL cổng thanh toán
// (ADR-0003).
[Authorize(Roles = Roles.Employer)]
public record CreateJobPackagePaymentCommand : ICommand<PaymentInstructionsDto>
{
    public Guid UserId { get; init; }
    public Guid JobId { get; init; }
    public Guid PackageId { get; init; }
}

public class CreateJobPackagePaymentCommandValidator : AbstractValidator<CreateJobPackagePaymentCommand>
{
    public CreateJobPackagePaymentCommandValidator()
    {
        RuleFor(x => x.JobId).NotEmpty();
        RuleFor(x => x.PackageId).NotEmpty();
    }
}

public class CreateJobPackagePaymentCommandHandler : ICommandHandler<CreateJobPackagePaymentCommand, PaymentInstructionsDto>
{
    private readonly IApplicationDbContext _context;

    public CreateJobPackagePaymentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<PaymentInstructionsDto> Handle(CreateJobPackagePaymentCommand command, CancellationToken cancellationToken)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == command.JobId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Job), command.JobId);

        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == job.OrganizationId && m.UserId == command.UserId, cancellationToken);
        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        if (job.Status != JobStatus.PendingPayment)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                nameof(command.JobId), "Tin không ở trạng thái chờ thanh toán.")]);
        }

        var package = await _context.JobPackages.FirstOrDefaultAsync(p => p.Id == command.PackageId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.JobPackage), command.PackageId);

        var hasPendingPayment = await _context.JobPurchases
            .Where(jp => jp.JobId == job.Id)
            .Join(_context.Payments, jp => jp.PaymentId, p => p.Id, (jp, p) => p)
            .AnyAsync(p => p.Status == PaymentStatus.Pending, cancellationToken);

        if (hasPendingPayment)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                nameof(command.JobId), "Tin đã có giao dịch thanh toán đang chờ xử lý.")]);
        }

        var payment = new Payment
        {
            OrganizationId = job.OrganizationId,
            Type = PaymentType.JobPackage,
            Amount = package.Price,
            ReferenceCode = PaymentReferenceCodeGenerator.Generate(),
        };

        _context.Payments.Add(payment);

        _context.JobPurchases.Add(new JobPurchase
        {
            JobId = job.Id,
            PackageId = package.Id,
            OrganizationId = job.OrganizationId,
            PaymentId = payment.Id,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(package.DurationDays),
        });

        await _context.SaveChangesAsync(cancellationToken);

        return new PaymentInstructionsDto
        {
            PaymentId = payment.Id,
            Amount = payment.Amount,
            ReferenceCode = payment.ReferenceCode!,
        };
    }
}
