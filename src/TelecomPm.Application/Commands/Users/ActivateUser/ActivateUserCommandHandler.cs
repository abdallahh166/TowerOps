namespace TelecomPM.Application.Commands.Users.ActivateUser;

using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Users;
using TelecomPM.Domain.Interfaces.Repositories;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ActivateUserCommandHandler(
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

    public async Task<Result<UserDto>> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result.Failure<UserDto>("User not found");

        try
        {
            user.Activate();
            await _userRepository.UpdateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var office = await _officeRepository.GetByIdAsNoTrackingAsync(user.OfficeId, cancellationToken);
            var dto = _mapper.Map<UserDto>(user);
            dto = dto with { OfficeName = office?.Name ?? string.Empty };

            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<UserDto>($"Failed to activate user: {ex.Message}");
        }
    }
}

