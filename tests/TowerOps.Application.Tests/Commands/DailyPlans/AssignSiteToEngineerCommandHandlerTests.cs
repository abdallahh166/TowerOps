using FluentAssertions;
using Moq;
using System.Globalization;
using TowerOps.Application.Commands.DailyPlans.AssignSiteToEngineer;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.DailyPlans;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.ValueObjects;
using Xunit;

namespace TowerOps.Application.Tests.Commands.DailyPlans;

public class AssignSiteToEngineerCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldApplyRamadanMaxSitesLimit()
    {
        var engineerId = Guid.NewGuid();
        var plan = DailyPlan.Create(Guid.NewGuid(), FindRamadanDate(2026), Guid.NewGuid());
        plan.AssignSiteToEngineer(
            engineerId,
            "CAI001",
            GeoLocation.Create(30.1000m, 31.2000m),
            VisitType.BM,
            "P2");

        var dailyPlanRepository = new Mock<IDailyPlanRepository>();
        dailyPlanRepository
            .Setup(r => r.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.GetBySiteCodeAsNoTrackingAsync("CAI002", It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSite("CAI002", 30.2, 31.3));

        var settingsService = new Mock<ISystemSettingsService>();
        settingsService
            .Setup(s => s.GetAsync("Route:MaxSitesPerEngineerPerDay", 8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(8);
        settingsService
            .Setup(s => s.GetAsync("Route:EnableRamadanScheduling", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        settingsService
            .Setup(s => s.GetAsync("Route:RamadanMaxSitesPerEngineerPerDay", 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var unitOfWork = new Mock<IUnitOfWork>();

        var sut = new AssignSiteToEngineerCommandHandler(
            dailyPlanRepository.Object,
            siteRepository.Object,
            settingsService.Object,
            unitOfWork.Object);

        var result = await sut.Handle(new AssignSiteToEngineerCommand
        {
            PlanId = plan.Id,
            EngineerId = engineerId,
            SiteCode = "CAI002",
            VisitType = VisitType.CM,
            Priority = "P2"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("maximum of 1 sites");

        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUseKhamsinAverageSpeed_WhenDateInsideConfiguredWindow()
    {
        var engineerId = Guid.NewGuid();
        var plan = DailyPlan.Create(Guid.NewGuid(), new DateOnly(2026, 4, 10), Guid.NewGuid());
        plan.AssignSiteToEngineer(
            engineerId,
            "CAI001",
            GeoLocation.Create(30.1000m, 31.2000m),
            VisitType.BM,
            "P2");

        var dailyPlanRepository = new Mock<IDailyPlanRepository>();
        dailyPlanRepository
            .Setup(r => r.GetByIdAsync(plan.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plan);

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.GetBySiteCodeAsNoTrackingAsync("CAI002", It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSite("CAI002", 30.35, 31.45));

        var settingsService = new Mock<ISystemSettingsService>();
        settingsService
            .Setup(s => s.GetAsync("Route:MaxSitesPerEngineerPerDay", 8, It.IsAny<CancellationToken>()))
            .ReturnsAsync(8);
        settingsService
            .Setup(s => s.GetAsync("Route:EnableRamadanScheduling", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        settingsService
            .Setup(s => s.GetAsync("Route:AverageSpeedKmh", 40m, It.IsAny<CancellationToken>()))
            .ReturnsAsync(40m);
        settingsService
            .Setup(s => s.GetAsync("Route:EnableKhamsinSeasonAdjustment", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        settingsService
            .Setup(s => s.GetAsync("Route:KhamsinStartMonthDay", "03-01", It.IsAny<CancellationToken>()))
            .ReturnsAsync("03-01");
        settingsService
            .Setup(s => s.GetAsync("Route:KhamsinEndMonthDay", "05-15", It.IsAny<CancellationToken>()))
            .ReturnsAsync("05-15");
        settingsService
            .Setup(s => s.GetAsync("Route:KhamsinAverageSpeedKmh", It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(20m);

        var unitOfWork = new Mock<IUnitOfWork>();

        var sut = new AssignSiteToEngineerCommandHandler(
            dailyPlanRepository.Object,
            siteRepository.Object,
            settingsService.Object,
            unitOfWork.Object);

        var result = await sut.Handle(new AssignSiteToEngineerCommand
        {
            PlanId = plan.Id,
            EngineerId = engineerId,
            SiteCode = "CAI002",
            VisitType = VisitType.CM,
            Priority = "P2"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.EngineerPlans.Should().ContainSingle();
        result.Value.EngineerPlans[0].Stops.Should().HaveCount(2);
        result.Value.EngineerPlans[0].TotalEstimatedTravelMinutes.Should().BeGreaterThan(0);

        settingsService.Verify(
            s => s.GetAsync("Route:KhamsinAverageSpeedKmh", It.IsAny<decimal>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static Site CreateSite(string siteCode, double latitude, double longitude)
    {
        return Site.Create(
            siteCode,
            $"Site {siteCode}",
            "OMC-1",
            Guid.NewGuid(),
            "Cairo",
            "East",
            Coordinates.Create(latitude, longitude),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);
    }

    private static DateOnly FindRamadanDate(int gregorianYear)
    {
        var calendar = new UmAlQuraCalendar();
        for (var month = 1; month <= 12; month++)
        {
            var days = DateTime.DaysInMonth(gregorianYear, month);
            for (var day = 1; day <= days; day++)
            {
                var date = new DateTime(gregorianYear, month, day);
                if (calendar.GetMonth(date) == 9)
                {
                    return DateOnly.FromDateTime(date);
                }
            }
        }

        throw new InvalidOperationException($"No Gregorian date mapped to Ramadan for year {gregorianYear}.");
    }
}
