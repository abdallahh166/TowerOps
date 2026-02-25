using FluentAssertions;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using Xunit;

namespace TowerOps.Domain.Tests.Entities;

public class VisitPhotoCapturedAtTests
{
    [Fact]
    public void Create_ShouldSetCapturedAtUtc()
    {
        var photo = VisitPhoto.Create(
            Guid.NewGuid(),
            PhotoType.Before,
            PhotoCategory.ShelterInside,
            "Panel",
            "panel.jpg",
            "/panel.jpg");

        photo.CapturedAtUtc.Should().NotBeNull();
        photo.CapturedAtUtc!.Value.Kind.Should().Be(DateTimeKind.Utc);
    }

    [Fact]
    public void SetCapturedAtUtc_WithLocalTime_ShouldNormalizeToUtc()
    {
        var photo = VisitPhoto.Create(
            Guid.NewGuid(),
            PhotoType.After,
            PhotoCategory.Tower,
            "Tower",
            "tower.jpg",
            "/tower.jpg");

        var local = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);
        photo.SetCapturedAtUtc(local);

        photo.CapturedAtUtc.Should().NotBeNull();
        photo.CapturedAtUtc!.Value.Kind.Should().Be(DateTimeKind.Utc);
    }
}
