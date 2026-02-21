namespace TelecomPM.Application.Commands.Auth.ResetPassword;

using MediatR;
using Microsoft.AspNetCore.Identity;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Interfaces.Repositories;

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IOtpService _otpService;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IOtpService otpService,
        IPasswordHasher<User> passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _otpService = otpService;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            return Result.Failure("Invalid or expired OTP.");
        }

        var token = await _passwordResetTokenRepository.GetLatestByEmailAsync(email, cancellationToken);
        if (token is null || token.IsUsed || token.IsExpired(DateTime.UtcNow))
        {
            return Result.Failure("Invalid or expired OTP.");
        }

        var otpValid = _otpService.VerifyOtp(request.Otp, token.HashedOtp);
        if (!otpValid)
        {
            return Result.Failure("Invalid or expired OTP.");
        }

        user.SetPassword(request.NewPassword, _passwordHasher);
        user.ClearPasswordChangeRequirement();
        token.MarkUsed();

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _passwordResetTokenRepository.UpdateAsync(token, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
