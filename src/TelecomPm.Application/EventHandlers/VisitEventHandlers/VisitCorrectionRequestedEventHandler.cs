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

// ← غيّر من VisitCorrectionRequestedEvent إلى DomainEventNotification<VisitCorrectionRequestedEvent>
public class VisitCorrectionRequestedEventHandler : INotificationHandler<DomainEventNotification<VisitCorrectionRequestedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VisitCorrectionRequestedEventHandler> _logger;

    public VisitCorrectionRequestedEventHandler(
        INotificationService notificationService,
        IUserRepository userRepository,
        ILogger<VisitCorrectionRequestedEventHandler> logger)
    {
        _notificationService = notificationService;
        _userRepository = userRepository;
        _logger = logger;
    }

    // ← غيّر الـ parameter type
    public async Task Handle(DomainEventNotification<VisitCorrectionRequestedEvent> notification, CancellationToken cancellationToken)
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
                    "🔄 Corrections Requested",
                    $"Your visit needs corrections. Visit ID: {domainEvent.VisitId}",
                    cancellationToken);

                await _notificationService.SendEmailAsync(
                    engineer.Email,
                    "Visit Corrections Required",
                    $"Dear {engineer.Name},\n\nYour visit requires corrections.\n\nVisit ID: {domainEvent.VisitId}\nSite ID: {domainEvent.SiteId}\n\nCorrection Notes:\n{domainEvent.CorrectionNotes}\n\nPlease make the necessary updates and resubmit.",
                    cancellationToken);
            }

            _logger.LogInformation(
                "Correction notification sent to engineer {EngineerId} for visit {VisitId}",
                domainEvent.EngineerId, domainEvent.VisitId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error handling VisitCorrectionRequestedEvent for visit {VisitId}",
                domainEvent.VisitId);
        }
    }
}