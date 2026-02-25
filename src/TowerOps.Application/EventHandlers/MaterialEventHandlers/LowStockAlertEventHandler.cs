using MediatR;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.Common.Events; // أضف هذا
using TowerOps.Domain.Enums;
using TowerOps.Domain.Events.MaterialEvents;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.EventHandlers.MaterialEventHandlers;

// غير من LowStockAlertEvent إلى DomainEventNotification<LowStockAlertEvent>
public class LowStockAlertEventHandler : INotificationHandler<DomainEventNotification<LowStockAlertEvent>>
{
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<LowStockAlertEventHandler> _logger;

    public LowStockAlertEventHandler(
        INotificationService notificationService,
        IUserRepository userRepository,
        ILogger<LowStockAlertEventHandler> logger)
    {
        _notificationService = notificationService;
        _userRepository = userRepository;
        _logger = logger;
    }

    // غير الـ parameter type
    public async Task Handle(DomainEventNotification<LowStockAlertEvent> notification, CancellationToken cancellationToken)
    {
        // استخرج الـ domain event من الـ wrapper
        var domainEvent = notification.DomainEvent;

        try
        {
            // Get admin users for the office
            var admins = await _userRepository.GetByRoleAsync(UserRole.Admin, cancellationToken);
            var officeAdmins = admins.Where(a => a.OfficeId == domainEvent.OfficeId).ToList();

            foreach (var admin in officeAdmins)
            {
                await _notificationService.SendPushNotificationAsync(
                    admin.Id,
                    "⚠️ Low Stock Alert",
                    $"Material '{domainEvent.MaterialName}' is running low: {domainEvent.CurrentStock} (Min: {domainEvent.MinimumStock})",
                    cancellationToken);

                await _notificationService.SendEmailAsync(
                    admin.Email,
                    "Low Stock Alert",
                    $"Dear {admin.Name},\n\nThe following material is running low in stock:\n\nMaterial: {domainEvent.MaterialName}\nCurrent Stock: {domainEvent.CurrentStock}\nMinimum Required: {domainEvent.MinimumStock}\n\nPlease arrange for restocking.",
                    cancellationToken);
            }

            _logger.LogWarning(
                "Low stock alert for material {MaterialId} in office {OfficeId}: Current={CurrentStock}, Min={MinimumStock}",
                domainEvent.MaterialId, domainEvent.OfficeId, domainEvent.CurrentStock, domainEvent.MinimumStock);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error handling LowStockAlertEvent for material {MaterialId}",
                domainEvent.MaterialId);
        }
    }
}