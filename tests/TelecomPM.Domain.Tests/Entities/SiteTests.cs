using FluentAssertions;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Domain.Tests.Entities;

public class SiteTests
{
    [Fact]
    public void Create_WithMinimalValidFields_ShouldInitializeDefaults()
    {
        var site = Site.Create(
            siteCode: "TNT001",
            name: "Tanta Central",
            omcName: "TNT-C",
            officeId: Guid.NewGuid(),
            region: "Delta",
            subRegion: "Gharbia",
            coordinates: Coordinates.Create(30.7865, 30.9925),
            address: Address.Create("Street 1", "Tanta", "Gharbia"),
            siteType: SiteType.Macro
        );

        site.Status.Should().Be(SiteStatus.OnAir);
        site.Complexity.Should().Be(SiteComplexity.Low);
        site.CanBeVisited().Should().BeTrue();
    }

    [Fact]
    public void RecalculateComplexity_WithMultipleSystems_ShouldIncreaseComplexity()
    {
        var site = BuildBasicSite();
        var radio = SiteRadioEquipment.Create(site.Id);
        radio.Enable2G(BTSVendor.Huawei, "BTS3900", 1, 2);
        radio.Enable3G(BTSVendor.Huawei, "NodeB3900", 2, 1);
        radio.Enable4G(2);
        site.SetRadioEquipment(radio);

        var power = SitePowerSystem.Create(site.Id, PowerConfiguration.Hybrid, RectifierBrand.Delta, BatteryType.AGM);
        power.SetRectifierDetails(3);
        power.SetBatteryDetails(2, 8, 200, 48);
        power.SetGenerator("Diesel", "GEN123", 30, 500);
        site.SetPowerSystem(power);

        site.Complexity.Should().NotBe(SiteComplexity.Low);
        site.EstimatedVisitDurationMinutes.Should().BeGreaterThan(60);
        site.RequiredPhotosCount.Should().BeGreaterThan(20);
    }

    private Site BuildBasicSite()
    {
        return Site.Create(
            siteCode: "TNT001",
            name: "Tanta Central",
            omcName: "TNT-C",
            officeId: Guid.NewGuid(),
            region: "Delta",
            subRegion: "Gharbia",
            coordinates: Coordinates.Create(30.7865, 30.9925),
            address: Address.Create("Street 1", "Tanta", "Gharbia"),
            siteType: SiteType.Macro
        );
    }
}
