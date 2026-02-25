namespace TowerOps.Application.Commands.Users.AssignUserToOffice;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.Users;
using TowerOps.Domain.Interfaces.Repositories;

public class AssignUserToOfficeCommandHandler : IRequestHandler<AssignUserToOfficeCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AssignUserToOfficeCommandHandler(
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

    public async Task<Result<UserDto>> Handle(AssignUserToOfficeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result.Failure<UserDto>("User not found");

        var office = await _officeRepository.GetByIdAsync(request.OfficeId, cancellationToken);
        if (office == null)
            return Result.Failure<UserDto>("Office not found");

        try
        {
            user.AssignToOffice(request.OfficeId);
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<UserDto>(user);
            dto = dto with { OfficeName = office.Name };

            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<UserDto>($"Failed to assign user to office: {ex.Message}");
        }
    }
}

