namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TelecomPM.Application.Queries.Reports.GetVisitReport;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ReportsController : ApiControllerBase
{
    [HttpGet("visits/{visitId:guid}")]
    public async Task<IActionResult> GetVisitReport(Guid visitId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetVisitReportQuery { VisitId = visitId },
            cancellationToken);

        return HandleResult(result);
    }
}

