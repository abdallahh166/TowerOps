using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TelecomPm.Api.Controllers;
using TelecomPM.Api.Errors;
using TelecomPM.Application.Common;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class ApiControllerBaseErrorSanitizationTests
{
    [Fact]
    public void HandleResult_ShouldHideSensitiveTechnicalErrors()
    {
        var controller = CreateController();
        var result = controller.Invoke(Result.Failure("SqlException: Cannot open database requested by login. at Microsoft.Data.SqlClient."));

        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);

        var payload = objectResult.Value.Should().BeOfType<ApiErrorResponse>().Subject;
        payload.Code.Should().Be(ApiErrorCodes.InternalError);
        payload.Message.Should().NotContain("SqlException");
        payload.Message.Should().NotContain("Microsoft.Data.SqlClient");
    }

    [Fact]
    public void HandleResult_ShouldKeepBusinessErrorDetails()
    {
        var controller = CreateController();
        var result = controller.Invoke(Result.Failure("Visit not found"));

        var objectResult = result.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(StatusCodes.Status404NotFound);

        var payload = objectResult.Value.Should().BeOfType<ApiErrorResponse>().Subject;
        payload.Code.Should().Be(ApiErrorCodes.ResourceNotFound);
        payload.Message.Should().Contain("Visit not found");
    }

    private static TestController CreateController()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddControllers();
        var provider = services.BuildServiceProvider();

        return new TestController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = provider
                }
            }
        };
    }

    private sealed class TestController : ApiControllerBase
    {
        public IActionResult Invoke(Result result) => HandleResult(result);
    }
}
