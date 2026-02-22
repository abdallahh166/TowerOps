using FluentAssertions;
using Moq;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.Queries.Portal.GetPortalSites;
using TelecomPM.Application.Queries.Portal.GetPortalVisits;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Application.Tests.Queries.Portal;

public class PortalQueriesTests
{
    [Fact]
    public async Task GetPortalSites_ShouldFilterByCurrentUserClientCode()
    {
        var portalUser = User.Create("Portal User", "portal@client.com", "010", UserRole.Manager, Guid.NewGuid());
        portalUser.EnableClientPortalAccess("ORANGE");

        var orangeSite = CreateSite("CAI001", "ORANGE");
        var vodafoneSite = CreateSite("ALX001", "VODAFONE");

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns(portalUser.Id);

        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByIdAsNoTrackingAsync(portalUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(portalUser);

        var sites = new Mock<ISiteRepository>();
        sites.Setup(r => r.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Site> { orangeSite, vodafoneSite });

        var visits = new Mock<IVisitRepository>();
        visits.Setup(r => r.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Visit>());

        var workOrders = new Mock<IWorkOrderRepository>();
        workOrders.Setup(r => r.GetAllAsNoTrackingAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Domain.Entities.WorkOrders.WorkOrder>());

        var sut = new GetPortalSitesQueryHandler(
            currentUser.Object,
            users.Object,
            sites.Object,
            visits.Object,
            workOrders.Object);

        var result = await sut.Handle(new GetPortalSitesQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].SiteCode.Should().Be("CAI001");
    }

    [Fact]
    public async Task GetPortalVisits_ShouldAnonymizeEngineerNames_WhenSettingEnabled()
    {
        var portalUser = User.Create("Portal User", "portal@client.com", "010", UserRole.Manager, Guid.NewGuid());
        portalUser.EnableClientPortalAccess("ORANGE");

        var site = CreateSite("CAI001", "ORANGE");
        var visit = Visit.Create(
            "V1001",
            site.Id,
            site.SiteCode.Value,
            site.Name,
            Guid.NewGuid(),
            "Ahmed Engineer",
            DateTime.UtcNow,
            VisitType.PreventiveMaintenance);

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns(portalUser.Id);

        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByIdAsNoTrackingAsync(portalUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(portalUser);

        var sites = new Mock<ISiteRepository>();
        sites.Setup(r => r.GetBySiteCodeAsNoTrackingAsync("CAI001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(site);

        var visits = new Mock<IVisitRepository>();
        visits.Setup(r => r.GetBySiteIdAsNoTrackingAsync(site.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Visit> { visit });

        var settings = new Mock<ISystemSettingsService>();
        settings.Setup(s => s.GetAsync("Portal:AnonymizeEngineers", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var sut = new GetPortalVisitsQueryHandler(
            currentUser.Object,
            users.Object,
            sites.Object,
            visits.Object,
            settings.Object);

        var result = await sut.Handle(new GetPortalVisitsQuery { SiteCode = "CAI001" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].EngineerDisplayName.Should().Be("Field Engineer");
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
            Coordinates.Create(30.1, 31.2),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);

        site.SetClientCode(clientCode);
        return site;
    }
}
