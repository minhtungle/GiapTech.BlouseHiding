using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Models;

namespace GiapTech.BlouseHiding.Application.Employers.Queries.SearchCandidates;

public record CandidateSearchResultDto
{
    public Guid Id { get; init; }
    public string? Headline { get; init; }
    public bool IsUnlocked { get; init; }
    public int UnlockCost { get; init; }

    // Chỉ có giá trị khi IsUnlocked = true (ERD mục 7 — kết quả tìm kiếm ẩn liên hệ cho tới khi unlock).
    public string? ContactEmail { get; init; }
}

public record SearchCandidatesQuery : IQuery<List<CandidateSearchResultDto>>
{
    public Guid OrganizationId { get; init; }
    public Guid UserId { get; init; }
    public Guid? SpecialtyId { get; init; }
    public Guid? LocationId { get; init; }
}

public class SearchCandidatesQueryHandler : IQueryHandler<SearchCandidatesQuery, List<CandidateSearchResultDto>>
{
    private const int UnlockCost = 15;

    private readonly IApplicationDbContext _context;
    private readonly IIdentityService _identityService;

    public SearchCandidatesQueryHandler(IApplicationDbContext context, IIdentityService identityService)
    {
        _context = context;
        _identityService = identityService;
    }

    public async ValueTask<List<CandidateSearchResultDto>> Handle(SearchCandidatesQuery query, CancellationToken cancellationToken)
    {
        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == query.OrganizationId && m.UserId == query.UserId, cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        var profiles = _context.CandidateProfiles.AsQueryable();

        if (query.SpecialtyId is not null)
        {
            profiles = profiles.Where(p => p.Specialties.Any(s => s.SpecialtyId == query.SpecialtyId));
        }

        if (query.LocationId is not null)
        {
            profiles = profiles.Where(p => p.LocationId == query.LocationId);
        }

        var unlockedCandidateIds = await _context.ProfileUnlocks
            .Where(u => u.OrganizationId == query.OrganizationId)
            .Select(u => u.CandidateId)
            .ToListAsync(cancellationToken);

        var results = await profiles
            .Select(p => new { p.Id, p.Headline, p.UserId })
            .ToListAsync(cancellationToken);

        var dtos = new List<CandidateSearchResultDto>();

        foreach (var p in results)
        {
            var isUnlocked = unlockedCandidateIds.Contains(p.Id);
            string? contactEmail = null;

            if (isUnlocked)
            {
                var user = await _identityService.FindByIdAsync(p.UserId);
                contactEmail = user?.Email;
            }

            dtos.Add(new CandidateSearchResultDto
            {
                Id = p.Id,
                Headline = p.Headline,
                IsUnlocked = isUnlocked,
                UnlockCost = UnlockCost,
                ContactEmail = contactEmail,
            });
        }

        return dtos;
    }
}
