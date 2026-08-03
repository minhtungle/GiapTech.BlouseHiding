namespace GiapTech.BlouseHiding.Application.Applications;

public record ApplicationDto
{
    public Guid Id { get; init; }
    public Guid JobId { get; init; }
    public string JobTitle { get; init; } = string.Empty;
    public Guid CandidateId { get; init; }
    public string CandidateFullName { get; init; } = string.Empty;
    public string? CoverLetter { get; init; }
    public string Stage { get; init; } = string.Empty;
    public int? Score { get; init; }
    public string? RejectedReason { get; init; }
    public DateTimeOffset AppliedAt { get; init; }
}

public record ApplicationStageHistoryDto
{
    public string FromStage { get; init; } = string.Empty;
    public string ToStage { get; init; } = string.Empty;
    public Guid ChangedBy { get; init; }
    public DateTimeOffset ChangedAt { get; init; }
    public bool Silent { get; init; }
}
