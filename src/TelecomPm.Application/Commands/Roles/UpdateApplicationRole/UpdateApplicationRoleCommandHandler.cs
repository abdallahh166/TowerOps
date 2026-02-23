using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Roles;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Roles.UpdateApplicationRole;

public sealed class UpdateApplicationRoleCommandHandler : IRequestHandler<UpdateApplicationRoleCommand, Result<ApplicationRoleDto>>
{
    private readonly IApplicationRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateApplicationRoleCommandHandler(
        IApplicationRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ApplicationRoleDto>> Handle(UpdateApplicationRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.Id.Trim(), cancellationToken);
        if (role is null)
            return Result.Failure<ApplicationRoleDto>("Role not found.");

        role.Update(
            request.DisplayName,
            request.Description,
            request.IsActive,
            request.Permissions);

        await _roleRepository.UpdateAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
