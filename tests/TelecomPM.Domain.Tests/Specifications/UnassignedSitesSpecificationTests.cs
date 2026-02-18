using FluentAssertions;
using TelecomPM.Domain.Specifications.SiteSpecifications;

namespace TelecomPM.Domain.Tests.Specifications;

public class UnassignedSitesSpecificationTests
{
    [Fact]
    public void DefaultCtor_ShouldSetPrimaryAndSecondaryOrdering()
    {
        var spec = new UnassignedSitesSpecification();

        spec.OrderBy.Should().NotBeNull();
        spec.ThenBy.Should().HaveCount(1);
        spec.OrderByDescending.Should().BeNull();
        spec.ThenByDescending.Should().BeEmpty();
    }

    [Fact]
    public void OfficeCtor_ShouldSetPrimaryAndSecondaryOrdering()
    {
        var officeId = Guid.NewGuid();
        var spec = new UnassignedSitesSpecification(officeId);

        spec.OrderBy.Should().NotBeNull();
        spec.ThenBy.Should().HaveCount(1);
        spec.OrderByDescending.Should().BeNull();
        spec.ThenByDescending.Should().BeEmpty();
    }
}


