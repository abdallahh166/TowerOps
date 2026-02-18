namespace TelecomPm.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPM.Api.Authorization;
using TelecomPm.Api.Contracts.Escalations;
using TelecomPM.Application.Commands.Escalations.CreateEscalation;
using TelecomPM.Application.Queries.Escalations.GetEscalationById;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class EscalationsController : ApiControllerBase
{
    [HttpPost]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageEscalations)]
    public async Task<IActionResult> Create([FromBody] CreateEscalationRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateEscalationCommand
        {
            WorkOrderId = request.WorkOrderId,
            IncidentId = request.IncidentId,
            SiteCode = request.SiteCode,
            SlaClass = request.SlaClass,
            FinancialImpactEgp = request.FinancialImpactEgp,
            SlaImpactPercentage = request.SlaImpactPercentage,
            EvidencePackage = request.EvidencePackage,
            PreviousActions = request.PreviousActions,
            RecommendedDecision = request.RecommendedDecision,
            Level = request.Level,
            SubmittedBy = request.SubmittedBy
        };

        var result = await Mediator.Send(command, cancellationToken);
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
        var result = await Mediator.Send(new GetEscalationByIdQuery { EscalationId = escalationId }, cancellationToken);
        return HandleResult(result);
    }
}
