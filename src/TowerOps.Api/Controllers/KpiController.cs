namespace TowerOps.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TowerOps.Api.Authorization;
using TowerOps.Api.Mappings;
using TowerOps.Domain.Enums;

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
        var query = KpiContractMapper.ToOperationsDashboardQuery(fromDateUtc, toDateUtc, officeCode, slaClass);
        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }
}
