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

public class MaterialConsumedEventHandler : INotificationHandler<DomainEventNotification<MaterialConsumedEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IMaterialRepository _materialRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<MaterialConsumedEventHandler> _logger;

    public MaterialConsumedEventHandler(
        INotificationService notificationService,
        IMaterialRepository materialRepository,
        IUserRepository userRepository,
        ILogger<MaterialConsumedEventHandler> logger)
    {
        _notificationService = notificationService;
        _materialRepository = materialRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<MaterialConsumedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        try
        {
            var material = await _materialRepository.GetByIdAsNoTrackingAsync(domainEvent.MaterialId, cancellationToken);
            
            if (material != null && material.IsStockLow())
            {
                // Notify office managers about low stock after consumption
                var managers = await _userRepository.GetByOfficeIdAsync(material.OfficeId, cancellationToken);
                var officeManagers = managers.Where(u => u.Role == Domain.Enums.UserRole.Manager).ToList();

                foreach (var manager in officeManagers)
                {
                    await _notificationService.SendPushNotificationAsync(
                        manager.Id,
                        "⚠️ Material Stock Low",
                        $"Material {material.Name} stock is low after consumption. Current: {material.CurrentStock.Value} {material.CurrentStock.Unit}",
                        cancellationToken);
                }
            }

            _logger.LogInformation(
                "Material consumed event handled: Material {MaterialId}, Visit {VisitId}, Quantity {Quantity} {Unit}",
                domainEvent.MaterialId,
                domainEvent.VisitId,
                domainEvent.ConsumedQuantity,
                domainEvent.Unit);
        }
        catch (System.Exception ex)
        {
            _logger.LogError(
                ex,
                "Error handling material consumed event for material {MaterialId}",
                domainEvent.MaterialId);
        }
    }
}

