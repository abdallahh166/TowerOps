using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Specifications.VisitSpecifications
{
    public sealed class ActiveVisitSpecification : BaseSpecification<Visit>
    {
        public ActiveVisitSpecification(Guid? engineerId = null)
            : base(v => v.Status == VisitStatus.InProgress &&
                        (!engineerId.HasValue || v.EngineerId == engineerId.Value) &&
                        !v.IsDeleted)
        {
            AddInclude(v => v.Photos);
            AddInclude(v => v.Readings);
            AddInclude(v => v.Checklists);
        }
    }
}
