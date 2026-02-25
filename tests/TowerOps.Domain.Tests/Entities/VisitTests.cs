using FluentAssertions;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.ValueObjects;
using Xunit;

namespace TowerOps.Domain.Tests.Entities;

public class VisitTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateVisit()
    {
        // Act
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

        // Assert
        visit.Should().NotBeNull();
        visit.VisitNumber.Should().Be("V2025001");
        visit.Status.Should().Be(VisitStatus.Scheduled);
        visit.CompletionPercentage.Should().Be(0);
        visit.DomainEvents.Should().ContainSingle();
    }

    [Fact]
    public void StartVisit_WithScheduledStatus_ShouldChangeToInProgress()
    {
        // Arrange
        var visit = CreateTestVisit();
        var coordinates = Coordinates.Create(30.7865, 30.9925);

        // Act
        visit.StartVisit(coordinates);

        // Assert
        visit.Status.Should().Be(VisitStatus.InProgress);
        visit.CheckInTime.Should().NotBeNull();
        visit.CheckInLocation.Should().Be(coordinates);
        visit.DomainEvents.Should().HaveCount(2); // Scheduled + Started
    }

    [Fact]
    public void StartVisit_WhenNotScheduled_ShouldThrowDomainException()
    {
        // Arrange
        var visit = CreateTestVisit();
        visit.StartVisit(Coordinates.Create(30.0, 30.0));
        var coordinates = Coordinates.Create(30.7865, 30.9925);

        // Act
        var act = () => visit.StartVisit(coordinates);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Scheduled*");
    }

    [Fact]
    public void StartVisit_WhenNotScheduled_ShouldSetLocalizationKey()
    {
        var visit = CreateTestVisit();
        visit.StartVisit(Coordinates.Create(30.0, 30.0));

        Action act = () => visit.StartVisit(Coordinates.Create(30.7865, 30.9925));

        var ex = act.Should().Throw<DomainException>().Which;
        ex.MessageKey.Should().Be("Visit.Start.RequiresScheduled");
    }

    [Fact]
    public void Submit_WhenCompleted_ShouldChangeStatusToSubmitted()
    {
        // Arrange
        var visit = CreateCompletedVisit();

        // Act
        visit.Submit();

        // Assert
        visit.Status.Should().Be(VisitStatus.Submitted);
    }

    [Fact]
    public void Submit_WhenNotCompleted_ShouldThrowDomainException()
    {
        // Arrange
        var visit = CreateTestVisit();

        // Act
        var act = () => visit.Submit();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*completed*");
    }

    [Fact]
    public void StartVisit_Twice_ShouldThrow()
    {
        var visit = CreateTestVisit();
        visit.StartVisit(Coordinates.Create(30, 30));
        var act = () => visit.StartVisit(Coordinates.Create(31, 31));
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Approve_WithoutSubmit_ShouldThrow()
    {
        var visit = CreateCompletedVisit();
        var act = () => visit.Approve(Guid.NewGuid(), "Mgr", "ok");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Approve_WithValidReviewer_ShouldChangeStatusToApproved()
    {
        // Arrange
        var visit = CreateSubmittedVisit();
        var reviewerId = Guid.NewGuid();
        var reviewerName = "Manager Name";

        // Act
        visit.StartReview();
        visit.Approve(reviewerId, reviewerName, "Good work");

        // Assert
        visit.Status.Should().Be(VisitStatus.Approved);
        visit.DomainEvents.Should().Contain(e => e.GetType().Name == "VisitApprovedEvent");
    }

    private Visit CreateTestVisit()
    {
        return Visit.Create(
            "V2025001",
            Guid.NewGuid(),
            "TNT001",
            "Test Site",
            Guid.NewGuid(),
            "Test Engineer",
            DateTime.Today,
            VisitType.BM
        );
    }

    private Visit CreateCompletedVisit()
    {
        var visit = CreateTestVisit();
        var coords = Coordinates.Create(30.0, 30.0);
        visit.StartVisit(coords);
        // Make duration valid for completion (>= 30 minutes)
        typeof(Visit).GetProperty("ActualStartTime", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)
            !.SetValue(visit, DateTime.UtcNow.AddHours(-1));

        // Add required photos and readings
        for (int i = 0; i < 30; i++)
        {
            var photo = VisitPhoto.Create(
                visit.Id,
                PhotoType.Before,
                PhotoCategory.Other,
                $"Item {i}",
                $"file{i}.jpg",
                $"/files/{i}.jpg");
            visit.AddPhoto(photo);
        }

        for (int i = 0; i < 30; i++)
        {
            var photo = VisitPhoto.Create(
                visit.Id,
                PhotoType.After,
                PhotoCategory.Other,
                $"Item {i+30}",
                $"file{i+30}.jpg",
                $"/files/{i+30}.jpg");
            visit.AddPhoto(photo);
        }

        for (int i = 0; i < 15; i++)
        {
            var reading = VisitReading.Create(
                visit.Id,
                readingType: "Phase1-Neutral Voltage",
                category: "Electrical",
                value: 220m,
                unit: "V");
            visit.AddReading(reading);
        }

        visit.AddChecklistItem(VisitChecklist.Create(visit.Id, "General", "Check Doors", "OK", true));
        visit.UpdateChecklistItem(visit.Checklists.Single().Id, CheckStatus.OK);

        visit.CompleteVisit();
        return visit;
    }

    private Visit CreateSubmittedVisit()
    {
        var visit = CreateCompletedVisit();
        visit.Submit();
        return visit;
    }
}
