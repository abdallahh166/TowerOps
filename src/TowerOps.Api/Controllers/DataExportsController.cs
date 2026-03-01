using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TowerOps.Application.Commands.Privacy.RequestMyOperationalDataExport;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.Queries.Privacy.GetMyOperationalDataExport;

namespace TowerOps.Api.Controllers;

[ApiController]
[Route("api/data-exports")]
[Authorize]
public sealed class DataExportsController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public DataExportsController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    [HttpPost("me")]
    public async Task<IActionResult> RequestMyOperationalDataExport(CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
            return UnauthorizedFailure();

        var result = await Mediator.Send(
            new RequestMyOperationalDataExportCommand { UserId = _currentUserService.UserId },
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("me/{requestId:guid}")]
    public async Task<IActionResult> DownloadMyOperationalDataExport(Guid requestId, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
            return UnauthorizedFailure();

        var result = await Mediator.Send(
            new GetMyOperationalDataExportQuery
            {
                UserId = _currentUserService.UserId,
                RequestId = requestId
            },
            cancellationToken);

        if (result.IsFailure || string.IsNullOrWhiteSpace(result.Value))
            return HandleResult(result);

        var bytes = Encoding.UTF8.GetBytes(result.Value);
        return File(
            bytes,
            "application/json",
            $"towerops-my-operational-data-{requestId:N}.json");
    }
}
