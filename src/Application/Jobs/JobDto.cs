namespace GiapTech.BlouseHiding.Application.Jobs;

public record JobDto
{
    public Guid Id { get; init; }
    public Guid OrganizationId { get; init; }
    public string OrganizationName { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string SpecialtyName { get; init; } = string.Empty;
    public string LocationName { get; init; } = string.Empty;
    public string EmploymentType { get; init; } = string.Empty;
    public int? SalaryMin { get; init; }
    public int? SalaryMax { get; init; }
    public bool SalaryNegotiable { get; init; }
    public bool RequiredLicense { get; init; }
    public int MinExperienceYears { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? Requirements { get; init; }
    public string? Benefits { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? RejectReason { get; init; }
    public DateTimeOffset? PublishedAt { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
}
