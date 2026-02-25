using FluentAssertions;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Domain.Tests.Entities;

public class VisitReviewFlowTests
{
    private Visit PrepareCompletedVisit()
    {
        var v = Visit.Create("V1", Guid.NewGuid(), "TNT001", "Site1", Guid.NewGuid(), "Eng", DateTime.Today, VisitType.BM);
        v.StartVisit(Coordinates.Create(30, 30));
        typeof(Visit).GetProperty("ActualStartTime")!.SetValue(v, DateTime.UtcNow - TimeSpan.FromMinutes(60));

        for (int i = 0; i < 30; i++)
        {
            var b = VisitPhoto.Create(v.Id, PhotoType.Before, PhotoCategory.ShelterInside, "", "b"+i, "/b");
            v.AddPhoto(b);
        }
        for (int i = 0; i < 30; i++)
        {
            var a = VisitPhoto.Create(v.Id, PhotoType.After, PhotoCategory.ShelterInside, "", "a"+i, "/a");
            v.AddPhoto(a);
        }
        for (int i = 0; i < 15; i++)
        {
            var r = VisitReading.Create(v.Id, "R"+i, "Electrical", 10, "V");
            r.SetValidationRange(0, 100);
            v.AddReading(r);
        }
        var chk = VisitChecklist.Create(v.Id, "Electrical", "C1", "desc", true);
        v.AddChecklistItem(chk);
        v.UpdateChecklistItem(chk.Id, CheckStatus.OK);
        v.CompleteVisit();
        return v;
    }

    [Fact]
    public void RequestCorrection_ShouldMoveToNeedsCorrection()
    {
        var v = PrepareCompletedVisit();
        v.Submit();
        v.StartReview();
        v.RequestCorrection(Guid.NewGuid(), "Reviewer", "fix photo angles");

        v.Status.Should().Be(VisitStatus.NeedsCorrection);
        v.ApprovalHistory.Should().NotBeEmpty();
    }

    [Fact]
    public void Reject_ShouldMoveToRejected()
    {
        var v = PrepareCompletedVisit();
        v.Submit();
        v.StartReview();
        v.Reject(Guid.NewGuid(), "Reviewer", "invalid readings");

        v.Status.Should().Be(VisitStatus.Rejected);
        v.ApprovalHistory.Should().NotBeEmpty();
    }

    [Fact]
    public void Submit_FromNeedsCorrection_ShouldAllowReworkLoop()
    {
        var v = PrepareCompletedVisit();
        v.Submit();
        v.StartReview();
        v.RequestCorrection(Guid.NewGuid(), "Reviewer", "fix checklist details");

        v.Submit();

        v.Status.Should().Be(VisitStatus.Submitted);
    }
}

