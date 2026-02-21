namespace TelecomPM.Application.Commands.Users.CreateUser;

using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Users;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Interfaces.Repositories;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IOfficeRepository _officeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IEmailService _emailService;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IOfficeRepository officeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordHasher<User> passwordHasher,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _officeRepository = officeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
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

            var temporaryPassword = GenerateTemporaryPassword();
            user.SetPassword(temporaryPassword, _passwordHasher);
            user.RequirePasswordChange();

            // Set engineer-specific properties if role is PMEngineer
            if (request.Role == Domain.Enums.UserRole.PMEngineer && request.MaxAssignedSites.HasValue)
            {
                user.SetEngineerCapacity(
                    request.MaxAssignedSites.Value,
                    request.Specializations ?? new List<string>());
            }

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            try
            {
                var emailBody =
                    $"<p>Welcome to TelecomPM.</p><p>Email: <b>{request.Email}</b></p><p>Temporary Password: <b>{temporaryPassword}</b></p><p>Please login and change your password immediately.</p>";
                await _emailService.SendEmailAsync(request.Email, "TelecomPM User Invitation", emailBody, cancellationToken);
            }
            catch
            {
                // Invitation mail failures should not roll back user creation.
            }

            var dto = _mapper.Map<UserDto>(user);
            dto = dto with { OfficeName = office.Name };

            return Result.Success(dto);
        }
        catch (System.Exception ex)
        {
            return Result.Failure<UserDto>($"Failed to create user: {ex.Message}");
        }
    }

    private static string GenerateTemporaryPassword()
    {
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string numbers = "0123456789";
        const string symbols = "!@#$%^&*";

        var all = uppercase + lowercase + numbers + symbols;
        Span<char> chars = stackalloc char[12];

        chars[0] = uppercase[RandomNumberGenerator.GetInt32(uppercase.Length)];
        chars[1] = lowercase[RandomNumberGenerator.GetInt32(lowercase.Length)];
        chars[2] = numbers[RandomNumberGenerator.GetInt32(numbers.Length)];
        chars[3] = symbols[RandomNumberGenerator.GetInt32(symbols.Length)];

        for (var i = 4; i < chars.Length; i++)
        {
            chars[i] = all[RandomNumberGenerator.GetInt32(all.Length)];
        }

        // Shuffle to avoid predictable prefix
        for (var i = chars.Length - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        return new string(chars);
    }
}
