using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.VisitSpecifications;

public sealed class ScheduledVisitsForDateSpecification : BaseSpecification<Visit>
{
    public ScheduledVisitsForDateSpecification(DateTime date, Guid? engineerId = null)
        : base(v => v.ScheduledDate.Date == date.Date && 
                    (v.Status == VisitStatus.Scheduled || v.Status == VisitStatus.InProgress) &&
                    (!engineerId.HasValue || v.EngineerId == engineerId.Value) &&
                    !v.IsDeleted)
    {
        ApplyOrderBy(v => v.ScheduledDate);
    }
}
