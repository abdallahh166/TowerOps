using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Commands.Auth.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IRefreshTokenService refreshTokenService,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenService = refreshTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = _refreshTokenService.HashToken(request.RefreshToken);
        var token = await _refreshTokenRepository.GetActiveByTokenHashAsync(tokenHash, cancellationToken);
        if (token is not null)
        {
            token.Revoke("Logout");
            await _refreshTokenRepository.UpdateAsync(token, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }
}
