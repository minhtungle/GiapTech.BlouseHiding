namespace GiapTech.BlouseHiding.Domain.Entities;

public class CandidateProfile : BaseAuditableEntity
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public DateOnly? Dob { get; set; }

    public Gender? Gender { get; set; }

    public string? AvatarUrl { get; set; }

    public string? Headline { get; set; }

    public string? Summary { get; set; }

    public string? Address { get; set; }

    public Guid? LocationId { get; set; }

    public int CompletionPct { get; set; }

    public List<License> Licenses { get; private set; } = [];

    public List<ProfileSpecialty> Specialties { get; private set; } = [];

    public License AddLicense(string licenseNo, string issuedBy, DateOnly issuedAt, DateOnly? expiredAt, string documentUrl, string? scope)
    {
        var license = new License
        {
            ProfileId = Id,
            LicenseNo = licenseNo,
            IssuedBy = issuedBy,
            Scope = scope,
            IssuedAt = issuedAt,
            ExpiredAt = expiredAt,
            DocumentUrl = documentUrl,
            VerifyStatus = LicenseVerifyStatus.Pending,
        };

        Licenses.Add(license);

        return license;
    }
}
