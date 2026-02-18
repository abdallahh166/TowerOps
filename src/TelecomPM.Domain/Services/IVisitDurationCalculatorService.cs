using System;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;

namespace TelecomPM.Domain.Services;

public interface IVisitDurationCalculatorService
{
    TimeSpan CalculateEstimatedDuration(Site site);
    bool IsVisitDurationReasonable(Visit visit);
}
