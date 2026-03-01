using FluentAssertions;
using Moq;
using TowerOps.Application.Commands.Privacy.RequestMyOperationalDataExport;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.Queries.Privacy.GetMyOperationalDataExport;
using TowerOps.Domain.Entities.UserDataExports;
using TowerOps.Domain.Entities.Users;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Entities.WorkOrders;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.ValueObjects;
using Xunit;

namespace TowerOps.Application.Tests.Commands.Privacy;

public class UserDataExportCommandTests
{
    [Fact]
    public async Task RequestMyOperationalDataExport_ShouldCreateCompletedExportRequest()
    {
        var userId = Guid.NewGuid();
        var user = User.Create("Export User", "export.user@towerops.com", "+201000000000", UserRole.PMEngineer, Guid.NewGuid());

        var visit = Visit.Create(
            "V-EXP-1",
            Guid.NewGuid(),
            "CAI001",
            "Export Site",
            userId,
            "Engineer",
            DateTime.UtcNow.Date,
            VisitType.BM);

        visit.AddPhoto(VisitPhoto.Create(visit.Id, PhotoType.Before, PhotoCategory.Other, "before", "before.jpg", "/before.jpg"));
        visit.AddReading(VisitReading.Create(visit.Id, "BatteryVoltage", "Power", 53m, "V"));

        var workOrder = WorkOrder.Create(
            "WO-EXP-1",
            "CAI001",
            "CAI",
            SlaClass.P2,
            "Rectifier check required");
        workOrder.Assign(userId, "Engineer", "Manager");

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByIdAsNoTrackingAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.GetByEngineerIdAsNoTrackingAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Visit> { visit });
        visitRepository
            .Setup(r => r.GetByIdAsNoTrackingAsync(visit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        var workOrderRepository = new Mock<IWorkOrderRepository>();
        workOrderRepository
            .Setup(r => r.GetByUserOwnershipAsNoTrackingAsync(userId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<WorkOrder> { workOrder });

        UserDataExportRequest? persistedRequest = null;
        var exportRepository = new Mock<IUserDataExportRequestRepository>();
        exportRepository
            .Setup(r => r.AddAsync(It.IsAny<UserDataExportRequest>(), It.IsAny<CancellationToken>()))
            .Callback<UserDataExportRequest, CancellationToken>((entity, _) => persistedRequest = entity)
            .Returns(Task.CompletedTask);

        var settingsService = new Mock<ISystemSettingsService>();
        settingsService
            .Setup(s => s.GetAsync("Privacy:Export:MaxItemsPerCollection", 2000, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2000);
        settingsService
            .Setup(s => s.GetAsync("Privacy:Export:TtlDays", 30, It.IsAny<CancellationToken>()))
            .ReturnsAsync(30);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var sut = new RequestMyOperationalDataExportCommandHandler(
            userRepository.Object,
            visitRepository.Object,
            workOrderRepository.Object,
            exportRepository.Object,
            settingsService.Object,
            unitOfWork.Object);

        var result = await sut.Handle(
            new RequestMyOperationalDataExportCommand { UserId = userId },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        persistedRequest.Should().NotBeNull();
        persistedRequest!.Status.Should().Be(UserDataExportStatus.Completed);
        persistedRequest.PayloadJson.Should().NotBeNullOrWhiteSpace();
        persistedRequest.PayloadJson.Should().Contain("\"visits\"");
        persistedRequest.PayloadJson.Should().Contain("\"workOrders\"");
    }

    [Fact]
    public async Task GetMyOperationalDataExport_ShouldFail_WhenRequestExpired()
    {
        var userId = Guid.NewGuid();
        var request = UserDataExportRequest.Create(userId, 1);
        request.Complete("{\"ok\":true}");

        var expiresAtProperty = typeof(UserDataExportRequest).GetProperty(nameof(UserDataExportRequest.ExpiresAtUtc));
        expiresAtProperty!.SetValue(request, DateTime.UtcNow.AddMinutes(-5));

        var repository = new Mock<IUserDataExportRequestRepository>();
        repository
            .Setup(r => r.GetByIdForUserAsync(request.Id, userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);
        repository
            .Setup(r => r.UpdateAsync(request, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork
            .Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var sut = new GetMyOperationalDataExportQueryHandler(repository.Object, unitOfWork.Object);

        var result = await sut.Handle(
            new GetMyOperationalDataExportQuery
            {
                UserId = userId,
                RequestId = request.Id
            },
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("expired");
        repository.Verify(r => r.UpdateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
