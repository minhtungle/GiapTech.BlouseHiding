using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Payments.Queries.GetPaymentById;

public record PaymentDto
{
    public Guid Id { get; init; }
    public string Type { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string ReferenceCode { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}

public record GetPaymentByIdQuery : IQuery<PaymentDto>
{
    public Guid UserId { get; init; }
    public Guid PaymentId { get; init; }
}

public class GetPaymentByIdQueryHandler : IQueryHandler<GetPaymentByIdQuery, PaymentDto>
{
    private readonly IApplicationDbContext _context;

    public GetPaymentByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<PaymentDto> Handle(GetPaymentByIdQuery query, CancellationToken cancellationToken)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.Id == query.PaymentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Payment), query.PaymentId);

        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == payment.OrganizationId && m.UserId == query.UserId, cancellationToken);
        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        return new PaymentDto
        {
            Id = payment.Id,
            Type = payment.Type.ToString(),
            Amount = payment.Amount,
            ReferenceCode = payment.ReferenceCode ?? string.Empty,
            Status = payment.Status.ToString(),
        };
    }
}
