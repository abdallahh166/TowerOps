namespace TelecomPm.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPm.Api.Contracts.Auth;
using TelecomPm.Api.Mappings;

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
            return Unauthorized("Invalid credentials.");
        }

        return Ok(result.Value.ToResponse());
    }
}
