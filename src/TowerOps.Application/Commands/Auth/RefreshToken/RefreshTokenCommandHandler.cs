using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Auth;
using DomainRefreshToken = TowerOps.Domain.Entities.RefreshTokens.RefreshToken;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.Auth.RefreshToken;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthTokenDto>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AuthTokenDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = _refreshTokenService.HashToken(request.RefreshToken);
        var existing = await _refreshTokenRepository.GetActiveByTokenHashAsync(tokenHash, cancellationToken);
        if (existing is null)
            return Result.Failure<AuthTokenDto>("Unauthorized.");

        var user = await _userRepository.GetByIdAsNoTrackingAsync(existing.UserId, cancellationToken);
        if (user is null || !user.IsActive)
            return Result.Failure<AuthTokenDto>("Unauthorized.");

        var newRefreshTokenValue = _refreshTokenService.GenerateToken();
        var newRefreshTokenHash = _refreshTokenService.HashToken(newRefreshTokenValue);
        var newRefreshExpiry = _refreshTokenService.GetRefreshTokenExpiryUtc();

        var replacement = DomainRefreshToken.Create(user.Id, newRefreshTokenHash, newRefreshExpiry);
        existing.Revoke("Rotated", replacement.Id);

        await _refreshTokenRepository.AddAsync(replacement, cancellationToken);
        await _refreshTokenRepository.UpdateAsync(existing, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var (accessToken, accessExpiry) = await _jwtTokenService.GenerateTokenAsync(user, cancellationToken);
        return Result.Success(new AuthTokenDto
        {
            AccessToken = accessToken,
            ExpiresAtUtc = accessExpiry,
            RefreshToken = newRefreshTokenValue,
            RefreshTokenExpiresAtUtc = newRefreshExpiry,
            UserId = user.Id,
            Email = user.Email,
            Role = user.Role.ToString(),
            OfficeId = user.OfficeId,
            RequiresPasswordChange = user.MustChangePassword
        });
    }
}
