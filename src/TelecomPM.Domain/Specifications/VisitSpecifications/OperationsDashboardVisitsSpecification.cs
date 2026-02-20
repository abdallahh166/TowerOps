using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Specifications.VisitSpecifications;

public sealed class OperationsDashboardVisitsSpecification : BaseSpecification<Visit>
{
    public OperationsDashboardVisitsSpecification(
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        bool reviewedOnly = false,
        bool rejectedOnly = false,
        bool withCorrectionsOnly = false,
        bool evidenceCompleteOnly = false,
        bool approvedWithDurationOnly = false)
        : base(v =>
            (!fromDateUtc.HasValue || v.CreatedAt >= fromDateUtc.Value) &&
            (!toDateUtc.HasValue || v.CreatedAt <= toDateUtc.Value) &&
            (!reviewedOnly || (v.Status == VisitStatus.Approved || v.Status == VisitStatus.Rejected)) &&
            (!rejectedOnly || v.Status == VisitStatus.Rejected) &&
            (!withCorrectionsOnly || v.ApprovalHistory.Any(h => h.Action == ApprovalAction.RequestCorrection)) &&
            (!evidenceCompleteOnly || (v.IsReadingsComplete && v.IsPhotosComplete && v.IsChecklistComplete)) &&
            (!approvedWithDurationOnly || (v.Status == VisitStatus.Approved && v.ActualDuration != null)))
    {
    }
}
