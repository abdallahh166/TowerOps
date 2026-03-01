using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Entities.WorkOrders;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Events;
using TowerOps.Domain.Interfaces.Services;
using TowerOps.Domain.ValueObjects;
using TowerOps.Infrastructure.Persistence;
using TowerOps.Infrastructure.Persistence.Repositories;
using Xunit;

namespace TowerOps.Infrastructure.Tests.Repositories;

public class PortalReadRepositoryTests
{
    [Fact]
    public async Task GetSitesAsync_ShouldEnforceClientIsolation_AndClampPageSize()
    {
        await using var context = CreateContext();

        var orangeSites = Enumerable.Range(1, 250)
            .Select(i => CreateSite($"ORA{i:000}", "ORANGE"))
            .ToList();
        var vodafoneSites = Enumerable.Range(1, 30)
            .Select(i => CreateSite($"VOD{i:000}", "VODAFONE"))
            .ToList();

        await context.Sites.AddRangeAsync(orangeSites);
        await context.Sites.AddRangeAsync(vodafoneSites);
        await context.SaveChangesAsync();

        var sut = new PortalReadRepository(context);

        var firstPage = await sut.GetSitesAsync("ORANGE", null, pageNumber: 1, pageSize: 1000, sortBy: "siteCode", sortDescending: false);
        var secondPage = await sut.GetSitesAsync("ORANGE", null, pageNumber: 2, pageSize: 1000, sortBy: "siteCode", sortDescending: false);

        firstPage.Should().HaveCount(200);
        secondPage.Should().HaveCount(50);
        firstPage.Concat(secondPage).Should().OnlyContain(x => x.SiteCode.StartsWith("ORA", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetWorkOrdersAsync_ShouldReturnOnlyClientScopedRecords()
    {
        await using var context = CreateContext();

        var orangeSite1 = CreateSite("ORA001", "ORANGE");
        var orangeSite2 = CreateSite("ORA002", "ORANGE");
        var vodafoneSite = CreateSite("VOD001", "VODAFONE");

        await context.Sites.AddRangeAsync(orangeSite1, orangeSite2, vodafoneSite);

        var orangeWorkOrders = new[]
        {
            WorkOrder.Create("WO-ORA-1", orangeSite1.SiteCode.Value, "ORA", SlaClass.P1, "Issue 1"),
            WorkOrder.Create("WO-ORA-2", orangeSite2.SiteCode.Value, "ORA", SlaClass.P2, "Issue 2"),
            WorkOrder.Create("WO-ORA-3", orangeSite1.SiteCode.Value, "ORA", SlaClass.P3, "Issue 3")
        };
        var vodafoneWorkOrders = new[]
        {
            WorkOrder.Create("WO-VOD-1", vodafoneSite.SiteCode.Value, "VOD", SlaClass.P1, "Issue 1"),
            WorkOrder.Create("WO-VOD-2", vodafoneSite.SiteCode.Value, "VOD", SlaClass.P2, "Issue 2")
        };

        await context.WorkOrders.AddRangeAsync(orangeWorkOrders);
        await context.WorkOrders.AddRangeAsync(vodafoneWorkOrders);
        await context.SaveChangesAsync();

        var sut = new PortalReadRepository(context);

        var results = await sut.GetWorkOrdersAsync("ORANGE", pageNumber: 1, pageSize: 50, sortBy: "createdAt", sortDescending: true);

        results.Should().HaveCount(3);
        results.Should().OnlyContain(x => x.SiteCode.StartsWith("ORA", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetVisitsAsync_ShouldReturnOnlyVisitsForClientAndRequestedSite()
    {
        await using var context = CreateContext();

        var orangeSite = CreateSite("ORA001", "ORANGE");
        var vodafoneSite = CreateSite("VOD001", "VODAFONE");

        await context.Sites.AddRangeAsync(orangeSite, vodafoneSite);

        var orangeVisit = Visit.Create(
            "V-ORA-1",
            orangeSite.Id,
            orangeSite.SiteCode.Value,
            orangeSite.Name,
            Guid.NewGuid(),
            "Orange Engineer",
            DateTime.UtcNow.Date,
            VisitType.BM);

        var vodafoneVisit = Visit.Create(
            "V-VOD-1",
            vodafoneSite.Id,
            vodafoneSite.SiteCode.Value,
            vodafoneSite.Name,
            Guid.NewGuid(),
            "Vodafone Engineer",
            DateTime.UtcNow.Date,
            VisitType.BM);

        await context.Visits.AddRangeAsync(orangeVisit, vodafoneVisit);
        await context.SaveChangesAsync();

        var sut = new PortalReadRepository(context);

        var results = await sut.GetVisitsAsync("ORANGE", orangeSite.SiteCode.Value, pageNumber: 1, pageSize: 50, sortBy: "scheduledDate", sortDescending: true, anonymizeEngineers: true);

        results.Should().HaveCount(1);
        results[0].VisitNumber.Should().Be("V-ORA-1");
        results[0].EngineerDisplayName.Should().Be("Field Engineer");
    }

    [Fact]
    public async Task SiteExistsForClientAsync_ShouldRespectClientBoundary()
    {
        await using var context = CreateContext();

        var orangeSite = CreateSite("ORA001", "ORANGE");
        await context.Sites.AddAsync(orangeSite);
        await context.SaveChangesAsync();

        var sut = new PortalReadRepository(context);

        var existsForOrange = await sut.SiteExistsForClientAsync("ORANGE", "ORA001");
        var existsForVodafone = await sut.SiteExistsForClientAsync("VODAFONE", "ORA001");

        existsForOrange.Should().BeTrue();
        existsForVodafone.Should().BeFalse();
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"portal-read-{Guid.NewGuid():N}")
            .Options;

        var dispatcher = new Mock<IDomainEventDispatcher>();
        dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new ApplicationDbContext(options, dispatcher.Object);
    }

    private static Site CreateSite(string siteCode, string clientCode)
    {
        var site = Site.Create(
            siteCode,
            $"Site {siteCode}",
            "OMC",
            Guid.NewGuid(),
            "Cairo",
            "East",
            Coordinates.Create(30.0, 31.0),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);

        site.SetClientCode(clientCode);
        return site;
    }
}
