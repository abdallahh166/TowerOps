using FluentAssertions;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.ValueObjects;
using Xunit;

namespace TowerOps.Domain.Tests.Events;

public class DomainEventTests
{
    [Fact]
    public void VisitLifecycle_ShouldRaiseExpectedEvents()
    {
        var visit = Visit.Create(
            visitNumber: "V2025001",
            siteId: Guid.NewGuid(),
            siteCode: "TNT001",
            siteName: "Tanta Central",
            engineerId: Guid.NewGuid(),
            engineerName: "Ahmed Hassan",
            scheduledDate: DateTime.Today.AddDays(1),
            type: VisitType.BM
        );

        visit.StartVisit(Coordinates.Create(30, 30));
        typeof(Visit).GetProperty("ActualStartTime", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)
            !.SetValue(visit, DateTime.UtcNow.AddHours(-1));
        // satisfy completion requirements
        for (int i = 0; i < 30; i++)
            visit.AddPhoto(VisitPhoto.Create(visit.Id, PhotoType.Before, PhotoCategory.Other, $"Item {i}", $"b{i}.jpg", $"/b{i}.jpg"));
        for (int i = 0; i < 30; i++)
            visit.AddPhoto(VisitPhoto.Create(visit.Id, PhotoType.After, PhotoCategory.Other, $"Item {i+30}", $"a{i}.jpg", $"/a{i}.jpg"));
        for (int i = 0; i < 15; i++)
            visit.AddReading(VisitReading.Create(visit.Id, "Phase1-Neutral Voltage", "Electrical", 220m, "V"));
        visit.AddChecklistItem(VisitChecklist.Create(visit.Id, "General", "Check Doors", "OK", true));
        visit.UpdateChecklistItem(visit.Checklists.Single().Id, CheckStatus.OK);

        visit.CompleteVisit();
        visit.Submit();

        visit.DomainEvents.Should().NotBeEmpty();
        visit.DomainEvents.Select(e => e.GetType().Name).Should().Contain(new[]
        {
            "VisitCreatedEvent",
            "VisitStartedEvent",
            "VisitCompletedEvent",
            "VisitSubmittedEvent"
        });
    }
}
