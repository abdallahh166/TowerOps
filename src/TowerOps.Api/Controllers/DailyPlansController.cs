namespace TowerOps.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TowerOps.Api.Authorization;
using TowerOps.Api.Contracts.DailyPlans;
using TowerOps.Api.Mappings;

[ApiController]
[Route("api/daily-plans")]
[Authorize(Policy = ApiAuthorizationPolicies.CanManageSites)]
public sealed class DailyPlansController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDailyPlanRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{officeId:guid}/{date}")]
    public async Task<IActionResult> GetByOfficeAndDate(Guid officeId, DateOnly date, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send((officeId, date).ToDailyPlanQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{planId:guid}/assign")]
    public async Task<IActionResult> AssignSite(Guid planId, [FromBody] AssignSiteToEngineerRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(planId), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{planId:guid}/assign")]
    public async Task<IActionResult> RemoveSite(Guid planId, [FromBody] RemoveSiteFromEngineerRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(planId), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{planId:guid}/suggest/{engineerId:guid}")]
    public async Task<IActionResult> GetSuggestedOrder(Guid planId, Guid engineerId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send((planId, engineerId).ToSuggestedOrderQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{officeId:guid}/{date}/unassigned")]
    public async Task<IActionResult> GetUnassignedSites(Guid officeId, DateOnly date, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send((officeId, date).ToUnassignedSitesQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{planId:guid}/publish")]
    public async Task<IActionResult> Publish(Guid planId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(planId.ToPublishCommand(), cancellationToken);
        return HandleResult(result);
    }
}
