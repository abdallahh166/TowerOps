using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common.Events;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Events.MaterialEvents;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.EventHandlers.MaterialEventHandlers;

public class MaterialRestockedEventHandler : INotificationHandler<DomainEventNotification<MaterialRestockedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IMaterialRepository _materialRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<MaterialRestockedEventHandler> _logger;

    public MaterialRestockedEventHandler(
        INotificationService notificationService,
        IMaterialRepository materialRepository,
        IUserRepository userRepository,
        ILogger<MaterialRestockedEventHandler> logger)
    {
        _notificationService = notificationService;
        _materialRepository = materialRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<MaterialRestockedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var material = await _materialRepository.GetByIdAsNoTrackingAsync(domainEvent.MaterialId, cancellationToken);
            
            if (material != null)
            {
                // Notify office managers about successful restock
                var managers = await _userRepository.GetByOfficeIdAsync(domainEvent.OfficeId, cancellationToken);
                var officeManagers = managers.Where(u => u.Role == Domain.Enums.UserRole.Manager).ToList();

                foreach (var manager in officeManagers)
                {
                    await _notificationService.SendPushNotificationAsync(
                        manager.Id,
                        "✅ Material Restocked",
                        $"Material {domainEvent.MaterialName} has been restocked. Quantity: {domainEvent.Quantity} {domainEvent.Unit}. New stock: {domainEvent.NewStock} {domainEvent.Unit}",
                        cancellationToken);
                }
            }

            _logger.LogInformation(
                "Material restocked event handled: Material {MaterialId}, Quantity {Quantity} {Unit}, New Stock {NewStock}",
                domainEvent.MaterialId,
                domainEvent.Quantity,
                domainEvent.Unit,
                domainEvent.NewStock);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling material restocked event for material {MaterialId}",
                domainEvent.MaterialId);
        }
    }
}

