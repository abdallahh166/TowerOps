namespace TowerOps.Api.Mappings;

using TowerOps.Api.Contracts.Auth;
using TowerOps.Application.Commands.Auth.ChangePassword;
using TowerOps.Application.Commands.Auth.ForgotPassword;
using TowerOps.Application.Commands.Auth.GenerateMfaSetup;
using TowerOps.Application.Commands.Auth.Login;
using TowerOps.Application.Commands.Auth.Logout;
using TowerOps.Application.Commands.Auth.RefreshToken;
using TowerOps.Application.Commands.Auth.ResetPassword;
using TowerOps.Application.Commands.Auth.VerifyMfaSetup;
using TowerOps.Application.DTOs.Auth;

public static class AuthContractMapper
{
    public static LoginCommand ToCommand(this LoginRequest request)
        => new()
        {
            Email = request.Email,
            Password = request.Password,
            MfaCode = request.MfaCode
        };

    public static ForgotPasswordCommand ToCommand(this ForgotPasswordRequest request)
        => new()
        {
            Email = request.Email
        };

    public static ResetPasswordCommand ToCommand(this ResetPasswordRequest request)
        => new()
        {
            Email = request.Email,
            Otp = request.Otp,
            NewPassword = request.NewPassword
        };

    public static ChangePasswordCommand ToCommand(this ChangePasswordRequest request)
        => new()
        {
            CurrentPassword = request.CurrentPassword,
            NewPassword = request.NewPassword,
            ConfirmPassword = request.ConfirmPassword
        };

    public static RefreshTokenCommand ToCommand(this RefreshTokenRequest request)
        => new()
        {
            RefreshToken = request.RefreshToken
        };

    public static LogoutCommand ToCommand(this LogoutRequest request)
        => new()
        {
            RefreshToken = request.RefreshToken
        };

    public static GenerateMfaSetupCommand ToCommand(this MfaSetupRequest request)
        => new()
        {
            Email = request.Email,
            Password = request.Password
        };

    public static VerifyMfaSetupCommand ToCommand(this VerifyMfaSetupRequest request)
        => new()
        {
            Email = request.Email,
            Password = request.Password,
            Code = request.Code
        };

    public static LoginResponse ToResponse(this AuthTokenDto dto)
        => new()
        {
            AccessToken = dto.AccessToken,
            ExpiresAtUtc = dto.ExpiresAtUtc,
            RefreshToken = dto.RefreshToken,
            RefreshTokenExpiresAtUtc = dto.RefreshTokenExpiresAtUtc,
            UserId = dto.UserId,
            Email = dto.Email,
            Role = dto.Role,
            OfficeId = dto.OfficeId,
            RequiresPasswordChange = dto.RequiresPasswordChange
        };

    public static MfaSetupResponse ToResponse(this MfaSetupDto dto)
        => new()
        {
            Secret = dto.Secret,
            OtpAuthUri = dto.OtpAuthUri
        };
}
