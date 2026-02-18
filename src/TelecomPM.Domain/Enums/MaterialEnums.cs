namespace TelecomPM.Domain.Enums;

// ==================== Material Related ====================
public enum MaterialCategory
{
    Cable = 1,
    Electrical = 2,
    Cooling = 3,
    Power = 4,
    Transmission = 5,
    Safety = 6,
    Cleaning = 7,
    Tools = 8,
    Other = 99
}

public enum MaterialUnit
{
    Pieces = 1,
    Meters = 2,
    Kilograms = 3,
    Liters = 4,
    Set = 5,
    Box = 6
}

public enum MaterialUsageStatus
{
    Logged = 1,
    Submitted = 2,
    Approved = 3,
    Rejected = 4
}
