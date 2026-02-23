using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using TelecomPM.Application.Commands.WorkOrders.AcceptByCustomer;
using TelecomPM.Application.Commands.WorkOrders.RejectByCustomer;
using TelecomPM.Application.Common;
using TelecomPM.Application.Common.Interfaces;
using TelecomPM.Application.DTOs.WorkOrders;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Users;
using TelecomPM.Domain.Entities.WorkOrders;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Application.Tests.Commands.WorkOrders;

public class CustomerDecisionPortalGuardTests
{
    [Fact]
    public async Task AcceptByCustomer_ShouldFail_WhenPortalUserDoesNotOwnSite()
    {
        var workOrder = CreatePendingCustomerAcceptanceWorkOrder();
        var portalUser = CreatePortalUser("ORANGE");
        var site = CreateSite(workOrder.SiteCode, "VODAFONE");

        var workOrderRepository = new Mock<IWorkOrderRepository>();
        workOrderRepository
            .Setup(r => r.GetByIdAsync(workOrder.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);

        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = CreateMapperMock();
        var sender = CreateSenderMock();
        var currentUser = CreateCurrentUserServiceMock(portalUser.Id, "portal@orange.com");

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByIdAsNoTrackingAsync(portalUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(portalUser);

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.GetBySiteCodeAsNoTrackingAsync(workOrder.SiteCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(site);

        var sut = new AcceptByCustomerCommandHandler(
            workOrderRepository.Object,
            unitOfWork.Object,
            mapper.Object,
            sender.Object,
            currentUser.Object,
            userRepository.Object,
            siteRepository.Object);

        var result = await sut.Handle(new AcceptByCustomerCommand
        {
            WorkOrderId = workOrder.Id,
            AcceptedBy = "portal@orange.com"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not allowed");
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        sender.Verify(s => s.Send(It.IsAny<IRequest<Result<Guid>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RejectByCustomer_ShouldFail_WhenPortalUserDoesNotOwnSite()
    {
        var workOrder = CreatePendingCustomerAcceptanceWorkOrder();
        var portalUser = CreatePortalUser("ORANGE");
        var site = CreateSite(workOrder.SiteCode, "WE");

        var workOrderRepository = new Mock<IWorkOrderRepository>();
        workOrderRepository
            .Setup(r => r.GetByIdAsync(workOrder.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workOrder);

        var unitOfWork = new Mock<IUnitOfWork>();
        var mapper = CreateMapperMock();
        var sender = CreateSenderMock();
        var currentUser = CreateCurrentUserServiceMock(portalUser.Id, "portal@orange.com");

        var userRepository = new Mock<IUserRepository>();
        userRepository
            .Setup(r => r.GetByIdAsNoTrackingAsync(portalUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(portalUser);

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.GetBySiteCodeAsNoTrackingAsync(workOrder.SiteCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(site);

        var sut = new RejectByCustomerCommandHandler(
            workOrderRepository.Object,
            unitOfWork.Object,
            mapper.Object,
            sender.Object,
            currentUser.Object,
            userRepository.Object,
            siteRepository.Object);

        var result = await sut.Handle(new RejectByCustomerCommand
        {
            WorkOrderId = workOrder.Id,
            Reason = "Client rejection"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not allowed");
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        sender.Verify(s => s.Send(It.IsAny<IRequest<Result<Guid>>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    private static WorkOrder CreatePendingCustomerAcceptanceWorkOrder()
    {
        var workOrder = WorkOrder.Create("WO-PORTAL-1", "CAI001", "CAI", SlaClass.P2, "Issue");
        workOrder.Assign(Guid.NewGuid(), "Engineer", "Manager");
        workOrder.Start();
        workOrder.Complete();
        workOrder.SubmitForCustomerAcceptance();
        return workOrder;
    }

    private static User CreatePortalUser(string clientCode)
    {
        var user = User.Create("Portal User", "portal@orange.com", "0100000000", UserRole.Manager, Guid.NewGuid());
        user.EnableClientPortalAccess(clientCode);
        return user;
    }

    private static Site CreateSite(string siteCode, string clientCode)
    {
        var site = Site.Create(
            siteCode,
            "Site",
            "OMC",
            Guid.NewGuid(),
            "Region",
            "Sub",
            Coordinates.Create(30.0, 31.0),
            Address.Create("Street", "City", "Region"),
            SiteType.Macro);
        site.SetClientCode(clientCode);
        return site;
    }

    private static Mock<IMapper> CreateMapperMock()
    {
        var mapper = new Mock<IMapper>();
        mapper
            .Setup(m => m.Map<WorkOrderDto>(It.IsAny<WorkOrder>()))
            .Returns(new WorkOrderDto());
        return mapper;
    }

    private static Mock<ISender> CreateSenderMock()
    {
        var sender = new Mock<ISender>();
        sender
            .Setup(s => s.Send(It.IsAny<IRequest<Result<Guid>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(Guid.NewGuid()));
        return sender;
    }

    private static Mock<ICurrentUserService> CreateCurrentUserServiceMock(Guid userId, string email)
    {
        var currentUser = new Mock<ICurrentUserService>();
        currentUser.SetupGet(x => x.IsAuthenticated).Returns(true);
        currentUser.SetupGet(x => x.UserId).Returns(userId);
        currentUser.SetupGet(x => x.Email).Returns(email);
        return currentUser;
    }
}
