using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Specifications.SiteSpecifications;

public class ActiveSitesSpecification : BaseSpecification<Site>
{
    public ActiveSitesSpecification()
        : base(s => s.Status == SiteStatus.OnAir)
    {
        AddOrderBy(s => s.Name);
    }

    public ActiveSitesSpecification(Guid officeId)
        : base(s => s.Status == SiteStatus.OnAir && s.OfficeId == officeId)
    {
        AddOrderBy(s => s.Name);
    }
}