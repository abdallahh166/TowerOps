using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common.Events;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Events.VisitEvents;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.EventHandlers.VisitEventHandlers;

public class VisitRejectedEventHandler : INotificationHandler<DomainEventNotification<VisitRejectedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VisitRejectedEventHandler> _logger;

    public VisitRejectedEventHandler(
        INotificationService notificationService,
        IUserRepository userRepository,
        ILogger<VisitRejectedEventHandler> logger)
    {
        _notificationService = notificationService;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<VisitRejectedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var engineer = await _userRepository.GetByIdAsync(domainEvent.EngineerId, cancellationToken);

            if (engineer != null)
            {
                await _notificationService.SendPushNotificationAsync(
                    engineer.Id,
                    "âŒ Visit Rejected",
                    $"Your visit has been rejected. Reason: {domainEvent.Reason}. Visit ID: {domainEvent.VisitId}",
                    cancellationToken);

                await _notificationService.SendEmailAsync(
                    engineer.Email,
                    "Visit Rejected",
                    $"Dear {engineer.Name},\n\nYour visit has been rejected.\n\nVisit ID: {domainEvent.VisitId}\nSite ID: {domainEvent.SiteId}\nReason: {domainEvent.Reason}\n\nPlease review the feedback and take appropriate action.",
                    cancellationToken);
            }

            _logger.LogInformation(
                "Notifications sent for rejected visit {VisitId} to engineer {EngineerId}",
                domainEvent.VisitId,
                domainEvent.EngineerId);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling visit rejected event for visit {VisitId}",
                domainEvent.VisitId);
        }
    }
}

