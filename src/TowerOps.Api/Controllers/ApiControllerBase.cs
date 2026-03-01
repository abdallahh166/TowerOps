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

        return Failure(result.Error);
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return result.Value is null ? Ok() : Ok(result.Value);
        }

        return Failure(result.Error);
    }

    protected IActionResult Failure(string error)
    {
        var correlationId = HttpContext?.TraceIdentifier ?? string.Empty;
        var mapped = ApiErrorFactory.FromFailureString(error, LocalizedText, correlationId);
        return StatusCode(mapped.StatusCode, mapped.Error);
    }

    protected IActionResult UnauthorizedFailure(string? message = null)
    {
        var correlationId = HttpContext?.TraceIdentifier ?? string.Empty;
        var mapped = ApiErrorFactory.Build(
            StatusCodes.Status401Unauthorized,
            ApiErrorCodes.Unauthorized,
            message ?? LocalizedText.Get("Unauthorized", "Unauthorized."),
            correlationId);

        return StatusCode(mapped.StatusCode, mapped.Error);
    }

    protected bool TryResolveSort(
        string? sortBy,
        string? sortDir,
        IReadOnlyCollection<string> allowlist,
        string defaultSortBy,
        out string resolvedSortBy,
        out bool sortDescending,
        out IActionResult? errorResult)
    {
        var normalizedAllowlist = allowlist
            .Select(v => v.Trim())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var requestedSortBy = string.IsNullOrWhiteSpace(sortBy) ? defaultSortBy : sortBy.Trim();
        if (!normalizedAllowlist.Contains(requestedSortBy))
        {
            resolvedSortBy = defaultSortBy;
            sortDescending = true;
            errorResult = Failure($"Invalid sortBy '{requestedSortBy}'. Allowed values: {string.Join(", ", normalizedAllowlist.OrderBy(v => v))}.");
            return false;
        }

        var requestedSortDir = string.IsNullOrWhiteSpace(sortDir) ? "desc" : sortDir.Trim();
        if (!requestedSortDir.Equals("asc", StringComparison.OrdinalIgnoreCase) &&
            !requestedSortDir.Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            resolvedSortBy = defaultSortBy;
            sortDescending = true;
            errorResult = Failure("Invalid sortDir. Allowed values: asc, desc.");
            return false;
        }

        resolvedSortBy = requestedSortBy;
        sortDescending = requestedSortDir.Equals("desc", StringComparison.OrdinalIgnoreCase);
        errorResult = null;
        return true;
    }
}

