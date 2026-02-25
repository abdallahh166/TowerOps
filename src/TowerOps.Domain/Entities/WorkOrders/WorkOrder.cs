using TowerOps.Domain.Common;
using TowerOps.Domain.Events.WorkOrderEvents;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Domain.Entities.WorkOrders;

public sealed class WorkOrder : AggregateRoot<Guid>
{
    private bool _wasBreached;

    public string WoNumber { get; private set; } = string.Empty;
    public string SiteCode { get; private set; } = string.Empty;
    public string OfficeCode { get; private set; } = string.Empty;
    public SlaClass SlaClass { get; private set; }
    public WorkOrderScope Scope { get; private set; }
    public WorkOrderStatus Status { get; private set; }
    public string IssueDescription { get; private set; } = string.Empty;

    public Guid? AssignedEngineerId { get; private set; }
    public string? AssignedEngineerName { get; private set; }
    public DateTime? AssignedAtUtc { get; private set; }
    public string? AssignedBy { get; private set; }

    public DateTime ResponseDeadlineUtc { get; private set; }
    public DateTime ResolutionDeadlineUtc { get; private set; }
    public Signature? ClientSignature { get; private set; }
    public Signature? EngineerSignature { get; private set; }
    public bool IsClientSigned => ClientSignature is not null;
    public bool IsEngineerSigned => EngineerSignature is not null;

    private WorkOrder() : base() { }

    private WorkOrder(
        string woNumber,
        string siteCode,
        string officeCode,
        SlaClass slaClass,
        WorkOrderScope scope,
        string issueDescription,
        int? responseMinutes = null,
        int? resolutionMinutes = null) : base(Guid.NewGuid())
    {
        WoNumber = woNumber;
        SiteCode = siteCode;
        OfficeCode = officeCode;
        SlaClass = slaClass;
        Scope = scope;
        IssueDescription = issueDescription;
        Status = WorkOrderStatus.Created;

        var now = DateTime.UtcNow;
        var effectiveResponseMinutes = responseMinutes ?? GetDefaultResponseMinutes(slaClass);
        var effectiveResolutionMinutes = resolutionMinutes ?? GetDefaultResolutionMinutes(slaClass);

        ResponseDeadlineUtc = now.AddMinutes(effectiveResponseMinutes);
        ResolutionDeadlineUtc = now.AddMinutes(effectiveResolutionMinutes);
    }

    public static WorkOrder Create(
        string woNumber,
        string siteCode,
        string officeCode,
        SlaClass slaClass,
        string issueDescription,
        WorkOrderScope scope = WorkOrderScope.ClientEquipment,
        int? responseMinutes = null,
        int? resolutionMinutes = null)
    {
        if (string.IsNullOrWhiteSpace(woNumber))
            throw new DomainException("WO number is required", "WorkOrder.WoNumber.Required");

        if (string.IsNullOrWhiteSpace(siteCode))
            throw new DomainException("Site code is required", "WorkOrder.SiteCode.Required");

        if (string.IsNullOrWhiteSpace(officeCode))
            throw new DomainException("Office code is required", "WorkOrder.OfficeCode.Required");

        if (string.IsNullOrWhiteSpace(issueDescription))
            throw new DomainException("Issue description is required", "WorkOrder.IssueDescription.Required");

        return new WorkOrder(
            woNumber,
            siteCode,
            officeCode,
            slaClass,
            scope,
            issueDescription,
            responseMinutes,
            resolutionMinutes);
    }

    public void Assign(Guid engineerId, string engineerName, string assignedBy)
    {
        if (Status != WorkOrderStatus.Created && Status != WorkOrderStatus.Rework)
            throw new DomainException("Work order can only be assigned from Created or Rework state", "WorkOrder.Assign.RequiresCreatedOrRework");

        if (engineerId == Guid.Empty)
            throw new DomainException("Engineer ID is required", "WorkOrder.Assign.EngineerIdRequired");

        if (string.IsNullOrWhiteSpace(engineerName))
            throw new DomainException("Engineer name is required", "WorkOrder.Assign.EngineerNameRequired");

        if (string.IsNullOrWhiteSpace(assignedBy))
            throw new DomainException("Assigned by is required", "WorkOrder.Assign.AssignedByRequired");

        AssignedEngineerId = engineerId;
        AssignedEngineerName = engineerName;
        AssignedBy = assignedBy;
        AssignedAtUtc = DateTime.UtcNow;
        Status = WorkOrderStatus.Assigned;
    }

