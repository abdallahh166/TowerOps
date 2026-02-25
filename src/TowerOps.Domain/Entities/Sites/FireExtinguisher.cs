namespace TowerOps.Domain.Entities.Sites;

// ==================== Fire Extinguisher ====================
public sealed class FireExtinguisher
{
    public string Type { get; private set; } = string.Empty; // CO2, Powder
    public string Brand { get; private set; } = string.Empty;
    public int CapacityKG { get; private set; }
    public string SerialNumber { get; private set; } = string.Empty;
    public DateTime MaintenanceDate { get; private set; }
    public DateTime NextMaintenanceDue { get; private set; }

    private FireExtinguisher() { }

    public static FireExtinguisher Create(
        string type,
        string brand,
        int capacity,
        string serialNumber,
        DateTime maintenanceDate)
    {
        return new FireExtinguisher
        {
            Type = type,
            Brand = brand,
            CapacityKG = capacity,
            SerialNumber = serialNumber,
            MaintenanceDate = maintenanceDate,
            NextMaintenanceDue = maintenanceDate.AddYears(1)
        };
    }

    public bool IsMaintenanceDue()
    {
        return DateTime.UtcNow >= NextMaintenanceDue;
    }
}
