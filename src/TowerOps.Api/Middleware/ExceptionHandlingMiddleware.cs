namespace TowerOps.Api.Middleware;

using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TowerOps.Api.Errors;
using TowerOps.Api.Localization;
using TowerOps.Domain.Exceptions;
using DomainUnauthorizedAccessException = TowerOps.Domain.Exceptions.UnauthorizedAccessException;
using AppBaseException = TowerOps.Application.Exceptions.ApplicationException;
using AppConflictException = TowerOps.Application.Exceptions.ConflictException;
using AppNotFoundException = TowerOps.Application.Exceptions.NotFoundException;
using AppUnauthorizedException = TowerOps.Application.Exceptions.UnauthorizedException;
using AppValidationException = TowerOps.Application.Exceptions.ValidationException;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            var localizer = context.RequestServices.GetService(typeof(ILocalizedTextService)) as ILocalizedTextService
                ?? new LocalizedTextService();
            var validationLocalizer = context.RequestServices.GetService(typeof(IValidationErrorLocalizer)) as IValidationErrorLocalizer
                ?? new ValidationErrorLocalizer(localizer);
            await HandleExceptionAsync(context, ex, localizer, validationLocalizer);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception,
        ILocalizedTextService localizer,
        IValidationErrorLocalizer validationLocalizer)
    {
        context.Response.ContentType = "application/json";
        var correlationId = context.TraceIdentifier;

        var mapped = exception switch
        {
            ValidationException validationException => ApiErrorFactory.Validation(
                validationLocalizer.Localize(validationException.Errors),
                localizer,
                correlationId),
            AppValidationException validationException => ApiErrorFactory.Validation(
                validationLocalizer.Localize(validationException.Errors),
                localizer,
                correlationId),
            EntityNotFoundException notFoundException => ApiErrorFactory.Build(
                (int)HttpStatusCode.NotFound,
                ApiErrorCodes.ResourceNotFound,
                ResolveLocalizedMessage(notFoundException, localizer),
                correlationId),
            BusinessRuleViolationException businessRuleException => ApiErrorFactory.Build(
                (int)HttpStatusCode.BadRequest,
                ApiErrorCodes.BusinessRuleViolation,
                ResolveLocalizedMessage(businessRuleException, localizer),
                correlationId,
                meta: new Dictionary<string, string> { ["Rule"] = businessRuleException.RuleName }),
            DomainUnauthorizedAccessException unauthorizedException => ApiErrorFactory.Build(
                (int)HttpStatusCode.Forbidden,
                ApiErrorCodes.Forbidden,
                ResolveLocalizedMessage(unauthorizedException, localizer),
                correlationId),
            DomainException domainException => ApiErrorFactory.Build(
                (int)HttpStatusCode.BadRequest,
                ApiErrorCodes.BusinessRuleViolation,
                ResolveLocalizedMessage(domainException, localizer),
                correlationId),
            AppNotFoundException notFoundException => ApiErrorFactory.Build(
                (int)HttpStatusCode.NotFound,
                ApiErrorCodes.ResourceNotFound,
                ResolveLocalizedMessage(notFoundException, localizer),
                correlationId),
            AppUnauthorizedException unauthorizedException => ApiErrorFactory.Build(
                (int)HttpStatusCode.Forbidden,
                ApiErrorCodes.Forbidden,
                ResolveLocalizedMessage(unauthorizedException, localizer),
                correlationId),
            AppConflictException conflictException => ApiErrorFactory.Build(
                (int)HttpStatusCode.Conflict,
                ApiErrorCodes.Conflict,
                ResolveLocalizedMessage(conflictException, localizer),
                correlationId),
            AppBaseException applicationException => ApiErrorFactory.Build(
                (int)HttpStatusCode.BadRequest,
                ApiErrorCodes.RequestFailed,
                ResolveLocalizedMessage(applicationException, localizer),
                correlationId),
            _ => ApiErrorFactory.Build(
                (int)HttpStatusCode.InternalServerError,
                ApiErrorCodes.InternalError,
                localizer.Get("InternalServerError", "An internal server error occurred"),
                correlationId)
        };

        context.Response.StatusCode = mapped.StatusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(mapped.Error));
    }

    private static string ResolveLocalizedMessage(Exception exception, ILocalizedTextService localizer)
    {
        if (exception is DomainException domainException)
            return ResolveByKey(localizer, domainException.MessageKey, domainException.Message, domainException.MessageArguments);

        if (exception is AppBaseException applicationException)
            return ResolveByKey(localizer, applicationException.MessageKey, applicationException.Message, applicationException.MessageArguments);

        return localizer.TranslateMessage(exception.Message);
    }

    private static string ResolveByKey(
        ILocalizedTextService localizer,
        string? key,
        string fallback,
        object[]? args)
    {
        if (string.IsNullOrWhiteSpace(key))
            return localizer.TranslateMessage(fallback);

        return args is { Length: > 0 }
            ? localizer.Get(key, fallback, args)
            : localizer.Get(key, fallback);
    }
}
