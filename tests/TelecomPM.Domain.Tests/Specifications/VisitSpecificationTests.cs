using FluentAssertions;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Specifications.VisitSpecifications;
using Xunit;

namespace TelecomPM.Domain.Tests.Specifications;

public class VisitSpecificationTests
{
    [Fact]
    public void VisitsByEngineerSpecification_ShouldFilterCorrectly()
    {
        var engineerId = Guid.NewGuid();
        var otherEngineerId = Guid.NewGuid();

        var v1 = Visit.Create("V1", Guid.NewGuid(), "TNT001", "S1", engineerId, "E1", DateTime.Today, VisitType.PreventiveMaintenance);
        var v2 = Visit.Create("V2", Guid.NewGuid(), "TNT002", "S2", otherEngineerId, "E2", DateTime.Today, VisitType.PreventiveMaintenance);

        var list = new List<Visit> { v1, v2 };
        var spec = new VisitsByEngineerSpecification(engineerId);
        spec.Criteria.Should().NotBeNull();

        var result = list.Where(spec.Criteria!.Compile()).ToList();
        result.Should().ContainSingle().Which.Id.Should().Be(v1.Id);
    }
}
