using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.VisitSpecifications;

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
