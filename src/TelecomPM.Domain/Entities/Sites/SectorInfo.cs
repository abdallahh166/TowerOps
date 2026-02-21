using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.Sites;

// ==================== Sector Info ====================
public sealed class SectorInfo
{
    public int SectorNumber { get; private set; }
    public Technology Technology { get; private set; }
    public int Azimuth { get; private set; }
    public decimal HeightAboveBase { get; private set; }
    public string AntennaType { get; private set; } = string.Empty;
    public int? ElectricalTilt { get; private set; }
    public int? MechanicalTilt { get; private set; }
    public string? SectorTechnology { get; private set; }
    public string? RruSolution { get; private set; }
    public string? FeederSize { get; private set; }
    public decimal? FeederLengthM { get; private set; }

    private SectorInfo() { }

    public static SectorInfo Create(
        int sectorNumber,
        Technology technology,
        int azimuth,
        decimal heightAboveBase,
        string antennaType)
    {
        if (azimuth < 0 || azimuth > 360)
            throw new DomainException("Azimuth must be between 0 and 360 degrees");

        return new SectorInfo
        {
            SectorNumber = sectorNumber,
            Technology = technology,
            Azimuth = azimuth,
            HeightAboveBase = heightAboveBase,
            AntennaType = antennaType
        };
    }

    public void SetTilt(int? electrical, int? mechanical)
    {
        ElectricalTilt = electrical;
        MechanicalTilt = mechanical;
    }

    public void SetDeploymentDetails(
        string? sectorTechnology,
        string? rruSolution,
        string? feederSize,
        decimal? feederLengthM,
        string? antennaType = null)
    {
        SectorTechnology = sectorTechnology;
        RruSolution = rruSolution;
        FeederSize = feederSize;
        FeederLengthM = feederLengthM;

        if (!string.IsNullOrWhiteSpace(antennaType))
        {
            AntennaType = antennaType;
        }
    }
}
