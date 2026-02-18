using TelecomPM.Domain.Common;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.Sites;

// ==================== Site Cooling System ====================
public sealed class SiteCoolingSystem : Entity<Guid>
{
    public Guid SiteId { get; private set; }
    public int ACUnitsCount { get; private set; }
    public bool HasVentilation { get; private set; }
    public bool HasDCACUnit { get; private set; }
    public List<ACUnit> ACUnits { get; private set; } = new();

    private SiteCoolingSystem() : base() { }

    private SiteCoolingSystem(Guid siteId, int acUnitsCount) : base(Guid.NewGuid())
    {
        SiteId = siteId;
        ACUnitsCount = acUnitsCount;
    }

    public static SiteCoolingSystem Create(Guid siteId, int acUnitsCount)
    {
        if (acUnitsCount <= 0)
            throw new DomainException("AC units count must be greater than zero");

        return new SiteCoolingSystem(siteId, acUnitsCount);
    }

    public void AddACUnit(ACUnit unit)
    {
        ACUnits.Add(unit);
    }

    public void SetVentilation(bool hasVentilation)
    {
        HasVentilation = hasVentilation;
    }

    public void SetDCACUnit(bool hasDC)
    {
        HasDCACUnit = hasDC;
    }
}
