using FluentAssertions;
using Moq;
using TelecomPM.Application.Queries.Visits.GetVisitEvidenceStatus;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Interfaces.Repositories;
using TelecomPM.Domain.Services;
using TelecomPM.Domain.ValueObjects;
using Xunit;


namespace TelecomPM.Application.Tests.Queries.Visits;

public class GetVisitEvidenceStatusQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithExistingVisit_ShouldReturnEvidenceSnapshot()
    {
        var visit = Visit.Create(
            "V-2001",
            Guid.NewGuid(),
            "S-TNT-100",
            "Demo Site",
            Guid.NewGuid(),
            "Engineer A",
            DateTime.UtcNow,
            VisitType.PreventiveMaintenance);

        for (var i = 0; i < 30; i++)
        {
           //s PhotoCategory.Panorama
            visit.AddPhoto(VisitPhoto.Create(visit.Id, PhotoType.Before, PhotoCategory.Other, string.Empty, $"b-{i}.jpg", $"/photos/b-{i}.jpg"));
            visit.AddPhoto(VisitPhoto.Create(visit.Id, PhotoType.After,  PhotoCategory.Other, string.Empty, $"a-{i}.jpg", $"/photos/a-{i}.jpg"));
        }

        for (var i = 0; i < 15; i++)
        {
            visit.AddReading(VisitReading.Create(visit.Id, $"R{i}", "Electrical", 12 + i, "V"));
        }

        var c1 = VisitChecklist.Create(visit.Id, "Power", "Battery", "Voltage check", true);
        var c2 = VisitChecklist.Create(visit.Id, "Power", "Rectifier", "Status check", true);
        visit.AddChecklistItem(c1);
        visit.AddChecklistItem(c2);
        visit.UpdateChecklistItem(c1.Id, CheckStatus.OK);
        visit.UpdateChecklistItem(c2.Id, CheckStatus.OK);

        var repo = new Mock<IVisitRepository>();
        repo.Setup(r => r.GetByIdAsync(visit.Id, It.IsAny<CancellationToken>())).ReturnsAsync(visit);
        var evidencePolicyService = new Mock<IEvidencePolicyService>();
        evidencePolicyService
            .Setup(s => s.GetEffectivePolicyAsync(visit.Type, It.IsAny<EvidencePolicy>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(EvidencePolicy.Create(VisitType.BM, 30, true, true, 100));

        var handler = new GetVisitEvidenceStatusQueryHandler(repo.Object, evidencePolicyService.Object);
        var result = await handler.Handle(new GetVisitEvidenceStatusQuery { VisitId = visit.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.BeforePhotos.Should().Be(30);
        result.Value.AfterPhotos.Should().Be(30);
        result.Value.ReadingsCount.Should().Be(15);
        result.Value.ChecklistItems.Should().Be(2);
        result.Value.CompletedChecklistItems.Should().Be(2);
        result.Value.CompletionPercentage.Should().Be(100);
    }

    [Fact]
    public async Task Handle_WhenVisitDoesNotExist_ShouldReturnFailure()
    {
        var repo = new Mock<IVisitRepository>();
        repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((Visit?)null);
        var evidencePolicyService = new Mock<IEvidencePolicyService>();

        var handler = new GetVisitEvidenceStatusQueryHandler(repo.Object, evidencePolicyService.Object);
        var result = await handler.Handle(new GetVisitEvidenceStatusQuery { VisitId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Visit not found");
    }
}
