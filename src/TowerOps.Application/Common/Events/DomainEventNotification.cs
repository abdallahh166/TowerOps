using MediatR;
using TowerOps.Domain.Events;

namespace TowerOps.Application.Common.Events;

/// <summary>
/// Wrapper to convert Domain Events to MediatR Notifications
/// This keeps the Domain layer clean from infrastructure dependencies
/// </summary>
public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent)
    : INotification
    where TDomainEvent : IDomainEvent;