using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Specifications;

namespace TelecomPM.Domain.Specifications.VisitSpecifications;

public sealed class VisitsByApprovalStatusSpecification : BaseSpecification<Visit>
{
    public VisitsByApprovalStatusSpecification(VisitStatus status, Guid? engineerId = null)
        : base(v => v.Status == status && 
                   (!engineerId.HasValue || v.EngineerId == engineerId.Value) &&
                   !v.IsDeleted)
    {
        AddInclude(v => v.Photos);
        AddInclude(v => v.Readings);
        AddInclude(v => v.ApprovalHistory);
        AddOrderByDescending(v => v.ScheduledDate);
    }
}

