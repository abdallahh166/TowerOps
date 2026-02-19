namespace TelecomPm.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPm.Api.Contracts.Auth;
using TelecomPM.Api.Services;
using TelecomPM.Domain.Interfaces.Repositories;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ApiControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var phone = request.PhoneNumber.Trim();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
        {
            return BadRequest("Email and phone number are required.");
        }

        var user = await _userRepository.GetByEmailAsNoTrackingAsync(email, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Unauthorized("Invalid credentials.");
        }

        if (!string.Equals(user.PhoneNumber, phone, StringComparison.Ordinal))
        {
            return Unauthorized("Invalid credentials.");
        }

        var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(user);
        return Ok(new LoginResponse
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc,
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString(),
            OfficeId = user.OfficeId
        });
    }
}
