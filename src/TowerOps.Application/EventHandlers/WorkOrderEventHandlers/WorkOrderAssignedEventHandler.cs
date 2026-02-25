using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TowerOps.Application.Common.Events;
using TowerOps.Domain.Events.WorkOrderEvents;

namespace TowerOps.Application.EventHandlers.WorkOrderEventHandlers;

public sealed class WorkOrderAssignedEventHandler : INotificationHandler<DomainEventNotification<WorkOrderAssignedEvent>>
{
    private readonly ILogger<WorkOrderAssignedEventHandler> _logger;

    public WorkOrderAssignedEventHandler(ILogger<WorkOrderAssignedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<WorkOrderAssignedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "AUDIT WorkOrderAssigned: WorkOrderId={WorkOrderId}, EngineerId={EngineerId}, EngineerName={EngineerName}, AssignedBy={AssignedBy}, AssignedAtUtc={AssignedAtUtc}, Status={Status}",
            domainEvent.WorkOrderId,
            domainEvent.EngineerId,
            domainEvent.EngineerName,
            domainEvent.AssignedBy,
            domainEvent.AssignedAtUtc,
            domainEvent.Status);

        return Task.CompletedTask;
    }
}
