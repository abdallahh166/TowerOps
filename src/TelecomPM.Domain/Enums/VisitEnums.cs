namespace TelecomPM.Domain.Enums;

// ==================== Visit Related ====================
public enum VisitStatus
{
    Scheduled = 1,
    InProgress = 2,
    Completed = 3,
    Submitted = 4,
    UnderReview = 5,
    NeedsCorrection = 6,
    Approved = 7,
    Rejected = 8,
    Cancelled = 9
}

public enum VisitType
{
    PreventiveMaintenance = 1,
    CorrectiveMaintenance = 2,
    Emergency = 3,
    Installation = 4,
    Upgrade = 5,
    Inspection = 6,
    Commissioning = 7,
    BM = 10,
    CM = 11,
    Audit = 12
}

public enum CheckStatus
{
    OK = 1,
    NOK = 2,
    NA = 3
}

public enum PhotoType
{
    Before = 1,
    After = 2,
    During = 3,
    Material = 4,
    Issue = 5
}

public enum PhotoCategory
{
    // Shelter
    ShelterInside = 1,
    ShelterOutside = 2,
    
    // Tower
    Tower = 10,
    Fence = 11,
    
    // Power System
    GEDP = 20,
    Rectifier = 21,
    Batteries = 22,
    PowerMeter = 23,
    SurgeArrestor = 24,
    
    // Cooling
    AirConditioner = 30,
    
    // Fire & Safety
    FirePanel = 40,
    FireExtinguisher = 41,
    
    // Radio Equipment
    BTS = 50,
    NodeB = 51,
    MW = 52,
    DDF = 53,
    
    // Earth System
    EarthBar = 60,
    EarthRod = 61,
    
    // Cable Management
    CableTray = 70,
    Roxtec = 71,
    
    // Generator
    Generator = 80,
    
    // Others
    PMLogbook = 90,
    Other = 99
}
