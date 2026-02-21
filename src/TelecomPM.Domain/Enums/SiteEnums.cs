namespace TelecomPM.Domain.Enums;

// ==================== Site Related ====================
public enum SiteType
{
    Macro = 1,
    Nodal = 2,
    BSC = 3,
    VIP = 4,
    Outdoor = 5,
    Indoor = 6,
    Repeater = 7,
    MicroNano = 8,
    BTS = 10,
    BSCExtended = 11,
    GreenField = 12,
    RoofTop = 13,
    IndoorExtended = 14
}

public enum SiteComplexity
{
    Low = 1,
    Medium = 2,
    High = 3
}

public enum SiteStatus
{
    OnAir = 1,
    OffAir = 2,
    UnderMaintenance = 3,
    Decommissioned = 4
}

public enum TowerType
{
    GFTower = 1,        // Ground Floor Tower
    RTTower = 2,        // Roof Top Tower
    GFMonopole = 3,
    RTMonopole = 4,
    PalmTree = 5,
    QuickSite = 6,
    Mast = 7,
    Billboard = 8,
    FlagPole = 9,
    MobileStation = 10,
    SpecialCamouflage = 11
}

public enum TransmissionType
{
    MW = 1,             // Microwave
    Fiber = 2,
    EBand = 3,
    Hybrid = 4
}

public enum PowerConfiguration
{
    ACOnly = 1,
    DCOnly = 2,
    Hybrid = 3,
    Solar = 4,
    Generator = 5,
    SolarHybrid = 6
}
