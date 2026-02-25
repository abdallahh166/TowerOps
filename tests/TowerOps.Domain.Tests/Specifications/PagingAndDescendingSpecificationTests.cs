using FluentAssertions;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Specifications;

namespace TowerOps.Domain.Tests.Specifications;

public class PagingAndDescendingSpecificationTests
{
    private sealed class DescSitesSpec : BaseSpecification<Site>
    {
        public DescSitesSpec()
            : base(s => s.Status == SiteStatus.OnAir)
        {
            AddOrderByDescending(s => s.Name);
            AddThenByDescending(s => s.Complexity);
            ApplyPaging(5, 10);
        }
    }

    [Fact]
    public void DescAndPaging_Metadata_ShouldBeSet()
    {
        var spec = new DescSitesSpec();
        spec.OrderByDescending.Should().NotBeNull();
        spec.ThenByDescending.Should().HaveCount(1);
        spec.IsPagingEnabled.Should().BeTrue();
        spec.Skip.Should().Be(5);
        spec.Take.Should().Be(10);
    }
}


