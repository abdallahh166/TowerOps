using TowerOps.Domain.Events;
using TowerOps.Domain.Enums;

namespace TowerOps.Domain.Events.UserEvents;

public sealed record UserRoleChangedEvent(
    Guid UserId,
    string UserName,
    UserRole OldRole,
    UserRole NewRole,
    Guid OfficeId
) : DomainEvent;

