using MediatR;
using TelecomPM.Application.Common.Events;
using TelecomPM.Domain.Events;
using TelecomPM.Domain.Interfaces.Services;

namespace TelecomPM.Infrastructure.Services;

public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IPublisher _publisher;

    public DomainEventDispatcher(IPublisher publisher)
    {
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        if (domainEvents == null)
            return;

        foreach (var domainEvent in domainEvents)
        {
            if (domainEvent == null)
                continue;

            var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());

            if (Activator.CreateInstance(notificationType, domainEvent) is INotification notification)
            {
                await _publisher.Publish(notification, cancellationToken);
            }
        }
    }
}

