using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Interfaces.Services;

// ==================== Domain Event Dispatcher Contract ====================
public interface IDomainEventDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}


