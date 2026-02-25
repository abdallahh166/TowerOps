using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Portal;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Portal.GetPortalDashboard;

public sealed class GetPortalDashboardQueryHandler : IRequestHandler<GetPortalDashboardQuery, Result<PortalDashboardDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IPortalReadRepository _portalReadRepository;

    public GetPortalDashboardQueryHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IPortalReadRepository portalReadRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _portalReadRepository = portalReadRepository;
    }

    public async Task<Result<PortalDashboardDto>> Handle(GetPortalDashboardQuery request, CancellationToken cancellationToken)
    {
        var portalUser = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (portalUser is null || !portalUser.IsClientPortalUser || string.IsNullOrWhiteSpace(portalUser.ClientCode))
            return Result.Failure<PortalDashboardDto>("Portal access is not enabled for this user.");

        var dashboard = await _portalReadRepository.GetDashboardAsync(portalUser.ClientCode, cancellationToken);
        return Result.Success(dashboard);
    }
}
