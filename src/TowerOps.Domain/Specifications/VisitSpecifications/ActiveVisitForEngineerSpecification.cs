using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.VisitSpecifications;

public sealed class ActiveVisitForEngineerSpecification : BaseSpecification<Visit>
{
    public ActiveVisitForEngineerSpecification(Guid engineerId)
        : base(v => v.EngineerId == engineerId && 
                    v.Status == VisitStatus.InProgress && 
                    !v.IsDeleted)
    {
        AddInclude(v => v.Photos);
        AddInclude(v => v.Readings);
        AddInclude(v => v.Checklists);
    }
}
