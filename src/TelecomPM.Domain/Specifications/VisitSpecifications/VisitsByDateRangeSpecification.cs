using System.Linq.Expressions;
using TelecomPM.Domain.Entities.Visits;

namespace TelecomPM.Domain.Specifications.VisitSpecifications;

public class VisitsByDateRangeSpecification : BaseSpecification<Visit>
{
    public VisitsByDateRangeSpecification(DateTime fromDate, DateTime toDate)
        : base(v => v.ScheduledDate >= fromDate && v.ScheduledDate <= toDate)
    {
        AddInclude(v => v.Photos);
        AddInclude(v => v.Readings);
        AddOrderBy(v => v.ScheduledDate);
    }
}