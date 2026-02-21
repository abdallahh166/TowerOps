using FluentAssertions;
using TelecomPM.Domain.Entities.ChecklistTemplates;
using TelecomPM.Domain.Enums;
using Xunit;

namespace TelecomPM.Domain.Tests.Entities;

public class ChecklistTemplateTests
{
    [Fact]
    public void Activate_ShouldSetState()
    {
        var template = ChecklistTemplate.Create(
            VisitType.BM,
            "v1.0",
            DateTime.UtcNow,
            "creator");

        template.Activate("manager");

        template.IsActive.Should().BeTrue();
        template.ApprovedBy.Should().Be("manager");
        template.ApprovedAtUtc.Should().NotBeNull();
    }

    [Fact]
    public void Supersede_ShouldSetEffectiveToUtcAndDeactivate()
    {
        var template = ChecklistTemplate.Create(
            VisitType.CM,
            "v1.0",
            DateTime.UtcNow,
            "creator");
        template.Activate("manager");

        var effectiveTo = DateTime.UtcNow.AddDays(1);
        template.Supersede(effectiveTo);

        template.IsActive.Should().BeFalse();
        template.EffectiveToUtc.Should().Be(effectiveTo);
    }

    [Fact]
    public void CreateNewVersion_ShouldIncrementVersion()
    {
        var previous = ChecklistTemplate.Create(
            VisitType.BM,
            "v1.0",
            DateTime.UtcNow,
            "creator");
        previous.AddItem("Power", "Rectifier Visual Check", null, true, 1);

        var next = ChecklistTemplate.CreateNewVersion(previous, "Added RF items", "creator2");

        next.Version.Should().Be("v1.1");
        next.IsActive.Should().BeFalse();
        next.Items.Should().HaveCount(1);
        next.Items[0].ItemName.Should().Be("Rectifier Visual Check");
    }
}
