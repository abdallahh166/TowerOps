using FluentAssertions;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Tests.Entities;

public class VisitEditCancelRulesTests
{
    private Visit CreateVisit()
    {
        return Visit.Create("V1", Guid.NewGuid(), "TNT001", "S1", Guid.NewGuid(), "Eng", DateTime.Today, VisitType.PreventiveMaintenance);
    }

    [Fact]
    public void Cancel_FromApproved_ShouldThrow()
    {
        var v = CreateVisit();
        v.StartVisit(Coordinates.Create(30, 30));
        typeof(Visit).GetProperty("ActualStartTime")!.SetValue(v, DateTime.UtcNow - TimeSpan.FromMinutes(60));
        // Complete requirements
        for (int i = 0; i < 60; i++)
        {
            var type = i < 30 ? PhotoType.Before : PhotoType.After;
            var p = VisitPhoto.Create(v.Id, type, PhotoCategory.ShelterInside, "", $"f{i}", "/p");
            v.AddPhoto(p);
        }
        for (int i = 0; i < 15; i++)
        {
            var r = VisitReading.Create(v.Id, $"R{i}", "Electrical", 10, "V");
            r.SetValidationRange(0, 100);
            v.AddReading(r);
        }
        var chk = VisitChecklist.Create(v.Id, "Electrical", "Item", "desc", true);
        v.AddChecklistItem(chk);
        v.UpdateChecklistItem(chk.Id, CheckStatus.OK);
        v.CompleteVisit();
        v.Submit();
        v.StartReview();
        v.Approve(Guid.NewGuid(), "Reviewer");

        Action act = () => v.Cancel("no need");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CanBeEdited_Rules_ShouldHoldAcrossStatuses()
    {
        var v = CreateVisit();
        v.CanBeEdited().Should().BeTrue(); // Scheduled
        v.StartVisit(Coordinates.Create(30, 30));
        v.CanBeEdited().Should().BeTrue(); // InProgress
        typeof(Visit).GetProperty("ActualStartTime")!.SetValue(v, DateTime.UtcNow - TimeSpan.FromMinutes(60));
        // Ensure minimal completion requirements before completion/submission
        for (int i = 0; i < 60; i++)
        {
            var type = i < 30 ? PhotoType.Before : PhotoType.After;
            var p = VisitPhoto.Create(v.Id, type, PhotoCategory.ShelterInside, "", $"f{i}", "/p");
            v.AddPhoto(p);
        }
        for (int i = 0; i < 15; i++)
        {
            var r = VisitReading.Create(v.Id, $"R{i}", "Electrical", 10, "V");
            r.SetValidationRange(0, 100);
            v.AddReading(r);
        }
        var chk = VisitChecklist.Create(v.Id, "Electrical", "Item", "desc", true);
        v.AddChecklistItem(chk);
        v.UpdateChecklistItem(chk.Id, CheckStatus.OK);
        v.CompleteVisit();
        v.CanBeEdited().Should().BeTrue(); // Completed
        v.Submit();
        v.StartReview();
        v.RequestCorrection(Guid.NewGuid(), "Reviewer", "notes");
        v.CanBeEdited().Should().BeTrue(); // NeedsCorrection
    }
}


