using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.VisitSpecifications;

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
