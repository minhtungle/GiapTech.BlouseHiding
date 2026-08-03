using GiapTech.BlouseHiding.Application.Candidates;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Candidates.Queries.GetMyProfile;

public record GetMyProfileQuery : IQuery<CandidateProfileDto>
{
    public Guid UserId { get; init; }
}

public class GetMyProfileQueryHandler : IQueryHandler<GetMyProfileQuery, CandidateProfileDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentLocale _locale;

    public GetMyProfileQueryHandler(IApplicationDbContext context, ICurrentLocale locale)
    {
        _context = context;
        _locale = locale;
    }

    public async ValueTask<CandidateProfileDto> Handle(GetMyProfileQuery query, CancellationToken cancellationToken)
    {
        var locale = _locale.Value;

        var profile = await _context.CandidateProfiles
            .Where(p => p.UserId == query.UserId)
            .Select(p => new CandidateProfileDto
            {
                Id = p.Id,
                FullName = p.FullName,
                Headline = p.Headline,
                Summary = p.Summary,
                CompletionPct = p.CompletionPct,
                Specialties = p.Specialties.Select(s => new ProfileSpecialtyDto
                {
                    SpecialtyId = s.SpecialtyId,
                    Code = s.Specialty!.Code,
                    Name = s.Specialty!.Translations.Where(t => t.Locale == locale).Select(t => t.Name).FirstOrDefault() ?? s.Specialty!.Name,
                    Level = s.Level.ToString(),
                }).ToList(),
                Licenses = p.Licenses.Select(l => new LicenseDto
                {
                    Id = l.Id,
                    LicenseNo = l.LicenseNo,
                    VerifyStatus = l.VerifyStatus.ToString(),
                    ExpiredAt = l.ExpiredAt,
                    RejectReason = l.RejectReason,
                }).ToList(),
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(CandidateProfile), query.UserId);

        return profile;
    }
}
