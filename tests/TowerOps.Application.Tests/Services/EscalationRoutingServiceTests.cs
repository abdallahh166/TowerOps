using FluentAssertions;
using TowerOps.Application.Services;
using TowerOps.Domain.Enums;
using Xunit;


namespace TowerOps.Application.Tests.Services;

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
