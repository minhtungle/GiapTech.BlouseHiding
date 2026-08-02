namespace GiapTech.BlouseHiding.Domain.Common;

// Domain KHÔNG reference thư viện mediator nào (Dependency Rule — xem CLAUDE.md mục 4) — đây chỉ là
// marker thuần domain. Infrastructure/Interceptors/DomainEventNotification bọc lại thành
// Mediator.INotification lúc dispatch, xem DispatchDomainEventsInterceptor.
public abstract class BaseEvent
{
}
