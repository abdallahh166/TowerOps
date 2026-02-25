using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.SiteSpecifications;

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
