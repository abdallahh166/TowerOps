using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common.Events;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Events.OfficeEvents;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.EventHandlers.OfficeEventHandlers;

public class OfficeCreatedEventHandler : INotificationHandler<DomainEventNotification<OfficeCreatedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<OfficeCreatedEventHandler> _logger;

    public OfficeCreatedEventHandler(
        INotificationService notificationService,
        IUserRepository userRepository,
        ILogger<OfficeCreatedEventHandler> logger)
    {
        _notificationService = notificationService;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<OfficeCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            // Notify all admins about new office
            var admins = await _userRepository.GetByRoleAsync(Domain.Enums.UserRole.Admin, cancellationToken);

            foreach (var admin in admins)
            {
                await _notificationService.SendPushNotificationAsync(
                    admin.Id,
                    "🏢 New Office Created",
                    $"A new office {domainEvent.Code} ({domainEvent.Name}) has been created in {domainEvent.Region}",
                    cancellationToken);
            }

            _logger.LogInformation(
                "Office created event handled: Office {OfficeId} ({Code}), Name {Name}, Region {Region}",
                domainEvent.OfficeId,
                domainEvent.Code,
                domainEvent.Name,
                domainEvent.Region);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling office created event for office {OfficeId}",
                domainEvent.OfficeId);
        }
    }
}

