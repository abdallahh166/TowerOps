using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TelecomPM.Domain.Entities.ApplicationRoles;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Events;
using TelecomPM.Domain.Interfaces.Services;
using TelecomPM.Domain.ValueObjects;
using TelecomPM.Infrastructure.Persistence;
using Xunit;

namespace TelecomPM.Infrastructure.Tests.Repositories;

public class SoftDeleteQueryFilterTests
{
    [Fact]
    public async Task SoftDeleteFilter_ShouldApplyTo_StringKeyAggregates()
    {
        await using var context = CreateContext();

        var activeRole = ApplicationRole.Create(
            "CustomRoleActive",
            "Custom Role Active",
            null,
            isSystem: false,
            isActive: true,
            permissions: Array.Empty<string>());

        var deletedRole = ApplicationRole.Create(
            "CustomRoleDeleted",
            "Custom Role Deleted",
            null,
            isSystem: false,
            isActive: true,
            permissions: Array.Empty<string>());
        deletedRole.MarkAsDeleted("test");

        await context.ApplicationRoles.AddRangeAsync(activeRole, deletedRole);
        await context.SaveChangesAsync();

        var filtered = await context.ApplicationRoles.AsNoTracking().ToListAsync();
        var allRows = await context.ApplicationRoles.IgnoreQueryFilters().AsNoTracking().ToListAsync();

        filtered.Should().ContainSingle(r => r.Name == "CustomRoleActive");
        filtered.Should().NotContain(r => r.Name == "CustomRoleDeleted");
        allRows.Should().HaveCount(2);
    }

    [Fact]
    public async Task SoftDeleteFilter_ShouldStillApplyTo_GuidKeyAggregates()
    {
        await using var context = CreateContext();

        var activeSite = CreateSite("TNT910");
        var deletedSite = CreateSite("TNT911");
        deletedSite.MarkAsDeleted("test");

        await context.Sites.AddRangeAsync(activeSite, deletedSite);
        await context.SaveChangesAsync();

        var filtered = await context.Sites.AsNoTracking().ToListAsync();
        var allRows = await context.Sites.IgnoreQueryFilters().AsNoTracking().ToListAsync();

        filtered.Should().ContainSingle(s => s.SiteCode.Value == "TNT910");
        filtered.Should().NotContain(s => s.SiteCode.Value == "TNT911");
        allRows.Should().HaveCount(2);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"soft-delete-filter-{Guid.NewGuid():N}")
            .Options;

        var dispatcher = new Mock<IDomainEventDispatcher>();
        dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new ApplicationDbContext(options, dispatcher.Object);
    }

    private static Site CreateSite(string siteCode)
    {
        return Site.Create(
            siteCode,
            $"Site {siteCode}",
            "OMC",
            Guid.NewGuid(),
            "Cairo",
            "East",
            Coordinates.Create(30.0, 31.0),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);
    }
}
