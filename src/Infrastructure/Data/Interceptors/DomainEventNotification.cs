using GiapTech.BlouseHiding.Domain.Common;
using Mediator;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Interceptors;

// Bọc BaseEvent (thuần domain, không implement INotification) thành notification Mediator hiểu được —
// giữ Domain không phụ thuộc thư viện mediator nào (xem BaseEvent.cs).
public class DomainEventNotification<TDomainEvent> : INotification
    where TDomainEvent : BaseEvent
{
    public TDomainEvent DomainEvent { get; }

    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }
}
