namespace TelecomPm.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TelecomPm.Api.Localization;
using TelecomPM.Application.Common;

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
        var normalizedError = error ?? string.Empty;
        if (string.IsNullOrWhiteSpace(error))
        {
            return Problem(
                title: LocalizedText.Get("RequestFailed", "Request failed"),
                statusCode: StatusCodes.Status500InternalServerError);
        }

        var statusCode = normalizedError.Contains("not found", StringComparison.OrdinalIgnoreCase)
            ? StatusCodes.Status404NotFound
            : normalizedError.Contains("unauthorized", StringComparison.OrdinalIgnoreCase)
                ? StatusCodes.Status401Unauthorized
                : normalizedError.Contains("forbidden", StringComparison.OrdinalIgnoreCase)
                    ? StatusCodes.Status403Forbidden
                    : StatusCodes.Status400BadRequest;

        return Problem(
            title: LocalizedText.Get("RequestFailed", "Request failed"),
            detail: LocalizedText.TranslateMessage(normalizedError),
            statusCode: statusCode);
    }
}

