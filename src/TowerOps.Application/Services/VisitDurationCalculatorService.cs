using System;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Services;

namespace TowerOps.Application.Services;

public sealed class VisitDurationCalculatorService : IVisitDurationCalculatorService
{
    public TimeSpan CalculateEstimatedDuration(Site site)
    {
        return TimeSpan.FromMinutes(site.EstimatedVisitDurationMinutes);
    }

    public bool IsVisitDurationReasonable(Visit visit)
    {
        if (visit.ActualDuration == null)
            return false;

        var duration = visit.ActualDuration.Duration;

        // Minimum 30 minutes, maximum 8 hours
        return duration >= TimeSpan.FromMinutes(30) && duration <= TimeSpan.FromHours(8);
    }
}
