using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq; // ← أضف هذا للـ Concat و ToList
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common.Events; // ← أضف هذا
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Events.VisitEvents;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.EventHandlers.VisitEventHandlers;

// ← غيّر من CriticalIssueReportedEvent إلى DomainEventNotification<CriticalIssueReportedEvent>
public class CriticalIssueReportedEventHandler : INotificationHandler<DomainEventNotification<CriticalIssueReportedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CriticalIssueReportedEventHandler> _logger;

    public CriticalIssueReportedEventHandler(
        INotificationService notificationService,
        IUserRepository userRepository,
        ILogger<CriticalIssueReportedEventHandler> logger)
    {
        _notificationService = notificationService;
        _userRepository = userRepository;
        _logger = logger;
    }

    // ← غيّر الـ parameter type
    public async Task Handle(DomainEventNotification<CriticalIssueReportedEvent> notification, CancellationToken cancellationToken)
    {
        // ← استخرج الـ domain event
        var domainEvent = notification.DomainEvent;

        try
        {
            // Get managers and supervisors
            var managers = await _userRepository.GetByRoleAsync(UserRole.Manager, cancellationToken);
            var supervisors = await _userRepository.GetByRoleAsync(UserRole.Supervisor, cancellationToken);

            // ← Concat هتشتغل دلوقتي بعد إضافة System.Linq
            var recipients = managers.Concat(supervisors).ToList();

            foreach (var recipient in recipients)
            {
                await _notificationService.SendPushNotificationAsync(
                    recipient.Id,
                    "⚠️ CRITICAL ISSUE REPORTED",
                    $"Critical issue at site {domainEvent.SiteId}: {domainEvent.IssueDescription}",
                    cancellationToken);

                await _notificationService.SendEmailAsync(
                    recipient.Email,
                    "⚠️ URGENT: Critical Issue Reported",
                    $"Dear {recipient.Name},\n\nA critical issue has been reported and requires immediate attention:\n\nSite ID: {domainEvent.SiteId}\nVisit ID: {domainEvent.VisitId}\n\nIssue Description:\n{domainEvent.IssueDescription}\n\nPlease take immediate action.",
                    cancellationToken);
            }

            _logger.LogWarning(
                "Critical issue reported for visit {VisitId} at site {SiteId}: {IssueDescription}",
                domainEvent.VisitId, domainEvent.SiteId, domainEvent.IssueDescription);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error handling CriticalIssueReportedEvent for visit {VisitId}",
                domainEvent.VisitId);
        }
    }
}