using TelecomPM.Application.DTOs.DailyPlans;
using TelecomPM.Domain.Entities.DailyPlans;

namespace TelecomPM.Application.Commands.DailyPlans;

internal static class DailyPlanMapper
{
    public static DailyPlanDto ToDto(DailyPlan plan)
    {
        return new DailyPlanDto
        {
            Id = plan.Id,
            OfficeId = plan.OfficeId,
            PlanDate = plan.PlanDate,
            OfficeManagerId = plan.OfficeManagerId,
            Status = plan.Status,
            EngineerPlans = plan.EngineerPlans
                .Select(ep => new EngineerDayPlanDto
                {
                    EngineerId = ep.EngineerId,
                    TotalEstimatedDistanceKm = ep.TotalEstimatedDistanceKm,
                    TotalEstimatedTravelMinutes = ep.TotalEstimatedTravelMinutes,
                    Stops = ep.Stops
                        .OrderBy(s => s.Order)
                        .Select(s => new PlannedVisitStopDto
                        {
                            Order = s.Order,
                            SiteCode = s.SiteCode,
                            Latitude = s.SiteLocation.Latitude,
                            Longitude = s.SiteLocation.Longitude,
                            VisitType = s.VisitType,
                            Priority = s.Priority,
                            DistanceFromPreviousKm = s.DistanceFromPreviousKm,
                            EstimatedTravelMinutes = s.EstimatedTravelMinutes
                        })
                        .ToList()
                })
                .ToList()
        };
    }
}
