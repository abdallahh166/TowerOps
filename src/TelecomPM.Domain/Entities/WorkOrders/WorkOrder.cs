using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.WorkOrders;

public sealed class WorkOrder : AggregateRoot<Guid>
{
    public string WoNumber { get; private set; } = string.Empty;
    public string SiteCode { get; private set; } = string.Empty;
    public string OfficeCode { get; private set; } = string.Empty;
    public SlaClass SlaClass { get; private set; }
    public WorkOrderStatus Status { get; private set; }
    public string IssueDescription { get; private set; } = string.Empty;

    public Guid? AssignedEngineerId { get; private set; }
    public string? AssignedEngineerName { get; private set; }
    public DateTime? AssignedAtUtc { get; private set; }
    public string? AssignedBy { get; private set; }

    public DateTime ResponseDeadlineUtc { get; private set; }
    public DateTime ResolutionDeadlineUtc { get; private set; }

    private WorkOrder() : base() { }

    private WorkOrder(
        string woNumber,
        string siteCode,
        string officeCode,
        SlaClass slaClass,
        string issueDescription) : base(Guid.NewGuid())
    {
        WoNumber = woNumber;
        SiteCode = siteCode;
        OfficeCode = officeCode;
        SlaClass = slaClass;
        IssueDescription = issueDescription;
        Status = WorkOrderStatus.Created;

        var now = DateTime.UtcNow;
        ResponseDeadlineUtc = now.Add(GetResponseSla(slaClass));
        ResolutionDeadlineUtc = now.Add(GetResolutionSla(slaClass));
    }

    public static WorkOrder Create(
        string woNumber,
        string siteCode,
        string officeCode,
        SlaClass slaClass,
        string issueDescription)
    {
        if (string.IsNullOrWhiteSpace(woNumber))
            throw new DomainException("WO number is required");

        if (string.IsNullOrWhiteSpace(siteCode))
            throw new DomainException("Site code is required");

        if (string.IsNullOrWhiteSpace(officeCode))
            throw new DomainException("Office code is required");

        if (string.IsNullOrWhiteSpace(issueDescription))
            throw new DomainException("Issue description is required");

        return new WorkOrder(woNumber, siteCode, officeCode, slaClass, issueDescription);
    }

    public void Assign(Guid engineerId, string engineerName, string assignedBy)
    {
        if (Status != WorkOrderStatus.Created && Status != WorkOrderStatus.Rework)
            throw new DomainException("Work order can only be assigned from Created or Rework state");

        if (engineerId == Guid.Empty)
            throw new DomainException("Engineer ID is required");

        if (string.IsNullOrWhiteSpace(engineerName))
            throw new DomainException("Engineer name is required");

        if (string.IsNullOrWhiteSpace(assignedBy))
            throw new DomainException("Assigned by is required");

        AssignedEngineerId = engineerId;
        AssignedEngineerName = engineerName;
        AssignedBy = assignedBy;
        AssignedAtUtc = DateTime.UtcNow;
        Status = WorkOrderStatus.Assigned;
    }

    public void Start()
    {
        if (Status != WorkOrderStatus.Assigned)
            throw new DomainException("Work order can only be started from Assigned state");

        Status = WorkOrderStatus.InProgress;
    }

    public void Complete()
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new DomainException("Work order can only be completed from InProgress state");

        Status = WorkOrderStatus.PendingReview;
    }

    public void Close()
    {
        if (Status != WorkOrderStatus.PendingReview && Status != WorkOrderStatus.PendingCustomerAcceptance)
            throw new DomainException("Work order can only be closed from PendingReview or PendingCustomerAcceptance state");

        Status = WorkOrderStatus.Closed;
    }

    public void Cancel()
    {
        if (Status == WorkOrderStatus.Closed || Status == WorkOrderStatus.Cancelled)
            throw new DomainException("Closed or cancelled work order cannot be cancelled");

        Status = WorkOrderStatus.Cancelled;
    }

    private static TimeSpan GetResponseSla(SlaClass slaClass)
    {
        return slaClass switch
        {
            SlaClass.P1 => TimeSpan.FromHours(1),
            SlaClass.P2 => TimeSpan.FromHours(4),
            SlaClass.P3 => TimeSpan.FromHours(24),
            _ => TimeSpan.FromHours(48)
        };
    }

    private static TimeSpan GetResolutionSla(SlaClass slaClass)
    {
        return slaClass switch
        {
            SlaClass.P1 => TimeSpan.FromHours(4),
            SlaClass.P2 => TimeSpan.FromHours(8),
            SlaClass.P3 => TimeSpan.FromHours(24),
            _ => TimeSpan.FromHours(48)
        };
    }
}
