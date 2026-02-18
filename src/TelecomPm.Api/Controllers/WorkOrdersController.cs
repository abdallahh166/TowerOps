namespace TelecomPm.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPM.Api.Authorization;
using TelecomPm.Api.Contracts.WorkOrders;
using TelecomPM.Application.Commands.WorkOrders.AssignWorkOrder;
using TelecomPM.Application.Commands.WorkOrders.CreateWorkOrder;
using TelecomPM.Application.Queries.WorkOrders.GetWorkOrderById;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class WorkOrdersController : ApiControllerBase
{
    [HttpPost]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageWorkOrders)]
    public async Task<IActionResult> Create([FromBody] CreateWorkOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateWorkOrderCommand
        {
            WoNumber = request.WoNumber,
            SiteCode = request.SiteCode,
            OfficeCode = request.OfficeCode,
            SlaClass = request.SlaClass,
            IssueDescription = request.IssueDescription
        };

        var result = await Mediator.Send(command, cancellationToken);
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
        var result = await Mediator.Send(new GetWorkOrderByIdQuery { WorkOrderId = workOrderId }, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{workOrderId:guid}/assign")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageWorkOrders)]
    public async Task<IActionResult> Assign(Guid workOrderId, [FromBody] AssignWorkOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new AssignWorkOrderCommand
        {
            WorkOrderId = workOrderId,
            EngineerId = request.EngineerId,
            EngineerName = request.EngineerName,
            AssignedBy = request.AssignedBy
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
