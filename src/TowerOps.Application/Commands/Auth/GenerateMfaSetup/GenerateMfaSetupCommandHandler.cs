using MediatR;
using Microsoft.AspNetCore.Identity;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Auth;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.Auth.GenerateMfaSetup;

public sealed class GenerateMfaSetupCommandHandler : IRequestHandler<GenerateMfaSetupCommand, Result<MfaSetupDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMfaService _mfaService;
    private readonly ISystemSettingsService _systemSettingsService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public GenerateMfaSetupCommandHandler(
        IUserRepository userRepository,
        IMfaService mfaService,
        ISystemSettingsService systemSettingsService,
        IPasswordHasher<User> passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _mfaService = mfaService;
        _systemSettingsService = systemSettingsService;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<MfaSetupDto>> Handle(GenerateMfaSetupCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null || !user.IsActive)
            return Result.Failure<MfaSetupDto>("Unauthorized.");

        if (!user.VerifyPassword(request.Password, _passwordHasher))
            return Result.Failure<MfaSetupDto>("Unauthorized.");

        var secret = _mfaService.GenerateSecret();
        user.ConfigureMfa(secret, enabled: false);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var issuer = await _systemSettingsService.GetAsync("Company:Name", "TowerOps", cancellationToken);
        var uri = _mfaService.BuildOtpAuthUri(user.Email, issuer, secret);

        return Result.Success(new MfaSetupDto
        {
            Secret = secret,
            OtpAuthUri = uri
        });
    }
}
