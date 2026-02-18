using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Specifications.SiteSpecifications;

public class UnassignedSitesSpecification : BaseSpecification<Site>
{
    public UnassignedSitesSpecification()
        : base(s => s.AssignedEngineerId == null && s.Status == SiteStatus.OnAir)
    {
        AddOrderBy(s => s.Complexity);
        AddThenBy(s => s.Name);
    }

    public UnassignedSitesSpecification(Guid officeId)
        : base(s => s.AssignedEngineerId == null &&
                    s.Status == SiteStatus.OnAir &&
                    s.OfficeId == officeId)
    {
        AddOrderBy(s => s.Complexity);
        AddThenBy(s => s.Name);
    }
}