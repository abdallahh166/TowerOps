using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.VisitSpecifications;

public sealed class VisitsNeedingCorrectionSpecification : BaseSpecification<Visit>
{
    public VisitsNeedingCorrectionSpecification(Guid? engineerId = null)
        : base(v => (!engineerId.HasValue || v.EngineerId == engineerId.Value) &&
                    v.Status == VisitStatus.NeedsCorrection && 
                    !v.IsDeleted)
    {
        ApplyOrderBy(v => (object?)v.UpdatedAt ?? DateTime.MinValue);
    }
}
