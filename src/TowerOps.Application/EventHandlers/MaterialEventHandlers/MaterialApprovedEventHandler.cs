using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common.Events;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Events.MaterialEvents;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.EventHandlers.MaterialEventHandlers;

public class MaterialApprovedEventHandler : INotificationHandler<DomainEventNotification<MaterialApprovedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IVisitRepository _visitRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<MaterialApprovedEventHandler> _logger;

    public MaterialApprovedEventHandler(
        INotificationService notificationService,
        IVisitRepository visitRepository,
        IUserRepository userRepository,
        ILogger<MaterialApprovedEventHandler> logger)
    {
        _notificationService = notificationService;
        _visitRepository = visitRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<MaterialApprovedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var visit = await _visitRepository.GetByIdAsNoTrackingAsync(domainEvent.VisitId, cancellationToken);
            var approver = await _userRepository.GetByIdAsNoTrackingAsync(domainEvent.ApprovedBy, cancellationToken);

            if (visit != null)
            {
                // Notify engineer that material usage has been approved
                await _notificationService.SendPushNotificationAsync(
                    visit.EngineerId,
                    "âœ… Material Usage Approved",
                    $"Material usage for visit {visit.VisitNumber} has been approved by {approver?.Name ?? "Manager"}",
                    cancellationToken);
            }

            _logger.LogInformation(
                "Material approved event handled: MaterialUsage {MaterialUsageId}, Visit {VisitId}, ApprovedBy {ApprovedBy}",
                domainEvent.MaterialUsageId,
                domainEvent.VisitId,
                domainEvent.ApprovedBy);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling material approved event for material usage {MaterialUsageId}",
                domainEvent.MaterialUsageId);
        }
    }
}

