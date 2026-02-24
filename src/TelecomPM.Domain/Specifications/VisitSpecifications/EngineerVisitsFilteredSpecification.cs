using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Specifications.VisitSpecifications;

public sealed class EngineerVisitsFilteredSpecification : BaseSpecification<Visit>
{
    public EngineerVisitsFilteredSpecification(
        Guid engineerId,
        VisitStatus? status = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        int? skip = null,
        int? take = null)
        : base(v => v.EngineerId == engineerId &&
                    (!status.HasValue || v.Status == status.Value) &&
                    (!fromUtc.HasValue || v.ScheduledDate >= fromUtc.Value) &&
                    (!toUtc.HasValue || v.ScheduledDate <= toUtc.Value) &&
                    !v.IsDeleted)
    {
        ApplyOrderByDescending(v => v.ScheduledDate);

        if (skip.HasValue && take.HasValue)
        {
            ApplyPaging(skip.Value, take.Value);
        }
    }
}
