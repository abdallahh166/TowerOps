using TelecomPM.Domain.Common;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.Sites;

// ==================== Site Sharing ====================
public sealed class SiteSharing : Entity<Guid>
{
    public Guid SiteId { get; private set; }
    public bool IsShared { get; private set; }
    public string? HostOperator { get; private set; }
    public List<string> GuestOperators { get; private set; } = new();
    public string? HostSiteCode { get; private set; }
    public int SharingRadioAntennaCount { get; private set; }
    public int SharingTxAntennaCount { get; private set; }
    public string? TxEnclosureType { get; private set; }
    public IReadOnlyList<SharedAntennaPosition> AntennaPositions { get; private set; } = new List<SharedAntennaPosition>();
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

    public void SetHostSiteCode(string? hostSiteCode)
    {
        HostSiteCode = string.IsNullOrWhiteSpace(hostSiteCode) ? null : hostSiteCode.Trim();
    }

    public void SetTxEnclosureType(string? txEnclosureType)
    {
        TxEnclosureType = string.IsNullOrWhiteSpace(txEnclosureType) ? null : txEnclosureType.Trim();
    }

    public void SetSharingDetails(bool powerShared, bool towerShared, bool hasLock)
    {
        PowerShared = powerShared;
        TowerShared = towerShared;
        HasSharingLock = hasLock;
    }

    public void AddAntennaPosition(SharedAntennaPosition position)
    {
        var positions = EnsureMutablePositions();

        if (position.Category == "Radio" && SharingRadioAntennaCount >= 8)
            throw new DomainException("Maximum 8 radio antenna positions are allowed");

        if (position.Category == "TX" && SharingTxAntennaCount >= 9)
            throw new DomainException("Maximum 9 TX antenna positions are allowed");

        if (positions.Any(p => p.Category == position.Category && p.Index == position.Index))
            throw new DomainException("Antenna position index already exists for this category");

        positions.Add(position);
        RecalculateAntennaCounts(positions);
    }

    public void SetAntennaPositions(IEnumerable<SharedAntennaPosition> positions)
    {
        var newPositions = positions?.ToList() ?? new List<SharedAntennaPosition>();

        var radioCount = newPositions.Count(p => p.Category == "Radio");
        var txCount = newPositions.Count(p => p.Category == "TX");

        if (radioCount > 8)
            throw new DomainException("Maximum 8 radio antenna positions are allowed");

        if (txCount > 9)
            throw new DomainException("Maximum 9 TX antenna positions are allowed");

        AntennaPositions = newPositions;
        SharingRadioAntennaCount = radioCount;
        SharingTxAntennaCount = txCount;
    }

    private List<SharedAntennaPosition> EnsureMutablePositions()
    {
        if (AntennaPositions is List<SharedAntennaPosition> positions)
            return positions;

        var mutable = AntennaPositions.ToList();
        AntennaPositions = mutable;
        return mutable;
    }

    private void RecalculateAntennaCounts(IEnumerable<SharedAntennaPosition> positions)
    {
        SharingRadioAntennaCount = positions.Count(p => p.Category == "Radio");
        SharingTxAntennaCount = positions.Count(p => p.Category == "TX");
    }
}
