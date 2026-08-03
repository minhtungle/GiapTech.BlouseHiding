using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;

namespace GiapTech.BlouseHiding.Application.Ops.Queries.GetPendingLicenses;

public record PendingLicenseDto
{
    public Guid Id { get; init; }
    public Guid ProfileId { get; init; }
    public string CandidateFullName { get; init; } = string.Empty;
    public string LicenseNo { get; init; } = string.Empty;
    public string IssuedBy { get; init; } = string.Empty;
    public string DocumentUrl { get; init; } = string.Empty;
    public DateOnly IssuedAt { get; init; }
}

[Authorize(Roles = "admin,moderator")]
public record GetPendingLicensesQuery : IQuery<List<PendingLicenseDto>>;

public class GetPendingLicensesQueryHandler : IQueryHandler<GetPendingLicensesQuery, List<PendingLicenseDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingLicensesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<List<PendingLicenseDto>> Handle(GetPendingLicensesQuery query, CancellationToken cancellationToken)
    {
        return await _context.Licenses
            .Where(l => l.VerifyStatus == Domain.Enums.LicenseVerifyStatus.Pending)
            .OrderBy(l => l.IssuedAt)
            .Select(l => new PendingLicenseDto
            {
                Id = l.Id,
                ProfileId = l.ProfileId,
                CandidateFullName = l.Profile!.FullName,
                LicenseNo = l.LicenseNo,
                IssuedBy = l.IssuedBy,
                DocumentUrl = l.DocumentUrl,
                IssuedAt = l.IssuedAt,
            })
            .ToListAsync(cancellationToken);
    }
}
