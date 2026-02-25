namespace TowerOps.Domain.Enums;

public enum AssetType
{
    Rectifier = 1,
    Battery = 2,
    ACUnit = 3,
    Generator = 4,
    BTS = 5,
    Antenna = 6,
    ODU = 7,
    Router = 8
}

public enum AssetStatus
{
    Active = 1,
    Faulty = 2,
    Replaced = 3,
    Decommissioned = 4
}
