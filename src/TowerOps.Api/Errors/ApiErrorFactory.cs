using System.Net;
using TowerOps.Api.Localization;

namespace TowerOps.Api.Errors;

public sealed record ApiErrorResult(
    int StatusCode,
    ApiErrorResponse Error);

public static class ApiErrorFactory
{
    public static ApiErrorResult FromFailureString(
        string? error,
        ILocalizedTextService localizer,
        string correlationId)
    {
        var normalizedError = error ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedError))
        {
            return Build(
                StatusCodes.Status500InternalServerError,
                ApiErrorCodes.InternalError,
                localizer.Get("UnexpectedError", "An unexpected error occurred."),
                correlationId);
        }

        if (LooksSensitiveError(normalizedError))
        {
            return Build(
                StatusCodes.Status500InternalServerError,
                ApiErrorCodes.InternalError,
                localizer.Get("UnexpectedError", "An unexpected error occurred."),
                correlationId);
        }

        if (normalizedError.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return Build(
                StatusCodes.Status404NotFound,
                ApiErrorCodes.ResourceNotFound,
                localizer.TranslateMessage(normalizedError),
                correlationId);
        }

        if (normalizedError.Contains("unauthorized", StringComparison.OrdinalIgnoreCase))
        {
            return Build(
                StatusCodes.Status401Unauthorized,
                ApiErrorCodes.Unauthorized,
                localizer.TranslateMessage(normalizedError),
                correlationId);
        }

        if (normalizedError.Contains("forbidden", StringComparison.OrdinalIgnoreCase))
        {
            return Build(
                StatusCodes.Status403Forbidden,
                ApiErrorCodes.Forbidden,
                localizer.TranslateMessage(normalizedError),
                correlationId);
        }

        if (normalizedError.Contains("conflict", StringComparison.OrdinalIgnoreCase) ||
            normalizedError.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        {
            return Build(
                StatusCodes.Status409Conflict,
                ApiErrorCodes.Conflict,
                localizer.TranslateMessage(normalizedError),
                correlationId);
        }

        if (normalizedError.Contains("validation", StringComparison.OrdinalIgnoreCase))
        {
            return Build(
                StatusCodes.Status400BadRequest,
                ApiErrorCodes.ValidationFailed,
                localizer.TranslateMessage(normalizedError),
                correlationId);
        }

        return Build(
            StatusCodes.Status400BadRequest,
            ApiErrorCodes.RequestFailed,
            localizer.TranslateMessage(normalizedError),
            correlationId);
    }

    public static ApiErrorResult Validation(
        IDictionary<string, string[]> errors,
        ILocalizedTextService localizer,
        string correlationId)
    {
        return new ApiErrorResult(
            StatusCodes.Status400BadRequest,
            new ApiErrorResponse
            {
                Code = ApiErrorCodes.ValidationFailed,
                Message = localizer.Get("ValidationFailed", "Validation failed"),
                CorrelationId = correlationId,
                Errors = errors
            });
    }

    public static ApiErrorResult Build(
        int statusCode,
        string code,
        string message,
        string correlationId,
        IDictionary<string, string[]>? errors = null,
        IDictionary<string, string>? meta = null)
    {
        return new ApiErrorResult(
            statusCode,
            new ApiErrorResponse
            {
                Code = code,
                Message = message,
                CorrelationId = correlationId,
                Errors = errors,
                Meta = meta
            });
    }

    public static bool LooksSensitiveError(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            return false;

        var sensitiveMarkers = new[]
        {
            "exception",
            "stack trace",
            " at ",
            "system.",
            "microsoft.",
            "sql",
            "connection string",
            "accountkey=",
            "inner exception",
            "timeout expired"
        };

        return sensitiveMarkers.Any(marker =>
            error.Contains(marker, StringComparison.OrdinalIgnoreCase));
    }
}
