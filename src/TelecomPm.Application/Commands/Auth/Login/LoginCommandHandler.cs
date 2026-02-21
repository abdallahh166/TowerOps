namespace TelecomPM.Application.Commands.Auth.Login;

using MediatR;
using Microsoft.AspNetCore.Identity;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Auth;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Interfaces.Repositories;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthTokenDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<AuthTokenDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var user = await _userRepository.GetByEmailAsNoTrackingAsync(email, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Result.Failure<AuthTokenDto>("Invalid credentials.");
        }

        if (!user.VerifyPassword(request.Password, _passwordHasher))
        {
            return Result.Failure<AuthTokenDto>("Invalid credentials.");
        }

        var (token, expiresAtUtc) = _jwtTokenService.GenerateToken(user);
        return Result.Success(new AuthTokenDto
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc,
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString(),
            OfficeId = user.OfficeId,
            RequiresPasswordChange = user.MustChangePassword
        });
    }
}
