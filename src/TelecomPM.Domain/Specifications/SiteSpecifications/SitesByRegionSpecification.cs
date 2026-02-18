using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.SiteSpecifications;

public sealed class SitesByRegionSpecification : BaseSpecification<Site>
{
    public SitesByRegionSpecification(string region, string? subRegion = null)
        : base(s => s.Region == region && 
                   (string.IsNullOrWhiteSpace(subRegion) || s.SubRegion == subRegion) &&
                   !s.IsDeleted)
    {
        ApplyOrderBy(s => s.SubRegion);
    }
}
