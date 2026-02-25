using System;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Services;

namespace TowerOps.Infrastructure.Services;

public sealed class VisitDurationCalculatorService : IVisitDurationCalculatorService
{
    public TimeSpan CalculateEstimatedDuration(Site site)
    {
        // If the site already has an estimated duration set (e.g. seeded or calculated elsewhere), respect it.
        if (site.EstimatedVisitDurationMinutes > 0)
        {
            return TimeSpan.FromMinutes(site.EstimatedVisitDurationMinutes);
        }

        var baseMinutes = 60; // 1 hour base

        // Add time based on complexity
        baseMinutes += site.Complexity switch
        {
            Domain.Enums.SiteComplexity.Low => 0,
            Domain.Enums.SiteComplexity.Medium => 30,
            Domain.Enums.SiteComplexity.High => 60,
            _ => 0
        };

        // Add time for technologies
        if (site.RadioEquipment != null)
        {
            if (site.RadioEquipment.Has2G) baseMinutes += 15;
            if (site.RadioEquipment.Has3G) baseMinutes += 15;
            if (site.RadioEquipment.Has4G) baseMinutes += 20;
        }

        // Add time for cooling units
        if (site.CoolingSystem != null)
        {
            baseMinutes += site.CoolingSystem.ACUnitsCount * 10;
        }

        // Add time for generator
        if (site.PowerSystem?.HasGenerator == true)
        {
            baseMinutes += 20;
        }

        // Add time for site sharing
        if (site.SharingInfo?.IsShared == true)
        {
            baseMinutes += 15;
        }

        return TimeSpan.FromMinutes(baseMinutes);
    }

    public bool IsVisitDurationReasonable(Visit visit)
    {
        if (visit.ActualDuration == null)
            return false;

        var duration = visit.ActualDuration.Duration;

        // Check minimum duration (30 minutes)
        if (duration < TimeSpan.FromMinutes(30))
            return false;

        // Check maximum duration (8 hours)
        if (duration > TimeSpan.FromHours(8))
            return false;

        return true;
    }
}

