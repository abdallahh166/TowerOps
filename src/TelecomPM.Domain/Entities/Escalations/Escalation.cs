using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.Escalations;

public sealed class Escalation : AggregateRoot<Guid>
{
    public Guid WorkOrderId { get; private set; }
    public string IncidentId { get; private set; } = string.Empty;
    public string SiteCode { get; private set; } = string.Empty;
    public SlaClass SlaClass { get; private set; }
    public decimal FinancialImpactEgp { get; private set; }
    public decimal SlaImpactPercentage { get; private set; }
    public string EvidencePackage { get; private set; } = string.Empty;
    public string PreviousActions { get; private set; } = string.Empty;
    public string RecommendedDecision { get; private set; } = string.Empty;
    public EscalationLevel Level { get; private set; }
    public EscalationStatus Status { get; private set; }
    public string SubmittedBy { get; private set; } = string.Empty;
    public DateTime SubmittedAtUtc { get; private set; }

    private Escalation() : base() { }

    private Escalation(
        Guid workOrderId,
        string incidentId,
        string siteCode,
        SlaClass slaClass,
        decimal financialImpactEgp,
        decimal slaImpactPercentage,
        string evidencePackage,
        string previousActions,
        string recommendedDecision,
        EscalationLevel level,
        string submittedBy) : base(Guid.NewGuid())
    {
        WorkOrderId = workOrderId;
        IncidentId = incidentId;
        SiteCode = siteCode;
        SlaClass = slaClass;
        FinancialImpactEgp = financialImpactEgp;
        SlaImpactPercentage = slaImpactPercentage;
        EvidencePackage = evidencePackage;
        PreviousActions = previousActions;
        RecommendedDecision = recommendedDecision;
        Level = level;
        Status = EscalationStatus.Submitted;
        SubmittedBy = submittedBy;
        SubmittedAtUtc = DateTime.UtcNow;
    }

    public static Escalation Create(
        Guid workOrderId,
        string incidentId,
        string siteCode,
        SlaClass slaClass,
        decimal financialImpactEgp,
        decimal slaImpactPercentage,
        string evidencePackage,
        string previousActions,
        string recommendedDecision,
        EscalationLevel level,
        string submittedBy)
    {
        if (workOrderId == Guid.Empty)
            throw new DomainException("WorkOrderId is required", "Escalation.WorkOrderId.Required");
        if (string.IsNullOrWhiteSpace(incidentId))
            throw new DomainException("Incident ID is required", "Escalation.IncidentId.Required");
        if (string.IsNullOrWhiteSpace(siteCode))
            throw new DomainException("Site code is required", "Escalation.SiteCode.Required");
        if (string.IsNullOrWhiteSpace(evidencePackage))
            throw new DomainException("Evidence package is required", "Escalation.EvidencePackage.Required");
        if (string.IsNullOrWhiteSpace(previousActions))
            throw new DomainException("Previous actions are required", "Escalation.PreviousActions.Required");
        if (string.IsNullOrWhiteSpace(recommendedDecision))
            throw new DomainException("Recommended decision is required", "Escalation.RecommendedDecision.Required");
        if (string.IsNullOrWhiteSpace(submittedBy))
            throw new DomainException("Submitted by is required", "Escalation.SubmittedBy.Required");

        if (financialImpactEgp < 0)
            throw new DomainException("Financial impact cannot be negative", "Escalation.FinancialImpact.NonNegative");

        if (slaImpactPercentage < 0 || slaImpactPercentage > 100)
            throw new DomainException("SLA impact percentage must be between 0 and 100", "Escalation.SlaImpact.PercentageRange");

        return new Escalation(
            workOrderId,
            incidentId,
            siteCode,
            slaClass,
            financialImpactEgp,
            slaImpactPercentage,
            evidencePackage,
            previousActions,
            recommendedDecision,
            level,
            submittedBy);
    }

    public void MarkUnderReview()
    {
        if (Status != EscalationStatus.Submitted)
            throw new DomainException("Only submitted escalation can be moved to review", "Escalation.MarkUnderReview.RequiresSubmitted");

        Status = EscalationStatus.UnderReview;
    }

    public void Approve()
    {
        if (Status != EscalationStatus.UnderReview)
            throw new DomainException("Only under-review escalation can be approved", "Escalation.Approve.RequiresUnderReview");

        Status = EscalationStatus.Approved;
    }

    public void Reject()
    {
        if (Status != EscalationStatus.UnderReview)
            throw new DomainException("Only under-review escalation can be rejected", "Escalation.Reject.RequiresUnderReview");

        Status = EscalationStatus.Rejected;
    }

    public void Close()
    {
        if (Status != EscalationStatus.Approved && Status != EscalationStatus.Rejected)
            throw new DomainException("Only approved or rejected escalation can be closed", "Escalation.Close.RequiresApprovedOrRejected");

        Status = EscalationStatus.Closed;
    }
}
