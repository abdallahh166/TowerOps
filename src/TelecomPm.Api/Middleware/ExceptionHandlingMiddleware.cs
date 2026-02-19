namespace TelecomPM.Api.Middleware;

using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TelecomPM.Domain.Exceptions;
using DomainUnauthorizedAccessException = TelecomPM.Domain.Exceptions.UnauthorizedAccessException;
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, responseBody) = exception switch
        {
            ValidationException validationException => (
                (int)HttpStatusCode.BadRequest,
                (object)new
                {
                    Message = "Validation failed",
                    validationException.Errors
                }),
            AppValidationException validationException => (
                (int)HttpStatusCode.BadRequest,
                (object)new
                {
                    Message = "Validation failed",
                    validationException.Errors
                }),
            EntityNotFoundException => (
                (int)HttpStatusCode.NotFound,
                (object)new
                {
                    Message = exception.Message
                }),
            BusinessRuleViolationException businessRuleException => (
                (int)HttpStatusCode.BadRequest,
                (object)new
                {
                    Message = businessRuleException.Message,
                    Rule = businessRuleException.RuleName
                }),
            DomainUnauthorizedAccessException => (
                (int)HttpStatusCode.Forbidden,
                (object)new
                {
                    Message = "You do not have permission to perform this action"
                }),
            DomainException => (
                (int)HttpStatusCode.BadRequest,
                (object)new
                {
                    Message = exception.Message
                }),
            _ => (
                (int)HttpStatusCode.InternalServerError,
                (object)new
                {
                    Message = "An internal server error occurred"
                })
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(responseBody));
    }
}
