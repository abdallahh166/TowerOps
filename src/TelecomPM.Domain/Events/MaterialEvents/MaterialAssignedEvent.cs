using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Events.MaterialEvents
{
    public sealed record MaterialAssignedEvent(
        Guid MaterialId,
        Guid VisitId,
        Guid EngineerId,
        MaterialQuantity Quantity,
        DateTime AssignedAt
    ) : DomainEvent;
}
