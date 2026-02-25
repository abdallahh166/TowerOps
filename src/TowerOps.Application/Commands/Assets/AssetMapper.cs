using TowerOps.Application.DTOs.Assets;
using TowerOps.Domain.Entities.Assets;

namespace TowerOps.Application.Commands.Assets;

internal static class AssetMapper
{
    public static AssetDto ToDto(Asset asset)
    {
        return new AssetDto
        {
            Id = asset.Id,
            AssetCode = asset.AssetCode,
            SiteCode = asset.SiteCode,
            Type = asset.Type,
            Brand = asset.Brand,
            Model = asset.Model,
            SerialNumber = asset.SerialNumber,
            Status = asset.Status,
            InstalledAtUtc = asset.InstalledAtUtc,
            WarrantyExpiresAtUtc = asset.WarrantyExpiresAtUtc,
            LastServicedAtUtc = asset.LastServicedAtUtc,
            ReplacedAtUtc = asset.ReplacedAtUtc,
            ReplacedByAssetId = asset.ReplacedByAssetId,
            ServiceHistory = asset.ServiceHistory
                .OrderByDescending(h => h.ServicedAtUtc)
                .Select(h => new AssetServiceRecordDto
                {
                    Id = h.Id,
                    ServicedAtUtc = h.ServicedAtUtc,
                    ServiceType = h.ServiceType,
                    EngineerId = h.EngineerId,
                    Notes = h.Notes,
                    VisitId = h.VisitId
                })
                .ToList()
        };
    }
}
