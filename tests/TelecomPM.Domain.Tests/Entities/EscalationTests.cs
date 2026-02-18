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
}
