using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.SiteSpecifications;

public sealed class SitesByComplexitySpecification : BaseSpecification<Site>
{
    public SitesByComplexitySpecification(SiteComplexity complexity, Guid? officeId = null)
        : base(s => s.Complexity == complexity && 
                    (!officeId.HasValue || s.OfficeId == officeId.Value) && 
                    !s.IsDeleted)
    {
        ApplyOrderBy(s => s.Name);
    }
}
