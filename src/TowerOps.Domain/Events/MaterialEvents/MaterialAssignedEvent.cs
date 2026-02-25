using TowerOps.Domain.ValueObjects;

namespace TowerOps.Domain.Events.MaterialEvents
{
    public sealed record MaterialAssignedEvent(
        Guid MaterialId,
        Guid VisitId,
        Guid EngineerId,
        MaterialQuantity Quantity,
        DateTime AssignedAt
    ) : DomainEvent;
}
