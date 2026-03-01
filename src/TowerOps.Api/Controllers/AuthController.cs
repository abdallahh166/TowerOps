namespace TowerOps.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TowerOps.Api.Contracts.Auth;
using TowerOps.Api.Mappings;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ApiControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
        {
            return UnauthorizedFailure(LocalizedText.Get("InvalidCredentials", "Invalid credentials."));
        }

        return Ok(result.Value.ToResponse());
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        if (result.IsFailure)
            return HandleResult(result);

        return Ok(new { message = LocalizedText.Get("ForgotPasswordGenericSuccess", "If the account exists, an OTP was sent to the registered email.") });
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        if (result.IsFailure)
            return HandleResult(result);

        return Ok(new { message = LocalizedText.Get("ResetPasswordSuccess", "Password has been reset successfully.") });
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        if (result.IsFailure)
            return HandleResult(result);

        return Ok(new { message = LocalizedText.Get("ChangePasswordSuccess", "Password changed successfully.") });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        if (!result.IsSuccess || result.Value is null)
            return UnauthorizedFailure();

        return Ok(result.Value.ToResponse());
    }

    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(
        [FromBody] LogoutRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("mfa/setup")]
    [AllowAnonymous]
    public async Task<IActionResult> SetupMfa(
        [FromBody] MfaSetupRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        if (result.IsFailure || result.Value is null)
            return HandleResult(result);

        return Ok(result.Value.ToResponse());
    }

    [HttpPost("mfa/verify")]
    [AllowAnonymous]
    public async Task<IActionResult> VerifyMfa(
        [FromBody] VerifyMfaSetupRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);
        return HandleResult(result);
    }
}
