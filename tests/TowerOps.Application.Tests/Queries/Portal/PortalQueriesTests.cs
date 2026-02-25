using FluentAssertions;
using Moq;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Portal;
using TowerOps.Application.Queries.Portal.GetPortalSites;
using TowerOps.Application.Queries.Portal.GetPortalVisitEvidence;
using TowerOps.Application.Queries.Portal.GetPortalVisits;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using Xunit;

namespace TowerOps.Application.Tests.Queries.Portal;

public class PortalQueriesTests
{
    [Fact]
    public async Task GetPortalSites_ShouldFilterByCurrentUserClientCode()
    {
        var portalUser = User.Create("Portal User", "portal@client.com", "010", UserRole.Manager, Guid.NewGuid());
        portalUser.EnableClientPortalAccess("ORANGE");

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns(portalUser.Id);

        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByIdAsNoTrackingAsync(portalUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(portalUser);

        var portalReadRepository = new Mock<IPortalReadRepository>();
        portalReadRepository
            .Setup(r => r.GetSitesAsync("ORANGE", null, 1, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PortalSiteDto>
            {
                new() { SiteCode = "CAI001", Name = "Site CAI001" }
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

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns(portalUser.Id);

        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByIdAsNoTrackingAsync(portalUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(portalUser);

        var settings = new Mock<ISystemSettingsService>();
        settings.Setup(s => s.GetAsync("Portal:AnonymizeEngineers", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var portalReadRepository = new Mock<IPortalReadRepository>();
        portalReadRepository
            .Setup(r => r.SiteExistsForClientAsync("ORANGE", "CAI001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
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
            settings.Object,
            portalReadRepository.Object);

        var result = await sut.Handle(new GetPortalVisitsQuery { SiteCode = "CAI001" }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].EngineerDisplayName.Should().Be("Field Engineer");
    }

    [Fact]
    public async Task GetPortalVisits_ShouldReturnNotFound_WhenSiteDoesNotBelongToPortalClient()
    {
        var portalUser = User.Create("Portal User", "portal@client.com", "010", UserRole.Manager, Guid.NewGuid());
        portalUser.EnableClientPortalAccess("ORANGE");

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns(portalUser.Id);

        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByIdAsNoTrackingAsync(portalUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(portalUser);

        var settings = new Mock<ISystemSettingsService>();
        var portalReadRepository = new Mock<IPortalReadRepository>();
        portalReadRepository
            .Setup(r => r.SiteExistsForClientAsync("ORANGE", "CAI001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var sut = new GetPortalVisitsQueryHandler(
            currentUser.Object,
            users.Object,
            settings.Object,
            portalReadRepository.Object);

        var result = await sut.Handle(new GetPortalVisitsQuery { SiteCode = "CAI001" }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Site not found.");
        portalReadRepository.Verify(
            r => r.GetVisitsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()),
            Times.Never);
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

    [Fact]
    public async Task GetPortalVisitEvidence_ShouldUseClientScopedRepositoryLookup()
    {
        var portalUser = User.Create("Portal User", "portal@client.com", "010", UserRole.Manager, Guid.NewGuid());
        portalUser.EnableClientPortalAccess("ORANGE");
        var visitId = Guid.NewGuid();

        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.UserId).Returns(portalUser.Id);

        var users = new Mock<IUserRepository>();
        users.Setup(r => r.GetByIdAsNoTrackingAsync(portalUser.Id, It.IsAny<CancellationToken>())).ReturnsAsync(portalUser);

        var portalReadRepository = new Mock<IPortalReadRepository>();
        portalReadRepository
            .Setup(r => r.GetVisitEvidenceAsync("ORANGE", visitId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PortalVisitEvidenceDto
            {
                VisitId = visitId,
                VisitNumber = "V1001",
                SiteCode = "CAI001"
            });

        var sut = new GetPortalVisitEvidenceQueryHandler(
            currentUser.Object,
            users.Object,
            portalReadRepository.Object);

        var result = await sut.Handle(new GetPortalVisitEvidenceQuery { VisitId = visitId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.VisitId.Should().Be(visitId);

        portalReadRepository.Verify(
            r => r.GetVisitEvidenceAsync("ORANGE", visitId, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
