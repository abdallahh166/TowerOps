using AutoMapper;
using FluentAssertions;
using Moq;
using TelecomPM.Application.Commands.Sites.UpdateSiteOwnership;
using TelecomPM.Application.DTOs.Sites;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Application.Tests.Commands.Sites;

public class UpdateSiteOwnershipCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldPersistOwnershipUpdates()
    {
        var site = Site.Create(
            "TNT001",
            "Site 1",
            "OMC-1",
            Guid.NewGuid(),
            "Region",
            "SubRegion",
            Coordinates.Create(30.0, 31.0),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.GetBySiteCodeAsync("TNT001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(site);

        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
        mapper.Setup(m => m.Map<SiteDetailDto>(site)).Returns(new SiteDetailDto
        {
            SiteCode = site.SiteCode.Value,
            TowerOwnershipType = site.TowerOwnershipType,
            ResponsibilityScope = site.ResponsibilityScope,
            TowerOwnerName = site.TowerOwnerName,
            HostContactName = site.HostContactName,
            HostContactPhone = site.HostContactPhone
        });

        var handler = new UpdateSiteOwnershipCommandHandler(
            siteRepository.Object,
            unitOfWork.Object,
            mapper.Object);

        var result = await handler.Handle(new UpdateSiteOwnershipCommand
        {
            SiteCode = "TNT001",
            TowerOwnershipType = TowerOwnershipType.IndependentTower,
            TowerOwnerName = "IHS Towers",
            SharingAgreementRef = "AGR-22",
            HostContactName = "Host Contact",
            HostContactPhone = "+201111111111"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        site.TowerOwnershipType.Should().Be(TowerOwnershipType.IndependentTower);
        site.ResponsibilityScope.Should().Be(ResponsibilityScope.EquipmentOnly);
        site.TowerOwnerName.Should().Be("IHS Towers");
        site.HostContactName.Should().Be("Host Contact");
        site.HostContactPhone.Should().Be("+201111111111");

        siteRepository.Verify(r => r.UpdateAsync(site, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
