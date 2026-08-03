using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Ops.Commands.RejectPayment;

// Sai số tiền/không nhận được chuyển khoản. JobPackage → job.pending_payment → draft (NTD sửa & nộp
// lại). CreditTopup → không cộng gì, chỉ đánh dấu failed (xem API-DESIGN.md mục 6).
[Authorize(Roles = "admin,moderator")]
public record RejectPaymentCommand : ICommand
{
    public Guid PaymentId { get; init; }
    public Guid ConfirmedBy { get; init; }
}

public class RejectPaymentCommandValidator : AbstractValidator<RejectPaymentCommand>
{
    public RejectPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty();
        RuleFor(x => x.ConfirmedBy).NotEmpty();
    }
}

public class RejectPaymentCommandHandler : ICommandHandler<RejectPaymentCommand>
{
    private readonly IApplicationDbContext _context;

    public RejectPaymentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(RejectPaymentCommand command, CancellationToken cancellationToken)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == command.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Payment), command.PaymentId);

        if (payment.Status != PaymentStatus.Pending)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                nameof(command.PaymentId), "Chỉ từ chối được giao dịch đang chờ.")]);
        }

        payment.ConfirmFailed(command.ConfirmedBy);

        if (payment.Type == PaymentType.JobPackage)
        {
            var jobPurchase = await _context.JobPurchases.FirstOrDefaultAsync(jp => jp.PaymentId == payment.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.JobPurchase), payment.Id);

            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobPurchase.JobId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.Job), jobPurchase.JobId);

            job.RejectPayment();
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
