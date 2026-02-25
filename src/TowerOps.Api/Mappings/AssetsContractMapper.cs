using TowerOps.Api.Contracts.Assets;
using TowerOps.Application.Commands.Assets.MarkAssetFaulty;
using TowerOps.Application.Commands.Assets.RecordAssetService;
using TowerOps.Application.Commands.Assets.RegisterAsset;
using TowerOps.Application.Commands.Assets.ReplaceAsset;
using TowerOps.Application.Queries.Assets.GetAssetByCode;
using TowerOps.Application.Queries.Assets.GetAssetHistory;
using TowerOps.Application.Queries.Assets.GetExpiringWarranties;
using TowerOps.Application.Queries.Assets.GetFaultyAssets;
using TowerOps.Application.Queries.Assets.GetSiteAssets;

namespace TowerOps.Api.Mappings;

public static class AssetsContractMapper
{
    public static RegisterAssetCommand ToCommand(this RegisterAssetRequest request)
        => new()
        {
            SiteCode = request.SiteCode,
            Type = request.Type,
            Brand = request.Brand,
            Model = request.Model,
            SerialNumber = request.SerialNumber,
            InstalledAtUtc = request.InstalledAtUtc,
            WarrantyExpiresAtUtc = request.WarrantyExpiresAtUtc
        };

    public static RecordAssetServiceCommand ToCommand(this RecordAssetServiceRequest request, string assetCode)
        => new()
        {
            AssetCode = assetCode,
            ServiceType = request.ServiceType,
            EngineerId = request.EngineerId,
            VisitId = request.VisitId,
            Notes = request.Notes
        };

    public static MarkAssetFaultyCommand ToCommand(this MarkAssetFaultyRequest request, string assetCode)
        => new()
        {
            AssetCode = assetCode,
            Reason = request.Reason,
            EngineerId = request.EngineerId
        };

    public static ReplaceAssetCommand ToCommand(this ReplaceAssetRequest request, string assetCode)
        => new()
        {
            AssetCode = assetCode,
            NewAssetId = request.NewAssetId
        };

    public static GetSiteAssetsQuery ToSiteAssetsQuery(this string siteCode)
        => new() { SiteCode = siteCode };

    public static GetAssetByCodeQuery ToAssetByCodeQuery(this string assetCode)
        => new() { AssetCode = assetCode };

    public static GetAssetHistoryQuery ToAssetHistoryQuery(this string assetCode)
        => new() { AssetCode = assetCode };

    public static GetExpiringWarrantiesQuery ToExpiringWarrantiesQuery(this int days)
        => new() { Days = days };

    public static GetFaultyAssetsQuery ToFaultyAssetsQuery(this object _)
        => new();
}
