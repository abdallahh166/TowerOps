using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using TelecomPM.Application.Common.Behaviors;
using Xunit;

namespace TelecomPM.Application.Tests.Behaviors;

public class UnhandledExceptionBehaviorTests
{
    public sealed record TestCommand(string Name) : IRequest<string>;

    [Fact]
    public async Task Handle_ShouldLogAndRethrowException()
    {
        var logger = new Mock<ILogger<UnhandledExceptionBehavior<TestCommand, string>>>();
        var behavior = new UnhandledExceptionBehavior<TestCommand, string>(logger.Object);

        var command = new TestCommand("cmd");
        var exception = new InvalidOperationException("boom");

        Func<Task> act = async () =>
        {
            await behavior.Handle(command, () => throw exception, CancellationToken.None);
        };

        await act.Should().ThrowAsync<InvalidOperationException>();
        logger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, _) => o.ToString()!.Contains("Unhandled exception")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}

