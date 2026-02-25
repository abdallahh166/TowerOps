using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.VisitSpecifications;

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
