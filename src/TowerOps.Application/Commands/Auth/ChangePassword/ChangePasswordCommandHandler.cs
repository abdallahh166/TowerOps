namespace TowerOps.Application.Commands.Auth.ChangePassword;

using MediatR;
using Microsoft.AspNetCore.Identity;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Interfaces.Repositories;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IPasswordHasher<User> passwordHasher,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
        _refreshTokenRepository = refreshTokenRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
            return Result.Failure("Unauthorized.");

        if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
            return Result.Failure("Confirm password does not match.");

        var user = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken);
        if (user is null)
            return Result.Failure("User not found.");

        if (!user.VerifyPassword(request.CurrentPassword, _passwordHasher))
            return Result.Failure("Current password is incorrect.");

        user.SetPassword(request.NewPassword, _passwordHasher);
        user.ClearPasswordChangeRequirement();

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _refreshTokenRepository.RevokeAllByUserIdAsync(user.Id, "PasswordChanged", cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
