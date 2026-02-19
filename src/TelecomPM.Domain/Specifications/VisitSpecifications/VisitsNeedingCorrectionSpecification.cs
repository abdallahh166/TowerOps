using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.VisitSpecifications;

public sealed class VisitsNeedingCorrectionSpecification : BaseSpecification<Visit>
{
    public VisitsNeedingCorrectionSpecification(Guid engineerId)
        : base(v => v.EngineerId == engineerId && 
                    v.Status == VisitStatus.NeedsCorrection && 
                    !v.IsDeleted)
    {
        ApplyOrderBy(v => (object?)v.UpdatedAt ?? DateTime.MinValue);
    }
}
