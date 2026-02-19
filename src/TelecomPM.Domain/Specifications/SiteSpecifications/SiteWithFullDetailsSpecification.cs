using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.SiteSpecifications;

public sealed class SiteWithFullDetailsSpecification : BaseSpecification<Site>
{
    public SiteWithFullDetailsSpecification(Guid siteId)
        : base(s => s.Id == siteId && !s.IsDeleted)
    {
        AddInclude(s => s.TowerInfo);
        AddInclude(s => s.PowerSystem);
        AddInclude(s => s.RadioEquipment);
        AddInclude(s => s.Transmission);
        AddInclude(s => s.CoolingSystem);
        AddInclude(s => s.FireSafety);
        AddInclude(s => s.SharingInfo!);
    }
}
