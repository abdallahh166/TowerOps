using MediatR;
using Microsoft.Extensions.Logging;
using TowerOps.Application.Common.Events;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Events.VisitEvents;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.EventHandlers.VisitEventHandlers;

public sealed class SuspiciousCheckInEventHandler : INotificationHandler<DomainEventNotification<SuspiciousCheckInEvent>>
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<SuspiciousCheckInEventHandler> _logger;

    public SuspiciousCheckInEventHandler(
        IUserRepository userRepository,
        INotificationService notificationService,
        ILogger<SuspiciousCheckInEventHandler> logger)
    {
        _userRepository = userRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<SuspiciousCheckInEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var supervisors = await _userRepository.GetByRoleAsync(UserRole.Supervisor, cancellationToken);
            var managers = await _userRepository.GetByRoleAsync(UserRole.Manager, cancellationToken);

            var recipients = supervisors
                .Concat(managers)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();

            foreach (var recipient in recipients)
            {
                await _notificationService.SendPushNotificationAsync(
                    recipient.Id,
                    "Suspicious Check-In",
                    $"Visit {domainEvent.VisitId} check-in is outside radius ({domainEvent.DistanceFromSiteMeters:F2}m).",
                    cancellationToken);
            }

            _logger.LogWarning(
                "Suspicious check-in handled for VisitId={VisitId}, SiteId={SiteId}, EngineerId={EngineerId}, Distance={DistanceMeters}",
                domainEvent.VisitId,
                domainEvent.SiteId,
                domainEvent.EngineerId,
                domainEvent.DistanceFromSiteMeters);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to handle SuspiciousCheckInEvent for VisitId={VisitId}",
                domainEvent.VisitId);
        }
    }
}
