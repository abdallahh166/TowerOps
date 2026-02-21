namespace TelecomPM.Domain.Entities.Sites;

public sealed class SiteRFStatus
{
    public int TotalRFCount { get; private set; }
    public int RFOnTowerCount { get; private set; }
    public int RFOnGroundCount { get; private set; }
    public int RFSectorCarryCount { get; private set; }
    public string? BandForRFOnTower { get; private set; }
    public string? BandForRFOnGround { get; private set; }
    public string? Notes { get; private set; }

    private SiteRFStatus()
    {
    }

    private SiteRFStatus(
        int totalRfCount,
        int rfOnTowerCount,
        int rfOnGroundCount,
        int rfSectorCarryCount,
        string? bandForRfOnTower,
        string? bandForRfOnGround,
        string? notes)
    {
        TotalRFCount = totalRfCount;
        RFOnTowerCount = rfOnTowerCount;
        RFOnGroundCount = rfOnGroundCount;
        RFSectorCarryCount = rfSectorCarryCount;
        BandForRFOnTower = bandForRfOnTower;
        BandForRFOnGround = bandForRfOnGround;
        Notes = notes;
    }

    public static SiteRFStatus Create(
        int totalRfCount,
        int rfOnTowerCount,
        int rfOnGroundCount,
        int rfSectorCarryCount,
        string? bandForRfOnTower = null,
        string? bandForRfOnGround = null,
        string? notes = null)
    {
        return new SiteRFStatus(
            totalRfCount,
            rfOnTowerCount,
            rfOnGroundCount,
            rfSectorCarryCount,
            bandForRfOnTower,
            bandForRfOnGround,
            notes);
    }
}
