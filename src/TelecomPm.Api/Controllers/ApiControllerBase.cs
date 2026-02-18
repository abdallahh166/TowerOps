namespace TelecomPm.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TelecomPM.Application.Common;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

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
        if (string.IsNullOrWhiteSpace(error))
        {
            return Problem(
                title: "Request failed",
                statusCode: StatusCodes.Status500InternalServerError);
        }

        var statusCode = error.Contains("not found", StringComparison.OrdinalIgnoreCase)
            ? StatusCodes.Status404NotFound
            : error.Contains("unauthorized", StringComparison.OrdinalIgnoreCase)
                ? StatusCodes.Status401Unauthorized
                : error.Contains("forbidden", StringComparison.OrdinalIgnoreCase)
                    ? StatusCodes.Status403Forbidden
                    : StatusCodes.Status400BadRequest;

        return Problem(
            title: "Request failed",
            detail: error,
            statusCode: statusCode);
    }
}

