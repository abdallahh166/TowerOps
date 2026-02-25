using FluentAssertions;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Events.SiteEvents;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Domain.Tests.Events;

public class SiteEventsTests
{
    private static Site NewSite()
    {
        var coords = Coordinates.Create(30, 31);
        var addr = Address.Create("City", "Street", "123");
        return Site.Create("TNT001", "Name", "OMC", Guid.NewGuid(), "Region", "Sub", coords, addr, SiteType.Macro);
    }

    [Fact]
    public void Create_ShouldRaiseSiteCreatedEvent()
    {
        var site = NewSite();
        site.DomainEvents.Should().ContainSingle(e => e.GetType() == typeof(SiteCreatedEvent));
    }

    [Fact]
    public void UpdateStatus_ShouldRaiseStatusChangedEvent()
    {
        var site = NewSite();
        site.ClearDomainEvents();
        site.UpdateStatus(SiteStatus.OffAir);

        site.DomainEvents.Should().ContainSingle(e => e.GetType() == typeof(SiteStatusChangedEvent) && ((SiteStatusChangedEvent)e).NewStatus == SiteStatus.OffAir);
    }

    [Fact]
    public void UpdateStatus_SameStatus_ShouldNotRaiseEvent()
    {
        var site = NewSite();
        site.ClearDomainEvents();
        site.UpdateStatus(SiteStatus.OnAir);

        site.DomainEvents.Should().BeEmpty();
    }
}


