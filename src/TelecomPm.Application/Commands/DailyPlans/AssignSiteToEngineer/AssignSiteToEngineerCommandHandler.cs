using MediatR;
using TelecomPM.Application.Commands.DailyPlans;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.DailyPlans;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Application.Commands.DailyPlans.AssignSiteToEngineer;

public sealed class AssignSiteToEngineerCommandHandler : IRequestHandler<AssignSiteToEngineerCommand, Result<DailyPlanDto>>
{
    private readonly IDailyPlanRepository _dailyPlanRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly ISystemSettingsService _settingsService;
    private readonly IUnitOfWork _unitOfWork;

    public AssignSiteToEngineerCommandHandler(
        IDailyPlanRepository dailyPlanRepository,
        ISiteRepository siteRepository,
        ISystemSettingsService settingsService,
        IUnitOfWork unitOfWork)
    {
        _dailyPlanRepository = dailyPlanRepository;
        _siteRepository = siteRepository;
        _settingsService = settingsService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DailyPlanDto>> Handle(AssignSiteToEngineerCommand request, CancellationToken cancellationToken)
    {
        var plan = await _dailyPlanRepository.GetByIdAsync(request.PlanId, cancellationToken);
        if (plan is null)
            return Result.Failure<DailyPlanDto>("Daily plan not found.");

        var site = await _siteRepository.GetBySiteCodeAsNoTrackingAsync(request.SiteCode, cancellationToken);
        if (site is null)
            return Result.Failure<DailyPlanDto>("Site not found.");

        var maxSites = await _settingsService.GetAsync("Route:MaxSitesPerEngineerPerDay", 8, cancellationToken);
        var assignedForEngineer = plan.EngineerPlans.FirstOrDefault(ep => ep.EngineerId == request.EngineerId)?.Stops.Count ?? 0;
        var alreadyAssignedToEngineer = plan.EngineerPlans
            .FirstOrDefault(ep => ep.EngineerId == request.EngineerId)?
            .Stops.Any(s => string.Equals(s.SiteCode, request.SiteCode, StringComparison.OrdinalIgnoreCase)) == true;

        if (!alreadyAssignedToEngineer && assignedForEngineer >= maxSites)
            return Result.Failure<DailyPlanDto>($"Engineer already has the maximum of {maxSites} sites.");

        try
        {
            plan.AssignSiteToEngineer(
                request.EngineerId,
                request.SiteCode,
                GeoLocation.Create((decimal)site.Coordinates.Latitude, (decimal)site.Coordinates.Longitude),
                request.VisitType,
                request.Priority);

            var speed = await _settingsService.GetAsync("Route:AverageSpeedKmh", 40m, cancellationToken);
            plan.SuggestOrder(request.EngineerId, speed);

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
