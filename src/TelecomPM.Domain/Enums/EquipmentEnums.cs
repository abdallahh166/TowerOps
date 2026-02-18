namespace TelecomPM.Domain.Enums;

// ==================== Equipment Related ====================
public enum RectifierBrand
{
    Delta = 1,
    Eltek = 2,
    Saft = 3,
    Lorain = 4,
    Ascom = 5,
    Other = 99
}

public enum BatteryType
{
    AGM = 1,
    Lithium = 2,
    VRLA = 3,
    Gel = 4,
    EHPC = 5,
    Marathon = 6
}

public enum BTSVendor
{
    ALU = 1,            // Alcatel-Lucent
    NSN = 2,            // Nokia Siemens Networks
    Huawei = 3,
    Motorola = 4,
    ZTE = 5,
    Ericsson = 6
}

public enum Technology
{
    TwoG = 1,
    ThreeG = 2,
    FourG = 3,
    FiveG = 4
}
