using MediatR;
using System.Globalization;
using TowerOps.Application.Commands.DailyPlans;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.DailyPlans;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Application.Commands.DailyPlans.AssignSiteToEngineer;

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

        var maxSites = await ResolveMaxSitesPerEngineerAsync(plan.PlanDate, cancellationToken);
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

            var speed = await ResolveAverageSpeedKmhAsync(plan.PlanDate, cancellationToken);
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

    private async Task<int> ResolveMaxSitesPerEngineerAsync(DateOnly planDate, CancellationToken cancellationToken)
    {
        var maxSites = await _settingsService.GetAsync("Route:MaxSitesPerEngineerPerDay", 8, cancellationToken);
        maxSites = Math.Clamp(maxSites, 1, 100);

        var enableRamadanScheduling = await _settingsService.GetAsync("Route:EnableRamadanScheduling", true, cancellationToken);
        if (!enableRamadanScheduling || !IsRamadan(planDate))
            return maxSites;

        var defaultRamadanMax = Math.Max(1, maxSites - 2);
        var ramadanMaxSites = await _settingsService.GetAsync(
            "Route:RamadanMaxSitesPerEngineerPerDay",
            defaultRamadanMax,
            cancellationToken);

        return Math.Clamp(Math.Min(maxSites, ramadanMaxSites), 1, 100);
    }

    private async Task<decimal> ResolveAverageSpeedKmhAsync(DateOnly planDate, CancellationToken cancellationToken)
    {
        var baseSpeed = await _settingsService.GetAsync("Route:AverageSpeedKmh", 40m, cancellationToken);
        baseSpeed = Math.Clamp(baseSpeed, 1m, 200m);

        var enableKhamsinAdjustment = await _settingsService.GetAsync("Route:EnableKhamsinSeasonAdjustment", true, cancellationToken);
        if (!enableKhamsinAdjustment)
            return baseSpeed;

        var khamsinStart = await _settingsService.GetAsync("Route:KhamsinStartMonthDay", "03-01", cancellationToken);
        var khamsinEnd = await _settingsService.GetAsync("Route:KhamsinEndMonthDay", "05-15", cancellationToken);

        if (!IsWithinMonthDayRange(planDate, khamsinStart, khamsinEnd))
            return baseSpeed;

        var defaultKhamsinSpeed = Math.Max(1m, baseSpeed - 10m);
        var khamsinSpeed = await _settingsService.GetAsync("Route:KhamsinAverageSpeedKmh", defaultKhamsinSpeed, cancellationToken);

        return Math.Clamp(Math.Min(baseSpeed, khamsinSpeed), 1m, 200m);
    }

    private static bool IsRamadan(DateOnly planDate)
    {
        var umAlQura = new UmAlQuraCalendar();
        var asDateTime = planDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        return umAlQura.GetMonth(asDateTime) == 9;
    }

    private static bool IsWithinMonthDayRange(DateOnly date, string startMonthDay, string endMonthDay)
    {
        if (!TryParseMonthDay(startMonthDay, out var startMonth, out var startDay))
            return false;

        if (!TryParseMonthDay(endMonthDay, out var endMonth, out var endDay))
            return false;

        var current = date.Month * 100 + date.Day;
        var start = startMonth * 100 + startDay;
        var end = endMonth * 100 + endDay;

        if (start <= end)
            return current >= start && current <= end;

        return current >= start || current <= end;
    }

    private static bool TryParseMonthDay(string value, out int month, out int day)
    {
        month = 0;
        day = 0;

        if (string.IsNullOrWhiteSpace(value))
            return false;

        var parts = value.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            return false;

        if (!int.TryParse(parts[0], out month) || !int.TryParse(parts[1], out day))
            return false;

        if (month < 1 || month > 12)
            return false;

        if (day < 1 || day > DateTime.DaysInMonth(2024, month))
            return false;

        return true;
    }
}
