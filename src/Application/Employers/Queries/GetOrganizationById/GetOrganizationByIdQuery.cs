using GiapTech.BlouseHiding.Application.Common.Interfaces;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Employers.Queries.GetOrganizationById;

public record OrganizationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string OrgType { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? LogoUrl { get; init; }
    public string? CoverUrl { get; init; }
    public string? Address { get; init; }
    public string VerifyStatus { get; init; } = string.Empty;
}

public record GetOrganizationByIdQuery : IQuery<OrganizationDto>
{
    public Guid OrganizationId { get; init; }
}

public class GetOrganizationByIdQueryHandler : IQueryHandler<GetOrganizationByIdQuery, OrganizationDto>
{
    private readonly IApplicationDbContext _context;

    public GetOrganizationByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<OrganizationDto> Handle(GetOrganizationByIdQuery query, CancellationToken cancellationToken)
    {
        return await _context.Organizations
            .Where(o => o.Id == query.OrganizationId)
            .Select(o => new OrganizationDto
            {
                Id = o.Id,
                Name = o.Name,
                OrgType = o.OrgType.ToString(),
                Description = o.Description,
                LogoUrl = o.LogoUrl,
                CoverUrl = o.CoverUrl,
                Address = o.Address,
                VerifyStatus = o.VerifyStatus.ToString(),
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Organization), query.OrganizationId);
    }
}
