using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Enums;

namespace GiapTech.BlouseHiding.Application.Ops.Queries.GetPendingPayments;

public record PendingPaymentDto
{
    public Guid Id { get; init; }
    public string ReferenceCode { get; init; } = string.Empty;
    public string OrganizationName { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

[Authorize(Roles = "admin,moderator")]
public record GetPendingPaymentsQuery : IQuery<List<PendingPaymentDto>>;

public class GetPendingPaymentsQueryHandler : IQueryHandler<GetPendingPaymentsQuery, List<PendingPaymentDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingPaymentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<List<PendingPaymentDto>> Handle(GetPendingPaymentsQuery query, CancellationToken cancellationToken)
    {
        return await _context.Payments
            .Where(p => p.Status == PaymentStatus.Pending)
            .OrderBy(p => p.Created)
            .Select(p => new PendingPaymentDto
            {
                Id = p.Id,
                ReferenceCode = p.ReferenceCode ?? string.Empty,
                OrganizationName = p.Organization != null ? p.Organization.Name : string.Empty,
                Type = p.Type.ToString(),
                Amount = p.Amount,
                CreatedAt = p.Created,
            })
            .ToListAsync(cancellationToken);
    }
}
