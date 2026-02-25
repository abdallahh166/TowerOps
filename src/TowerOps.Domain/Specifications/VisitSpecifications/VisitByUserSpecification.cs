using System.Linq.Expressions;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;

namespace TowerOps.Domain.Specifications.VisitSpecifications
{
    public sealed class VisitByUserSpecification : BaseSpecification<Visit>
    {
        public VisitByUserSpecification(Guid userId, UserRole role)
            : base(BuildCriteria(userId, role))
        {
            AddInclude(v => v.Photos);
            AddInclude(v => v.Readings);
            ApplyOrderByDescending(v => v.ScheduledDate);
        }

        private static Expression<Func<Visit, bool>> BuildCriteria(Guid userId, UserRole role)
        {
            return role switch
            {
                UserRole.PMEngineer => v => v.EngineerId == userId && !v.IsDeleted,
                UserRole.Supervisor => v => v.SupervisorId == userId && !v.IsDeleted,
                UserRole.Manager or UserRole.Admin => v => !v.IsDeleted,
                _ => v => false
            };
        }
    }
}
