using FluentAssertions;
using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Tests.Entities;

public class WorkOrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldInitializeCreatedStatusAndDeadlines()
    {
        var workOrder = WorkOrder.Create(
            woNumber: "WO-1001",
            siteCode: "S-TNT-001",
            officeCode: "TNT",
            slaClass: SlaClass.P2,
            issueDescription: "Power alarm and partial degradation");

        workOrder.Status.Should().Be(WorkOrderStatus.Created);
        workOrder.ResponseDeadlineUtc.Should().BeAfter(workOrder.CreatedAt);
        workOrder.ResolutionDeadlineUtc.Should().BeAfter(workOrder.ResponseDeadlineUtc);
    }

    [Fact]
    public void Assign_FromCreated_ShouldMoveToAssigned()
    {
        var workOrder = WorkOrder.Create("WO-1002", "S-TNT-002", "TNT", SlaClass.P3, "Checklist mismatch");

        workOrder.Assign(Guid.NewGuid(), "Engineer A", "Dispatcher");

        workOrder.Status.Should().Be(WorkOrderStatus.Assigned);
        workOrder.AssignedAtUtc.Should().NotBeNull();
        workOrder.AssignedEngineerName.Should().Be("Engineer A");
        workOrder.AssignedBy.Should().Be("Dispatcher");
    }

    [Fact]
    public void Assign_WhenNotCreatedOrRework_ShouldThrowDomainException()
    {
        var workOrder = WorkOrder.Create("WO-1003", "S-TNT-003", "TNT", SlaClass.P1, "Critical outage");
        workOrder.Assign(Guid.NewGuid(), "Engineer A", "Dispatcher");

        Action reassign = () => workOrder.Assign(Guid.NewGuid(), "Engineer B", "Dispatcher");

        reassign.Should().Throw<DomainException>()
            .WithMessage("*assigned from Created or Rework state*");
    }

    [Fact]
    public void Create_WithoutIssueDescription_ShouldThrowDomainException()
    {
        Action act = () => WorkOrder.Create("WO-1004", "S-TNT-004", "TNT", SlaClass.P4, string.Empty);

        act.Should().Throw<DomainException>()
            .WithMessage("*Issue description is required*");
    }

    [Fact]
    public void Assign_WithEmptyEngineerId_ShouldThrowDomainException()
    {
        var workOrder = WorkOrder.Create("WO-1005", "S-TNT-005", "TNT", SlaClass.P3, "Door alarm");

        Action act = () => workOrder.Assign(Guid.Empty, "Engineer A", "Dispatcher");

        act.Should().Throw<DomainException>()
            .WithMessage("*Engineer ID is required*");
    }
    [Fact]
    public void CreateAndAssign_ShouldPersistUtcTimestamps()
    {
        var workOrder = WorkOrder.Create("WO-UTC-1", "S-TNT-010", "TNT", SlaClass.P2, "UTC validation");

        workOrder.CreatedAt.Kind.Should().Be(DateTimeKind.Utc);
        workOrder.Assign(Guid.NewGuid(), "Engineer UTC", "Dispatcher");

        workOrder.AssignedAtUtc.Should().NotBeNull();
        workOrder.AssignedAtUtc!.Value.Kind.Should().Be(DateTimeKind.Utc);
    }

}
