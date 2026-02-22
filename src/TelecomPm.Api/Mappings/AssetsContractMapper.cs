using TelecomPm.Api.Contracts.Assets;
using TelecomPM.Application.Commands.Assets.MarkAssetFaulty;
using TelecomPM.Application.Commands.Assets.RecordAssetService;
using TelecomPM.Application.Commands.Assets.RegisterAsset;
using TelecomPM.Application.Commands.Assets.ReplaceAsset;
using TelecomPM.Application.Queries.Assets.GetAssetByCode;
using TelecomPM.Application.Queries.Assets.GetAssetHistory;
using TelecomPM.Application.Queries.Assets.GetExpiringWarranties;
using TelecomPM.Application.Queries.Assets.GetFaultyAssets;
using TelecomPM.Application.Queries.Assets.GetSiteAssets;

namespace TelecomPm.Api.Mappings;

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
