namespace TelecomPm.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPM.Application.Common.Interfaces;
using TelecomPm.Api.Contracts.Sync;
using TelecomPm.Api.Mappings;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class SyncController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public SyncController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<IActionResult> ProcessBatch([FromBody] SyncBatchRequest request, CancellationToken cancellationToken)
    {
        var engineerId = request.EngineerId;
        if (string.IsNullOrWhiteSpace(engineerId) && _currentUserService.UserId != Guid.Empty)
        {
            engineerId = _currentUserService.UserId.ToString();
        }

        if (string.IsNullOrWhiteSpace(engineerId))
            return BadRequest("EngineerId is required.");

        var result = await Mediator.Send(request.ToCommand(engineerId), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("status/{deviceId}")]
    public async Task<IActionResult> GetStatus(string deviceId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(deviceId.ToStatusQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("conflicts/{engineerId}")]
    public async Task<IActionResult> GetConflicts(string engineerId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(engineerId.ToConflictsQuery(), cancellationToken);
        return HandleResult(result);
    }
}
