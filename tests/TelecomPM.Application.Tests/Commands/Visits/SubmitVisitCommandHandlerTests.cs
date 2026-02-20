using FluentAssertions;
using Moq;
using TelecomPM.Application.Commands.Visits.SubmitVisit;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Services;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Application.Tests.Commands.Visits;

public class SubmitVisitCommandHandlerTests
{
    [Fact]
    public async Task Handle_WhenEvidencePolicyNotMet_ShouldReturnFailure()
    {
        var visit = Visit.Create(
            "V-SUBMIT-1",
            Guid.NewGuid(),
            "S-TNT-777",
            "Site 777",
            Guid.NewGuid(),
            "Engineer Submit",
            DateTime.UtcNow.AddDays(1),
            VisitType.PreventiveMaintenance);

        var site = Site.Create(
            "TNT777",
            "Site 777",
            "OMC-1",
            Guid.NewGuid(),
            "Cairo",
            "Nasr City",
            Coordinates.Create(30.0, 31.0),
            Address.Create("Street", "Cairo", "Cairo"),
            SiteType.Macro);

        var visitRepository = new Mock<IVisitRepository>();
        visitRepository
            .Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(visit);

        var siteRepository = new Mock<ISiteRepository>();
        siteRepository
            .Setup(r => r.GetByIdAsync(visit.SiteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(site);

        var validationService = new Mock<IVisitValidationService>();
        validationService
            .Setup(v => v.ValidateVisitCompletion(visit, site))
            .Returns(new ValidationResult());

        var evidencePolicyService = new Mock<IEvidencePolicyService>();
        var evidenceFailure = new ValidationResult();
        evidenceFailure.AddError("EvidencePolicy.Photos", "Insufficient photos");
        evidencePolicyService
            .Setup(v => v.Validate(visit, It.IsAny<EvidencePolicy>()))
            .Returns(evidenceFailure);

        var unitOfWork = new Mock<IUnitOfWork>();

        var handler = new SubmitVisitCommandHandler(
            visitRepository.Object,
            siteRepository.Object,
            validationService.Object,
            evidencePolicyService.Object,
            unitOfWork.Object);

        var result = await handler.Handle(new SubmitVisitCommand { VisitId = visit.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Evidence policy validation failed");
        visitRepository.Verify(r => r.UpdateAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
