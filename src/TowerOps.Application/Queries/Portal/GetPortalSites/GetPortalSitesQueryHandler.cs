using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Portal;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Portal.GetPortalSites;

public sealed class GetPortalSitesQueryHandler : IRequestHandler<GetPortalSitesQuery, Result<IReadOnlyList<PortalSiteDto>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IPortalReadRepository _portalReadRepository;

    public GetPortalSitesQueryHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IPortalReadRepository portalReadRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _portalReadRepository = portalReadRepository;
    }

    public async Task<Result<IReadOnlyList<PortalSiteDto>>> Handle(GetPortalSitesQuery request, CancellationToken cancellationToken)
    {
        var portalUser = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (portalUser is null || !portalUser.IsClientPortalUser || string.IsNullOrWhiteSpace(portalUser.ClientCode))
            return Result.Failure<IReadOnlyList<PortalSiteDto>>("Portal access is not enabled for this user.");

        var safePageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var safePageSize = Math.Clamp(request.PageSize, 1, 200);

        var result = await _portalReadRepository.GetSitesAsync(
            portalUser.ClientCode,
            request.SiteCode,
            safePageNumber,
            safePageSize,
            cancellationToken);

        return Result.Success(result);
    }
}
