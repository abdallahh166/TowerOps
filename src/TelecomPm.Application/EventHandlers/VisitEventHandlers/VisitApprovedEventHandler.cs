using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common.Events; // ← أضف هذا
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Events.VisitEvents;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.EventHandlers.VisitEventHandlers;

// ← غيّر من VisitApprovedEvent إلى DomainEventNotification<VisitApprovedEvent>
public class VisitApprovedEventHandler : INotificationHandler<DomainEventNotification<VisitApprovedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VisitApprovedEventHandler> _logger;

    public VisitApprovedEventHandler(
        INotificationService notificationService,
        IUserRepository userRepository,
        ILogger<VisitApprovedEventHandler> logger)
    {
        _notificationService = notificationService;
        _userRepository = userRepository;
        _logger = logger;
    }

    // ← غيّر الـ parameter type
    public async Task Handle(DomainEventNotification<VisitApprovedEvent> notification, CancellationToken cancellationToken)
    {
        // ← استخرج الـ domain event
        var domainEvent = notification.DomainEvent;

        try
        {
            var engineer = await _userRepository.GetByIdAsync(domainEvent.EngineerId, cancellationToken);

            if (engineer != null)
            {
                await _notificationService.SendPushNotificationAsync(
                    engineer.Id,
                    "✅ Visit Approved",
                    $"Congratulations! Your visit has been approved. Visit ID: {domainEvent.VisitId}",
                    cancellationToken);

                await _notificationService.SendEmailAsync(
                    engineer.Email,
                    "Visit Approved",
                    $"Dear {engineer.Name},\n\nYour visit has been approved.\n\nVisit ID: {domainEvent.VisitId}\nSite ID: {domainEvent.SiteId}\n\nGreat work!",
                    cancellationToken);
            }

            _logger.LogInformation(
                "Notification sent to engineer {EngineerId} for approved visit {VisitId}",
                domainEvent.EngineerId, domainEvent.VisitId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error handling VisitApprovedEvent for visit {VisitId}",
                domainEvent.VisitId);
        }
    }
}