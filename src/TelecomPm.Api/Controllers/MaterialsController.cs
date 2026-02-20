namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TelecomPM.Api.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPm.Api.Contracts.Materials;
using TelecomPm.Api.Mappings;
using TelecomPM.Application.Common.Interfaces;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ApiAuthorizationPolicies.CanViewMaterials)]
public sealed class MaterialsController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public MaterialsController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageMaterials)]
    public async Task<IActionResult> Create([FromBody] CreateMaterialRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
        }

        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageMaterials)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMaterialRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageMaterials)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(id.ToDeleteMaterialCommand(ResolveActor()), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/stock/add")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageMaterials)]
    public async Task<IActionResult> AddStock(Guid id, [FromBody] AddStockRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(id, ResolveActor()), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/stock/reserve")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageMaterials)]
    public async Task<IActionResult> ReserveStock(Guid id, [FromBody] ReserveStockRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/stock/consume")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageMaterials)]
    public async Task<IActionResult> ConsumeStock(Guid id, [FromBody] ConsumeStockRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(id, ResolveActor()), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(id.ToMaterialByIdQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid officeId, [FromQuery] bool? onlyInStock, CancellationToken cancellationToken)
    {
        var effectiveOfficeId = officeId != Guid.Empty ? officeId : _currentUserService.OfficeId;
        if (effectiveOfficeId == Guid.Empty)
        {
            return BadRequest("officeId is required.");
        }

        var result = await Mediator.Send(effectiveOfficeId.ToQuery(onlyInStock), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("low-stock/{officeId:guid}")]
    public async Task<IActionResult> GetLowStockMaterials(Guid officeId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(officeId.ToLowStockQuery(), cancellationToken);
        return HandleResult(result);
    }

    private string ResolveActor()
    {
        if (_currentUserService.IsAuthenticated && _currentUserService.UserId != Guid.Empty)
        {
            return _currentUserService.UserId.ToString();
        }

        return "System";
    }
}
