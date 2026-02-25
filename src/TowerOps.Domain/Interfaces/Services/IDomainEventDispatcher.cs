using TowerOps.Domain.Events;

namespace TowerOps.Domain.Interfaces.Services;

// ==================== Domain Event Dispatcher Contract ====================
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}


