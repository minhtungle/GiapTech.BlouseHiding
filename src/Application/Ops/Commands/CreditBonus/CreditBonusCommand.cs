using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Ops.Commands.CreditBonus;

// Vận hành cộng Credit thủ công — dùng cho khuyến mãi/hỗ trợ, và tạm thời để test unlock end-to-end
// trước khi bounded context Payments (nạp Credit thật qua manual_transfer) được làm (xem
// docs/nghiep-vu/TIEN-DO-DU-AN.md). Khác `refund` (ERD mục 2.7 chỉ tạo qua credit-refund riêng).
[Authorize(Roles = "admin")]
public record CreditBonusCommand : ICommand
{
    public Guid OrganizationId { get; init; }
    public Guid CreatedBy { get; init; }
    public int Amount { get; init; }
}

public class CreditBonusCommandValidator : AbstractValidator<CreditBonusCommand>
{
    public CreditBonusCommandValidator()
    {
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.CreatedBy).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public class CreditBonusCommandHandler : ICommandHandler<CreditBonusCommand>
{
    private readonly IApplicationDbContext _context;

    public CreditBonusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(CreditBonusCommand command, CancellationToken cancellationToken)
    {
        var wallet = await _context.CreditWallets
            .FirstOrDefaultAsync(w => w.OrganizationId == command.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.CreditWallet), command.OrganizationId);

        var transaction = wallet.Credit(command.Amount, CreditTransactionReason.Bonus, referenceId: null, command.CreatedBy);
        _context.CreditTransactions.Add(transaction);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
