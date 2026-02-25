using MediatR;
using TowerOps.Application.Commands.DailyPlans;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.DailyPlans;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.DailyPlans.PublishDailyPlan;

public sealed class PublishDailyPlanCommandHandler : IRequestHandler<PublishDailyPlanCommand, Result<DailyPlanDto>>
{
    private readonly IDailyPlanRepository _dailyPlanRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishDailyPlanCommandHandler(
        IDailyPlanRepository dailyPlanRepository,
        IUnitOfWork unitOfWork)
    {
        _dailyPlanRepository = dailyPlanRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DailyPlanDto>> Handle(PublishDailyPlanCommand request, CancellationToken cancellationToken)
    {
        var plan = await _dailyPlanRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan is null)
            return Result.Failure<DailyPlanDto>("Daily plan not found.");

        try
        {
            plan.Publish();
            await _dailyPlanRepository.UpdateAsync(plan, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success(DailyPlanMapper.ToDto(plan));
        }
        catch (DomainException ex)
        {
            return Result.Failure<DailyPlanDto>(ex.Message);
        }
    }
}
