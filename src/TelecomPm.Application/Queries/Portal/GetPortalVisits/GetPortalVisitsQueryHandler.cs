using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Portal;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.Portal.GetPortalVisits;

public sealed class GetPortalVisitsQueryHandler : IRequestHandler<GetPortalVisitsQuery, Result<IReadOnlyList<PortalVisitDto>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly ISystemSettingsService _settingsService;
    private readonly IPortalReadRepository _portalReadRepository;

    public GetPortalVisitsQueryHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        ISiteRepository siteRepository,
        ISystemSettingsService settingsService,
        IPortalReadRepository portalReadRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _siteRepository = siteRepository;
        _settingsService = settingsService;
        _portalReadRepository = portalReadRepository;
    }

    public async Task<Result<IReadOnlyList<PortalVisitDto>>> Handle(GetPortalVisitsQuery request, CancellationToken cancellationToken)
    {
        var portalUser = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (portalUser is null || !portalUser.IsClientPortalUser || string.IsNullOrWhiteSpace(portalUser.ClientCode))
            return Result.Failure<IReadOnlyList<PortalVisitDto>>("Portal access is not enabled for this user.");

        var site = await _siteRepository.GetBySiteCodeAsNoTrackingAsync(request.SiteCode, cancellationToken);
        if (site is null || !string.Equals(site.ClientCode, portalUser.ClientCode, StringComparison.OrdinalIgnoreCase))
            return Result.Failure<IReadOnlyList<PortalVisitDto>>("Site not found.");

        var anonymizeEngineers = await _settingsService.GetAsync("Portal:AnonymizeEngineers", true, cancellationToken);
        var safePageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var safePageSize = Math.Clamp(request.PageSize, 1, 200);

        var result = await _portalReadRepository.GetVisitsAsync(
            portalUser.ClientCode,
            request.SiteCode,
            safePageNumber,
            safePageSize,
            anonymizeEngineers,
            cancellationToken);

        return Result.Success<IReadOnlyList<PortalVisitDto>>(result);
    }
}
