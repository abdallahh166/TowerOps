namespace TelecomPM.Domain.Events;

// ==================== Domain Event Interface ====================
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
    Guid EventId { get; }
}