using FluentAssertions;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Tests.Entities;

public class SiteExtendedTests
{
    private static Site CreateBasicSite(Guid officeId)
    {
        var coords = Coordinates.Create(30.0, 31.0);
        var address = Address.Create("City", "Street", "12345");
        return Site.Create("TNT001", "Tanta Central", "OMC-1", officeId, "Gharbia", "Tanta", coords, address, SiteType.Macro);
    }

    [Fact]
    public void AssignAndUnassignEngineer_ShouldUpdateStateAndEvents()
    {
        var officeId = Guid.NewGuid();
        var site = CreateBasicSite(officeId);
        var engineerId = Guid.NewGuid();

        site.AssignToEngineer(engineerId);
        site.AssignedEngineerId.Should().Be(engineerId);
        site.DomainEvents.Should().NotBeEmpty();

        site.ClearDomainEvents();
        site.UnassignEngineer();
        site.AssignedEngineerId.Should().BeNull();
    }

    [Fact]
    public void UpdatingComponents_ShouldRecalculateComplexityAndDerivedFields()
    {
        var officeId = Guid.NewGuid();
        var site = CreateBasicSite(officeId);

        var tower = SiteTowerInfo.Create(site.Id, TowerType.GFTower, 45, "TEData");
        tower.UpdateStructure(2, 6);
        site.SetTowerInfo(tower);

        var ps = SitePowerSystem.Create(site.Id, PowerConfiguration.ACOnly, RectifierBrand.Delta, BatteryType.VRLA);
        ps.SetRectifierDetails(4);
        ps.SetBatteryDetails(2, 4, 100, 48);
        ps.SetGenerator("Diesel", "GEN123", 50, 500);
        ps.SetSolarPanel(3000, 10);
        site.SetPowerSystem(ps);

        var radio = SiteRadioEquipment.Create(site.Id);
        radio.Enable2G(BTSVendor.Huawei, "BTS3900", 1, 2);
        radio.Enable3G(BTSVendor.Huawei, "NodeB", 2, 1);
        radio.Enable4G(2);
        site.SetRadioEquipment(radio);

        var cooling = SiteCoolingSystem.Create(site.Id, 2);
        site.SetCoolingSystem(cooling);

        var fire = SiteFireSafety.Create(site.Id, "Honeywell");
        fire.SetSensors(2, 2, 0);
        site.SetFireSafety(fire);

        var sharing = SiteSharing.Create(site.Id);
        sharing.EnableSharing("Vodafone", new List<string> { "Etisalat" });
        sharing.SetSharingDetails(powerShared: true, towerShared: true, hasLock: false);
        site.SetSharingInfo(sharing);

        site.Complexity.Should().BeOneOf(SiteComplexity.Medium, SiteComplexity.High);
        site.EstimatedVisitDurationMinutes.Should().BeGreaterThan(60);
        site.RequiredPhotosCount.Should().BeGreaterThan(20);
    }

    [Fact]
    public void UpdateStatus_ShouldChangeStatus()
    {
        var site = CreateBasicSite(Guid.NewGuid());
        site.UpdateStatus(SiteStatus.OffAir);
        site.Status.Should().Be(SiteStatus.OffAir);
    }

    [Fact]
    public void SetCoolingSystem_ShouldUpdateDerivedMetrics()
    {
        var site = CreateBasicSite(Guid.NewGuid());
        var initialPhotos = site.RequiredPhotosCount;
        var initialDuration = site.EstimatedVisitDurationMinutes;

        var cooling = SiteCoolingSystem.Create(site.Id, 2);
        site.SetCoolingSystem(cooling);

        site.RequiredPhotosCount.Should().BeGreaterThan(initialPhotos);
        site.EstimatedVisitDurationMinutes.Should().BeGreaterThan(initialDuration);
    }

    [Fact]
    public void SetOwnership_IndependentTower_ShouldForceEquipmentOnlyScope()
    {
        var site = CreateBasicSite(Guid.NewGuid());

        site.SetOwnership(
            TowerOwnershipType.IndependentTower,
            "IHS Towers",
            "AGR-IND-1",
            "Host Contact",
            "+201111111111");

        site.TowerOwnershipType.Should().Be(TowerOwnershipType.IndependentTower);
        site.ResponsibilityScope.Should().Be(ResponsibilityScope.EquipmentOnly);
    }

    [Fact]
    public void SetOwnership_Host_ShouldForceFullScope()
    {
        var site = CreateBasicSite(Guid.NewGuid());

        site.SetOwnership(
            TowerOwnershipType.Host,
            "Orange Egypt",
            "AGR-HOST-1",
            "Host Contact",
            "+201111111111");

        site.TowerOwnershipType.Should().Be(TowerOwnershipType.Host);
        site.ResponsibilityScope.Should().Be(ResponsibilityScope.Full);
    }
}


