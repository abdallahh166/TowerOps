using TelecomPM.Domain.Events;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Events.UserEvents;

public sealed record UserRoleChangedEvent(
    Guid UserId,
    string UserName,
    UserRole OldRole,
    UserRole NewRole,
    Guid OfficeId
) : DomainEvent;

