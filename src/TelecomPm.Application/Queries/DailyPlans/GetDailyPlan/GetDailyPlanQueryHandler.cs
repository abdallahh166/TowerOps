using MediatR;
using TelecomPM.Application.Commands.DailyPlans;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.DailyPlans;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.DailyPlans.GetDailyPlan;

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
