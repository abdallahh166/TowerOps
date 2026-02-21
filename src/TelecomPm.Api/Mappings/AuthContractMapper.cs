namespace TelecomPm.Api.Mappings;

using TelecomPm.Api.Contracts.Auth;
using TelecomPM.Application.Commands.Auth.ChangePassword;
using TelecomPM.Application.Commands.Auth.ForgotPassword;
using TelecomPM.Application.Commands.Auth.Login;
using TelecomPM.Application.Commands.Auth.ResetPassword;
using TelecomPM.Application.DTOs.Auth;

public static class AuthContractMapper
{
    public static LoginCommand ToCommand(this LoginRequest request)
        => new()
        {
            Email = request.Email,
            Password = request.Password
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

    public static LoginResponse ToResponse(this AuthTokenDto dto)
        => new()
        {
            AccessToken = dto.AccessToken,
            ExpiresAtUtc = dto.ExpiresAtUtc,
            UserId = dto.UserId,
            Email = dto.Email,
            Role = dto.Role,
            OfficeId = dto.OfficeId,
            RequiresPasswordChange = dto.RequiresPasswordChange
        };
}
