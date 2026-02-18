using TelecomPM.Domain.Common;

namespace TelecomPM.Domain.Entities.Sites;

// ==================== Site Sharing ====================
public sealed class SiteSharing : Entity<Guid>
{
    public Guid SiteId { get; private set; }
    public bool IsShared { get; private set; }
    public string? HostOperator { get; private set; }
    public List<string> GuestOperators { get; private set; } = new();
    public bool PowerShared { get; private set; }
    public bool TowerShared { get; private set; }
    public bool HasSharingLock { get; private set; }

    private SiteSharing() : base() { }

    private SiteSharing(Guid siteId) : base(Guid.NewGuid())
    {
        SiteId = siteId;
        IsShared = false;
    }

    public static SiteSharing Create(Guid siteId)
    {
        return new SiteSharing(siteId);
    }

    public void EnableSharing(string hostOperator, List<string> guestOperators)
    {
        IsShared = true;
        HostOperator = hostOperator;
        GuestOperators = guestOperators;
    }

    public void SetSharingDetails(bool powerShared, bool towerShared, bool hasLock)
    {
        PowerShared = powerShared;
        TowerShared = towerShared;
        HasSharingLock = hasLock;
    }
}
