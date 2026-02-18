using FluentAssertions;
using TelecomPM.Application.Services;
using TelecomPM.Domain.Enums;
using Xunit;


namespace TelecomPM.Application.Tests.Services;

public class EscalationRoutingServiceTests
{
    private readonly EscalationRoutingService _service = new();

    [Fact]
    public void DetermineLevel_ShouldRouteToProjectSponsor_WhenFinancialImpactIsVeryHigh()
    {
        var level = _service.DetermineLevel(SlaClass.P2, 300000m, 10m);

        level.Should().Be(EscalationLevel.ProjectSponsor);
    }

    [Fact]
    public void DetermineLevel_ShouldRouteToBmManagement_WhenImpactIsHigh()
    {
        var level = _service.DetermineLevel(SlaClass.P3, 60000m, 10m);

        level.Should().Be(EscalationLevel.BMManagement);
    }
}
