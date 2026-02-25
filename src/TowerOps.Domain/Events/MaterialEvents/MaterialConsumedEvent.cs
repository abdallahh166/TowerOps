using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.MaterialEvents
{
    public sealed record MaterialConsumedEvent(
        Guid MaterialId,
        string MaterialName,
        Guid VisitId,
        decimal ConsumedQuantity,
        string Unit,
        string PerformedBy,
        DateTime ConsumedAt
    ) : DomainEvent;
}
