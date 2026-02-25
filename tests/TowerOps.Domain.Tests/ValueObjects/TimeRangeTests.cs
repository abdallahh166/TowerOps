using FluentAssertions;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.ValueObjects;
using Xunit;

namespace TowerOps.Domain.Tests.ValueObjects;

public class TimeRangeTests
{
    [Fact]
    public void Create_WithEndAfterStart_ShouldCreate()
    {
        var start = DateTime.UtcNow;
        var end = start.AddHours(1);
        var tr = TimeRange.Create(start, end);

        tr.Duration.Should().Be(TimeSpan.FromHours(1));
        tr.IsValid().Should().BeTrue();
    }

    [Fact]
    public void Create_WithEndBeforeStart_ShouldThrow()
    {
        var start = DateTime.UtcNow;
        var end = start.AddMinutes(-5);
        var act = () => TimeRange.Create(start, end);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void IsValid_WithTooShortOrTooLong_ShouldBeFalse()
    {
        var start = DateTime.UtcNow;
        TimeRange.Create(start, start.AddMinutes(10)).IsValid().Should().BeFalse();
        TimeRange.Create(start, start.AddHours(10)).IsValid().Should().BeFalse();
    }
}


