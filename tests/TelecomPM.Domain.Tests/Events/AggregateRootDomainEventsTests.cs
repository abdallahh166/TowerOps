using FluentAssertions;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Tests.Events;

public class AggregateRootDomainEventsTests
{
    [Fact]
    public void Aggregate_ShouldCollectAndClearDomainEvents()
    {
        var visit = Visit.Create("V1", Guid.NewGuid(), "TNT001", "S1", Guid.NewGuid(), "Eng", DateTime.Today, VisitType.PreventiveMaintenance);

        visit.StartVisit(Coordinates.Create(30, 30));
        typeof(Visit).GetProperty("ActualStartTime")!.SetValue(visit, DateTime.UtcNow - TimeSpan.FromMinutes(60));
        // Ensure minimal completion requirements
        for (int i = 0; i < 60; i++)
        {
            var type = i < 30 ? PhotoType.Before : PhotoType.After;
            var p = VisitPhoto.Create(visit.Id, type, PhotoCategory.ShelterInside, "", $"f{i}", "/p");
            visit.AddPhoto(p);
        }
        for (int i = 0; i < 15; i++)
        {
            var r = VisitReading.Create(visit.Id, $"R{i}", "Electrical", 10, "V");
            r.SetValidationRange(0, 100);
            visit.AddReading(r);
        }
        var chk = VisitChecklist.Create(visit.Id, "Electrical", "Item", "desc", true);
        visit.AddChecklistItem(chk);
        visit.UpdateChecklistItem(chk.Id, CheckStatus.OK);

        visit.CompleteVisit();
        visit.Submit();

        visit.DomainEvents.Should().NotBeEmpty();
        visit.ClearDomainEvents();
        visit.DomainEvents.Should().BeEmpty();
    }
}


