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

    [Fact]
    public void Start_FromCreated_ShouldThrowDomainException()
    {
        var workOrder = WorkOrder.Create("WO-1006", "S-TNT-006", "TNT", SlaClass.P2, "No assignment");

        Action act = () => workOrder.Start();

        act.Should().Throw<DomainException>()
            .WithMessage("*started from Assigned state*");
    }

    [Fact]
    public void Complete_FromAssigned_ShouldThrowDomainException()
    {
        var workOrder = WorkOrder.Create("WO-1007", "S-TNT-007", "TNT", SlaClass.P2, "Issue");
        workOrder.Assign(Guid.NewGuid(), "Engineer B", "Dispatcher");

        Action act = () => workOrder.Complete();

        act.Should().Throw<DomainException>()
            .WithMessage("*completed from InProgress state*");
    }

    [Fact]
    public void Close_FromInProgress_ShouldThrowDomainException()
    {
        var workOrder = WorkOrder.Create("WO-1008", "S-TNT-008", "TNT", SlaClass.P2, "Issue");
        workOrder.Assign(Guid.NewGuid(), "Engineer C", "Dispatcher");
        workOrder.Start();

        Action act = () => workOrder.Close();

        act.Should().Throw<DomainException>()
            .WithMessage("*closed from PendingInternalReview or PendingCustomerAcceptance state*");
    }

    [Fact]
    public void Cancel_FromClosed_ShouldThrowDomainException()
    {
        var workOrder = WorkOrder.Create("WO-1009", "S-TNT-009", "TNT", SlaClass.P2, "Issue");
        workOrder.Assign(Guid.NewGuid(), "Engineer D", "Dispatcher");
        workOrder.Start();
        workOrder.Complete();
        workOrder.Close();

        Action act = () => workOrder.Cancel();

        act.Should().Throw<DomainException>()
            .WithMessage("*cannot be cancelled*");
    }

    [Fact]
    public void StartCompleteClose_ShouldTransitionStatuses()
    {
        var workOrder = WorkOrder.Create("WO-1010", "S-TNT-011", "TNT", SlaClass.P2, "Issue");
        workOrder.Assign(Guid.NewGuid(), "Engineer E", "Dispatcher");

        workOrder.Start();
        workOrder.Status.Should().Be(WorkOrderStatus.InProgress);

        workOrder.Complete();
        workOrder.Status.Should().Be(WorkOrderStatus.PendingInternalReview);

        workOrder.Close();
        workOrder.Status.Should().Be(WorkOrderStatus.Closed);
    }

    [Fact]
    public void SubmitForCustomerAcceptance_FromPendingInternalReview_ShouldMoveToPendingCustomerAcceptance()
    {
        var workOrder = WorkOrder.Create("WO-1011", "S-TNT-012", "TNT", SlaClass.P2, "Issue");
        workOrder.Assign(Guid.NewGuid(), "Engineer F", "Dispatcher");
        workOrder.Start();
        workOrder.Complete();

        workOrder.SubmitForCustomerAcceptance();

        workOrder.Status.Should().Be(WorkOrderStatus.PendingCustomerAcceptance);
    }

    [Fact]
    public void AcceptByCustomer_FromPendingCustomerAcceptance_ShouldMoveToClosed()
    {
        var workOrder = WorkOrder.Create("WO-1012", "S-TNT-013", "TNT", SlaClass.P2, "Issue");
        workOrder.Assign(Guid.NewGuid(), "Engineer G", "Dispatcher");
        workOrder.Start();
        workOrder.Complete();
        workOrder.SubmitForCustomerAcceptance();

        workOrder.AcceptByCustomer("Customer Ops");

        workOrder.Status.Should().Be(WorkOrderStatus.Closed);
    }

    [Fact]
    public void RejectByCustomer_FromPendingCustomerAcceptance_ShouldMoveToRework()
    {
        var workOrder = WorkOrder.Create("WO-1013", "S-TNT-014", "TNT", SlaClass.P2, "Issue");
        workOrder.Assign(Guid.NewGuid(), "Engineer H", "Dispatcher");
        workOrder.Start();
        workOrder.Complete();
        workOrder.SubmitForCustomerAcceptance();

        workOrder.RejectByCustomer("Need additional fixes");

        workOrder.Status.Should().Be(WorkOrderStatus.Rework);
    }

    [Fact]
    public void CustomerAcceptanceTransitions_FromInvalidState_ShouldThrowDomainException()
    {
        var workOrder = WorkOrder.Create("WO-1014", "S-TNT-015", "TNT", SlaClass.P2, "Issue");

        Action submit = () => workOrder.SubmitForCustomerAcceptance();
        Action accept = () => workOrder.AcceptByCustomer("Customer Ops");
        Action reject = () => workOrder.RejectByCustomer("Not acceptable");

        submit.Should().Throw<DomainException>()
            .WithMessage("*submitted for customer acceptance from PendingInternalReview state*");
        accept.Should().Throw<DomainException>()
            .WithMessage("*accepted by customer from PendingCustomerAcceptance state*");
        reject.Should().Throw<DomainException>()
            .WithMessage("*rejected by customer from PendingCustomerAcceptance state*");
    }
}
