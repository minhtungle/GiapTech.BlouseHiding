using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;

namespace GiapTech.BlouseHiding.Application.Employers.Queries.GetOrganizationMembers;

public record MemberDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string MemberRole { get; init; } = string.Empty;
    public DateTimeOffset? JoinedAt { get; init; }
}

public record PendingInvitationDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string InvitedRole { get; init; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; init; }
}

public record OrganizationMembersDto
{
    public List<MemberDto> Members { get; init; } = [];
    public List<PendingInvitationDto> PendingInvitations { get; init; } = [];
}

public record GetOrganizationMembersQuery : IQuery<OrganizationMembersDto>
{
    public Guid UserId { get; init; }
    public Guid OrganizationId { get; init; }
}

public class GetOrganizationMembersQueryHandler : IQueryHandler<GetOrganizationMembersQuery, OrganizationMembersDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public GetOrganizationMembersQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async ValueTask<OrganizationMembersDto> Handle(GetOrganizationMembersQuery query, CancellationToken cancellationToken)
    {
        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == query.OrganizationId && m.UserId == query.UserId, cancellationToken);
        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        var members = await _context.EmployerMembers
            .Where(m => m.OrganizationId == query.OrganizationId)
            .Select(m => new { m.Id, m.UserId, m.MemberRole, m.JoinedAt })
            .ToListAsync(cancellationToken);

        var memberDtos = new List<MemberDto>();
        foreach (var m in members)
        {
            var user = await _identityService.FindByIdAsync(m.UserId);
            memberDtos.Add(new MemberDto
            {
                Id = m.Id,
                UserId = m.UserId,
                Email = user?.Email ?? string.Empty,
                MemberRole = m.MemberRole.ToString(),
                JoinedAt = m.JoinedAt,
            });
        }

        var pendingInvitations = await _context.OrganizationInvitations
            .Where(i => i.OrganizationId == query.OrganizationId && i.AcceptedAt == null && i.ExpiresAt > DateTimeOffset.UtcNow)
            .Select(i => new PendingInvitationDto
            {
                Id = i.Id,
                Email = i.Email,
                InvitedRole = i.InvitedRole.ToString(),
                ExpiresAt = i.ExpiresAt,
            })
            .ToListAsync(cancellationToken);

        return new OrganizationMembersDto
        {
            Members = memberDtos,
            PendingInvitations = pendingInvitations,
        };
    }
}
