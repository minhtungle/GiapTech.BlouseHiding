using System.ComponentModel.DataAnnotations.Schema;

namespace GiapTech.BlouseHiding.Domain.Common;

public abstract class BaseEntity
{
    // uuid PK cho mọi bảng — khớp docs/database/ERD-CHI-TIET.md (đổi từ int mặc định của template).
    public Guid Id { get; set; } = Guid.NewGuid();

    private readonly List<BaseEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
