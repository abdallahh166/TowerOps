namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TelecomPM.Application.Queries.Materials.GetLowStockMaterials;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class MaterialsController : ApiControllerBase
{
    [HttpGet("low-stock/{officeId:guid}")]
    public async Task<IActionResult> GetLowStockMaterials(Guid officeId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetLowStockMaterialsQuery { OfficeId = officeId },
            cancellationToken);

        return HandleResult(result);
    }
}

