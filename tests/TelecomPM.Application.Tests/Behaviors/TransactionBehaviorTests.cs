using FluentAssertions;
using MediatR;
using Moq;
using TelecomPM.Application.Common.Behaviors;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Behaviors;

public class TransactionBehaviorTests
{
    public sealed record TestCommand(string Name) : IRequest<string>;
    public sealed record TestQuery(string Name) : IRequest<string>;

    [Fact]
    public async Task Handle_Command_ShouldExecuteInTransaction_WhenNoActiveTransaction()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionBehavior<TestCommand, string>>>();

        unitOfWork.Setup(u => u.HasActiveTransaction).Returns(false);
        unitOfWork
            .Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task<string>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task<string>>, CancellationToken>((func, _) => func());

        var behavior = new TransactionBehavior<TestCommand, string>(unitOfWork.Object, logger.Object);
        var command = new TestCommand("cmd");
        var expectedResult = "ok";

        // Act
        var result = await behavior.Handle(command, () => Task.FromResult(expectedResult), CancellationToken.None);

        // Assert
        result.Should().Be(expectedResult);
        unitOfWork.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task<string>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Command_ShouldSkipTransaction_WhenAlreadyActive()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionBehavior<TestCommand, string>>>();

        unitOfWork.Setup(u => u.HasActiveTransaction).Returns(true);

        var behavior = new TransactionBehavior<TestCommand, string>(unitOfWork.Object, logger.Object);
        var command = new TestCommand("cmd");

        var result = await behavior.Handle(command, () => Task.FromResult("ok"), CancellationToken.None);

        result.Should().Be("ok");
        unitOfWork.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task<string>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Query_ShouldNotUseTransaction()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionBehavior<TestQuery, string>>>();
        unitOfWork.Setup(u => u.HasActiveTransaction).Returns(false);

        var behavior = new TransactionBehavior<TestQuery, string>(unitOfWork.Object, logger.Object);
        var query = new TestQuery("q");

        var result = await behavior.Handle(query, () => Task.FromResult("q-ok"), CancellationToken.None);

        result.Should().Be("q-ok");
        unitOfWork.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task<string>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

