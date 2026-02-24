using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.VisitSpecifications;

public sealed class VisitsBySiteSpecification : BaseSpecification<Visit>
{
    public VisitsBySiteSpecification(Guid siteId, DateTime? fromDate = null, DateTime? toDate = null)
        : base(v => v.SiteId == siteId &&
                    (!fromDate.HasValue || v.ScheduledDate >= fromDate.Value) &&
                    (!toDate.HasValue || v.ScheduledDate <= toDate.Value) &&
                    !v.IsDeleted)
    {
        ApplyOrderByDescending(v => v.ScheduledDate);
    }
}
