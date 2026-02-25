using System.Threading;
using FluentAssertions;
using MediatR;
using Moq;
using TowerOps.Application.Common.Behaviors;
using TowerOps.Application.Common;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Behaviors;

public class TransactionBehaviorTests
{
    public sealed record TestCommand(string Name) : ICommand<string>;
    public sealed record TestQuery(string Name) : IQuery<string>;

    [Fact]
    public async Task Handle_Command_ShouldExecuteInTransaction_WhenNoActiveTransaction()
    {
        // Arrange
        var unitOfWork = new Mock<IUnitOfWork>();
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionBehavior<TestCommand, Result<string>>>>();

        unitOfWork.Setup(u => u.HasActiveTransaction).Returns(false);
        unitOfWork
            .Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task<Result<string>>>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task<Result<string>>>, CancellationToken>((func, _) => func());

        var behavior = new TransactionBehavior<TestCommand, Result<string>>(unitOfWork.Object, logger.Object);
        var command = new TestCommand("cmd");

        // Act
        var result = await behavior.Handle(command, () => Task.FromResult(Result.Success("ok")), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("ok");
        unitOfWork.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task<Result<string>>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Command_ShouldSkipTransaction_WhenAlreadyActive()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionBehavior<TestCommand, Result<string>>>>();

        unitOfWork.Setup(u => u.HasActiveTransaction).Returns(true);

        var behavior = new TransactionBehavior<TestCommand, Result<string>>(unitOfWork.Object, logger.Object);
        var command = new TestCommand("cmd");

        var result = await behavior.Handle(command, () => Task.FromResult(Result.Success("ok")), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("ok");
        unitOfWork.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task<Result<string>>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Query_ShouldNotUseTransaction()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger<TransactionBehavior<TestQuery, Result<string>>>>();
        unitOfWork.Setup(u => u.HasActiveTransaction).Returns(false);

        var behavior = new TransactionBehavior<TestQuery, Result<string>>(unitOfWork.Object, logger.Object);
        var query = new TestQuery("q");

        var result = await behavior.Handle(query, () => Task.FromResult(Result.Success("q-ok")), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be("q-ok");
        unitOfWork.Verify(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task<Result<string>>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

