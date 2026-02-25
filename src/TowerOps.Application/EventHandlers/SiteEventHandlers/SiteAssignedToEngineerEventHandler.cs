using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common.Events;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Events.SiteEvents;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.EventHandlers.SiteEventHandlers;

public class SiteAssignedToEngineerEventHandler : INotificationHandler<DomainEventNotification<SiteAssignedToEngineerEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly ISiteRepository _siteRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SiteAssignedToEngineerEventHandler> _logger;

    public SiteAssignedToEngineerEventHandler(
        INotificationService notificationService,
        ISiteRepository siteRepository,
        IUserRepository userRepository,
        ILogger<SiteAssignedToEngineerEventHandler> logger)
    {
        _notificationService = notificationService;
        _siteRepository = siteRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<SiteAssignedToEngineerEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var site = await _siteRepository.GetByIdAsync(domainEvent.SiteId, cancellationToken);
            var engineer = await _userRepository.GetByIdAsync(domainEvent.EngineerId, cancellationToken);

            if (site != null && engineer != null)
            {
                await _notificationService.SendPushNotificationAsync(
                    engineer.Id,
                    "ðŸ“‹ New Site Assignment",
                    $"You have been assigned to site {site.SiteCode.Value} ({site.Name}). Site ID: {domainEvent.SiteId}",
                    cancellationToken);

                await _notificationService.SendEmailAsync(
                    engineer.Email,
                    $"New Site Assignment: {site.SiteCode.Value}",
                    $"Dear {engineer.Name},\n\nYou have been assigned to a new site:\n\n" +
                    $"Site Code: {site.SiteCode.Value}\n" +
                    $"Site Name: {site.Name}\n" +
                    $"Region: {site.Region}\n" +
                    $"Sub-Region: {site.SubRegion}\n" +
                    $"Site Type: {site.SiteType}\n" +
                    $"Complexity: {site.Complexity}\n" +
                    $"Estimated Visit Duration: {site.EstimatedVisitDurationMinutes} minutes\n\n" +
                    "Please review the site details and plan your visits accordingly.",
                    cancellationToken);
            }

            _logger.LogInformation(
                "Site assigned to engineer event handled: Site {SiteId} assigned to Engineer {EngineerId}",
                domainEvent.SiteId,
                domainEvent.EngineerId);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling site assigned to engineer event for site {SiteId}",
                domainEvent.SiteId);
        }
    }
}
