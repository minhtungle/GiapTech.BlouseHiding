using GiapTech.BlouseHiding.Application.Common.Interfaces;

namespace GiapTech.BlouseHiding.Application.Employers.Queries.GetMyOrganizations;

public record MyOrganizationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string OrgType { get; init; } = string.Empty;
    public string VerifyStatus { get; init; } = string.Empty;
    public string MemberRole { get; init; } = string.Empty;
}

public record GetMyOrganizationsQuery : IQuery<List<MyOrganizationDto>>
{
    public Guid UserId { get; init; }
}

public class GetMyOrganizationsQueryHandler : IQueryHandler<GetMyOrganizationsQuery, List<MyOrganizationDto>>
{
    private readonly IApplicationDbContext _context;

    public GetMyOrganizationsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<List<MyOrganizationDto>> Handle(GetMyOrganizationsQuery query, CancellationToken cancellationToken)
    {
        return await _context.EmployerMembers
            .Where(m => m.UserId == query.UserId)
            .Select(m => new MyOrganizationDto
            {
                Id = m.Organization!.Id,
                Name = m.Organization!.Name,
                OrgType = m.Organization!.OrgType.ToString(),
                VerifyStatus = m.Organization!.VerifyStatus.ToString(),
                MemberRole = m.MemberRole.ToString(),
            })
            .ToListAsync(cancellationToken);
    }
}
