using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Roles;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.Roles.GetAllApplicationRoles;

public sealed class GetAllApplicationRolesQueryHandler : IRequestHandler<GetAllApplicationRolesQuery, Result<IReadOnlyList<ApplicationRoleDto>>>
{
    private readonly IApplicationRoleRepository _roleRepository;

    public GetAllApplicationRolesQueryHandler(IApplicationRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<IReadOnlyList<ApplicationRoleDto>>> Handle(GetAllApplicationRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _roleRepository.GetAllAsNoTrackingAsync(cancellationToken);
        var result = roles
            .OrderBy(r => r.Name)
            .Select(MapToDto)
            .ToList();

        return Result.Success<IReadOnlyList<ApplicationRoleDto>>(result);
    }

    private static ApplicationRoleDto MapToDto(Domain.Entities.ApplicationRoles.ApplicationRole role)
    {
        return new ApplicationRoleDto
        {
            Name = role.Name,
            DisplayName = role.DisplayName,
            Description = role.Description,
            IsSystem = role.IsSystem,
            IsActive = role.IsActive,
            Permissions = role.Permissions.ToList()
        };
    }
}
