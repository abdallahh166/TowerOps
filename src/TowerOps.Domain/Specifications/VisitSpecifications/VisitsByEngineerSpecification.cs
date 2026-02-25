using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.VisitSpecifications;

public sealed class VisitsByEngineerSpecification : BaseSpecification<Visit>
{
    public VisitsByEngineerSpecification(Guid engineerId)
        : base(v => v.EngineerId == engineerId && !v.IsDeleted)
    {
        AddInclude(v => v.Photos);
        AddInclude(v => v.Readings);
        AddInclude(v => v.MaterialsUsed);
        ApplyOrderByDescending(v => v.ScheduledDate);
    }
}
