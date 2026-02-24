using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Entities.DailyPlans;

public sealed class DailyPlan : AggregateRoot<Guid>
{
    private readonly List<EngineerDayPlan> _engineerPlans = new();

    public Guid OfficeId { get; private set; }
    public DateOnly PlanDate { get; private set; }
    public Guid OfficeManagerId { get; private set; }
    public DailyPlanStatus Status { get; private set; }
    public IReadOnlyCollection<EngineerDayPlan> EngineerPlans => _engineerPlans.AsReadOnly();

    private DailyPlan() : base()
    {
    }

    private DailyPlan(Guid officeId, DateOnly planDate, Guid officeManagerId) : base(Guid.NewGuid())
    {
        OfficeId = officeId;
        PlanDate = planDate;
        OfficeManagerId = officeManagerId;
        Status = DailyPlanStatus.Draft;
    }

    public static DailyPlan Create(Guid officeId, DateOnly planDate, Guid officeManagerId)
    {
        if (officeId == Guid.Empty)
            throw new DomainException("OfficeId is required.", "DailyPlan.OfficeId.Required");

        if (officeManagerId == Guid.Empty)
            throw new DomainException("OfficeManagerId is required.", "DailyPlan.OfficeManagerId.Required");

        return new DailyPlan(officeId, planDate, officeManagerId);
    }

    public void AssignSiteToEngineer(
        Guid engineerId,
        string siteCode,
        GeoLocation siteLocation,
        VisitType visitType,
        string priority)
    {
        EnsureModifiable();

        if (engineerId == Guid.Empty)
            throw new DomainException("EngineerId is required.", "DailyPlan.EngineerId.Required");

        if (string.IsNullOrWhiteSpace(siteCode))
            throw new DomainException("SiteCode is required.", "DailyPlan.SiteCode.Required");

        // Reassign support: ensure a site is assigned to one engineer only.
        foreach (var plan in _engineerPlans)
        {
            plan.RemoveStop(siteCode);
        }

        var engineerPlan = _engineerPlans.FirstOrDefault(p => p.EngineerId == engineerId);
        if (engineerPlan is null)
        {
            engineerPlan = EngineerDayPlan.Create(engineerId);
            _engineerPlans.Add(engineerPlan);
        }

        engineerPlan.AddStop(siteCode, siteLocation, visitType, priority);
    }

    public void RemoveSiteFromEngineer(Guid engineerId, string siteCode)
    {
        EnsureModifiable();

        var engineerPlan = _engineerPlans.FirstOrDefault(p => p.EngineerId == engineerId);
        if (engineerPlan is null)
            return;

        engineerPlan.RemoveStop(siteCode);
    }

    public IReadOnlyList<PlannedVisitStop> SuggestOrder(Guid engineerId, decimal averageSpeedKmh)
    {
        if (averageSpeedKmh <= 0)
            throw new DomainException("Average speed must be greater than zero.", "DailyPlan.AverageSpeed.Positive");

        var engineerPlan = _engineerPlans.FirstOrDefault(p => p.EngineerId == engineerId);
        if (engineerPlan is null)
            return Array.Empty<PlannedVisitStop>();

        return engineerPlan.SuggestOrder(averageSpeedKmh);
    }

    public IReadOnlyList<string> GetAssignedSiteCodes()
    {
        return _engineerPlans
            .SelectMany(x => x.Stops.Select(s => s.SiteCode))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public void Publish()
    {
        EnsureModifiable();
        Status = DailyPlanStatus.Published;
        MarkAsUpdated("OfficeManager");
    }

    private void EnsureModifiable()
    {
        if (Status == DailyPlanStatus.Published || Status == DailyPlanStatus.InProgress || Status == DailyPlanStatus.Completed)
            throw new DomainException("Published or completed plans cannot be modified.", "DailyPlan.Modification.NotAllowedInCurrentStatus");
    }
}
