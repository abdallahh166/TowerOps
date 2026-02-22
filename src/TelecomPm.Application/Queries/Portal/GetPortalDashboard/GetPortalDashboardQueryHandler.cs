using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Portal;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.Portal.GetPortalDashboard;

public sealed class GetPortalDashboardQueryHandler : IRequestHandler<GetPortalDashboardQuery, Result<PortalDashboardDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IWorkOrderRepository _workOrderRepository;

    public GetPortalDashboardQueryHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        ISiteRepository siteRepository,
        IWorkOrderRepository workOrderRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _siteRepository = siteRepository;
        _workOrderRepository = workOrderRepository;
    }

    public async Task<Result<PortalDashboardDto>> Handle(GetPortalDashboardQuery request, CancellationToken cancellationToken)
    {
        var portalUser = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (portalUser is null || !portalUser.IsClientPortalUser || string.IsNullOrWhiteSpace(portalUser.ClientCode))
            return Result.Failure<PortalDashboardDto>("Portal access is not enabled for this user.");

        var sites = (await _siteRepository.GetAllAsNoTrackingAsync(cancellationToken))
            .Where(s => string.Equals(s.ClientCode, portalUser.ClientCode, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var workOrders = (await _workOrderRepository.GetAllAsNoTrackingAsync(cancellationToken))
            .Where(w => sites.Any(s => s.SiteCode.Value == w.SiteCode))
            .ToList();

        var totalSites = sites.Count;
        var onAirPercent = totalSites == 0
            ? 0m
            : decimal.Round(sites.Count(s => s.Status == SiteStatus.OnAir) * 100m / totalSites, 2);

        var closed = workOrders.Where(w => w.Status == WorkOrderStatus.Closed).ToList();
        var compliant = closed.Count(w => DateTime.UtcNow <= w.ResolutionDeadlineUtc || w.SlaClass == SlaClass.P4);
        var slaCompliance = closed.Count == 0
            ? 0m
            : decimal.Round(compliant * 100m / closed.Count, 2);

        var pendingCmCount = workOrders.Count(w =>
            w.Status != WorkOrderStatus.Closed &&
            w.Status != WorkOrderStatus.Cancelled &&
            (w.SlaClass == SlaClass.P1 || w.SlaClass == SlaClass.P2 || w.SlaClass == SlaClass.P3));

        var overdueBmCount = workOrders.Count(w =>
            w.SlaClass == SlaClass.P4 &&
            w.Status != WorkOrderStatus.Closed &&
            w.Status != WorkOrderStatus.Cancelled &&
            DateTime.UtcNow > w.ResolutionDeadlineUtc);

        return Result.Success(new PortalDashboardDto
        {
            TotalSites = totalSites,
            OnAirPercent = onAirPercent,
            SlaComplianceRatePercent = slaCompliance,
            PendingCmCount = pendingCmCount,
            OverdueBmCount = overdueBmCount
        });
    }
}
