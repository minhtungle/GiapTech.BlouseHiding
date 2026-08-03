namespace GiapTech.BlouseHiding.Domain.Entities;

public class ApplicationNote : BaseEntity
{
    public Guid ApplicationId { get; set; }

    public Guid AuthorUserId { get; set; }

    public string NoteText { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
