using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Roles;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Specifications.RoleSpecifications;

namespace TowerOps.Application.Queries.Roles.GetAllApplicationRoles;

public sealed class GetAllApplicationRolesQueryHandler : IRequestHandler<GetAllApplicationRolesQuery, Result<PaginatedList<ApplicationRoleDto>>>
{
    private readonly IApplicationRoleRepository _roleRepository;

    public GetAllApplicationRolesQueryHandler(IApplicationRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<Result<PaginatedList<ApplicationRoleDto>>> Handle(GetAllApplicationRolesQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.Page < 1 ? 1 : request.Page;
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var sortBy = string.IsNullOrWhiteSpace(request.SortBy) ? "name" : request.SortBy.Trim();

        var specification = new AllApplicationRolesSpecification(
            (pageNumber - 1) * pageSize,
            pageSize,
            sortBy,
            request.SortDescending);
        var totalCount = await _roleRepository.CountAsync(specification, cancellationToken);
        var roles = await _roleRepository.FindAsNoTrackingAsync(specification, cancellationToken);
        var result = roles
            .Select(MapToDto)
            .ToList();

        var paged = new PaginatedList<ApplicationRoleDto>(result, totalCount, pageNumber, pageSize);
        return Result.Success(paged);
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
