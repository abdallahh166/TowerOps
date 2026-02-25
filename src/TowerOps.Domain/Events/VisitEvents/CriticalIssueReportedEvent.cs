using TowerOps.Domain.Events;

namespace TowerOps.Domain.Events.VisitEvents;

public sealed record CriticalIssueReportedEvent(
    Guid VisitId,
    Guid SiteId,
    string IssueDescription) : DomainEvent;
