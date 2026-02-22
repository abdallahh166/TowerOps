using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.DailyPlans;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.DailyPlans.GetSuggestedOrder;

public sealed class GetSuggestedOrderQueryHandler : IRequestHandler<GetSuggestedOrderQuery, Result<IReadOnlyList<PlannedVisitStopDto>>>
{
    private readonly IDailyPlanRepository _dailyPlanRepository;
    private readonly ISystemSettingsService _settingsService;
    private readonly IUnitOfWork _unitOfWork;

    public GetSuggestedOrderQueryHandler(
        IDailyPlanRepository dailyPlanRepository,
        ISystemSettingsService settingsService,
        IUnitOfWork unitOfWork)
    {
        _dailyPlanRepository = dailyPlanRepository;
        _settingsService = settingsService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IReadOnlyList<PlannedVisitStopDto>>> Handle(GetSuggestedOrderQuery request, CancellationToken cancellationToken)
    {
        var plan = await _dailyPlanRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan is null)
            return Result.Failure<IReadOnlyList<PlannedVisitStopDto>>("Daily plan not found.");

        var speed = await _settingsService.GetAsync("Route:AverageSpeedKmh", 40m, cancellationToken);
        var ordered = plan.SuggestOrder(request.EngineerId, speed);

        await _dailyPlanRepository.UpdateAsync(plan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = ordered
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
            .ToList();

        return Result.Success<IReadOnlyList<PlannedVisitStopDto>>(result);
    }
}
