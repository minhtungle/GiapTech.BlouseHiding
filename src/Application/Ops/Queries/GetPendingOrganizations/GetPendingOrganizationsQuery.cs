using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Enums;

namespace GiapTech.BlouseHiding.Application.Ops.Queries.GetPendingOrganizations;

public record PendingOrganizationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string OrgType { get; init; } = string.Empty;
    public string? LicenseNo { get; init; }
}

[Authorize(Roles = "admin,moderator")]
public record GetPendingOrganizationsQuery : IQuery<List<PendingOrganizationDto>>;

public class GetPendingOrganizationsQueryHandler : IQueryHandler<GetPendingOrganizationsQuery, List<PendingOrganizationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingOrganizationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<List<PendingOrganizationDto>> Handle(GetPendingOrganizationsQuery query, CancellationToken cancellationToken)
    {
        return await _context.Organizations
            .Where(o => o.VerifyStatus == OrganizationVerifyStatus.Pending)
            .OrderBy(o => o.Created)
            .Select(o => new PendingOrganizationDto
            {
                Id = o.Id,
                Name = o.Name,
                OrgType = o.OrgType.ToString(),
                LicenseNo = o.LicenseNo,
            })
            .ToListAsync(cancellationToken);
    }
}
