using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Roles;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.RoleSpecifications;

namespace TowerOps.Application.Queries.Roles.GetAllApplicationRoles;

public sealed class GetAllApplicationRolesQueryHandler : IRequestHandler<GetAllApplicationRolesQuery, Result<IReadOnlyList<ApplicationRoleDto>>>
{
    private readonly IApplicationRoleRepository _roleRepository;

    public GetAllApplicationRolesQueryHandler(IApplicationRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<IReadOnlyList<ApplicationRoleDto>>> Handle(GetAllApplicationRolesQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber.GetValueOrDefault(1);
        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        var pageSize = request.PageSize.GetValueOrDefault(100);
        if (pageSize < 1)
        {
            pageSize = 1;
        }

        if (pageSize > 200)
        {
            pageSize = 200;
        }

        var specification = new AllApplicationRolesSpecification(
            (pageNumber - 1) * pageSize,
            pageSize);
        var roles = await _roleRepository.FindAsNoTrackingAsync(specification, cancellationToken);
        var result = roles
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
