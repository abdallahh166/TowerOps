using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.Sites;

// ==================== Site Tower Info ====================
public sealed class SiteTowerInfo : Entity<Guid>
{
    public Guid SiteId { get; private set; }
    public TowerType Type { get; private set; }
    public int Height { get; private set; }
    public string Owner { get; private set; } = string.Empty;
    public int NumberOfMasts { get; private set; }
    public int WiresPerMast { get; private set; }
    public bool NeedsRepair { get; private set; }
    public bool WiresNeedRepair { get; private set; }

    private SiteTowerInfo() : base() { }

    private SiteTowerInfo(
        Guid siteId,
        TowerType type,
        int height,
        string owner) : base(Guid.NewGuid())
    {
        SiteId = siteId;
        Type = type;
        Height = height;
        Owner = owner;
    }

    public static SiteTowerInfo Create(
        Guid siteId,
        TowerType type,
        int height,
        string owner)
    {
        if (height <= 0)
            throw new DomainException("Tower height must be greater than zero");

        return new SiteTowerInfo(siteId, type, height, owner);
    }

    public void UpdateStructure(int numberOfMasts, int wiresPerMast)
    {
        NumberOfMasts = numberOfMasts;
        WiresPerMast = wiresPerMast;
    }

    public void MarkForRepair(bool towerRepair, bool wiresRepair)
    {
        NeedsRepair = towerRepair;
        WiresNeedRepair = wiresRepair;
    }
}
