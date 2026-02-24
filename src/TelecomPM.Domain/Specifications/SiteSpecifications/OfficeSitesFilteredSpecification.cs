using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Specifications.SiteSpecifications;

public sealed class OfficeSitesFilteredSpecification : BaseSpecification<Site>
{
    public OfficeSitesFilteredSpecification(
        Guid officeId,
        SiteComplexity? complexity = null,
        SiteStatus? status = null,
        int? skip = null,
        int? take = null)
        : base(s => s.OfficeId == officeId &&
                    (!complexity.HasValue || s.Complexity == complexity.Value) &&
                    (!status.HasValue || s.Status == status.Value) &&
                    !s.IsDeleted)
    {
        ApplyOrderBy(s => s.SiteCode.Value);

        if (skip.HasValue && take.HasValue)
        {
            ApplyPaging(skip.Value, take.Value);
        }
    }
}
