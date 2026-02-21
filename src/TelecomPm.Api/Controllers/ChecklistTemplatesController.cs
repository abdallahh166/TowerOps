using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPM.Api.Authorization;
using TelecomPm.Api.Contracts.ChecklistTemplates;
using TelecomPm.Api.Mappings;
using TelecomPM.Domain.Enums;

namespace TelecomPm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ChecklistTemplatesController : ApiControllerBase
{
    [HttpPost("import")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageWorkOrders)]
    public async Task<IActionResult> Import(
        [FromForm] ImportChecklistTemplateRequest request,
        CancellationToken cancellationToken)
    {
        var fileBytes = await ReadExcelBytesOrNullAsync(request.File, cancellationToken);
        if (fileBytes is null)
            return BadRequest("Excel file is required.");

        var result = await Mediator.Send(request.ToImportCommand(fileBytes), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetActive([FromQuery] VisitType visitType, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(visitType.ToGetActiveQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(id.ToGetByIdQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory([FromQuery] VisitType visitType, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(visitType.ToGetHistoryQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageWorkOrders)]
    public async Task<IActionResult> Create([FromBody] CreateChecklistTemplateRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        if (result.IsSuccess)
        {
            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
        }

        return HandleResult(result);
    }

    [HttpPost("{id:guid}/activate")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageWorkOrders)]
    public async Task<IActionResult> Activate(
        Guid id,
        [FromBody] ActivateChecklistTemplateRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(id), cancellationToken);
        return HandleResult(result);
    }

    private static async Task<byte[]?> ReadExcelBytesOrNullAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return null;

        await using var stream = file.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, cancellationToken);
        return ms.ToArray();
    }
}
