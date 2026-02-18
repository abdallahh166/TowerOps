using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.VisitSpecifications;

public sealed class OverdueVisitsSpecification : BaseSpecification<Visit>
{
    public OverdueVisitsSpecification()
        : base(v => v.ScheduledDate < DateTime.UtcNow && 
                    v.Status == VisitStatus.Scheduled && 
                    !v.IsDeleted)
    {
        ApplyOrderBy(v => v.ScheduledDate);
    }
}
