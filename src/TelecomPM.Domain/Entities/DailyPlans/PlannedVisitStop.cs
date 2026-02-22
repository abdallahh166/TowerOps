using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Entities.DailyPlans;

public sealed class PlannedVisitStop
{
    public Guid Id { get; private set; }
    public int Order { get; private set; }
    public string SiteCode { get; private set; } = string.Empty;
    public GeoLocation SiteLocation { get; private set; } = null!;
    public VisitType VisitType { get; private set; }
    public string Priority { get; private set; } = string.Empty;
    public decimal DistanceFromPreviousKm { get; private set; }
    public int EstimatedTravelMinutes { get; private set; }

    private PlannedVisitStop()
    {
        Id = Guid.NewGuid();
    }

    private PlannedVisitStop(
        string siteCode,
        GeoLocation siteLocation,
        VisitType visitType,
        string priority)
    {
        Id = Guid.NewGuid();
        SiteCode = siteCode;
        SiteLocation = siteLocation;
        VisitType = visitType;
        Priority = string.IsNullOrWhiteSpace(priority) ? "P3" : priority.Trim().ToUpperInvariant();
    }

    public static PlannedVisitStop Create(
        string siteCode,
        GeoLocation siteLocation,
        VisitType visitType,
        string priority)
    {
        if (string.IsNullOrWhiteSpace(siteCode))
            throw new DomainException("SiteCode is required.");

        return new PlannedVisitStop(siteCode.Trim().ToUpperInvariant(), siteLocation, visitType, priority);
    }

    public void UpdateRouting(int order, decimal distanceFromPreviousKm, int estimatedTravelMinutes)
    {
        Order = order;
        DistanceFromPreviousKm = distanceFromPreviousKm;
        EstimatedTravelMinutes = estimatedTravelMinutes;
    }
}
