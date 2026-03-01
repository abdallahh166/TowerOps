using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.DTOs.Portal;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.Portal.GetPortalWorkOrders;

public sealed class GetPortalWorkOrdersQueryHandler : IRequestHandler<GetPortalWorkOrdersQuery, Result<PaginatedList<PortalWorkOrderDto>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserRepository _userRepository;
    private readonly IPortalReadRepository _portalReadRepository;

    public GetPortalWorkOrdersQueryHandler(
        ICurrentUserService currentUserService,
        IUserRepository userRepository,
        IPortalReadRepository portalReadRepository)
    {
        _currentUserService = currentUserService;
        _userRepository = userRepository;
        _portalReadRepository = portalReadRepository;
    }

    public async Task<Result<PaginatedList<PortalWorkOrderDto>>> Handle(GetPortalWorkOrdersQuery request, CancellationToken cancellationToken)
    {
        var portalUser = await _userRepository.GetByIdAsNoTrackingAsync(_currentUserService.UserId, cancellationToken);
        if (portalUser is null || !portalUser.IsClientPortalUser || string.IsNullOrWhiteSpace(portalUser.ClientCode))
            return Result.Failure<PaginatedList<PortalWorkOrderDto>>("Portal access is not enabled for this user.");

        var safePageNumber = request.Page < 1 ? 1 : request.Page;
        var safePageSize = Math.Clamp(request.PageSize, 1, 100);
        var sortBy = string.IsNullOrWhiteSpace(request.SortBy) ? "createdAt" : request.SortBy.Trim();

        var totalCount = await _portalReadRepository.CountWorkOrdersAsync(
            portalUser.ClientCode,
            cancellationToken);

        var workOrders = await _portalReadRepository.GetWorkOrdersAsync(
            portalUser.ClientCode,
            safePageNumber,
            safePageSize,
            sortBy,
            request.SortDescending,
            cancellationToken);

        var paged = new PaginatedList<PortalWorkOrderDto>(workOrders.ToList(), totalCount, safePageNumber, safePageSize);
        return Result.Success(paged);
    }
}
