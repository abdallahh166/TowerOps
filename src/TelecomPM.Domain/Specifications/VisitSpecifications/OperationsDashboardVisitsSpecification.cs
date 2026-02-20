using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Specifications.VisitSpecifications;

public sealed class OperationsDashboardVisitsSpecification : BaseSpecification<Visit>
{
    public OperationsDashboardVisitsSpecification(
        DateTime? fromDateUtc,
        DateTime? toDateUtc,
        bool submittedOnly = false,
        bool reviewedOnly = false,
        bool rejectedOnly = false,
        bool withCorrectionsOnly = false,
        bool evidenceCompleteOnly = false,
        bool approvedWithDurationOnly = false)
        : base(v =>
            (!fromDateUtc.HasValue || v.CreatedAt >= fromDateUtc.Value) &&
            (!toDateUtc.HasValue || v.CreatedAt <= toDateUtc.Value) &&
            (!submittedOnly || (
                v.Status == VisitStatus.Submitted ||
                v.Status == VisitStatus.UnderReview ||
                v.Status == VisitStatus.NeedsCorrection ||
                v.Status == VisitStatus.Approved ||
                v.Status == VisitStatus.Rejected)) &&
            (!reviewedOnly || (v.Status == VisitStatus.Approved || v.Status == VisitStatus.Rejected)) &&
            (!rejectedOnly || v.Status == VisitStatus.Rejected) &&
            (!withCorrectionsOnly || v.ApprovalHistory.Any(h => h.Action == ApprovalAction.RequestCorrection)) &&
            (!evidenceCompleteOnly || (
                v.Photos.Count >= 2 &&
                v.Readings.Any() &&
                v.Checklists.Any() &&
                (v.Checklists.Count(c => c.Status != CheckStatus.NA) * 100 / v.Checklists.Count) >= 80)) &&
            (!approvedWithDurationOnly || (v.Status == VisitStatus.Approved && v.ActualDuration != null)))
    {
    }
}
