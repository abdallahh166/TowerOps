using MediatR;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.Portal;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;

namespace TelecomPM.Application.Queries.Portal.GetPortalWorkOrders;

public sealed class GetPortalWorkOrdersQueryHandler : IRequestHandler<GetPortalWorkOrdersQuery, Result<IReadOnlyList<PortalWorkOrderDto>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IWorkOrderRepository _workOrderRepository;

    public GetPortalWorkOrdersQueryHandler(
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

    public async Task<Result<IReadOnlyList<PortalWorkOrderDto>>> Handle(GetPortalWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var portalUser = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (portalUser is null || !portalUser.IsClientPortalUser || string.IsNullOrWhiteSpace(portalUser.ClientCode))
            return Result.Failure<IReadOnlyList<PortalWorkOrderDto>>("Portal access is not enabled for this user.");

        var sites = await _siteRepository.GetAllAsNoTrackingAsync(cancellationToken);
        var siteCodes = sites
            .Where(s => string.Equals(s.ClientCode, portalUser.ClientCode, StringComparison.OrdinalIgnoreCase))
            .Select(s => s.SiteCode.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var workOrders = (await _workOrderRepository.GetAllAsNoTrackingAsync(cancellationToken))
            .Where(w => siteCodes.Contains(w.SiteCode))
            .Select(w => new PortalWorkOrderDto
            {
                WorkOrderId = w.Id,
                SiteCode = w.SiteCode,
                Status = w.Status,
                Priority = w.SlaClass,
                SlaDeadline = w.ResolutionDeadlineUtc,
                CreatedAt = w.CreatedAt,
                CompletedAt = w.Status == WorkOrderStatus.Closed ? w.UpdatedAt : null
            })
            .OrderByDescending(w => w.CreatedAt)
            .ToList();

        return Result.Success<IReadOnlyList<PortalWorkOrderDto>>(workOrders);
    }
}
