using TowerOps.Domain.Events;
using TowerOps.Domain.Enums;

namespace TowerOps.Domain.Events.UserEvents;

public sealed record UserCreatedEvent(
    Guid UserId,
    string Name,
    string Email,
    UserRole Role,
    Guid OfficeId
) : DomainEvent;

