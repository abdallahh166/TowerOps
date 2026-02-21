using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Events.ChecklistTemplateEvents;

public sealed record ChecklistTemplateActivatedEvent(
    Guid ChecklistTemplateId,
    VisitType VisitType,
    string Version) : DomainEvent;
