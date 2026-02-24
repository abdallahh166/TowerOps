using System.Globalization;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using TelecomPM.Api.Errors;
using TelecomPM.Api.Middleware;
using TelecomPm.Api.Localization;
using TelecomPM.Domain.Exceptions;
using Xunit;

namespace TelecomPM.Application.Tests.Middleware;

public class ExceptionHandlingMiddlewareLocalizationTests
{
    [Fact]
    public async Task InvokeAsync_ShouldReturnUnifiedErrorContract_ForDomainNotFound()
    {
        var context = await InvokePipelineAsync(
            new EntityNotFoundException("Visit", "V-1"),
            "ar-EG");

        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        GetRequestCulture(context).Should().Be("ar-EG");

        var payload = await ReadJsonAsync(context.Response);
        payload.GetProperty("Code").GetString().Should().Be(ApiErrorCodes.ResourceNotFound);
        payload.GetProperty("Message").GetString().Should().NotBeNullOrWhiteSpace();
        payload.GetProperty("Message").GetString().Should().Contain("V-1");
        payload.GetProperty("CorrelationId").GetString().Should().Be(context.TraceIdentifier);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnUnifiedErrorContract_ForApplicationConflict()
    {
        var context = await InvokePipelineAsync(
            new TelecomPM.Application.Exceptions.ConflictException("Conflict."),
            "ar-EG");

        context.Response.StatusCode.Should().Be(StatusCodes.Status409Conflict);
        GetRequestCulture(context).Should().Be("ar-EG");

        var payload = await ReadJsonAsync(context.Response);
        payload.GetProperty("Code").GetString().Should().Be(ApiErrorCodes.Conflict);
        payload.GetProperty("Message").GetString().Should().NotBeNullOrWhiteSpace();
        payload.GetProperty("Message").GetString().Should().NotBe("Conflict.");
        payload.GetProperty("CorrelationId").GetString().Should().Be(context.TraceIdentifier);
    }

    private static async Task<DefaultHttpContext> InvokePipelineAsync(Exception exception, string acceptLanguage)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddLocalization();
        services.AddScoped<ILocalizedTextService, LocalizedTextService>();
        services.AddScoped<IValidationErrorLocalizer, ValidationErrorLocalizer>();
        var provider = services.BuildServiceProvider();

        var options = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("en-US"),
            SupportedCultures = [new CultureInfo("en-US"), new CultureInfo("ar-EG")],
            SupportedUICultures = [new CultureInfo("en-US"), new CultureInfo("ar-EG")],
            RequestCultureProviders = [new AcceptLanguageHeaderRequestCultureProvider()]
        };

        var exceptionMiddleware = new ExceptionHandlingMiddleware(
            _ => throw exception,
            NullLogger<ExceptionHandlingMiddleware>.Instance);

        var localizationMiddleware = new RequestLocalizationMiddleware(
            context => exceptionMiddleware.InvokeAsync(context),
            Options.Create(options),
            NullLoggerFactory.Instance);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = provider
        };
        httpContext.Request.Headers.AcceptLanguage = acceptLanguage;
        httpContext.Response.Body = new MemoryStream();

        await localizationMiddleware.Invoke(httpContext);
        return httpContext;
    }

    private static async Task<JsonElement> ReadJsonAsync(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(response.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.Clone();
    }

    private static string? GetRequestCulture(HttpContext context)
    {
        return context.Features.Get<IRequestCultureFeature>()?.RequestCulture.UICulture.Name;
    }
}
