using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.VisitSpecifications;

public sealed class VisitsBySiteSpecification : BaseSpecification<Visit>
{
    public VisitsBySiteSpecification(Guid siteId)
        : base(v => v.SiteId == siteId && !v.IsDeleted)
    {
        ApplyOrderByDescending(v => v.ScheduledDate);
    }
}
