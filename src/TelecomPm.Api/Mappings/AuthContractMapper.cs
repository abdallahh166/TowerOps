namespace TelecomPm.Api.Mappings;

using TelecomPm.Api.Contracts.Auth;
using TelecomPM.Application.Commands.Auth.Login;
using TelecomPM.Application.DTOs.Auth;

public static class AuthContractMapper
{
    public static LoginCommand ToCommand(this LoginRequest request)
        => new()
        {
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

    public static LoginResponse ToResponse(this AuthTokenDto dto)
        => new()
        {
            AccessToken = dto.AccessToken,
            ExpiresAtUtc = dto.ExpiresAtUtc,
            UserId = dto.UserId,
            Email = dto.Email,
            Role = dto.Role,
            OfficeId = dto.OfficeId
        };
}
