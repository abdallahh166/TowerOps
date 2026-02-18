namespace TelecomPM.Application.Commands.Users.ChangeUserRole;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Users;
using TelecomPM.Domain.Interfaces.Repositories;

public class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ChangeUserRoleCommandHandler(
        IUserRepository userRepository,
        IOfficeRepository officeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _officeRepository = officeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<UserDto>> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result.Failure<UserDto>("User not found");

        if (user.Role == request.NewRole)
            return Result.Failure<UserDto>("User already has this role");

        try
        {
            var oldRole = user.Role;
            user.UpdateRole(request.NewRole);

            // Clear engineer-specific data if changing from PMEngineer
            if (oldRole == Domain.Enums.UserRole.PMEngineer && request.NewRole != Domain.Enums.UserRole.PMEngineer)
            {
                // Clear site assignments and specializations
                foreach (var siteId in user.AssignedSiteIds.ToList())
                {
                    user.UnassignSite(siteId);
                }
                user.SetEngineerCapacity(0, new List<string>());
            }

            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var office = await _officeRepository.GetByIdAsNoTrackingAsync(user.OfficeId, cancellationToken);
            var dto = _mapper.Map<UserDto>(user);
            dto = dto with { OfficeName = office?.Name ?? string.Empty };

            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<UserDto>($"Failed to change user role: {ex.Message}");
        }
    }
}

