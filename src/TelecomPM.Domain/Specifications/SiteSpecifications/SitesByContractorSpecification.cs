using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.SiteSpecifications;

public sealed class SitesByContractorSpecification : BaseSpecification<Site>
{
    public SitesByContractorSpecification(string contractor, Guid? officeId = null)
        : base(s => !string.IsNullOrWhiteSpace(s.Subcontractor) && 
                   s.Subcontractor.ToUpper() == contractor.ToUpper() &&
                   (!officeId.HasValue || s.OfficeId == officeId.Value) &&
                   !s.IsDeleted)
    {
        AddInclude(s => s.TowerInfo);
        AddInclude(s => s.PowerSystem);
        AddOrderBy(s => s.Name);
    }
}

