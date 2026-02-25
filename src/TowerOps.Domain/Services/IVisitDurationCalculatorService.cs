using System;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Entities.Visits;

namespace TowerOps.Domain.Services;

public interface IVisitDurationCalculatorService
{
    TimeSpan CalculateEstimatedDuration(Site site);
    bool IsVisitDurationReasonable(Visit visit);
}
