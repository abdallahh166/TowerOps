using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;

namespace TelecomPM.Domain.Entities.Sites;

// ==================== Site Radio Equipment ====================
public sealed class SiteRadioEquipment : Entity<Guid>
{
    public Guid SiteId { get; private set; }
    
    // Technology availability
    public bool Has2G { get; private set; }
    public bool Has3G { get; private set; }
    public bool Has4G { get; private set; }
    public bool HasSRAN { get; private set; }
    
    // BTS Info
    public BTSVendor? BTSVendor { get; private set; }
    public string? BTSType { get; private set; }
    public int? BTSCount { get; private set; }
    public int? TwoGModulesCount { get; private set; }
    
    // 3G Info
    public BTSVendor? NodeBVendor { get; private set; }
    public string? NodeBType { get; private set; }
    public int? ThreeGRadioModules { get; private set; }
    public int? ThreeGTransmissionModules { get; private set; }
    
    // 4G Info
    public int? FourGModulesCount { get; private set; }
    
    // Sectors
    public int SectorsCount { get; private set; }
    public List<SectorInfo> Sectors { get; private set; } = new();

    private SiteRadioEquipment() : base() { }

    private SiteRadioEquipment(Guid siteId) : base(Guid.NewGuid())
    {
        SiteId = siteId;
    }

    public static SiteRadioEquipment Create(Guid siteId)
    {
        return new SiteRadioEquipment(siteId);
    }

    public void Enable2G(BTSVendor vendor, string type, int btsCount, int modulesCount)
    {
        Has2G = true;
        BTSVendor = vendor;
        BTSType = type;
        BTSCount = btsCount;
        TwoGModulesCount = modulesCount;
    }

    public void Enable3G(BTSVendor vendor, string type, int radioModules, int transmissionModules)
    {
        Has3G = true;
        NodeBVendor = vendor;
        NodeBType = type;
        ThreeGRadioModules = radioModules;
        ThreeGTransmissionModules = transmissionModules;
    }

    public void Enable4G(int modulesCount)
    {
        Has4G = true;
        FourGModulesCount = modulesCount;
    }

    public void SetSectors(List<SectorInfo> sectors)
    {
        Sectors = sectors;
        SectorsCount = sectors.Count;
    }
}
