using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common.Events;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Events.SiteEvents;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.EventHandlers.SiteEventHandlers;

public class SiteCreatedEventHandler : INotificationHandler<DomainEventNotification<SiteCreatedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IOfficeRepository _officeRepository;
    private readonly ILogger<SiteCreatedEventHandler> _logger;

    public SiteCreatedEventHandler(
        INotificationService notificationService,
        IOfficeRepository officeRepository,
        ILogger<SiteCreatedEventHandler> logger)
    {
        _notificationService = notificationService;
        _officeRepository = officeRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<SiteCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var office = await _officeRepository.GetByIdAsync(domainEvent.OfficeId, cancellationToken);

            if (office != null)
            {
                // Notify office managers about new site
                await _notificationService.SendPushNotificationAsync(
                    office.Id, // This would need to be a manager ID in practice
                    "📍 New Site Created",
                    $"A new site has been created: {domainEvent.SiteCode}. Site ID: {domainEvent.SiteId}",
                    cancellationToken);
            }

            _logger.LogInformation(
                "Site created event handled: Site {SiteId}, Code {SiteCode}, Office {OfficeId}",
                domainEvent.SiteId,
                domainEvent.SiteCode,
                domainEvent.OfficeId);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling site created event for site {SiteId}",
                domainEvent.SiteId);
        }
    }
}
