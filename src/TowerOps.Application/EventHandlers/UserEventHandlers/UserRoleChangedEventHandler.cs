using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common.Events;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Events.UserEvents;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.EventHandlers.UserEventHandlers;

public class UserRoleChangedEventHandler : INotificationHandler<DomainEventNotification<UserRoleChangedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserRoleChangedEventHandler> _logger;

    public UserRoleChangedEventHandler(
        INotificationService notificationService,
        IUserRepository userRepository,
        ILogger<UserRoleChangedEventHandler> logger)
    {
        _notificationService = notificationService;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<UserRoleChangedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            // Notify office managers about role change
            var managers = await _userRepository.GetByOfficeIdAsync(domainEvent.OfficeId, cancellationToken);
            var officeManagers = managers.Where(u => u.Role == Domain.Enums.UserRole.Manager).ToList();

            foreach (var manager in officeManagers)
            {
                await _notificationService.SendPushNotificationAsync(
                    manager.Id,
                    "ðŸ”„ User Role Changed",
                    $"User {domainEvent.UserName} role has been changed from {domainEvent.OldRole} to {domainEvent.NewRole}",
                    cancellationToken);
            }

            // Notify the user about their role change
            var user = await _userRepository.GetByIdAsNoTrackingAsync(domainEvent.UserId, cancellationToken);
            if (user != null)
            {
                await _notificationService.SendPushNotificationAsync(
                    domainEvent.UserId,
                    "ðŸ”„ Role Changed",
                    $"Your role has been changed from {domainEvent.OldRole} to {domainEvent.NewRole}",
                    cancellationToken);
            }

            _logger.LogInformation(
                "User role changed event handled: User {UserId} ({UserName}), Old Role {OldRole}, New Role {NewRole}",
                domainEvent.UserId,
                domainEvent.UserName,
                domainEvent.OldRole,
                domainEvent.NewRole);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling user role changed event for user {UserId}",
                domainEvent.UserId);
        }
    }
}

