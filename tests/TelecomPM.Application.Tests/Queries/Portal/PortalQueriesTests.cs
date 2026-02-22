using FluentAssertions;
using Moq;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Portal;
using TelecomPM.Application.Queries.Portal.GetPortalSites;
using TelecomPM.Application.Queries.Portal.GetPortalVisits;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Users;
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

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns(portalUser.Id);

        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByIdAsNoTrackingAsync(portalUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(portalUser);

        var portalReadRepository = new Mock<IPortalReadRepository>();
        portalReadRepository
            .Setup(r => r.GetSitesAsync("ORANGE", null, 1, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PortalSiteDto>
            {
                new() { SiteCode = orangeSite.SiteCode.Value, Name = orangeSite.Name }
            });

        var sut = new GetPortalSitesQueryHandler(
            currentUser.Object,
            users.Object,
            portalReadRepository.Object);

        var result = await sut.Handle(new GetPortalSitesQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].SiteCode.Should().Be("CAI001");

        portalReadRepository.Verify(
            r => r.GetSitesAsync("ORANGE", null, 1, 50, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetPortalVisits_ShouldAnonymizeEngineerNames_WhenSettingEnabled()
    {
        var portalUser = User.Create("Portal User", "portal@client.com", "010", UserRole.Manager, Guid.NewGuid());
        portalUser.EnableClientPortalAccess("ORANGE");

        var site = CreateSite("CAI001", "ORANGE");

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns(portalUser.Id);

        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByIdAsNoTrackingAsync(portalUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(portalUser);

        var sites = new Mock<ISiteRepository>();
        sites.Setup(r => r.GetBySiteCodeAsNoTrackingAsync("CAI001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(site);

        var settings = new Mock<ISystemSettingsService>();
        settings.Setup(s => s.GetAsync("Portal:AnonymizeEngineers", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var portalReadRepository = new Mock<IPortalReadRepository>();
        portalReadRepository
            .Setup(r => r.GetVisitsAsync("ORANGE", "CAI001", 1, 50, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PortalVisitDto>
            {
                new()
                {
                    VisitId = Guid.NewGuid(),
                    VisitNumber = "V1001",
                    Status = VisitStatus.Scheduled,
                    Type = VisitType.BM,
                    ScheduledDate = DateTime.UtcNow,
                    EngineerDisplayName = "Field Engineer"
                }
            });

        var sut = new GetPortalVisitsQueryHandler(
            currentUser.Object,
            users.Object,
            sites.Object,
            settings.Object,
            portalReadRepository.Object);

        var result = await sut.Handle(new GetPortalVisitsQuery { SiteCode = "CAI001" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].EngineerDisplayName.Should().Be("Field Engineer");
    }

    [Fact]
    public async Task GetPortalSites_ShouldClampInvalidPaginationValues()
    {
        var portalUser = User.Create("Portal User", "portal@client.com", "010", UserRole.Manager, Guid.NewGuid());
        portalUser.EnableClientPortalAccess("ORANGE");

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns(portalUser.Id);

        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByIdAsNoTrackingAsync(portalUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(portalUser);

        var portalReadRepository = new Mock<IPortalReadRepository>();
        portalReadRepository
            .Setup(r => r.GetSitesAsync("ORANGE", null, 1, 200, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<PortalSiteDto>());

        var sut = new GetPortalSitesQueryHandler(
            currentUser.Object,
            users.Object,
            portalReadRepository.Object);

        var result = await sut.Handle(new GetPortalSitesQuery { PageNumber = 0, PageSize = 500 }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        portalReadRepository.Verify(
            r => r.GetSitesAsync("ORANGE", null, 1, 200, It.IsAny<CancellationToken>()),
            Times.Once);
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
