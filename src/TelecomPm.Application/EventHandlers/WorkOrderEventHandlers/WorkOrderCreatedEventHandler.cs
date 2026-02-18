using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TelecomPM.Application.Common.Events;
using TelecomPM.Domain.Events.WorkOrderEvents;

namespace TelecomPM.Application.EventHandlers.WorkOrderEventHandlers;

public sealed class WorkOrderCreatedEventHandler : INotificationHandler<DomainEventNotification<WorkOrderCreatedEvent>>
{
    private readonly ILogger<WorkOrderCreatedEventHandler> _logger;

    public WorkOrderCreatedEventHandler(ILogger<WorkOrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<WorkOrderCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "AUDIT WorkOrderCreated: WorkOrderId={WorkOrderId}, WoNumber={WoNumber}, SiteCode={SiteCode}, OfficeCode={OfficeCode}, SlaClass={SlaClass}, ResponseDeadlineUtc={ResponseDeadlineUtc}, ResolutionDeadlineUtc={ResolutionDeadlineUtc}",
            domainEvent.WorkOrderId,
            domainEvent.WoNumber,
            domainEvent.SiteCode,
            domainEvent.OfficeCode,
            domainEvent.SlaClass,
            domainEvent.ResponseDeadlineUtc,
            domainEvent.ResolutionDeadlineUtc);

        return Task.CompletedTask;
    }
}
