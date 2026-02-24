namespace TelecomPM.Api.Middleware;

using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TelecomPm.Api.Localization;
using TelecomPM.Domain.Exceptions;
using DomainUnauthorizedAccessException = TelecomPM.Domain.Exceptions.UnauthorizedAccessException;
using AppBaseException = TelecomPM.Application.Exceptions.ApplicationException;
using AppConflictException = TelecomPM.Application.Exceptions.ConflictException;
using AppNotFoundException = TelecomPM.Application.Exceptions.NotFoundException;
using AppUnauthorizedException = TelecomPM.Application.Exceptions.UnauthorizedException;
using AppValidationException = TelecomPM.Application.Exceptions.ValidationException;

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

        var (statusCode, responseBody) = exception switch
        {
            ValidationException validationException => (
                (int)HttpStatusCode.BadRequest,
                (object)new
                {
                    Message = ResolveLocalizedMessage(validationException, localizer),
                    Errors = validationLocalizer.Localize(validationException.Errors)
                }),
            AppValidationException validationException => (
                (int)HttpStatusCode.BadRequest,
                (object)new
                {
                    Message = ResolveLocalizedMessage(validationException, localizer),
                    Errors = validationLocalizer.Localize(validationException.Errors)
                }),
            EntityNotFoundException notFoundException => (
                (int)HttpStatusCode.NotFound,
                (object)new
                {
                    Message = ResolveLocalizedMessage(notFoundException, localizer)
                }),
            BusinessRuleViolationException businessRuleException => (
                (int)HttpStatusCode.BadRequest,
                (object)new
                {
                    Message = ResolveLocalizedMessage(businessRuleException, localizer),
                    Rule = businessRuleException.RuleName
                }),
            DomainUnauthorizedAccessException unauthorizedException => (
                (int)HttpStatusCode.Forbidden,
                (object)new
                {
                    Message = ResolveLocalizedMessage(unauthorizedException, localizer)
                }),
            DomainException domainException => (
                (int)HttpStatusCode.BadRequest,
                (object)new
                {
                    Message = ResolveLocalizedMessage(domainException, localizer)
                }),
            AppNotFoundException notFoundException => (
                (int)HttpStatusCode.NotFound,
                (object)new
                {
                    Message = ResolveLocalizedMessage(notFoundException, localizer)
                }),
            AppUnauthorizedException unauthorizedException => (
                (int)HttpStatusCode.Forbidden,
                (object)new
                {
                    Message = ResolveLocalizedMessage(unauthorizedException, localizer)
                }),
            AppConflictException conflictException => (
                (int)HttpStatusCode.Conflict,
                (object)new
                {
                    Message = ResolveLocalizedMessage(conflictException, localizer)
                }),
            AppBaseException applicationException => (
                (int)HttpStatusCode.BadRequest,
                (object)new
                {
                    Message = ResolveLocalizedMessage(applicationException, localizer)
                }),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                (object)new
                {
                    Message = localizer.Get("InternalServerError", "An internal server error occurred")
                })
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(responseBody));
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
