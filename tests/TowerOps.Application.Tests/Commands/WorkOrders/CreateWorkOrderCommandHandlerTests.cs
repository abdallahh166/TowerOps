using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using TowerOps.Application.Commands.AuditLogs.LogAuditEntry;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.Commands.WorkOrders.CreateWorkOrder;
using TowerOps.Application.DTOs.WorkOrders;
using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Interfaces.Repositories;
using TowerOps.Domain.ValueObjects;
using Xunit;

namespace TowerOps.Application.Tests.Commands.WorkOrders;

public class CreateWorkOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenSiteIsEquipmentOnly_AndScopeIsTowerInfrastructure_ShouldReturnFailure()
    {
        var workOrderRepository = new Mock<IWorkOrderRepository>();
        workOrderRepository
            .Setup(r => r.GetByWoNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TowerOps.Domain.Entities.WorkOrders.WorkOrder?)null);

        var guestSite = Site.Create(
            "TNT001",
            "Site 1",
            "OMC-1",
            Guid.NewGuid(),
            "Region",
            "SubRegion",
            Coordinates.Create(30.0, 31.0),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);
        guestSite.SetOwnership(TowerOwnershipType.Guest, "Tower Owner", "AGR-1", "Host", "+201111111111");

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.GetBySiteCodeAsNoTrackingAsync("TNT001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(guestSite);

        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = new Mock<IMapper>();
        var sender = new Mock<ISender>();
        var settings = new Mock<ISystemSettingsService>();

        var handler = new CreateWorkOrderCommandHandler(
            workOrderRepository.Object,
            siteRepository.Object,
            unitOfWork.Object,
            mapper.Object,
            sender.Object,
            settings.Object);

        var result = await handler.Handle(new CreateWorkOrderCommand
        {
            WoNumber = "WO-001",
            SiteCode = "TNT001",
            OfficeCode = "CAI",
            WorkOrderType = WorkOrderType.CM,
            SlaClass = SlaClass.P2,
            Scope = WorkOrderScope.TowerInfrastructure,
            IssueDescription = "Tower issue"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("TowerInfrastructure scope is not allowed");
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        workOrderRepository.Verify(r => r.AddAsync(It.IsAny<TowerOps.Domain.Entities.WorkOrders.WorkOrder>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUseSettingsSnapshot_ForSlaDeadlinesAtCreation()
    {
        var workOrderRepository = new Mock<IWorkOrderRepository>();
        workOrderRepository
            .Setup(r => r.GetByWoNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TowerOps.Domain.Entities.WorkOrders.WorkOrder?)null);

        TowerOps.Domain.Entities.WorkOrders.WorkOrder? capturedWorkOrder = null;
        workOrderRepository
            .Setup(r => r.AddAsync(It.IsAny<TowerOps.Domain.Entities.WorkOrders.WorkOrder>(), It.IsAny<CancellationToken>()))
            .Callback<TowerOps.Domain.Entities.WorkOrders.WorkOrder, CancellationToken>((wo, _) => capturedWorkOrder = wo)
            .Returns(Task.CompletedTask);

        var site = Site.Create(
            "TNT002",
            "Site 2",
            "OMC-1",
            Guid.NewGuid(),
            "Region",
            "SubRegion",
            Coordinates.Create(30.0, 31.0),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.GetBySiteCodeAsNoTrackingAsync("TNT002", It.IsAny<CancellationToken>()))
            .ReturnsAsync(site);

        var unitOfWork = new Mock<IUnitOfWork>();

        var mapper = new Mock<IMapper>();
        mapper
            .Setup(m => m.Map<WorkOrderDto>(It.IsAny<TowerOps.Domain.Entities.WorkOrders.WorkOrder>()))
            .Returns(new WorkOrderDto());

        var sender = new Mock<ISender>();
        sender
            .Setup(s => s.Send(It.IsAny<LogAuditEntryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TowerOps.Application.Common.Result.Success(Guid.NewGuid()));

        var settings = new Mock<ISystemSettingsService>();
        settings
            .Setup(s => s.GetAsync("SLA:CM:P1:ResponseMinutes", 60, It.IsAny<CancellationToken>()))
            .ReturnsAsync(90);
        settings
            .Setup(s => s.GetAsync("SLA:CM:P1:ResolutionMinutes", 240, It.IsAny<CancellationToken>()))
            .ReturnsAsync(360);

        var handler = new CreateWorkOrderCommandHandler(
            workOrderRepository.Object,
            siteRepository.Object,
            unitOfWork.Object,
            mapper.Object,
            sender.Object,
            settings.Object);

        var result = await handler.Handle(new CreateWorkOrderCommand
        {
            WoNumber = "WO-002",
            SiteCode = "TNT002",
            OfficeCode = "CAI",
            WorkOrderType = WorkOrderType.CM,
            SlaClass = SlaClass.P1,
            Scope = WorkOrderScope.ClientEquipment,
            IssueDescription = "Power issue"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        capturedWorkOrder.Should().NotBeNull();
        (capturedWorkOrder!.ResponseDeadlineUtc - capturedWorkOrder.CreatedAt).TotalMinutes.Should().BeApproximately(90, 0.01);
        (capturedWorkOrder.ResolutionDeadlineUtc - capturedWorkOrder.CreatedAt).TotalMinutes.Should().BeApproximately(360, 0.01);

        settings.Verify(s => s.GetAsync("SLA:CM:P1:ResponseMinutes", 60, It.IsAny<CancellationToken>()), Times.Once);
        settings.Verify(s => s.GetAsync("SLA:CM:P1:ResolutionMinutes", 240, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenPmWorkOrder_ShouldUseScheduledVisitDateAsSlaStart()
    {
        var workOrderRepository = new Mock<IWorkOrderRepository>();
        workOrderRepository
            .Setup(r => r.GetByWoNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TowerOps.Domain.Entities.WorkOrders.WorkOrder?)null);

        TowerOps.Domain.Entities.WorkOrders.WorkOrder? capturedWorkOrder = null;
        workOrderRepository
            .Setup(r => r.AddAsync(It.IsAny<TowerOps.Domain.Entities.WorkOrders.WorkOrder>(), It.IsAny<CancellationToken>()))
            .Callback<TowerOps.Domain.Entities.WorkOrders.WorkOrder, CancellationToken>((wo, _) => capturedWorkOrder = wo)
            .Returns(Task.CompletedTask);

        var site = Site.Create(
            "TNT003",
            "Site 3",
            "OMC-1",
            Guid.NewGuid(),
            "Region",
            "SubRegion",
            Coordinates.Create(30.0, 31.0),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.GetBySiteCodeAsNoTrackingAsync("TNT003", It.IsAny<CancellationToken>()))
            .ReturnsAsync(site);

        var unitOfWork = new Mock<IUnitOfWork>();

        var mapper = new Mock<IMapper>();
        mapper
            .Setup(m => m.Map<WorkOrderDto>(It.IsAny<TowerOps.Domain.Entities.WorkOrders.WorkOrder>()))
            .Returns(new WorkOrderDto());

        var sender = new Mock<ISender>();
        sender
            .Setup(s => s.Send(It.IsAny<LogAuditEntryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(TowerOps.Application.Common.Result.Success(Guid.NewGuid()));

        var settings = new Mock<ISystemSettingsService>();
        settings
            .Setup(s => s.GetAsync("SLA:PM:P1:ResponseMinutes", 60, It.IsAny<CancellationToken>()))
            .ReturnsAsync(60);
        settings
            .Setup(s => s.GetAsync("SLA:PM:P1:ResolutionMinutes", 240, It.IsAny<CancellationToken>()))
            .ReturnsAsync(240);

        var handler = new CreateWorkOrderCommandHandler(
            workOrderRepository.Object,
            siteRepository.Object,
            unitOfWork.Object,
            mapper.Object,
            sender.Object,
            settings.Object);

        var scheduled = new DateTime(2026, 3, 1, 8, 0, 0, DateTimeKind.Utc);

        var result = await handler.Handle(new CreateWorkOrderCommand
        {
            WoNumber = "WO-003",
            SiteCode = "TNT003",
            OfficeCode = "CAI",
            WorkOrderType = WorkOrderType.PM,
            ScheduledVisitDateUtc = scheduled,
            SlaClass = SlaClass.P1,
            Scope = WorkOrderScope.ClientEquipment,
            IssueDescription = "Planned maintenance"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        capturedWorkOrder.Should().NotBeNull();
        capturedWorkOrder!.SlaStartAtUtc.Should().Be(scheduled);
        capturedWorkOrder.ResponseDeadlineUtc.Should().Be(scheduled.AddMinutes(60));
        capturedWorkOrder.ResolutionDeadlineUtc.Should().Be(scheduled.AddMinutes(240));
    }
}
