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

public class UserCreatedEventHandler : INotificationHandler<DomainEventNotification<UserCreatedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(
        INotificationService notificationService,
        IUserRepository userRepository,
        ILogger<UserCreatedEventHandler> logger)
    {
        _notificationService = notificationService;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<UserCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            // Notify office managers about new user
            var managers = await _userRepository.GetByOfficeIdAsync(domainEvent.OfficeId, cancellationToken);
            var officeManagers = managers.Where(u => u.Role == Domain.Enums.UserRole.Manager).ToList();

            foreach (var manager in officeManagers)
            {
                await _notificationService.SendPushNotificationAsync(
                    manager.Id,
                    "ðŸ‘¤ New User Created",
                    $"A new {domainEvent.Role} user {domainEvent.Name} has been created in your office",
                    cancellationToken);
            }

            _logger.LogInformation(
                "User created event handled: User {UserId} ({UserName}), Role {Role}, Office {OfficeId}",
                domainEvent.UserId,
                domainEvent.Name,
                domainEvent.Role,
                domainEvent.OfficeId);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling user created event for user {UserId}",
                domainEvent.UserId);
        }
    }
}

