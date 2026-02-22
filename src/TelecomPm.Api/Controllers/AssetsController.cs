namespace TelecomPm.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPM.Api.Authorization;
using TelecomPm.Api.Contracts.Assets;
using TelecomPm.Api.Mappings;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class AssetsController : ApiControllerBase
{
    [HttpGet("site/{siteCode}")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanViewSites)]
    public async Task<IActionResult> GetBySite(string siteCode, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(siteCode.ToSiteAssetsQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{assetCode}")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanViewSites)]
    public async Task<IActionResult> GetByCode(string assetCode, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(assetCode.ToAssetByCodeQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{assetCode}/history")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanViewSites)]
    public async Task<IActionResult> GetHistory(string assetCode, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(assetCode.ToAssetHistoryQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageSites)]
    public async Task<IActionResult> Register([FromBody] RegisterAssetRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{assetCode}/service")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageSites)]
    public async Task<IActionResult> RecordService(string assetCode, [FromBody] RecordAssetServiceRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(assetCode), cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{assetCode}/fault")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageSites)]
    public async Task<IActionResult> MarkFault(string assetCode, [FromBody] MarkAssetFaultyRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(assetCode), cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{assetCode}/replace")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageSites)]
    public async Task<IActionResult> Replace(string assetCode, [FromBody] ReplaceAssetRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(assetCode), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("expiring-warranties")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanViewSites)]
    public async Task<IActionResult> GetExpiringWarranties([FromQuery] int days = 30, CancellationToken cancellationToken = default)
    {
        var result = await Mediator.Send(days.ToExpiringWarrantiesQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("faulty")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanViewSites)]
    public async Task<IActionResult> GetFaulty(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new object().ToFaultyAssetsQuery(), cancellationToken);
        return HandleResult(result);
    }
}
