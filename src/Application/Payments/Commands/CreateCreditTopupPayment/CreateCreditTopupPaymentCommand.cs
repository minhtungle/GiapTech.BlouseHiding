using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Application.Payments.Commands.CreateJobPackagePayment;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Entities;
using GiapTech.BlouseHiding.Domain.Enums;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Payments.Commands.CreateCreditTopupPayment;

// 1 Credit = 1.000đ — quy đổi tạm thời cho MVP, chưa có bảng giá gói Credit riêng (khác job_packages).
[Authorize(Roles = Roles.Employer)]
public record CreateCreditTopupPaymentCommand : ICommand<PaymentInstructionsDto>
{
    public Guid UserId { get; init; }
    public Guid OrganizationId { get; init; }
    public int CreditAmount { get; init; }
}

public class CreateCreditTopupPaymentCommandValidator : AbstractValidator<CreateCreditTopupPaymentCommand>
{
    public CreateCreditTopupPaymentCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.CreditAmount).GreaterThan(0);
    }
}

public class CreateCreditTopupPaymentCommandHandler : ICommandHandler<CreateCreditTopupPaymentCommand, PaymentInstructionsDto>
{
    private const int VndPerCredit = 1_000;

    private readonly IApplicationDbContext _context;

    public CreateCreditTopupPaymentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<PaymentInstructionsDto> Handle(CreateCreditTopupPaymentCommand command, CancellationToken cancellationToken)
    {
        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == command.OrganizationId && m.UserId == command.UserId, cancellationToken);
        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        var hasPendingTopup = await _context.Payments.AnyAsync(
            p => p.OrganizationId == command.OrganizationId
                && p.Type == PaymentType.CreditTopup
                && p.Status == PaymentStatus.Pending,
            cancellationToken);

        if (hasPendingTopup)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                nameof(command.OrganizationId), "Tổ chức đã có giao dịch nạp Credit đang chờ xử lý.")]);
        }

        var payment = new Payment
        {
            OrganizationId = command.OrganizationId,
            Type = PaymentType.CreditTopup,
            Amount = command.CreditAmount * VndPerCredit,
            ReferenceCode = PaymentReferenceCodeGenerator.Generate(),
            CreditAmount = command.CreditAmount,
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync(cancellationToken);

        return new PaymentInstructionsDto
        {
            PaymentId = payment.Id,
            Amount = payment.Amount,
            ReferenceCode = payment.ReferenceCode!,
        };
    }
}
