using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using TelecomPM.Api.Middleware;
using Xunit;

namespace TelecomPM.Application.Tests.Middleware;

public class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_WithIncomingCorrelationHeader_ShouldPreserveAndEchoHeader()
    {
        var expectedCorrelationId = "corr-12345";
        var capturedCorrelationId = string.Empty;

        var middleware = new CorrelationIdMiddleware(
            next: context =>
            {
                capturedCorrelationId = context.TraceIdentifier;
                context.Response.StatusCode = StatusCodes.Status200OK;
                return Task.CompletedTask;
            },
            logger: NullLogger<CorrelationIdMiddleware>.Instance);

        var context = new DefaultHttpContext();
        context.Request.Headers[CorrelationIdMiddleware.HeaderName] = expectedCorrelationId;
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        capturedCorrelationId.Should().Be(expectedCorrelationId);
        context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString().Should().Be(expectedCorrelationId);
        context.Items[CorrelationIdMiddleware.HeaderName].Should().Be(expectedCorrelationId);
    }

    [Fact]
    public async Task InvokeAsync_WithoutIncomingCorrelationHeader_ShouldGenerateAndEchoHeader()
    {
        var middleware = new CorrelationIdMiddleware(
            next: context =>
            {
                context.Response.StatusCode = StatusCodes.Status200OK;
                return Task.CompletedTask;
            },
            logger: NullLogger<CorrelationIdMiddleware>.Instance);

        var context = new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };

        await middleware.InvokeAsync(context);

        context.TraceIdentifier.Should().NotBeNullOrWhiteSpace();
        context.Response.Headers[CorrelationIdMiddleware.HeaderName].ToString()
            .Should().Be(context.TraceIdentifier);
    }
}
