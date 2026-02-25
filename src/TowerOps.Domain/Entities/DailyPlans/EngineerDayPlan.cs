using TowerOps.Domain.Enums;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Domain.Entities.DailyPlans;

public sealed class EngineerDayPlan
{
    private readonly List<PlannedVisitStop> _stops = new();

    public Guid EngineerId { get; private set; }
    public decimal TotalEstimatedDistanceKm { get; private set; }
    public int TotalEstimatedTravelMinutes { get; private set; }
    public IReadOnlyCollection<PlannedVisitStop> Stops => _stops.AsReadOnly();

    private EngineerDayPlan()
    {
    }

    private EngineerDayPlan(Guid engineerId)
    {
        EngineerId = engineerId;
    }

    public static EngineerDayPlan Create(Guid engineerId)
    {
        if (engineerId == Guid.Empty)
            throw new DomainException("EngineerId is required.", "EngineerDayPlan.EngineerId.Required");

        return new EngineerDayPlan(engineerId);
    }

    public void AddStop(string siteCode, GeoLocation siteLocation, VisitType visitType, string priority)
    {
        if (_stops.Any(s => string.Equals(s.SiteCode, siteCode, StringComparison.OrdinalIgnoreCase)))
            return;

        _stops.Add(PlannedVisitStop.Create(siteCode, siteLocation, visitType, priority));
    }

    public void RemoveStop(string siteCode)
    {
        var existing = _stops.FirstOrDefault(s => string.Equals(s.SiteCode, siteCode, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            _stops.Remove(existing);
        }
    }

    public IReadOnlyList<PlannedVisitStop> SuggestOrder(decimal averageSpeedKmh)
    {
        if (_stops.Count == 0)
            return Array.Empty<PlannedVisitStop>();

        var ordered = new List<PlannedVisitStop>();
        GeoLocation? previous = null;

        foreach (var bucket in GetBucketsInPriorityOrder())
        {
            var remaining = bucket.ToList();
            while (remaining.Count > 0)
            {
                PlannedVisitStop next;
                if (previous is null)
                {
                    next = remaining[0];
                }
                else
                {
                    next = remaining
                        .OrderBy(x => previous.DistanceTo(x.SiteLocation))
                        .First();
                }

                ordered.Add(next);
                previous = next.SiteLocation;
                remaining.Remove(next);
            }
        }

        var totalDistanceKm = 0m;
        var totalMinutes = 0;
        GeoLocation? prevPoint = null;
        var order = 1;

        foreach (var stop in ordered)
        {
            var distanceKm = 0m;
            if (prevPoint is not null)
            {
                distanceKm = decimal.Round(prevPoint.DistanceTo(stop.SiteLocation) / 1000m, 3);
            }

            var minutes = distanceKm <= 0m
                ? 0
                : (int)Math.Round((distanceKm / averageSpeedKmh) * 60m, MidpointRounding.AwayFromZero);

            stop.UpdateRouting(order++, distanceKm, minutes);
            totalDistanceKm += distanceKm;
            totalMinutes += minutes;
            prevPoint = stop.SiteLocation;
        }

        TotalEstimatedDistanceKm = decimal.Round(totalDistanceKm, 3);
        TotalEstimatedTravelMinutes = totalMinutes;

        return ordered;
    }

    private IEnumerable<IReadOnlyList<PlannedVisitStop>> GetBucketsInPriorityOrder()
    {
        var p1 = _stops.Where(s => s.Priority.Equals("P1", StringComparison.OrdinalIgnoreCase)).ToList();
        var p2 = _stops.Where(s => s.Priority.Equals("P2", StringComparison.OrdinalIgnoreCase)).ToList();
        var normal = _stops.Where(s =>
            !s.Priority.Equals("P1", StringComparison.OrdinalIgnoreCase) &&
            !s.Priority.Equals("P2", StringComparison.OrdinalIgnoreCase) &&
            !s.Priority.Equals("BM", StringComparison.OrdinalIgnoreCase))
            .ToList();
        var bm = _stops.Where(s => s.Priority.Equals("BM", StringComparison.OrdinalIgnoreCase)).ToList();

        if (p1.Count > 0) yield return p1;
        if (p2.Count > 0) yield return p2;
        if (normal.Count > 0) yield return normal;
        if (bm.Count > 0) yield return bm;
    }
}
