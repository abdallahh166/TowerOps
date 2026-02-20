namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPM.Api.Authorization;
using TelecomPM.Application.Common.Interfaces;
using TelecomPm.Api.Contracts.Sites;
using TelecomPm.Api.Mappings;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ApiAuthorizationPolicies.CanViewSites)]
public sealed class SitesController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public SitesController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageSites)]
    public async Task<IActionResult> Create([FromBody] CreateSiteRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetById), new { siteId = result.Value.Id }, result.Value);
        }

        return HandleResult(result);
    }

    [HttpPut("{siteId:guid}")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageSites)]
    public async Task<IActionResult> Update(Guid siteId, [FromBody] UpdateSiteRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name) ||
            string.IsNullOrWhiteSpace(request.OMCName) ||
            !request.SiteType.HasValue)
        {
            return BadRequest("Name, OMCName, and SiteType are required.");
        }

        var result = await Mediator.Send(request.ToCommand(siteId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{siteId:guid}/status")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageSites)]
    public async Task<IActionResult> UpdateStatus(
        Guid siteId,
        [FromBody] UpdateSiteStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(siteId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{siteId:guid}/assign")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageSites)]
    public async Task<IActionResult> AssignEngineer(
        Guid siteId,
        [FromBody] AssignEngineerRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await Mediator.Send(request.ToCommand(siteId, _currentUserService.UserId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{siteId:guid}/unassign")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageSites)]
    public async Task<IActionResult> UnassignEngineer(Guid siteId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(siteId.ToUnassignCommand(), cancellationToken);
        return HandleResult(result);
    }

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
