using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Commands.Roles.DeleteApplicationRole;

public sealed class DeleteApplicationRoleCommandHandler : IRequestHandler<DeleteApplicationRoleCommand, Result>
{
    private readonly IApplicationRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteApplicationRoleCommandHandler(
        IApplicationRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteApplicationRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _roleRepository.GetByIdAsync(request.Id.Trim(), cancellationToken);
        if (role is null)
            return Result.Failure("Role not found.");

        if (!role.CanBeDeleted())
            return Result.Failure("System roles cannot be deleted.");

        await _roleRepository.DeleteAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
