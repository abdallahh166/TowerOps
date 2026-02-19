using FluentAssertions;
using TelecomPM.Application.Services;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Tests.Services;

public class PhotoChecklistGeneratorServiceTests
{
    private static Site CreateSiteWithComponents(bool gen, bool solar, int acCount, bool twoG, bool threeG, bool fourG, bool shared)
    {
        var site = Site.Create(
            "TNT001",
            "Site1",
            "OMC",
            Guid.NewGuid(),
            "Cairo",
            "Nasr City",
            Coordinates.Create(30, 31),
            Address.Create("Street", "Cairo", "Cairo"),
            SiteType.Macro);
        var tower = SiteTowerInfo.Create(Guid.Empty, TowerType.GFTower, 45, "TEData");
        tower.UpdateStructure(1, 4);
        typeof(Site).GetProperty("TowerInfo")!.SetValue(site, tower);
        var ps = SitePowerSystem.Create(Guid.NewGuid(), PowerConfiguration.ACOnly, RectifierBrand.Delta, BatteryType.VRLA);
        if (gen) ps.SetGenerator("Diesel", "G1", 30, 300);
        if (solar) ps.SetSolarPanel(3000, 10);
        typeof(Site).GetProperty("PowerSystem")!.SetValue(site, ps);
        typeof(Site).GetProperty("CoolingSystem")!.SetValue(site, SiteCoolingSystem.Create(Guid.Empty, acCount));
        var radio = SiteRadioEquipment.Create(Guid.Empty);
        if (twoG) radio.Enable2G(BTSVendor.Huawei, "BTS", 1, 2);
        if (threeG) radio.Enable3G(BTSVendor.Huawei, "NodeB", 2, 1);
        if (fourG) radio.Enable4G(2);
        typeof(Site).GetProperty("RadioEquipment")!.SetValue(site, radio);
        typeof(Site).GetProperty("FireSafety")!.SetValue(site, SiteFireSafety.Create(Guid.Empty, "Honeywell"));
        var sharing = SiteSharing.Create(Guid.Empty);
        if (shared) sharing.EnableSharing("Vodafone", new List<string>());
        typeof(Site).GetProperty("SharingInfo")!.SetValue(site, sharing);
        return site;
    }

    [Fact]
    public void GenerateChecklist_ShouldIncreaseWithComponents()
    {
        var svc = new PhotoChecklistGeneratorService();
        var baseSite = CreateSiteWithComponents(false, false, 1, false, false, false, false);
        var enrichedSite = CreateSiteWithComponents(true, true, 2, true, true, true, true);

        var baseList = svc.GenerateChecklistForSite(baseSite);
        var richList = svc.GenerateChecklistForSite(enrichedSite);

        richList.Count.Should().BeGreaterThan(baseList.Count);
    }
}
