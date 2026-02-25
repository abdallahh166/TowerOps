using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Specifications.VisitSpecifications;

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

