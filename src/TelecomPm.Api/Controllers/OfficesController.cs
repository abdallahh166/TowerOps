namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPm.Api.Contracts.Offices;
using TelecomPm.Api.Mappings;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class OfficesController : ApiControllerBase
{
    [HttpPost]
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
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(OfficesContractMapper.ToGetAllQuery(), cancellationToken);
        return HandleResult(result);
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
    public async Task<IActionResult> Update(
        Guid officeId,
        [FromBody] UpdateOfficeRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(officeId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{officeId:guid}/contact")]
    public async Task<IActionResult> UpdateContact(
        Guid officeId,
        [FromBody] UpdateOfficeContactRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(officeId), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{officeId:guid}")]
    public async Task<IActionResult> Delete(
        Guid officeId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(officeId.ToDeleteCommand(), cancellationToken);
        return HandleResult(result);
    }
}
