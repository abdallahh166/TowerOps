namespace TowerOps.Application.Commands.Auth.Login;

using MediatR;
using Microsoft.AspNetCore.Identity;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Auth;
using TowerOps.Domain.Entities.RefreshTokens;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthTokenDto>>
{
    private const int MaxFailedAttempts = 5;
    private const int TemporaryLockoutMinutes = 15;
    private const int ManualLockoutThreshold = 3;
    private const int ManualLockoutWindowHours = 24;

    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IMfaService _mfaService;
    private readonly IEmailService _emailService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IJwtTokenService jwtTokenService,
        IRefreshTokenService refreshTokenService,
        IMfaService mfaService,
        IEmailService emailService,
        IPasswordHasher<User> passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtTokenService = jwtTokenService;
        _refreshTokenService = refreshTokenService;
        _mfaService = mfaService;
        _emailService = emailService;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthTokenDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var nowUtc = DateTime.UtcNow;

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Result.Failure<AuthTokenDto>("Invalid credentials.");
        }

        if (user.IsLockedOut(nowUtc))
        {
            return Result.Failure<AuthTokenDto>(user.IsManualLockout
                ? "Unauthorized. Account is locked pending admin unlock."
                : "Unauthorized. Account is temporarily locked due to failed attempts.");
        }

        if (!user.VerifyPassword(request.Password, _passwordHasher))
        {
            var enteredManualLockout = user.RegisterFailedLoginAttempt(
                nowUtc,
                MaxFailedAttempts,
                TemporaryLockoutMinutes,
                ManualLockoutThreshold,
                ManualLockoutWindowHours);

            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (enteredManualLockout)
            {
                await NotifyAdminsForManualLockAsync(user, cancellationToken);
                return Result.Failure<AuthTokenDto>("Unauthorized. Account is locked pending admin unlock.");
            }

            if (user.IsLockedOut(nowUtc))
            {
                return Result.Failure<AuthTokenDto>("Unauthorized. Account is temporarily locked due to failed attempts.");
            }

            return Result.Failure<AuthTokenDto>("Invalid credentials.");
        }

        if (IsMfaMandatoryRole(user.Role))
        {
            if (!user.IsMfaEnabled || string.IsNullOrWhiteSpace(user.MfaSecret))
            {
                return Result.Failure<AuthTokenDto>("Unauthorized. MFA setup is required for this account.");
            }

            if (!_mfaService.VerifyCode(user.MfaSecret, request.MfaCode ?? string.Empty, nowUtc))
            {
                var enteredManualLockout = user.RegisterFailedLoginAttempt(
                    nowUtc,
                    MaxFailedAttempts,
                    TemporaryLockoutMinutes,
                    ManualLockoutThreshold,
                    ManualLockoutWindowHours);

                await _userRepository.UpdateAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (enteredManualLockout)
                {
                    await NotifyAdminsForManualLockAsync(user, cancellationToken);
                    return Result.Failure<AuthTokenDto>("Unauthorized. Account is locked pending admin unlock.");
                }

                return Result.Failure<AuthTokenDto>("Unauthorized. Invalid MFA code.");
            }
        }

        user.RegisterSuccessfulLogin(nowUtc);

        var (token, expiresAtUtc) = await _jwtTokenService.GenerateTokenAsync(user, cancellationToken);
        var refreshTokenValue = _refreshTokenService.GenerateToken();
        var refreshTokenHash = _refreshTokenService.HashToken(refreshTokenValue);
        var refreshTokenExpiry = _refreshTokenService.GetRefreshTokenExpiryUtc();

        var refreshToken = RefreshToken.Create(user.Id, refreshTokenHash, refreshTokenExpiry);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new AuthTokenDto
        {
            AccessToken = token,
            ExpiresAtUtc = expiresAtUtc,
            RefreshToken = refreshTokenValue,
            RefreshTokenExpiresAtUtc = refreshTokenExpiry,
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString(),
            OfficeId = user.OfficeId,
            RequiresPasswordChange = user.MustChangePassword
        });
    }

    private static bool IsMfaMandatoryRole(UserRole role)
    {
        return role is UserRole.Admin or UserRole.Manager;
    }

    private async Task NotifyAdminsForManualLockAsync(User lockedUser, CancellationToken cancellationToken)
    {
        try
        {
            var admins = await _userRepository.GetByRoleAsNoTrackingAsync(UserRole.Admin, cancellationToken);
            if (admins.Count == 0)
                return;

            var subject = "TowerOps Security Alert: Account manually locked";
            var body =
                $"<p>User account has been moved to manual lockout after repeated failed login attempts.</p>" +
                $"<p>Email: <b>{lockedUser.Email}</b></p>" +
                $"<p>User Id: <b>{lockedUser.Id}</b></p>" +
                $"<p>Timestamp (UTC): <b>{DateTime.UtcNow:O}</b></p>";

            foreach (var admin in admins.Where(x => x.IsActive && !string.IsNullOrWhiteSpace(x.Email)))
            {
                await _emailService.SendEmailAsync(admin.Email, subject, body, cancellationToken);
            }
        }
        catch
        {
            // Login flow should not fail if notification delivery fails.
        }
    }
}
