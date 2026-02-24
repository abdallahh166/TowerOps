using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.VisitSpecifications;

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
