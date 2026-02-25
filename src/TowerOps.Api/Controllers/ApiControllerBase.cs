namespace TowerOps.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TowerOps.Api.Errors;
using TowerOps.Api.Localization;
using TowerOps.Application.Common;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;
    private ILocalizedTextService? _localizedTextService;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
    protected ILocalizedTextService LocalizedText => _localizedTextService ??= HttpContext.RequestServices.GetService<ILocalizedTextService>() ?? new LocalizedTextService();

    protected IActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return Ok();
        }

        return HandleFailure(result.Error);
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value is null ? Ok() : Ok(result.Value);
        }

        return HandleFailure(result.Error);
    }

    private IActionResult HandleFailure(string error)
    {
        var correlationId = HttpContext?.TraceIdentifier ?? string.Empty;
        var mapped = ApiErrorFactory.FromFailureString(error, LocalizedText, correlationId);
        return StatusCode(mapped.StatusCode, mapped.Error);
    }
}

