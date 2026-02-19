using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TelecomPm.Api.Contracts.Auth;
using TelecomPm.Api.Controllers;
using TelecomPM.Application.Commands.Auth.Login;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Auth;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class AuthControllerTests
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        var sender = new Mock<ISender>();
        sender.Setup(s => s.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new AuthTokenDto
            {
                AccessToken = "jwt-token",
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(30),
                UserId = Guid.NewGuid(),
                Email = "user@example.com",
                Role = "Manager",
                OfficeId = Guid.NewGuid()
            }));

        var controller = BuildController(sender.Object);

        var response = await controller.Login(new LoginRequest
        {
            Email = "user@example.com",
            PhoneNumber = "01000000000"
        }, CancellationToken.None);

        var ok = response.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().BeOfType<LoginResponse>();
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        var sender = new Mock<ISender>();
        sender.Setup(s => s.Send(It.IsAny<LoginCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<AuthTokenDto>("Invalid credentials."));

        var controller = BuildController(sender.Object);

        var response = await controller.Login(new LoginRequest
        {
            Email = "user@example.com",
            PhoneNumber = "01111111111"
        }, CancellationToken.None);

        response.Should().BeOfType<UnauthorizedObjectResult>();
    }

    private static AuthController BuildController(ISender sender)
    {
        var services = new ServiceCollection();
        services.AddSingleton(sender);
        var provider = services.BuildServiceProvider();

        var controller = new AuthController
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = provider
                }
            }
        };

        return controller;
    }
}
