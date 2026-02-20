namespace TelecomPm.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPM.Api.Authorization;
using TelecomPm.Api.Contracts.Escalations;
using TelecomPm.Api.Mappings;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class EscalationsController : ApiControllerBase
{
    [HttpPost]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageEscalations)]
    public async Task<IActionResult> Create([FromBody] CreateEscalationRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetById), new { escalationId = result.Value.Id }, result.Value);
        }

        return HandleResult(result);
    }

    [HttpGet("{escalationId:guid}")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageEscalations)]
    public async Task<IActionResult> GetById(Guid escalationId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(escalationId.ToEscalationByIdQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/review")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageEscalations)]
    public async Task<IActionResult> Review(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(id.ToReviewEscalationCommand(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/approve")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageEscalations)]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(id.ToApproveEscalationCommand(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/reject")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageEscalations)]
    public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(id.ToRejectEscalationCommand(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{id:guid}/close")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageEscalations)]
    public async Task<IActionResult> Close(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(id.ToCloseEscalationCommand(), cancellationToken);
        return HandleResult(result);
    }
}
