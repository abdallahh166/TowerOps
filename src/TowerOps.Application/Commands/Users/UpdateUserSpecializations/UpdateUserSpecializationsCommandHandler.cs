namespace TowerOps.Application.Commands.Users.UpdateUserSpecializations;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;
using TowerOps.Domain.Interfaces.Repositories;

public class UpdateUserSpecializationsCommandHandler : IRequestHandler<UpdateUserSpecializationsCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateUserSpecializationsCommandHandler(
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

    public async Task<Result<UserDto>> Handle(UpdateUserSpecializationsCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result.Failure<UserDto>("User not found");

        if (user.Role != Domain.Enums.UserRole.PMEngineer)
            return Result.Failure<UserDto>("Only PM Engineers can have specializations");

        try
        {
            user.SetEngineerCapacity(request.MaxAssignedSites, request.Specializations);
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var office = await _officeRepository.GetByIdAsNoTrackingAsync(user.OfficeId, cancellationToken);
            var dto = _mapper.Map<UserDto>(user);
            dto = dto with { OfficeName = office?.Name ?? string.Empty };

            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<UserDto>($"Failed to update user specializations: {ex.Message}");
        }
    }
}

