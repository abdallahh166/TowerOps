namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TelecomPM.Api.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPm.Api.Mappings;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ApiAuthorizationPolicies.CanViewReports)]
public sealed class ReportsController : ApiControllerBase
{
    [HttpGet("visits/{visitId:guid}")]
    public async Task<IActionResult> GetVisitReport(Guid visitId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(visitId.ToVisitReportQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("scorecard")]
    public async Task<IActionResult> GetScorecard(
        [FromQuery] string officeCode,
        [FromQuery] int month,
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            ReportsContractMapper.ToGenerateContractorScorecardCommand(officeCode, month, year),
            cancellationToken);

        if (result.IsFailure || result.Value is null)
            return HandleResult(result);

        var fileName = $"ContractorScorecard_{officeCode}_{year:D4}-{month:D2}.xlsx";
        return File(
            result.Value,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}
