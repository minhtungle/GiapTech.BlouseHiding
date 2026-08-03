namespace GiapTech.BlouseHiding.Application.Candidates;

public record CandidateProfileDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string? Headline { get; init; }
    public string? Summary { get; init; }
    public int CompletionPct { get; init; }
    public List<ProfileSpecialtyDto> Specialties { get; init; } = [];
    public List<LicenseDto> Licenses { get; init; } = [];
}

public record ProfileSpecialtyDto
{
    public Guid SpecialtyId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Level { get; init; } = string.Empty;
}

public record LicenseDto
{
    public Guid Id { get; init; }
    public string LicenseNo { get; init; } = string.Empty;
    public string VerifyStatus { get; init; } = string.Empty;
    public DateOnly? ExpiredAt { get; init; }
    public string? RejectReason { get; init; }
}
