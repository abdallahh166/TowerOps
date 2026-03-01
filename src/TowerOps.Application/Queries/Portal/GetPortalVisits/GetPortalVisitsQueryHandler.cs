using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Portal;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Portal.GetPortalVisits;

public sealed class GetPortalVisitsQueryHandler : IRequestHandler<GetPortalVisitsQuery, Result<PaginatedList<PortalVisitDto>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly ISystemSettingsService _settingsService;
    private readonly IPortalReadRepository _portalReadRepository;

    public GetPortalVisitsQueryHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        ISystemSettingsService settingsService,
        IPortalReadRepository portalReadRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _settingsService = settingsService;
        _portalReadRepository = portalReadRepository;
    }

    public async Task<Result<PaginatedList<PortalVisitDto>>> Handle(GetPortalVisitsQuery request, CancellationToken cancellationToken)
    {
        var portalUser = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (portalUser is null || !portalUser.IsClientPortalUser || string.IsNullOrWhiteSpace(portalUser.ClientCode))
            return Result.Failure<PaginatedList<PortalVisitDto>>("Portal access is not enabled for this user.");

        var siteExistsForClient = await _portalReadRepository.SiteExistsForClientAsync(
            portalUser.ClientCode,
            request.SiteCode,
            cancellationToken);

        if (!siteExistsForClient)
            return Result.Failure<PaginatedList<PortalVisitDto>>("Site not found.");

        var anonymizeEngineers = await _settingsService.GetAsync("Portal:AnonymizeEngineers", true, cancellationToken);
        var safePageNumber = request.Page < 1 ? 1 : request.Page;
        var safePageSize = Math.Clamp(request.PageSize, 1, 100);
        var sortBy = string.IsNullOrWhiteSpace(request.SortBy) ? "scheduledDate" : request.SortBy.Trim();

        var totalCount = await _portalReadRepository.CountVisitsAsync(
            portalUser.ClientCode,
            request.SiteCode,
            cancellationToken);

        var result = await _portalReadRepository.GetVisitsAsync(
            portalUser.ClientCode,
            request.SiteCode,
            safePageNumber,
            safePageSize,
            sortBy,
            request.SortDescending,
            anonymizeEngineers,
            cancellationToken);

        var paged = new PaginatedList<PortalVisitDto>(result.ToList(), totalCount, safePageNumber, safePageSize);
        return Result.Success(paged);
    }
}
