using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common.Events;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Events.SiteEvents;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.EventHandlers.SiteEventHandlers;

public class SiteStatusChangedEventHandler : INotificationHandler<DomainEventNotification<SiteStatusChangedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly ISiteRepository _siteRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SiteStatusChangedEventHandler> _logger;

    public SiteStatusChangedEventHandler(
        INotificationService notificationService,
        ISiteRepository siteRepository,
        IUserRepository userRepository,
        ILogger<SiteStatusChangedEventHandler> logger)
    {
        _notificationService = notificationService;
        _siteRepository = siteRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<SiteStatusChangedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var site = await _siteRepository.GetByIdAsync(domainEvent.SiteId, cancellationToken);

            if (site != null)
            {
                // Notify assigned engineer if site status changed to critical states
                if (site.AssignedEngineerId.HasValue && 
                    domainEvent.NewStatus == Domain.Enums.SiteStatus.OffAir)
                {
                    var engineer = await _userRepository.GetByIdAsync(site.AssignedEngineerId.Value, cancellationToken);

                    if (engineer != null)
                    {
                        await _notificationService.SendPushNotificationAsync(
                            engineer.Id,
                            "⚠️ Site Status Changed",
                            $"Site {site.SiteCode.Value} status changed from {domainEvent.OldStatus} to {domainEvent.NewStatus}",
                            cancellationToken);

                        await _notificationService.SendEmailAsync(
                            engineer.Email,
                            $"Site Status Changed: {site.SiteCode.Value}",
                            $"Dear {engineer.Name},\n\nSite {site.SiteCode.Value} ({site.Name}) status has changed:\n\n" +
                            $"Old Status: {domainEvent.OldStatus}\n" +
                            $"New Status: {domainEvent.NewStatus}\n\n" +
                            "Please review and take appropriate action.",
                            cancellationToken);
                    }
                }
            }

            _logger.LogInformation(
                "Site status changed event handled: Site {SiteId}, {OldStatus} -> {NewStatus}",
                domainEvent.SiteId,
                domainEvent.OldStatus,
                domainEvent.NewStatus);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling site status changed event for site {SiteId}",
                domainEvent.SiteId);
        }
    }
}
