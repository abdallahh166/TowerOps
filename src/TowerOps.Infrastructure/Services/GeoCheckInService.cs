using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Services;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Infrastructure.Services;

public sealed class GeoCheckInService : IGeoCheckInService
{
    public GeoCheckInResult CheckIn(Visit visit, GeoLocation engineerLocation, GeoLocation siteLocation, decimal allowedRadiusMeters)
    {
        var distance = engineerLocation.DistanceTo(siteLocation);
        var isWithinSiteRadius = distance <= allowedRadiusMeters;

        visit.RecordCheckIn(engineerLocation, distance, isWithinSiteRadius);
        return new GeoCheckInResult(distance, isWithinSiteRadius);
    }
}
