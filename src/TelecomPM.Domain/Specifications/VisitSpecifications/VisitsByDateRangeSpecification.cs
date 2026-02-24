using TelecomPM.Domain.Entities.Visits;

namespace TelecomPM.Domain.Specifications.VisitSpecifications;

public class VisitsByDateRangeSpecification : BaseSpecification<Visit>
{
    public VisitsByDateRangeSpecification(
        DateTime fromDate,
        DateTime toDate,
        Guid? engineerId = null,
        Guid? siteId = null,
        IReadOnlyCollection<Guid>? siteIds = null)
        : base(v => v.ScheduledDate >= fromDate &&
                    v.ScheduledDate <= toDate &&
                    (!engineerId.HasValue || v.EngineerId == engineerId.Value) &&
                    (!siteId.HasValue || v.SiteId == siteId.Value) &&
                    (siteIds == null || siteIds.Count == 0 || siteIds.Contains(v.SiteId)) &&
                    !v.IsDeleted)
    {
        AddOrderBy(v => v.ScheduledDate);
    }
}
