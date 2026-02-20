using FluentAssertions;
using Moq;
using TelecomPM.Application.Commands.Visits.RescheduleVisit;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Application.Tests.Commands.Visits;

public class RescheduleVisitCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenVisitIsScheduled_ShouldRescheduleAndPersist()
    {
        var visit = CreateVisit();
        var newDate = DateTime.Today.AddDays(2);

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new RescheduleVisitCommandHandler(visitRepository.Object, unitOfWork.Object);

        var result = await handler.Handle(
            new RescheduleVisitCommand
            {
                VisitId = visit.Id,
                NewScheduledDate = newDate,
                Reason = "Customer requested change"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        visit.ScheduledDate.Should().Be(newDate);
        visitRepository.Verify(r => r.UpdateAsync(visit, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenVisitIsNotScheduled_ShouldReturnFailure()
    {
        var visit = CreateVisit();
        visit.StartVisit(Coordinates.Create(30.1, 31.2));

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new RescheduleVisitCommandHandler(visitRepository.Object, unitOfWork.Object);

        var result = await handler.Handle(
            new RescheduleVisitCommand
            {
                VisitId = visit.Id,
                NewScheduledDate = DateTime.Today.AddDays(1),
                Reason = "Conflict"
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only scheduled visits can be rescheduled");
        visitRepository.Verify(r => r.UpdateAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Visit CreateVisit()
        => Visit.Create(
            "V-6001",
            Guid.NewGuid(),
            "S-TNT-600",
            "Site 600",
            Guid.NewGuid(),
            "Engineer Y",
            DateTime.Today.AddDays(1),
            VisitType.PreventiveMaintenance);
}
