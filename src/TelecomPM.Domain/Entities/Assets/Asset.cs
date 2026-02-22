using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Events.AssetEvents;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.Assets;

public sealed class Asset : AggregateRoot<Guid>
{
    private readonly List<AssetServiceRecord> _serviceHistory = new();

    public string AssetCode { get; private set; } = string.Empty;
    public string SiteCode { get; private set; } = string.Empty;
    public AssetType Type { get; private set; }
    public string? Brand { get; private set; }
    public string? Model { get; private set; }
    public string? SerialNumber { get; private set; }
    public AssetStatus Status { get; private set; }
    public DateTime InstalledAtUtc { get; private set; }
    public DateTime? WarrantyExpiresAtUtc { get; private set; }
    public DateTime? LastServicedAtUtc { get; private set; }
    public DateTime? ReplacedAtUtc { get; private set; }
    public Guid? ReplacedByAssetId { get; private set; }
    public IReadOnlyCollection<AssetServiceRecord> ServiceHistory => _serviceHistory.AsReadOnly();

    private Asset() : base()
    {
    }

    private Asset(
        string siteCode,
        AssetType type,
        string? brand,
        string? model,
        string? serialNumber,
        DateTime installedAtUtc,
        DateTime? warrantyExpiresAtUtc) : base(Guid.NewGuid())
    {
        SiteCode = siteCode;
        Type = type;
        Brand = brand;
        Model = model;
        SerialNumber = serialNumber;
        InstalledAtUtc = DateTime.SpecifyKind(installedAtUtc, DateTimeKind.Utc);
        WarrantyExpiresAtUtc = warrantyExpiresAtUtc.HasValue
            ? DateTime.SpecifyKind(warrantyExpiresAtUtc.Value, DateTimeKind.Utc)
            : null;
        Status = AssetStatus.Active;
        AssetCode = GenerateAssetCode(siteCode, type);
    }

    public static Asset Create(
        string siteCode,
        AssetType type,
        string? brand,
        string? model,
        string? serialNumber,
        DateTime installedAtUtc,
        DateTime? warrantyExpiresAtUtc)
    {
        if (string.IsNullOrWhiteSpace(siteCode))
            throw new DomainException("SiteCode is required.");

        return new Asset(
            siteCode.Trim().ToUpperInvariant(),
            type,
            brand,
            model,
            serialNumber,
            installedAtUtc,
            warrantyExpiresAtUtc);
    }

    public void RecordService(string serviceType, string? engineerId, Guid? visitId, string? notes)
    {
        if (string.IsNullOrWhiteSpace(serviceType))
            throw new DomainException("Service type is required.");

        var record = AssetServiceRecord.Create(serviceType, engineerId, visitId, notes);
        _serviceHistory.Add(record);
        LastServicedAtUtc = record.ServicedAtUtc;
        MarkAsUpdated(engineerId ?? "System");
    }

    public void MarkFaulty(string? reason, string? engineerId)
    {
        Status = AssetStatus.Faulty;
        MarkAsUpdated(engineerId ?? "System");
        AddDomainEvent(new AssetFaultedEvent(Id, AssetCode, SiteCode, reason, engineerId));
    }

    public void Replace(Guid newAssetId)
    {
        if (newAssetId == Guid.Empty)
            throw new DomainException("New asset id is required.");

        Status = AssetStatus.Replaced;
        ReplacedAtUtc = DateTime.UtcNow;
        ReplacedByAssetId = newAssetId;
        MarkAsUpdated("System");
    }

    public bool IsWarrantyExpired()
    {
        return WarrantyExpiresAtUtc.HasValue && WarrantyExpiresAtUtc.Value < DateTime.UtcNow;
    }

    private static string GenerateAssetCode(string siteCode, AssetType type)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
        return $"AST-{siteCode}-{type.ToString().ToUpperInvariant()}-{suffix}";
    }
}

public sealed class AssetServiceRecord
{
    public Guid Id { get; private set; }
    public DateTime ServicedAtUtc { get; private set; }
    public string ServiceType { get; private set; } = string.Empty;
    public string? EngineerId { get; private set; }
    public string? Notes { get; private set; }
    public Guid? VisitId { get; private set; }

    private AssetServiceRecord()
    {
        Id = Guid.NewGuid();
    }

    private AssetServiceRecord(
        string serviceType,
        string? engineerId,
        Guid? visitId,
        string? notes)
    {
        Id = Guid.NewGuid();
        ServicedAtUtc = DateTime.UtcNow;
        ServiceType = serviceType;
        EngineerId = engineerId;
        VisitId = visitId;
        Notes = notes;
    }

    public static AssetServiceRecord Create(
        string serviceType,
        string? engineerId,
        Guid? visitId,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(serviceType))
            throw new DomainException("Service type is required.");

        return new AssetServiceRecord(serviceType.Trim(), engineerId, visitId, notes);
    }
}
