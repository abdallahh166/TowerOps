using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common.Events;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Events.VisitEvents;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.EventHandlers.VisitEventHandlers;

public class VisitCreatedEventHandler : INotificationHandler<DomainEventNotification<VisitCreatedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VisitCreatedEventHandler> _logger;

    public VisitCreatedEventHandler(
        INotificationService notificationService,
        IUserRepository userRepository,
        ILogger<VisitCreatedEventHandler> logger)
    {
        _notificationService = notificationService;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<VisitCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var engineer = await _userRepository.GetByIdAsync(domainEvent.EngineerId, cancellationToken);

            if (engineer != null)
            {
                await _notificationService.SendPushNotificationAsync(
                    engineer.Id,
                    "ðŸ“… New Visit Scheduled",
                    $"A new visit has been scheduled for {domainEvent.ScheduledDate:yyyy-MM-dd}. Visit ID: {domainEvent.VisitId}",
                    cancellationToken);

                await _notificationService.SendEmailAsync(
                    engineer.Email,
                    "New Visit Scheduled",
                    $"Dear {engineer.Name},\n\nA new visit has been scheduled for you.\n\nVisit ID: {domainEvent.VisitId}\nSite ID: {domainEvent.SiteId}\nScheduled Date: {domainEvent.ScheduledDate:yyyy-MM-dd}\n\nPlease prepare accordingly.",
                    cancellationToken);
            }

            _logger.LogInformation(
                "Notifications sent for created visit {VisitId} to engineer {EngineerId}",
                domainEvent.VisitId,
                domainEvent.EngineerId);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling visit created event for visit {VisitId}",
                domainEvent.VisitId);
        }
    }
}

