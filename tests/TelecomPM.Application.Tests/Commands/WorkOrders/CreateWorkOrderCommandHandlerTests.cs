using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using TelecomPM.Application.Commands.WorkOrders.CreateWorkOrder;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Application.Tests.Commands.WorkOrders;

public class CreateWorkOrderCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenSiteIsEquipmentOnly_AndScopeIsTowerInfrastructure_ShouldReturnFailure()
    {
        var workOrderRepository = new Mock<IWorkOrderRepository>();
        workOrderRepository
            .Setup(r => r.GetByWoNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TelecomPM.Domain.Entities.WorkOrders.WorkOrder?)null);

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

        var handler = new CreateWorkOrderCommandHandler(
            workOrderRepository.Object,
            siteRepository.Object,
            unitOfWork.Object,
            mapper.Object,
            sender.Object);

        var result = await handler.Handle(new CreateWorkOrderCommand
        {
            WoNumber = "WO-001",
            SiteCode = "TNT001",
            OfficeCode = "CAI",
            SlaClass = SlaClass.P2,
            Scope = WorkOrderScope.TowerInfrastructure,
            IssueDescription = "Tower issue"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("TowerInfrastructure scope is not allowed");
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        workOrderRepository.Verify(r => r.AddAsync(It.IsAny<TelecomPM.Domain.Entities.WorkOrders.WorkOrder>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
