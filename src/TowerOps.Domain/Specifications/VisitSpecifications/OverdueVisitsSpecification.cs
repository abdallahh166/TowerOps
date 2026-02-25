using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.VisitSpecifications;

public sealed class OverdueVisitsSpecification : BaseSpecification<Visit>
{
    public OverdueVisitsSpecification(Guid? engineerId = null)
        : base(v => v.ScheduledDate < DateTime.UtcNow && 
                    v.Status == VisitStatus.Scheduled && 
                    (!engineerId.HasValue || v.EngineerId == engineerId.Value) &&
                    !v.IsDeleted)
    {
        ApplyOrderBy(v => v.ScheduledDate);
    }
}
