using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common.Events;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Events.VisitEvents;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.EventHandlers.VisitEventHandlers;

public class VisitCompletedEventHandler : INotificationHandler<DomainEventNotification<VisitCompletedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VisitCompletedEventHandler> _logger;

    public VisitCompletedEventHandler(
        INotificationService notificationService,
        IUserRepository userRepository,
        ILogger<VisitCompletedEventHandler> logger)
    {
        _notificationService = notificationService;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<VisitCompletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var engineer = await _userRepository.GetByIdAsync(domainEvent.EngineerId, cancellationToken);

            if (engineer != null)
            {
                await _notificationService.SendPushNotificationAsync(
                    engineer.Id,
                    "âœ… Visit Completed",
                    $"Your visit has been completed. Duration: {domainEvent.Duration}. Please submit for review.",
                    cancellationToken);
            }

            _logger.LogInformation(
                "Notifications sent for completed visit {VisitId} with duration {Duration}",
                domainEvent.VisitId,
                domainEvent.Duration);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling visit completed event for visit {VisitId}",
                domainEvent.VisitId);
        }
    }
}

