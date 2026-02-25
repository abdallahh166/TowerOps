using FluentAssertions;
using Moq;
using TowerOps.Application.Commands.AuditLogs.LogAuditEntry;
using TowerOps.Domain.Entities.AuditLogs;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Commands.AuditLogs;

public class LogAuditEntryCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldPersistAuditEntry()
    {
        var auditRepo = new Mock<IAuditLogRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new LogAuditEntryCommandHandler(auditRepo.Object, unitOfWork.Object);

        var command = new LogAuditEntryCommand
        {
            EntityType = "WorkOrder",
            EntityId = Guid.NewGuid(),
            Action = "Assigned",
            ActorId = Guid.NewGuid(),
            ActorRole = "Supervisor",
            PreviousState = "Created",
            NewState = "Assigned",
            Notes = "assignment done"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        auditRepo.Verify(r => r.AddAsync(It.IsAny<AuditLog>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
