using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Employers.Queries.GetCreditTransactions;

public record CreditTransactionDto
{
    public Guid Id { get; init; }
    public int Amount { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
}

public record GetCreditTransactionsQuery : IQuery<List<CreditTransactionDto>>
{
    public Guid OrganizationId { get; init; }
    public Guid UserId { get; init; }
}

public class GetCreditTransactionsQueryHandler : IQueryHandler<GetCreditTransactionsQuery, List<CreditTransactionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCreditTransactionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<List<CreditTransactionDto>> Handle(GetCreditTransactionsQuery query, CancellationToken cancellationToken)
    {
        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == query.OrganizationId && m.UserId == query.UserId, cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        var wallet = await _context.CreditWallets.FirstOrDefaultAsync(w => w.OrganizationId == query.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.CreditWallet), query.OrganizationId);

        return await _context.CreditTransactions
            .Where(t => t.WalletId == wallet.Id)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new CreditTransactionDto
            {
                Id = t.Id,
                Amount = t.Amount,
                Reason = t.Reason.ToString(),
                CreatedAt = t.CreatedAt,
            })
            .ToListAsync(cancellationToken);
    }
}
