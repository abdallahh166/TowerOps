using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Roles;
using TelecomPM.Domain.Entities.ApplicationRoles;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Roles.CreateApplicationRole;

public sealed class CreateApplicationRoleCommandHandler : IRequestHandler<CreateApplicationRoleCommand, Result<ApplicationRoleDto>>
{
    private readonly IApplicationRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateApplicationRoleCommandHandler(
        IApplicationRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ApplicationRoleDto>> Handle(CreateApplicationRoleCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        var existing = await _roleRepository.GetByIdAsync(name, cancellationToken);
        if (existing is not null)
            return Result.Failure<ApplicationRoleDto>($"Role '{name}' already exists.");

        var role = ApplicationRole.Create(
            name,
            request.DisplayName,
            request.Description,
            isSystem: false,
            isActive: request.IsActive,
            request.Permissions);

        await _roleRepository.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(MapToDto(role));
    }

    private static ApplicationRoleDto MapToDto(ApplicationRole role)
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
