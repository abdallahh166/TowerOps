using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common.Events;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Events.VisitEvents;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.EventHandlers.VisitEventHandlers;

public class VisitStartedEventHandler : INotificationHandler<DomainEventNotification<VisitStartedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VisitStartedEventHandler> _logger;

    public VisitStartedEventHandler(
        INotificationService notificationService,
        IUserRepository userRepository,
        ILogger<VisitStartedEventHandler> logger)
    {
        _notificationService = notificationService;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<VisitStartedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            // Notify supervisors/managers that visit has started
            var managers = await _userRepository.GetByRoleAsync(Domain.Enums.UserRole.Manager, cancellationToken);

            foreach (var manager in managers)
            {
                await _notificationService.SendPushNotificationAsync(
                    manager.Id,
                    "🔵 Visit Started",
                    $"Engineer has started a visit. Visit ID: {domainEvent.VisitId}",
                    cancellationToken);
            }

            _logger.LogInformation(
                "Notifications sent for started visit {VisitId}",
                domainEvent.VisitId);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling visit started event for visit {VisitId}",
                domainEvent.VisitId);
        }
    }
}

