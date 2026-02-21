namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using TelecomPM.Api.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPM.Domain.Enums;
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
            ReportsContractMapper.ToExportScorecardCommand(officeCode, month, year),
            cancellationToken);

        if (result.IsFailure || result.Value is null)
            return HandleResult(result);

        var fileName = $"ContractorScorecard_{officeCode}_{year:D4}-{month:D2}.xlsx";
        return File(
            result.Value,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpGet("checklist")]
    public async Task<IActionResult> ExportChecklist(
        [FromQuery] Guid? visitId,
        [FromQuery] VisitType? visitType,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            ReportsContractMapper.ToExportChecklistCommand(visitId, visitType),
            cancellationToken);

        if (result.IsFailure || result.Value is null)
            return HandleResult(result);

        return File(
            result.Value,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "GH-DE Checklist.xlsx");
    }

    [HttpGet("bdt")]
    public async Task<IActionResult> ExportBdt(
        [FromQuery] DateTime? fromDateUtc,
        [FromQuery] DateTime? toDateUtc,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            ReportsContractMapper.ToExportBdtCommand(fromDateUtc, toDateUtc),
            cancellationToken);

        if (result.IsFailure || result.Value is null)
            return HandleResult(result);

        return File(
            result.Value,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "GH-BDT_BDT.xlsx");
    }

    [HttpGet("data-collection")]
    public async Task<IActionResult> ExportDataCollection(
        [FromQuery] string? officeCode,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            ReportsContractMapper.ToExportDataCollectionCommand(officeCode),
            cancellationToken);

        if (result.IsFailure || result.Value is null)
            return HandleResult(result);

        return File(
            result.Value,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "GH-DE Data Collection.xlsx");
    }
}
