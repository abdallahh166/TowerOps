namespace TelecomPM.Application.Commands.Auth.ForgotPassword;

using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Domain.Entities.PasswordResetTokens;
using TelecomPM.Domain.Interfaces.Repositories;

public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IEmailService _emailService;
    private readonly IOtpService _otpService;
    private readonly IUnitOfWork _unitOfWork;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IEmailService emailService,
        IOtpService otpService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _emailService = emailService;
        _otpService = otpService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            return Result.Success();
        }

        var otp = _otpService.GenerateOtp();
        var hashedOtp = _otpService.HashOtp(otp);

        var token = PasswordResetToken.Create(
            email,
            hashedOtp,
            DateTime.UtcNow.AddMinutes(15));

        await _passwordResetTokenRepository.AddAsync(token, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            var body = $"<p>Your TelecomPM OTP is <b>{otp}</b>.</p><p>It expires in 15 minutes.</p>";
            await _emailService.SendEmailAsync(email, "TelecomPM Password Reset OTP", body, cancellationToken);
        }
        catch
        {
            // Intentionally swallow send failures to keep endpoint behavior non-disclosing.
        }

        return Result.Success();
    }
}
