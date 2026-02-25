using TowerOps.Domain.Enums;

namespace TowerOps.Domain.Events.ChecklistTemplateEvents;

public sealed record ChecklistTemplateActivatedEvent(
    Guid ChecklistTemplateId,
    VisitType VisitType,
    string Version) : DomainEvent;
