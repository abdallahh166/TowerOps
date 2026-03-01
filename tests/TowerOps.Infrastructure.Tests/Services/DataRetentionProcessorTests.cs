using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Events;
using TowerOps.Domain.Interfaces.Services;
using TowerOps.Domain.ValueObjects;
using TowerOps.Infrastructure.Persistence;
using TowerOps.Infrastructure.Persistence.Repositories;
using TowerOps.Infrastructure.Services;
using Xunit;

namespace TowerOps.Infrastructure.Tests.Services;

public class DataRetentionProcessorTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldHardDeleteSoftDeletedRows_AndRespectLegalHold()
    {
        await using var context = CreateContext();

        var softDeletedUser = User.Create("Soft Deleted", "soft@towerops.com", "+201111111111", UserRole.Technician, Guid.NewGuid());
        softDeletedUser.MarkAsDeleted("test");
        SetDeletedAtUtc(softDeletedUser, DateTime.UtcNow.AddDays(-120));

        var legalHoldUser = User.Create("Legal Hold", "legal@towerops.com", "+201222222222", UserRole.Technician, Guid.NewGuid());
        legalHoldUser.MarkAsDeleted("test");
        legalHoldUser.ApplyLegalHold("Active dispute", "test");
        SetDeletedAtUtc(legalHoldUser, DateTime.UtcNow.AddDays(-120));

        var site = Site.Create(
            "CAI902",
            "Retention Site",
            "OMC",
            Guid.NewGuid(),
            "Cairo",
            "East",
            Coordinates.Create(30.0, 31.0),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);

        var visit = Visit.Create(
            "V-RET-1",
            site.Id,
            site.SiteCode.Value,
            site.Name,
            Guid.NewGuid(),
            "Engineer",
            DateTime.UtcNow.Date,
            VisitType.BM);

        var softDeletedPhoto = VisitPhoto.CreatePendingUpload(
            visit.Id,
            PhotoType.Before,
            PhotoCategory.Rectifier,
            "Rectifier panel",
            "rectifier.jpg",
            "/q/rectifier.jpg");
        softDeletedPhoto.MarkAsDeleted("test");
        SetDeletedAtUtc(softDeletedPhoto, DateTime.UtcNow.AddDays(-120));

        var legalHoldPhoto = VisitPhoto.CreatePendingUpload(
            visit.Id,
            PhotoType.After,
            PhotoCategory.Rectifier,
            "Rectifier after",
            "rectifier-after.jpg",
            "/q/rectifier-after.jpg");
        legalHoldPhoto.MarkAsDeleted("test");
        legalHoldPhoto.ApplyLegalHold("Active dispute", "test");
        SetDeletedAtUtc(legalHoldPhoto, DateTime.UtcNow.AddDays(-120));

        visit.AddPhoto(softDeletedPhoto);
        visit.AddPhoto(legalHoldPhoto);

        await context.Users.AddRangeAsync(softDeletedUser, legalHoldUser);
        await context.Sites.AddAsync(site);
        await context.Visits.AddAsync(visit);
        await context.SaveChangesAsync();

        var settings = new Mock<ISystemSettingsService>();
        settings
            .Setup(s => s.GetAsync("Privacy:Retention:CleanupBatchSize", 200, It.IsAny<CancellationToken>()))
            .ReturnsAsync(200);
        settings
            .Setup(s => s.GetAsync("Privacy:Retention:SoftDeleteGraceDays", 90, It.IsAny<CancellationToken>()))
            .ReturnsAsync(90);
        settings
            .Setup(s => s.GetAsync("Privacy:Retention:OperationalYears", 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);
        settings
            .Setup(s => s.GetAsync("Privacy:Retention:SignatureYears", 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(7);
        settings
            .Setup(s => s.GetAsync("Privacy:Retention:AuditLogYears", 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(7);

        var unitOfWork = new UnitOfWork(context);
        var exportRepository = new UserDataExportRequestRepository(context);
        var sut = new DataRetentionProcessor(
            context,
            settings.Object,
            exportRepository,
            unitOfWork,
            Mock.Of<ILogger<DataRetentionProcessor>>());

        var result = await sut.ExecuteAsync(CancellationToken.None);

        result.HardDeletedUsers.Should().Be(1);
        result.HardDeletedPhotos.Should().Be(1);

        var usersAfter = await context.Users.IgnoreQueryFilters().ToListAsync();
        usersAfter.Should().ContainSingle(u => u.Email == "legal@towerops.com");
        usersAfter.Should().NotContain(u => u.Email == "soft@towerops.com");

        var photosAfter = await context.VisitPhotos.IgnoreQueryFilters().ToListAsync();
        photosAfter.Should().ContainSingle(p => p.FileName == "rectifier-after.jpg");
        photosAfter.Should().NotContain(p => p.FileName == "rectifier.jpg");
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"retention-processor-{Guid.NewGuid():N}")
            .Options;

        var dispatcher = new Mock<IDomainEventDispatcher>();
        dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new ApplicationDbContext(options, dispatcher.Object);
    }

    private static void SetDeletedAtUtc(object entity, DateTime deletedAtUtc)
    {
        var deletedAtProperty = entity.GetType().GetProperty("DeletedAt");
        deletedAtProperty!.SetValue(entity, deletedAtUtc);
    }
}
