using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Roles;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Roles.GetApplicationRoleById;

public sealed class GetApplicationRoleByIdQueryHandler : IRequestHandler<GetApplicationRoleByIdQuery, Result<ApplicationRoleDto>>
{
    private readonly IApplicationRoleRepository _roleRepository;

    public GetApplicationRoleByIdQueryHandler(IApplicationRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<ApplicationRoleDto>> Handle(GetApplicationRoleByIdQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
            return Result.Failure<ApplicationRoleDto>("Role id is required.");

        var role = await _roleRepository.GetByIdAsNoTrackingAsync(request.Id.Trim(), cancellationToken);
        if (role is null)
            return Result.Failure<ApplicationRoleDto>("Role not found.");

        return Result.Success(MapToDto(role));
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
