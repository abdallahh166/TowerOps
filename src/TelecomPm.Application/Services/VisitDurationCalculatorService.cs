using System;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Services;

namespace TelecomPM.Application.Services;

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
