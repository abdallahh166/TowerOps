namespace TelecomPm.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPM.Api.Authorization;
using TelecomPm.Api.Contracts.WorkOrders;
using TelecomPm.Api.Mappings;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class WorkOrdersController : ApiControllerBase
{
    [HttpPost]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageWorkOrders)]
    public async Task<IActionResult> Create([FromBody] CreateWorkOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetById), new { workOrderId = result.Value.Id }, result.Value);
        }

        return HandleResult(result);
    }

    [HttpGet("{workOrderId:guid}")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanViewWorkOrders)]
    public async Task<IActionResult> GetById(Guid workOrderId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(workOrderId.ToWorkOrderByIdQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{workOrderId:guid}/assign")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageWorkOrders)]
    public async Task<IActionResult> Assign(Guid workOrderId, [FromBody] AssignWorkOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(workOrderId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/start")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageWorkOrders)]
    public async Task<IActionResult> Start(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(id.ToStartCommand(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/complete")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageWorkOrders)]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(id.ToCompleteCommand(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/close")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageWorkOrders)]
    public async Task<IActionResult> Close(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(id.ToCloseCommand(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/cancel")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageWorkOrders)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(id.ToCancelCommand(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/submit-for-acceptance")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageWorkOrders)]
    public async Task<IActionResult> SubmitForAcceptance(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(id.ToSubmitForCustomerAcceptanceCommand(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/customer-accept")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageWorkOrders)]
    public async Task<IActionResult> CustomerAccept(Guid id, [FromBody] CustomerAcceptWorkOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToAcceptByCustomerCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/customer-reject")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageWorkOrders)]
    public async Task<IActionResult> CustomerReject(Guid id, [FromBody] CustomerRejectWorkOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToRejectByCustomerCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
