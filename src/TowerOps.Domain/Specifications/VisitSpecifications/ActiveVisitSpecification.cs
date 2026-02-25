using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;

namespace TowerOps.Domain.Specifications.VisitSpecifications
{
    public sealed class ActiveVisitSpecification : BaseSpecification<Visit>
    {
        public ActiveVisitSpecification(Guid? engineerId = null)
            : base(v => v.Status == VisitStatus.InProgress &&
                        (!engineerId.HasValue || v.EngineerId == engineerId.Value) &&
                        !v.IsDeleted)
        {
            AddInclude(v => v.Photos);
            AddInclude(v => v.Readings);
            AddInclude(v => v.Checklists);
        }
    }
}
