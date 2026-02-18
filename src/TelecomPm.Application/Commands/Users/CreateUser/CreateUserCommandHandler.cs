namespace TelecomPM.Application.Commands.Users.CreateUser;

using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Users;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Interfaces.Repositories;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateUserCommandHandler(
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

    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Validate office exists
        var office = await _officeRepository.GetByIdAsync(request.OfficeId, cancellationToken);
        if (office == null)
            return Result.Failure<UserDto>("Office not found");

        // Check if email already exists
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
            return Result.Failure<UserDto>($"User with email {request.Email} already exists");

        try
        {
            var user = User.Create(
                request.Name,
                request.Email,
                request.PhoneNumber,
                request.Role,
                request.OfficeId);

            // Set engineer-specific properties if role is PMEngineer
            if (request.Role == Domain.Enums.UserRole.PMEngineer && request.MaxAssignedSites.HasValue)
            {
                user.SetEngineerCapacity(
                    request.MaxAssignedSites.Value,
                    request.Specializations ?? new List<string>());
            }

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<UserDto>(user);
            dto = dto with { OfficeName = office.Name };

            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<UserDto>($"Failed to create user: {ex.Message}");
        }
    }
}

