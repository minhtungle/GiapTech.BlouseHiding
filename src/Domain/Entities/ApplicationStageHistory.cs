namespace GiapTech.BlouseHiding.Domain.Entities;

public class ApplicationStageHistory : BaseEntity
{
    public Guid ApplicationId { get; set; }

    public ApplicationStage FromStage { get; set; }

    public ApplicationStage ToStage { get; set; }

    public Guid ChangedBy { get; set; }

    public DateTimeOffset ChangedAt { get; set; }

    public bool Silent { get; set; }
}
