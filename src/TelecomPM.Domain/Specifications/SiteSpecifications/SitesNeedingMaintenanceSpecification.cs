using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.SiteSpecifications;

public sealed class SitesNeedingMaintenanceSpecification : BaseSpecification<Site>
{
    public SitesNeedingMaintenanceSpecification(int daysThreshold)
        : base(s => (!s.LastVisitDate.HasValue || 
                     s.LastVisitDate.Value.AddDays(daysThreshold) <= DateTime.UtcNow) &&
                    s.Status == SiteStatus.OnAir && 
                    !s.IsDeleted)
    {
        ApplyOrderBy(s => (object?)s.LastVisitDate ?? DateTime.MinValue);
    }
}
