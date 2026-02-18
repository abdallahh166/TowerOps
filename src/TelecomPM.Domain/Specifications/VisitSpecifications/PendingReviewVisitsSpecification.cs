using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.VisitSpecifications;

public sealed class PendingReviewVisitsSpecification : BaseSpecification<Visit>
{
    public PendingReviewVisitsSpecification()
        : base(v => v.Status == VisitStatus.Submitted && !v.IsDeleted)
    {
        ApplyOrderBy(v => v.CreatedAt);
    }
}
