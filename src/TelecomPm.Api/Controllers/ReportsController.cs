namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPm.Api.Mappings;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ReportsController : ApiControllerBase
{
    [HttpGet("visits/{visitId:guid}")]
    public async Task<IActionResult> GetVisitReport(Guid visitId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(visitId.ToVisitReportQuery(), cancellationToken);
        return HandleResult(result);
    }
}
