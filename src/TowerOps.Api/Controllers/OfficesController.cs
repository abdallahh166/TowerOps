namespace TowerOps.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TowerOps.Api.Authorization;
using Microsoft.AspNetCore.Mvc;
using TowerOps.Api.Contracts.Offices;
using TowerOps.Api.Mappings;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ApiAuthorizationPolicies.CanManageOffices)]
public sealed class OfficesController : ApiControllerBase
{
    [HttpPost]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageOffices)]
    public async Task<IActionResult> Create(
        [FromBody] CreateOfficeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(
                nameof(GetById),
                new { officeId = result.Value.Id },
                result.Value);
        }

        return HandleResult(result);
    }

    [HttpGet("{officeId:guid}")]
    public async Task<IActionResult> GetById(
        Guid officeId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(officeId.ToOfficeByIdQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] bool? onlyActive,
        [FromQuery(Name = "page")] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDir = "desc",
        CancellationToken cancellationToken = default)
    {
        var safePage = page < 1 ? 1 : page;
        var safePageSize = Math.Clamp(pageSize, 1, 100);

        if (!TryResolveSort(
                sortBy,
                sortDir,
                new[] { "code", "name", "region", "isActive", "createdAt" },
                defaultSortBy: "code",
                out var resolvedSortBy,
                out var sortDescending,
                out var sortError))
        {
            return sortError!;
        }

        var result = await Mediator.Send(
            OfficesContractMapper.ToGetAllQuery(onlyActive, safePage, safePageSize, resolvedSortBy, sortDescending),
            cancellationToken);

        if (!result.IsSuccess || result.Value is null)
            return HandleResult(result);

        return Ok(result.Value.ToPagedResponse());
    }

    [HttpGet("region/{region}")]
    public async Task<IActionResult> GetByRegion(
        string region,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(region.ToRegionQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{officeId:guid}/statistics")]
    public async Task<IActionResult> GetStatistics(
        Guid officeId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(officeId.ToOfficeStatisticsQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{officeId:guid}")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageOffices)]
    public async Task<IActionResult> Update(
        Guid officeId,
        [FromBody] UpdateOfficeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(officeId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{officeId:guid}/contact")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageOffices)]
    public async Task<IActionResult> UpdateContact(
        Guid officeId,
        [FromBody] UpdateOfficeContactRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(officeId), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{officeId:guid}")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageOffices)]
    public async Task<IActionResult> Delete(
        Guid officeId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(officeId.ToDeleteCommand(), cancellationToken);
        return HandleResult(result);
    }
}
