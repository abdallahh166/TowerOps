using FluentAssertions;
using Moq;
using TelecomPM.Application.Services;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class EditableVisitMutationServiceTests
{
    [Fact]
    public async Task ExecuteAsync_WhenVisitMissing_ReturnsFailure()
    {
        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Visit?)null);

        var unitOfWork = new Mock<IUnitOfWork>();
        var service = new EditableVisitMutationService(visitRepository.Object, unitOfWork.Object);

        var result = await service.ExecuteAsync(Guid.NewGuid(), _ => Task.FromResult("ok"), "Failed", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Visit not found");
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenVisitIsNotEditable_ReturnsFailure()
    {
        var visit = CreateVisit();
        visit.Cancel("locked");

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        var unitOfWork = new Mock<IUnitOfWork>();
        var service = new EditableVisitMutationService(visitRepository.Object, unitOfWork.Object);

        var result = await service.ExecuteAsync(visit.Id, _ => Task.FromResult("ok"), "Failed", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Visit cannot be edited");
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenMutationSucceeds_PersistsAndReturnsValue()
    {
        var visit = CreateVisit();

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        var unitOfWork = new Mock<IUnitOfWork>();
        var service = new EditableVisitMutationService(visitRepository.Object, unitOfWork.Object);

        var result = await service.ExecuteAsync(visit.Id, _ => Task.FromResult(42), "Failed", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
        visitRepository.Verify(r => r.UpdateAsync(visit, It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Visit CreateVisit()
        => Visit.Create(
            "V-9000",
            Guid.NewGuid(),
            "S-9000",
            "Site",
            Guid.NewGuid(),
            "Engineer",
            DateTime.UtcNow.AddDays(1),
            VisitType.PreventiveMaintenance);
}
