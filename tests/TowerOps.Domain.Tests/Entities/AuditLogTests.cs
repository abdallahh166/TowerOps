using FluentAssertions;
using TowerOps.Domain.Entities.AuditLogs;

namespace TowerOps.Domain.Tests.Entities;

public class AuditLogTests
{
    [Fact]
    public void Create_WithValidData_ShouldInitializeAuditLog()
    {
        var entityId = Guid.NewGuid();
        var actorId = Guid.NewGuid();

        var auditLog = AuditLog.Create(
            "WorkOrder",
            entityId,
            "Created",
            actorId,
            "Manager",
            null,
            "Created",
            "created through api");

        auditLog.EntityType.Should().Be("WorkOrder");
        auditLog.EntityId.Should().Be(entityId);
        auditLog.Action.Should().Be("Created");
        auditLog.ActorId.Should().Be(actorId);
        auditLog.ActorRole.Should().Be("Manager");
        auditLog.OccurredAtUtc.Kind.Should().Be(DateTimeKind.Utc);
        auditLog.NewState.Should().Be("Created");
    }
}
