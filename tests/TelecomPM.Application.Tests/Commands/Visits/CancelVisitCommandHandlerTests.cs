using FluentAssertions;
using Moq;
using TelecomPM.Application.Commands.Visits.CancelVisit;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Commands.Visits;

public class CancelVisitCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenVisitExists_ShouldCancelVisitAndPersist()
    {
        var visit = CreateVisit();
        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new CancelVisitCommandHandler(visitRepository.Object, unitOfWork.Object);

        var command = new CancelVisitCommand
        {
            VisitId = visit.Id,
            Reason = "Weather conditions"
        };

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        visit.Status.Should().Be(VisitStatus.Cancelled);
        visit.EngineerNotes.Should().Be("Weather conditions");
        visitRepository.Verify(r => r.UpdateAsync(visit, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenVisitDoesNotExist_ShouldReturnFailure()
    {
        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Visit?)null);

        var unitOfWork = new Mock<IUnitOfWork>();
        var handler = new CancelVisitCommandHandler(visitRepository.Object, unitOfWork.Object);

        var result = await handler.Handle(
            new CancelVisitCommand { VisitId = Guid.NewGuid(), Reason = "No access" },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Visit not found");
        visitRepository.Verify(r => r.UpdateAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Visit CreateVisit()
        => Visit.Create(
            "V-5001",
            Guid.NewGuid(),
            "S-TNT-500",
            "Site 500",
            Guid.NewGuid(),
            "Engineer Z",
            DateTime.UtcNow.AddDays(1),
            VisitType.PreventiveMaintenance);
}
