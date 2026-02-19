namespace TelecomPm.Api.Mappings;

using TelecomPm.Api.Contracts.Users;
using TelecomPM.Application.Commands.Users.ChangeUserRole;
using TelecomPM.Application.Commands.Users.CreateUser;
using TelecomPM.Application.Commands.Users.UpdateUser;
using TelecomPM.Application.Queries.Users.GetUserPerformance;
using TelecomPM.Application.Queries.Users.GetUsersByRole;
using TelecomPM.Domain.Enums;

public static class UsersContractMapper
{
    public static CreateUserCommand ToCommand(this CreateUserRequest request)
        => new()
        {
            Name = request.Name,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Role = request.Role,
            OfficeId = request.OfficeId,
            MaxAssignedSites = request.MaxAssignedSites,
            Specializations = request.Specializations
        };

    public static UpdateUserCommand ToCommand(this UpdateUserRequest request, Guid userId)
        => new()
        {
            UserId = userId,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber
        };

    public static ChangeUserRoleCommand ToCommand(this ChangeUserRoleRequest request, Guid userId)
        => new()
        {
            UserId = userId,
            NewRole = request.NewRole
        };

    public static GetUsersByRoleQuery ToQuery(this UserRole role)
        => new()
        {
            Role = role
        };

    public static GetUserPerformanceQuery ToQuery(this Guid userId, DateTime? fromDate, DateTime? toDate)
        => new()
        {
            UserId = userId,
            FromDate = fromDate,
            ToDate = toDate
        };
}
