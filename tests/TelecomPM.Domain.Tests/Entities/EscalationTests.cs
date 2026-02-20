using FluentAssertions;
using TelecomPM.Domain.Entities.Escalations;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Tests.Entities;

public class EscalationTests
{
    [Fact]
    public void Create_WithValidData_ShouldInitializeSubmittedState()
    {
        var escalation = Escalation.Create(
            Guid.NewGuid(),
            "INC-1001",
            "S-TNT-001",
            SlaClass.P1,
            120000,
            25,
            "photos+logs",
            "reset rectifier",
            "dispatch BM team",
            EscalationLevel.AreaManager,
            "dispatcher");

        escalation.Status.Should().Be(EscalationStatus.Submitted);
        escalation.SubmittedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Create_WithoutEvidencePackage_ShouldThrow()
    {
        Action act = () => Escalation.Create(
            Guid.NewGuid(),
            "INC-1002",
            "S-TNT-002",
            SlaClass.P2,
            1000,
            5,
            string.Empty,
            "actions",
            "decision",
            EscalationLevel.BMManagement,
            "dispatcher");

        act.Should().Throw<DomainException>().WithMessage("*Evidence package is required*");
    }

    [Fact]
    public void Approve_FromSubmitted_ShouldThrow()
    {
        var escalation = CreateEscalation();

        Action act = escalation.Approve;

        act.Should().Throw<DomainException>().WithMessage("*Only under-review escalation can be approved*");
    }

    [Fact]
    public void Reject_FromSubmitted_ShouldThrow()
    {
        var escalation = CreateEscalation();

        Action act = escalation.Reject;

        act.Should().Throw<DomainException>().WithMessage("*Only under-review escalation can be rejected*");
    }

    [Fact]
    public void MarkUnderReview_FromApproved_ShouldThrow()
    {
        var escalation = CreateEscalation();
        escalation.MarkUnderReview();
        escalation.Approve();

        Action act = escalation.MarkUnderReview;

        act.Should().Throw<DomainException>().WithMessage("*Only submitted escalation can be moved to review*");
    }

    [Fact]
    public void Close_FromSubmitted_ShouldThrow()
    {
        var escalation = CreateEscalation();

        Action act = escalation.Close;

        act.Should().Throw<DomainException>().WithMessage("*Only approved or rejected escalation can be closed*");
    }

    [Fact]
    public void Close_AfterApproval_ShouldSetClosed()
    {
        var escalation = CreateEscalation();
        escalation.MarkUnderReview();
        escalation.Approve();

        escalation.Close();

        escalation.Status.Should().Be(EscalationStatus.Closed);
    }

    private static Escalation CreateEscalation()
    {
        return Escalation.Create(
            Guid.NewGuid(),
            "INC-1003",
            "S-TNT-003",
            SlaClass.P2,
            1000,
            5,
            "photos+logs",
            "actions",
            "decision",
            EscalationLevel.BMManagement,
            "dispatcher");
    }
}
