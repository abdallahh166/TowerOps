using TelecomPM.Domain.Common;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.UnusedAssets;

public sealed class UnusedAsset : AggregateRoot<Guid>
{
    public Guid SiteId { get; private set; }
    public Guid? VisitId { get; private set; }
    public string AssetName { get; private set; } = string.Empty;
    public decimal Quantity { get; private set; }
    public string? Unit { get; private set; }
    public string? Notes { get; private set; }
    public DateTime RecordedAtUtc { get; private set; }

    private UnusedAsset() : base()
    {
    }

    private UnusedAsset(
        Guid siteId,
        Guid? visitId,
        string assetName,
        decimal quantity,
        string? unit,
        string? notes,
        DateTime recordedAtUtc) : base(Guid.NewGuid())
    {
        SiteId = siteId;
        VisitId = visitId;
        AssetName = assetName;
        Quantity = quantity;
        Unit = unit;
        Notes = notes;
        RecordedAtUtc = EnsureUtc(recordedAtUtc);
    }

    public static UnusedAsset Create(
        Guid siteId,
        Guid? visitId,
        string assetName,
        decimal quantity,
        string? unit,
        DateTime recordedAtUtc,
        string? notes = null)
    {
        if (siteId == Guid.Empty)
            throw new DomainException("SiteId is required.");

        if (string.IsNullOrWhiteSpace(assetName))
            throw new DomainException("Asset name is required.");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        return new UnusedAsset(
            siteId,
            visitId,
            assetName.Trim(),
            quantity,
            string.IsNullOrWhiteSpace(unit) ? null : unit.Trim(),
            string.IsNullOrWhiteSpace(notes) ? null : notes.Trim(),
            recordedAtUtc);
    }

    public void UpdateQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        Quantity = quantity;
    }

    public void UpdateNotes(string? notes)
    {
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
