using TelecomPM.Domain.Common;

namespace TelecomPM.Domain.Entities.Sites;

// ==================== Site Fire Safety ====================
public sealed class SiteFireSafety : Entity<Guid>
{
    public Guid SiteId { get; private set; }
    public string FirePanelType { get; private set; } = string.Empty;
    public int HeatSensorsCount { get; private set; }
    public int SmokeSensorsCount { get; private set; }
    public int FlameSensorsCount { get; private set; }
    public List<FireExtinguisher> Extinguishers { get; private set; } = new();

    private SiteFireSafety() : base() { }

    private SiteFireSafety(
        Guid siteId,
        string firePanelType) : base(Guid.NewGuid())
    {
        SiteId = siteId;
        FirePanelType = firePanelType;
    }

    public static SiteFireSafety Create(Guid siteId, string firePanelType)
    {
        return new SiteFireSafety(siteId, firePanelType);
    }

    public void SetSensors(int heat, int smoke, int flame)
    {
        HeatSensorsCount = heat;
        SmokeSensorsCount = smoke;
        FlameSensorsCount = flame;
    }

    public void AddExtinguisher(FireExtinguisher extinguisher)
    {
        Extinguishers.Add(extinguisher);
    }
}
