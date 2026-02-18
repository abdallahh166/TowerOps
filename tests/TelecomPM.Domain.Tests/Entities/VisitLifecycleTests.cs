using FluentAssertions;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Tests.Entities;

public class VisitLifecycleTests
{
    private Visit CreateVisit()
    {
        return Visit.Create("V1", Guid.NewGuid(), "TNT001", "Site1", Guid.NewGuid(), "Eng", DateTime.Today, VisitType.PreventiveMaintenance);
    }

    [Fact]
    public void StartCompleteSubmitApprove_ShouldTransitionThroughStatuses()
    {
        var visit = CreateVisit();
        visit.StartVisit(Coordinates.Create(30, 30));
        // Simulate realistic duration
        typeof(Visit).GetProperty("ActualStartTime")!.SetValue(visit, DateTime.UtcNow - TimeSpan.FromMinutes(60));

        // Add minimal required data
        for (int i = 0; i < 30; i++)
        {
            var p1 = VisitPhoto.Create(visit.Id, PhotoType.Before, PhotoCategory.ShelterInside, "", "b" + i, "/b");
            visit.AddPhoto(p1);
        }
        for (int i = 0; i < 30; i++)
        {
            var p2 = VisitPhoto.Create(visit.Id, PhotoType.After, PhotoCategory.ShelterInside, "", "a" + i, "/a");
            visit.AddPhoto(p2);
        }

        for (int i = 0; i < 15; i++)
        {
            var r = VisitReading.Create(visit.Id, "R" + i, "Electrical", 10, "V");
            r.SetValidationRange(0, 100);
            visit.AddReading(r);
        }

        var chk = VisitChecklist.Create(visit.Id, "Electrical", "Item", "desc", true);
        visit.AddChecklistItem(chk);
        visit.UpdateChecklistItem(chk.Id, CheckStatus.OK);

        // Complete and submit
        visit.CompleteVisit();
        visit.Status.Should().Be(VisitStatus.Completed);
        visit.Submit();
        visit.Status.Should().Be(VisitStatus.Submitted);

        // Review and approve
        visit.StartReview();
        visit.Approve(Guid.NewGuid(), "Reviewer");
        visit.Status.Should().Be(VisitStatus.Approved);
    }

    [Fact]
    public void InvalidTransitions_ShouldThrow()
    {
        var visit = CreateVisit();
        Action submitEarly = () => visit.Submit();
        submitEarly.Should().Throw<DomainException>();

        Action completeEarly = () => visit.CompleteVisit();
        completeEarly.Should().Throw<DomainException>();
    }
}


