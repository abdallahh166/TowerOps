using TelecomPM.Domain.Events;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Events.UserEvents;

public sealed record UserCreatedEvent(
    Guid UserId,
    string Name,
    string Email,
    UserRole Role,
    Guid OfficeId
) : DomainEvent;

