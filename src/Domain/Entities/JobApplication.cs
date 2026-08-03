namespace GiapTech.BlouseHiding.Domain.Entities;

// ERD gọi bảng "applications" — entity đặt tên JobApplication (không phải Application) để tránh
// xung đột với namespace gốc của project Application layer (GiapTech.BlouseHiding.Application).
public class JobApplication : BaseEntity
{
    public Guid JobId { get; set; }

    public Job? Job { get; set; }

    public Guid CandidateId { get; set; }

    public CandidateProfile? Candidate { get; set; }

    public string? CoverLetter { get; set; }

    public string CvSnapshot { get; set; } = string.Empty;

    public ApplicationStage Stage { get; set; } = ApplicationStage.New;

    public int? Score { get; set; }

    public string? RejectedReason { get; set; }

    public DateTimeOffset AppliedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<ApplicationStageHistory> StageHistory { get; private set; } = [];

    public void TransitionStage(ApplicationStage newStage, Guid changedBy, bool silent)
    {
        var history = new ApplicationStageHistory
        {
            ApplicationId = Id,
            FromStage = Stage,
            ToStage = newStage,
            ChangedBy = changedBy,
            ChangedAt = DateTimeOffset.UtcNow,
            Silent = silent,
        };

        Stage = newStage;
        StageHistory.Add(history);
    }
}
