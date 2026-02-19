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
public sealed class MaterialsController : ApiControllerBase
{
    [HttpGet("low-stock/{officeId:guid}")]
    public async Task<IActionResult> GetLowStockMaterials(Guid officeId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(officeId.ToLowStockQuery(), cancellationToken);
        return HandleResult(result);
    }
}
