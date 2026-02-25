using TowerOps.Domain.Enums;

namespace TowerOps.Application.DTOs.Assets;

public sealed record AssetDto
{
    public Guid Id { get; init; }
    public string AssetCode { get; init; } = string.Empty;
    public string SiteCode { get; init; } = string.Empty;
    public AssetType Type { get; init; }
    public string? Brand { get; init; }
    public string? Model { get; init; }
    public string? SerialNumber { get; init; }
    public AssetStatus Status { get; init; }
    public DateTime InstalledAtUtc { get; init; }
    public DateTime? WarrantyExpiresAtUtc { get; init; }
    public DateTime? LastServicedAtUtc { get; init; }
    public DateTime? ReplacedAtUtc { get; init; }
    public Guid? ReplacedByAssetId { get; init; }
    public IReadOnlyList<AssetServiceRecordDto> ServiceHistory { get; init; } = Array.Empty<AssetServiceRecordDto>();
}

public sealed record AssetServiceRecordDto
{
    public Guid Id { get; init; }
    public DateTime ServicedAtUtc { get; init; }
    public string ServiceType { get; init; } = string.Empty;
    public string? EngineerId { get; init; }
    public string? Notes { get; init; }
    public Guid? VisitId { get; init; }
}
