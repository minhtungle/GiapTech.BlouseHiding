using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Employers.Queries.GetCreditWallet;

public record CreditWalletDto
{
    public Guid OrganizationId { get; init; }
    public int Balance { get; init; }
}

public record GetCreditWalletQuery : IQuery<CreditWalletDto>
{
    public Guid OrganizationId { get; init; }
    public Guid UserId { get; init; }
}

public class GetCreditWalletQueryHandler : IQueryHandler<GetCreditWalletQuery, CreditWalletDto>
{
    private readonly IApplicationDbContext _context;

    public GetCreditWalletQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<CreditWalletDto> Handle(GetCreditWalletQuery query, CancellationToken cancellationToken)
    {
        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == query.OrganizationId && m.UserId == query.UserId, cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        var wallet = await _context.CreditWallets
            .Where(w => w.OrganizationId == query.OrganizationId)
            .Select(w => new CreditWalletDto { OrganizationId = w.OrganizationId, Balance = w.Balance })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.CreditWallet), query.OrganizationId);

        return wallet;
    }
}
