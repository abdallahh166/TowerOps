namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TelecomPm.Api.Contracts.Sites;
using TelecomPM.Application.Queries.Sites.GetOfficeSites;
using TelecomPM.Application.Queries.Sites.GetSiteById;
using TelecomPM.Application.Queries.Sites.GetSitesNeedingMaintenance;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SitesController : ApiControllerBase
{
    [HttpGet("{siteId:guid}")]
    public async Task<IActionResult> GetById(Guid siteId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetSiteByIdQuery { SiteId = siteId },
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("office/{officeId:guid}")]
    public async Task<IActionResult> GetForOffice(
        Guid officeId,
        [FromQuery] OfficeSitesQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var query = new GetOfficeSitesQuery
        {
            OfficeId = officeId,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            Complexity = parameters.Complexity,
            Status = parameters.Status
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("maintenance")]
    public async Task<IActionResult> GetNeedingMaintenance(
        [FromQuery] MaintenanceSitesQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var query = new GetSitesNeedingMaintenanceQuery
        {
            DaysThreshold = parameters.DaysThreshold,
            OfficeId = parameters.OfficeId
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}

