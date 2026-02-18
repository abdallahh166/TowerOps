namespace TelecomPm.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPM.Api.Authorization;
using TelecomPM.Application.Queries.Kpi.GetOperationsDashboard;
using TelecomPM.Domain.Enums;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class KpiController : ApiControllerBase
{
    [HttpGet("operations")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanViewKpis)]
    public async Task<IActionResult> GetOperationsDashboard(
        [FromQuery] DateTime? fromDateUtc,
        [FromQuery] DateTime? toDateUtc,
        [FromQuery] string? officeCode,
        [FromQuery] SlaClass? slaClass,
        CancellationToken cancellationToken)
    {
        var query = new GetOperationsDashboardQuery
        {
            FromDateUtc = fromDateUtc,
            ToDateUtc = toDateUtc,
            OfficeCode = officeCode,
            SlaClass = slaClass
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}
