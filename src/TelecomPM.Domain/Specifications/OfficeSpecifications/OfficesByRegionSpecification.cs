using TelecomPM.Domain.Entities.Offices;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.OfficeSpecifications;

public sealed class OfficesByRegionSpecification : BaseSpecification<Office>
{
    public OfficesByRegionSpecification(string region, bool onlyActive = true)
        : base(o => o.Region == region &&
                    (!onlyActive || o.IsActive) &&
                    !o.IsDeleted)
    {
        ApplyOrderBy(o => o.Name);
    }
}