    public void Start()
    {
        if (Status != WorkOrderStatus.Assigned)
            throw new DomainException("Work order can only be started from Assigned state", "WorkOrder.Start.RequiresAssigned");

        Status = WorkOrderStatus.InProgress;
    }

    public void Complete()
    {
        if (Status != WorkOrderStatus.InProgress)
            throw new DomainException("Work order can only be completed from InProgress state", "WorkOrder.Complete.RequiresInProgress");

        Status = WorkOrderStatus.PendingInternalReview;
    }

    public void Close()
    {
        if (Status != WorkOrderStatus.PendingInternalReview && Status != WorkOrderStatus.PendingCustomerAcceptance)
            throw new DomainException("Work order can only be closed from PendingInternalReview or PendingCustomerAcceptance state", "WorkOrder.Close.RequiresPendingReviewOrCustomerAcceptance");

        Status = WorkOrderStatus.Closed;
    }

    public void SubmitForCustomerAcceptance()
    {
        if (Status != WorkOrderStatus.PendingInternalReview)
            throw new DomainException("Work order can only be submitted for customer acceptance from PendingInternalReview state", "WorkOrder.SubmitForCustomerAcceptance.RequiresPendingInternalReview");

        Status = WorkOrderStatus.PendingCustomerAcceptance;
        AddDomainEvent(new WorkOrderSubmittedForCustomerAcceptanceEvent(Id, WoNumber));
    }

    public void AcceptByCustomer(string acceptedBy)
    {
        if (Status != WorkOrderStatus.PendingCustomerAcceptance)
            throw new DomainException("Work order can only be accepted by customer from PendingCustomerAcceptance state", "WorkOrder.AcceptByCustomer.RequiresPendingCustomerAcceptance");

        if (string.IsNullOrWhiteSpace(acceptedBy))
            throw new DomainException("Accepted by is required", "WorkOrder.AcceptByCustomer.AcceptedByRequired");

        Status = WorkOrderStatus.Closed;
        AddDomainEvent(new WorkOrderAcceptedByCustomerEvent(Id, WoNumber, acceptedBy.Trim()));
    }

    public void RejectByCustomer(string reason)
    {
        if (Status != WorkOrderStatus.PendingCustomerAcceptance)
            throw new DomainException("Work order can only be rejected by customer from PendingCustomerAcceptance state", "WorkOrder.RejectByCustomer.RequiresPendingCustomerAcceptance");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Rejection reason is required", "WorkOrder.RejectByCustomer.ReasonRequired");

        Status = WorkOrderStatus.Rework;
        AddDomainEvent(new WorkOrderRejectedByCustomerEvent(Id, WoNumber, reason.Trim()));
    }

    public void Cancel()
    {
        if (Status == WorkOrderStatus.Closed || Status == WorkOrderStatus.Cancelled)
            throw new DomainException("Closed or cancelled work order cannot be cancelled", "WorkOrder.Cancel.NotAllowedWhenClosedOrCancelled");

        Status = WorkOrderStatus.Cancelled;
    }

    public void ApplySlaStatus(SlaStatus slaStatus, DateTime evaluatedAtUtc)
    {
        if (slaStatus == SlaStatus.Breached)
        {
            if (_wasBreached)
                return;

            _wasBreached = true;
            AddDomainEvent(new SlaBreachedEvent(Id, WoNumber, ResolutionDeadlineUtc, evaluatedAtUtc));
            return;
        }

        _wasBreached = false;
    }

    public void CaptureClientSignature(Signature signature)
    {
        if (ClientSignature is not null)
            throw new DomainException("Client signature already captured.", "WorkOrder.Signature.ClientAlreadyCaptured");

        ClientSignature = signature;
        AddDomainEvent(new WorkOrderClientSignedEvent(Id, WoNumber, signature.SignerName, signature.SignedAtUtc));
    }

    public void CaptureEngineerSignature(Signature signature)
    {
        if (EngineerSignature is not null)
            throw new DomainException("Engineer signature already captured.", "WorkOrder.Signature.EngineerAlreadyCaptured");

        EngineerSignature = signature;
    }

    private static int GetDefaultResponseMinutes(SlaClass slaClass)
    {
        return slaClass switch
        {
            SlaClass.P1 => 60,
            SlaClass.P2 => 240,
            SlaClass.P3 => 1440,
            _ => 2880
        };
    }

    private static int GetDefaultResolutionMinutes(SlaClass slaClass)
    {
        return slaClass switch
        {
            SlaClass.P1 => 240,
            SlaClass.P2 => 480,
            SlaClass.P3 => 1440,
            _ => 2880
        };
    }
}
