using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.SiteSpecifications;

public sealed class SitesByOfficeSpecification : BaseSpecification<Site>
{
    public SitesByOfficeSpecification(Guid officeId)
        : base(s => s.OfficeId == officeId && !s.IsDeleted)
    {
        AddInclude(s => s.PowerSystem);
        AddInclude(s => s.RadioEquipment);
        ApplyOrderBy(s => s.SiteCode.Value);
    }
}
