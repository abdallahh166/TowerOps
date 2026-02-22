using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Portal;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.Portal.GetPortalSlaReport;

public sealed class GetPortalSlaReportQueryHandler : IRequestHandler<GetPortalSlaReportQuery, Result<PortalSlaReportDto>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IWorkOrderRepository _workOrderRepository;

    public GetPortalSlaReportQueryHandler(
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

    public async Task<Result<PortalSlaReportDto>> Handle(GetPortalSlaReportQuery request, CancellationToken cancellationToken)
    {
        var portalUser = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (portalUser is null || !portalUser.IsClientPortalUser || string.IsNullOrWhiteSpace(portalUser.ClientCode))
            return Result.Failure<PortalSlaReportDto>("Portal access is not enabled for this user.");

        var siteCodes = (await _siteRepository.GetAllAsNoTrackingAsync(cancellationToken))
            .Where(s => string.Equals(s.ClientCode, portalUser.ClientCode, StringComparison.OrdinalIgnoreCase))
            .Select(s => s.SiteCode.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var relevantWorkOrders = (await _workOrderRepository.GetAllAsNoTrackingAsync(cancellationToken))
            .Where(w => siteCodes.Contains(w.SiteCode))
            .ToList();

        var monthly = relevantWorkOrders
            .GroupBy(w => new { w.CreatedAt.Year, w.CreatedAt.Month, w.SlaClass })
            .Select(g =>
            {
                var total = g.Count();
                var breaches = g.Count(x => DateTime.UtcNow > x.ResolutionDeadlineUtc && x.Status != WorkOrderStatus.Closed);
                var compliant = total - breaches;
                var compliancePercent = total == 0 ? 0m : decimal.Round(compliant * 100m / total, 2);
                var avgResponseMinutes = g.Average(x => (decimal)(x.ResponseDeadlineUtc - x.CreatedAt).TotalMinutes);

                return new PortalSlaMonthlyMetricDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    SlaClass = g.Key.SlaClass,
                    CompliancePercent = compliancePercent,
                    BreachCount = breaches,
                    AverageResponseMinutes = decimal.Round(avgResponseMinutes, 2)
                };
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ThenBy(x => x.SlaClass)
            .ToList();

        return Result.Success(new PortalSlaReportDto
        {
            Monthly = monthly
        });
    }
}
