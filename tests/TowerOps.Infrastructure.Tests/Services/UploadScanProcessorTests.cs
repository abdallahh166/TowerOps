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
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.Interfaces.Services;
using TowerOps.Domain.ValueObjects;
using TowerOps.Infrastructure.Persistence;
using TowerOps.Infrastructure.Services;
using Xunit;

namespace TowerOps.Infrastructure.Tests.Services;

public class UploadScanProcessorTests
{
    [Fact]
    public async Task EvaluateBatchAsync_ShouldApprovePendingPhotos_WhenScannerIsClean()
    {
        await using var context = CreateContext();
        var photo = await SeedPendingPhotoAsync(context);

        var scanner = new Mock<IFileMalwareScanService>();
        scanner
            .Setup(s => s.ScanAsync(photo.FileName, photo.FilePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FileMalwareScanResult(true, "OK"));

        var settings = new Mock<ISystemSettingsService>();
        settings
            .Setup(s => s.GetAsync("UploadSecurity:Scan:BatchSize", 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(100);

        var unitOfWork = new Mock<IUnitOfWork>();

        var sut = new UploadScanProcessor(
            context,
            scanner.Object,
            settings.Object,
            Mock.Of<INotificationService>(),
            Mock.Of<IUserRepository>(),
            unitOfWork.Object,
            Mock.Of<ILogger<UploadScanProcessor>>());

        var processed = await sut.EvaluateBatchAsync(CancellationToken.None);

        processed.Should().Be(1);
        photo.FileStatus.Should().Be(UploadedFileStatus.Approved);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task EvaluateBatchAsync_ShouldQuarantinePendingPhotos_WhenScannerFails()
    {
        await using var context = CreateContext();
        var photo = await SeedPendingPhotoAsync(context);

        var scanner = new Mock<IFileMalwareScanService>();
        scanner
            .Setup(s => s.ScanAsync(photo.FileName, photo.FilePath, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FileMalwareScanResult(false, "EICAR test signature"));

        var settings = new Mock<ISystemSettingsService>();
        settings
            .Setup(s => s.GetAsync("UploadSecurity:Scan:BatchSize", 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync(100);

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByIdAsNoTrackingAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        userRepository
            .Setup(r => r.GetByRoleAsNoTrackingAsync(UserRole.Admin, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<User>());

        var notificationService = new Mock<INotificationService>();
        var unitOfWork = new Mock<IUnitOfWork>();

        var sut = new UploadScanProcessor(
            context,
            scanner.Object,
            settings.Object,
            notificationService.Object,
            userRepository.Object,
            unitOfWork.Object,
            Mock.Of<ILogger<UploadScanProcessor>>());

        var processed = await sut.EvaluateBatchAsync(CancellationToken.None);

        processed.Should().Be(1);
        photo.FileStatus.Should().Be(UploadedFileStatus.Quarantined);
        photo.QuarantineReason.Should().Contain("EICAR");
        notificationService.Verify(
            n => n.SendPushNotificationAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"upload-scan-{Guid.NewGuid():N}")
            .Options;

        var dispatcher = new Mock<IDomainEventDispatcher>();
        dispatcher
            .Setup(d => d.DispatchAsync(It.IsAny<IEnumerable<IDomainEvent>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        return new ApplicationDbContext(options, dispatcher.Object);
    }

    private static async Task<VisitPhoto> SeedPendingPhotoAsync(ApplicationDbContext context)
    {
        var site = Site.Create(
            "CAI900",
            "Upload Scan Site",
            "OMC",
            Guid.NewGuid(),
            "Cairo",
            "East",
            Coordinates.Create(30.0, 31.0),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);

        var visit = Visit.Create(
            "V-UPLOAD-1",
            site.Id,
            site.SiteCode.Value,
            site.Name,
            Guid.NewGuid(),
            "Engineer",
            DateTime.UtcNow.Date,
            VisitType.BM);

        var photo = VisitPhoto.CreatePendingUpload(
            visit.Id,
            PhotoType.Before,
            PhotoCategory.Rectifier,
            "Rectifier panel",
            "rectifier.jpg",
            "https://storage.local/quarantine/rectifier.jpg");

        visit.AddPhoto(photo);

        await context.Sites.AddAsync(site);
        await context.Visits.AddAsync(visit);
        await context.SaveChangesAsync();

        return visit.Photos.Single();
    }
}
