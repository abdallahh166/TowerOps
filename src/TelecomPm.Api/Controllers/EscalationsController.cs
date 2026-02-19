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
    [Authorize(Policy = ApiAuthorizationPolicies.CanViewEscalations)]
    public async Task<IActionResult> GetById(Guid escalationId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(escalationId.ToEscalationByIdQuery(), cancellationToken);
        return HandleResult(result);
    }
}
