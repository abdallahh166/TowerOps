using TelecomPM.Domain.Events;

namespace TelecomPM.Domain.Events.VisitEvents;

public sealed record CriticalIssueReportedEvent(
    Guid VisitId,
    Guid SiteId,
    string IssueDescription) : DomainEvent;
