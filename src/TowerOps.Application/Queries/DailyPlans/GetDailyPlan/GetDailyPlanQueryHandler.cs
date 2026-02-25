using MediatR;
using TowerOps.Application.Commands.DailyPlans;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.DailyPlans;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.DailyPlans.GetDailyPlan;

public sealed class GetDailyPlanQueryHandler : IRequestHandler<GetDailyPlanQuery, Result<DailyPlanDto>>
{
    private readonly IDailyPlanRepository _dailyPlanRepository;

    public GetDailyPlanQueryHandler(IDailyPlanRepository dailyPlanRepository)
    {
        _dailyPlanRepository = dailyPlanRepository;
    }

    public async Task<Result<DailyPlanDto>> Handle(GetDailyPlanQuery request, CancellationToken cancellationToken)
    {
        var plan = await _dailyPlanRepository.GetByOfficeAndDateAsNoTrackingAsync(request.OfficeId, request.Date, cancellationToken);
        if (plan is null)
            return Result.Failure<DailyPlanDto>("Daily plan not found.");

        return Result.Success(DailyPlanMapper.ToDto(plan));
    }
}
