using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.Sites;

public sealed class SharedAntennaPosition
{
    public Guid Id { get; private set; }
    public Guid SiteSharingId { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public int Index { get; private set; }
    public decimal Azimuth { get; private set; }
    public decimal HbaMeters { get; private set; }

    private SharedAntennaPosition()
    {
    }

    private SharedAntennaPosition(
        Guid siteSharingId,
        string category,
        int index,
        decimal azimuth,
        decimal hbaMeters)
    {
        Id = Guid.NewGuid();
        SiteSharingId = siteSharingId;
        Category = NormalizeCategory(category);
        Index = index;
        Azimuth = azimuth;
        HbaMeters = hbaMeters;
    }

    public static SharedAntennaPosition Create(
        Guid siteSharingId,
        string category,
        int index,
        decimal azimuth,
        decimal hbaMeters)
    {
        if (siteSharingId == Guid.Empty)
            throw new DomainException("SiteSharingId is required");

        if (index <= 0)
            throw new DomainException("Index must be greater than zero");

        if (azimuth < 0 || azimuth > 360)
            throw new DomainException("Azimuth must be between 0 and 360 degrees");

        if (hbaMeters <= 0)
            throw new DomainException("HBA meters must be greater than zero");

        return new SharedAntennaPosition(siteSharingId, category, index, azimuth, hbaMeters);
    }

    private static string NormalizeCategory(string category)
    {
        if (string.IsNullOrWhiteSpace(category))
            throw new DomainException("Category is required");

        var normalized = category.Trim().ToUpperInvariant();
        return normalized switch
        {
            "RADIO" => "Radio",
            "TX" => "TX",
            _ => throw new DomainException("Category must be either 'Radio' or 'TX'")
        };
    }
}
