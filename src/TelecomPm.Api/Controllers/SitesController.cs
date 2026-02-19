namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TelecomPM.Api.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TelecomPm.Api.Contracts.Sites;
using TelecomPm.Api.Mappings;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ApiAuthorizationPolicies.CanViewSites)]
public sealed class SitesController : ApiControllerBase
{
    [HttpGet("{siteId:guid}")]
    public async Task<IActionResult> GetById(Guid siteId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(siteId.ToSiteByIdQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("office/{officeId:guid}")]
    public async Task<IActionResult> GetForOffice(
        Guid officeId,
        [FromQuery] OfficeSitesQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(parameters.ToOfficeSitesQuery(officeId), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("maintenance")]
    public async Task<IActionResult> GetNeedingMaintenance(
        [FromQuery] MaintenanceSitesQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(parameters.ToMaintenanceQuery(), cancellationToken);
        return HandleResult(result);
    }
}
