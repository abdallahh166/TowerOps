using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Services;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Infrastructure.Services;

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
