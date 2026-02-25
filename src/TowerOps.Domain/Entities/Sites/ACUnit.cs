namespace TowerOps.Domain.Entities.Sites;

// ==================== AC Unit ====================
public sealed class ACUnit
{
    public string Type { get; private set; } = string.Empty;
    public int HorsePower { get; private set; }
    public int BTUCapacity { get; private set; }
    public string SerialNumber { get; private set; } = string.Empty;
    public DateTime InstallationDate { get; private set; }

    private ACUnit() { }

    public static ACUnit Create(
        string type,
        int hp,
        int btuCapacity,
        string serialNumber,
        DateTime installationDate)
    {
        return new ACUnit
        {
            Type = type,
            HorsePower = hp,
            BTUCapacity = btuCapacity,
            SerialNumber = serialNumber,
            InstallationDate = installationDate
        };
    }
}
