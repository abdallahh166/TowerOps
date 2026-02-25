using MediatR;
using Microsoft.Extensions.Logging;
using TowerOps.Application.Common.Events;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Events.WorkOrderEvents;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.EventHandlers.WorkOrderEventHandlers;

public sealed class SlaBreachedEventHandler : INotificationHandler<DomainEventNotification<SlaBreachedEvent>>
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<SlaBreachedEventHandler> _logger;

    public SlaBreachedEventHandler(
        IUserRepository userRepository,
        INotificationService notificationService,
        ILogger<SlaBreachedEventHandler> logger)
    {
        _userRepository = userRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<SlaBreachedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var managers = await _userRepository.GetByRoleAsync(UserRole.Manager, cancellationToken);
            var admins = await _userRepository.GetByRoleAsync(UserRole.Admin, cancellationToken);

            var recipients = managers
                .Concat(admins)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .ToList();

            foreach (var recipient in recipients)
            {
                await _notificationService.SendPushNotificationAsync(
                    recipient.Id,
                    "SLA Breach Alert",
                    $"Work order {domainEvent.WoNumber} breached SLA at {domainEvent.BreachedAtUtc:O}.",
                    cancellationToken);
            }

            _logger.LogWarning(
                "SLA breach notification sent for WorkOrderId={WorkOrderId}, WoNumber={WoNumber}, BreachedAtUtc={BreachedAtUtc}",
                domainEvent.WorkOrderId,
                domainEvent.WoNumber,
                domainEvent.BreachedAtUtc);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to handle SlaBreachedEvent for WorkOrderId={WorkOrderId}, WoNumber={WoNumber}",
                domainEvent.WorkOrderId,
                domainEvent.WoNumber);
        }
    }
}
