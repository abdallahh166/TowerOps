using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Portal;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.Portal.GetPortalSites;

public sealed class GetPortalSitesQueryHandler : IRequestHandler<GetPortalSitesQuery, Result<IReadOnlyList<PortalSiteDto>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly IWorkOrderRepository _workOrderRepository;

    public GetPortalSitesQueryHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        ISiteRepository siteRepository,
        IVisitRepository visitRepository,
        IWorkOrderRepository workOrderRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _siteRepository = siteRepository;
        _visitRepository = visitRepository;
        _workOrderRepository = workOrderRepository;
    }

    public async Task<Result<IReadOnlyList<PortalSiteDto>>> Handle(GetPortalSitesQuery request, CancellationToken cancellationToken)
    {
        var portalUser = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (portalUser is null || !portalUser.IsClientPortalUser || string.IsNullOrWhiteSpace(portalUser.ClientCode))
            return Result.Failure<IReadOnlyList<PortalSiteDto>>("Portal access is not enabled for this user.");

        var sites = await _siteRepository.GetAllAsNoTrackingAsync(cancellationToken);
        var visits = await _visitRepository.GetAllAsNoTrackingAsync(cancellationToken);
        var workOrders = await _workOrderRepository.GetAllAsNoTrackingAsync(cancellationToken);
        var now = DateTime.UtcNow;

        var filteredSites = sites
            .Where(s => string.Equals(s.ClientCode, portalUser.ClientCode, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(request.SiteCode))
        {
            filteredSites = filteredSites.Where(s => string.Equals(s.SiteCode.Value, request.SiteCode, StringComparison.OrdinalIgnoreCase));
        }

        var result = filteredSites
            .Select(site =>
            {
                var siteVisits = visits
                    .Where(v => v.SiteId == site.Id)
                    .OrderByDescending(v => v.ScheduledDate)
                    .ToList();

                var latestVisit = siteVisits.FirstOrDefault();

                var siteWorkOrders = workOrders.Where(w => w.SiteCode == site.SiteCode.Value).ToList();
                var openWorkOrders = siteWorkOrders.Count(w => w.Status != WorkOrderStatus.Closed && w.Status != WorkOrderStatus.Cancelled);
                var breached = siteWorkOrders.Count(w => now > w.ResolutionDeadlineUtc && w.Status != WorkOrderStatus.Closed && w.Status != WorkOrderStatus.Cancelled);

                return new PortalSiteDto
                {
                    SiteCode = site.SiteCode.Value,
                    Name = site.Name,
                    Status = site.Status,
                    Region = site.Region,
                    LastVisitDate = latestVisit?.ScheduledDate,
                    LastVisitType = latestVisit?.Type,
                    OpenWorkOrdersCount = openWorkOrders,
                    BreachedSlaCount = breached
                };
            })
            .OrderBy(x => x.SiteCode)
            .ToList();

        return Result.Success<IReadOnlyList<PortalSiteDto>>(result);
    }
}
