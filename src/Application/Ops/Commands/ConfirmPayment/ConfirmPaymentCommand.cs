using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Ops.Commands.ConfirmPayment;

// Đối soát sao kê ngân hàng theo ReferenceCode rồi xác nhận (xem API-DESIGN.md mục 6, ADR-0003).
// JobPackage → chuyển job.pending_payment → pending (vào hàng đợi duyệt nội dung).
// CreditTopup → cộng Credit vào ví (reason = Purchase, khác Bonus của CreditBonusCommand).
[Authorize(Roles = "admin,moderator")]
public record ConfirmPaymentCommand : ICommand
{
    public Guid PaymentId { get; init; }
    public Guid ConfirmedBy { get; init; }
}

public class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
{
    public ConfirmPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
        RuleFor(x => x.ConfirmedBy).NotEmpty();
    }
}

public class ConfirmPaymentCommandHandler : ICommandHandler<ConfirmPaymentCommand>
{
    private readonly IApplicationDbContext _context;

    public ConfirmPaymentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(ConfirmPaymentCommand command, CancellationToken cancellationToken)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == command.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Payment), command.PaymentId);

        if (payment.Status != PaymentStatus.Pending)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                nameof(command.PaymentId), "Chỉ xác nhận được giao dịch đang chờ.")]);
        }

        payment.ConfirmSuccess(command.ConfirmedBy);

        if (payment.Type == PaymentType.JobPackage)
        {
            var jobPurchase = await _context.JobPurchases.FirstOrDefaultAsync(jp => jp.PaymentId == payment.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.JobPurchase), payment.Id);

            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobPurchase.JobId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Job), jobPurchase.JobId);

            var package = await _context.JobPackages.FirstOrDefaultAsync(p => p.Id == jobPurchase.PackageId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.JobPackage), jobPurchase.PackageId);

            job.ConfirmPayment(package.DurationDays);
        }
        else
        {
            var wallet = await _context.CreditWallets.FirstOrDefaultAsync(w => w.OrganizationId == payment.OrganizationId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.CreditWallet), payment.OrganizationId);

            var transaction = wallet.Credit(payment.CreditAmount!.Value, CreditTransactionReason.Purchase, payment.Id, command.ConfirmedBy);
            _context.CreditTransactions.Add(transaction);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
