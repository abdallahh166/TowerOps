using MediatR;
using TowerOps.Application.Common;
using TowerOps.Application.DTOs.DailyPlans;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;

namespace TowerOps.Application.Queries.DailyPlans.GetUnassignedSites;

public sealed class GetUnassignedSitesQueryHandler : IRequestHandler<GetUnassignedSitesQuery, Result<IReadOnlyList<UnassignedSiteDto>>>
{
    private readonly IDailyPlanRepository _dailyPlanRepository;
    private readonly ISiteRepository _siteRepository;

    public GetUnassignedSitesQueryHandler(
        IDailyPlanRepository dailyPlanRepository,
        ISiteRepository siteRepository)
    {
        _dailyPlanRepository = dailyPlanRepository;
        _siteRepository = siteRepository;
    }

    public async Task<Result<IReadOnlyList<UnassignedSiteDto>>> Handle(GetUnassignedSitesQuery request, CancellationToken cancellationToken)
    {
        var plan = await _dailyPlanRepository.GetByOfficeAndDateAsNoTrackingAsync(request.OfficeId, request.Date, cancellationToken);
        var assignedCodes = plan?.GetAssignedSiteCodes() ?? Array.Empty<string>();

        var officeSites = await _siteRepository.GetByOfficeIdAsNoTrackingAsync(request.OfficeId, cancellationToken);
        var unassigned = officeSites
            .Where(s => s.Status == SiteStatus.OnAir)
            .Where(s => !assignedCodes.Contains(s.SiteCode.Value, StringComparer.OrdinalIgnoreCase))
            .Select(s => new UnassignedSiteDto
            {
                SiteId = s.Id,
                SiteCode = s.SiteCode.Value,
                Name = s.Name
            })
            .OrderBy(s => s.SiteCode)
            .ToList();

        return Result.Success<IReadOnlyList<UnassignedSiteDto>>(unassigned);
    }
}
