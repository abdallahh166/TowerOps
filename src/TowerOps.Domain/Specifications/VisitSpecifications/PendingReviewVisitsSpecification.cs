using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.VisitSpecifications;

public sealed class PendingReviewVisitsSpecification : BaseSpecification<Visit>
{
    public PendingReviewVisitsSpecification()
        : base(v => v.Status == VisitStatus.Submitted && !v.IsDeleted)
    {
        ApplyOrderBy(v => v.CreatedAt);
    }
}
