using MediatR;
using Microsoft.AspNetCore.Identity;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.Auth.VerifyMfaSetup;

public sealed class VerifyMfaSetupCommandHandler : IRequestHandler<VerifyMfaSetupCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IMfaService _mfaService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public VerifyMfaSetupCommandHandler(
        IUserRepository userRepository,
        IMfaService mfaService,
        IPasswordHasher<User> passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _mfaService = mfaService;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(VerifyMfaSetupCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null || !user.IsActive)
            return Result.Failure("Unauthorized.");

        if (!user.VerifyPassword(request.Password, _passwordHasher))
            return Result.Failure("Unauthorized.");

        if (string.IsNullOrWhiteSpace(user.MfaSecret))
            return Result.Failure("MFA setup not initialized.");

        if (!_mfaService.VerifyCode(user.MfaSecret, request.Code, DateTime.UtcNow))
            return Result.Failure("Invalid MFA code.");

        user.ConfigureMfa(user.MfaSecret, enabled: true);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
