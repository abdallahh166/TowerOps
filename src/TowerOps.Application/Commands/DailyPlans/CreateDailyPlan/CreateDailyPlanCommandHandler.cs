using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.DailyPlans;
using TowerOps.Domain.Entities.DailyPlans;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.DailyPlans.CreateDailyPlan;

public sealed class CreateDailyPlanCommandHandler : IRequestHandler<CreateDailyPlanCommand, Result<DailyPlanDto>>
{
    private readonly IDailyPlanRepository _dailyPlanRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDailyPlanCommandHandler(
        IDailyPlanRepository dailyPlanRepository,
        IUnitOfWork unitOfWork)
    {
        _dailyPlanRepository = dailyPlanRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DailyPlanDto>> Handle(CreateDailyPlanCommand request, CancellationToken cancellationToken)
    {
        var existing = await _dailyPlanRepository.GetByOfficeAndDateAsync(request.OfficeId, request.PlanDate, cancellationToken);
        if (existing is not null)
            return Result.Failure<DailyPlanDto>("Daily plan already exists for this office and date.");

        var dailyPlan = DailyPlan.Create(request.OfficeId, request.PlanDate, request.OfficeManagerId);
        await _dailyPlanRepository.AddAsync(dailyPlan, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(DailyPlanMapper.ToDto(dailyPlan));
    }
}
