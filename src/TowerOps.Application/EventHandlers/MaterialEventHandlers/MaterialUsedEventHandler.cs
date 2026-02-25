using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common.Events;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Events.MaterialEvents;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.EventHandlers.MaterialEventHandlers;

public class MaterialUsedEventHandler : INotificationHandler<DomainEventNotification<MaterialUsedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IMaterialRepository _materialRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly ILogger<MaterialUsedEventHandler> _logger;

    public MaterialUsedEventHandler(
        INotificationService notificationService,
        IMaterialRepository materialRepository,
        IVisitRepository visitRepository,
        ILogger<MaterialUsedEventHandler> logger)
    {
        _notificationService = notificationService;
        _materialRepository = materialRepository;
        _visitRepository = visitRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<MaterialUsedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var material = await _materialRepository.GetByIdAsNoTrackingAsync(domainEvent.MaterialId, cancellationToken);
            var visit = await _visitRepository.GetByIdAsNoTrackingAsync(domainEvent.VisitId, cancellationToken);

            if (material != null && visit != null)
            {
                // Log material usage for reporting
                _logger.LogInformation(
                    "Material used in visit: Material {MaterialId} ({MaterialName}), Visit {VisitId}, Quantity {Quantity}, Cost {Cost}",
                    domainEvent.MaterialId,
                    material.Name,
                    domainEvent.VisitId,
                    domainEvent.Quantity,
                    domainEvent.TotalCost);
            }

            _logger.LogInformation(
                "Material used event handled: Material {MaterialId}, Visit {VisitId}, Quantity {Quantity}",
                domainEvent.MaterialId,
                domainEvent.VisitId,
                domainEvent.Quantity);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling material used event for material {MaterialId}",
                domainEvent.MaterialId);
        }
    }
}

