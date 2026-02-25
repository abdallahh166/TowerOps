using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common.Events; // ← أضف هذا
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Events.VisitEvents;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.EventHandlers.VisitEventHandlers;

// ← غيّر من VisitSubmittedEvent إلى DomainEventNotification<VisitSubmittedEvent>
public class VisitSubmittedEventHandler : INotificationHandler<DomainEventNotification<VisitSubmittedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VisitSubmittedEventHandler> _logger;

    public VisitSubmittedEventHandler(
        INotificationService notificationService,
        IUserRepository userRepository,
        ILogger<VisitSubmittedEventHandler> logger)
    {
        _notificationService = notificationService;
        _userRepository = userRepository;
        _logger = logger;
    }

    // ← غيّر الـ parameter type
    public async Task Handle(DomainEventNotification<VisitSubmittedEvent> notification, CancellationToken cancellationToken)
    {
        // ← استخرج الـ domain event
        var domainEvent = notification.DomainEvent;

        try
        {
            // Get all managers
            var managers = await _userRepository.GetByRoleAsync(UserRole.Manager, cancellationToken);

            // Send notifications
            foreach (var manager in managers)
            {
                await _notificationService.SendPushNotificationAsync(
                    manager.Id,
                    "New Visit Submitted",
                    $"A new visit has been submitted for review. Visit ID: {domainEvent.VisitId}",
                    cancellationToken);

                await _notificationService.SendEmailAsync(
                    manager.Email,
                    "New Visit Submitted for Review",
                    $"Dear {manager.Name},\n\nA new visit has been submitted and requires your review.\n\nVisit ID: {domainEvent.VisitId}\nSite ID: {domainEvent.SiteId}\nEngineer ID: {domainEvent.EngineerId}\n\nPlease review at your earliest convenience.",
                    cancellationToken);
            }

            _logger.LogInformation(
                "Notifications sent for submitted visit {VisitId}",
                domainEvent.VisitId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error handling VisitSubmittedEvent for visit {VisitId}",
                domainEvent.VisitId);
        }
    }
}