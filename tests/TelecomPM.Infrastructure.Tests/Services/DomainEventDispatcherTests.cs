using FluentAssertions;
using MediatR;
using Moq;
using TelecomPM.Domain.Events;
using TelecomPM.Infrastructure.Services;
using Xunit;

namespace TelecomPM.Infrastructure.Tests.Services;

public class DomainEventDispatcherTests
{
    private sealed record TestDomainEvent(string Value) : DomainEvent;

    [Fact]
    public async Task DispatchAsync_ShouldPublishNotification_ForEachEvent()
    {
        // Arrange
        var publisherMock = new Mock<IPublisher>();
        var dispatcher = new DomainEventDispatcher(publisherMock.Object);
        var events = new IDomainEvent[]
        {
            new TestDomainEvent("A"),
            new TestDomainEvent("B")
        };

        // Act
        await dispatcher.DispatchAsync(events, CancellationToken.None);

        // Assert
        publisherMock.Verify(
            p => p.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()),
            Times.Exactly(events.Length));
    }

    [Fact]
    public async Task DispatchAsync_ShouldIgnoreNullEvents()
    {
        var publisherMock = new Mock<IPublisher>();
        var dispatcher = new DomainEventDispatcher(publisherMock.Object);

        await dispatcher.DispatchAsync(new IDomainEvent[] { null! }, CancellationToken.None);

        publisherMock.Verify(
            p => p.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}

