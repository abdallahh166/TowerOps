using FluentAssertions;
using Moq;
using TowerOps.Application.Common.Interfaces;
using TowerOps.Application.Services;
using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.ValueObjects;
using Xunit;

namespace TowerOps.Application.Tests.Services;

public class EvidencePolicyServiceTests
{
    private readonly Mock<ISystemSettingsService> _settingsServiceMock;
    private readonly EvidencePolicyService _service;

    public EvidencePolicyServiceTests()
    {
        _settingsServiceMock = new Mock<ISystemSettingsService>();
        _settingsServiceMock
            .Setup(s => s.GetAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string _, int fallback, CancellationToken _) => fallback);
        _service = new EvidencePolicyService(_settingsServiceMock.Object);
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenPhotosAreInsufficient()
    {
        var visit = CreateVisit(VisitType.BM);
        visit.AddPhoto(CreatePhoto(visit.Id, "before-1.jpg"));

        var policy = EvidencePolicy.Create(
            VisitType.BM,
            minPhotosRequired: 3,
            readingsRequired: false,
            checklistRequired: false,
            minChecklistCompletionPercent: 0);

        var result = await _service.ValidateAsync(visit, policy);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainKey("EvidencePolicy.Photos");
    }

    [Fact]
    public async Task Validate_ShouldPass_WhenPolicyIsMet()
    {
        var visit = CreateVisit(VisitType.BM);

        visit.AddPhoto(CreatePhoto(visit.Id, "before-1.jpg"));
        visit.AddPhoto(CreatePhoto(visit.Id, "before-2.jpg"));
        visit.AddPhoto(CreatePhoto(visit.Id, "before-3.jpg"));

        visit.AddReading(VisitReading.Create(visit.Id, "Rectifier DC Voltage", "Power", 53m, "V"));

        var checklist1 = VisitChecklist.Create(visit.Id, "Electrical", "Check 1", "desc");
        checklist1.UpdateStatus(CheckStatus.OK);
        visit.AddChecklistItem(checklist1);

        var checklist2 = VisitChecklist.Create(visit.Id, "Electrical", "Check 2", "desc");
        checklist2.UpdateStatus(CheckStatus.OK);
        visit.AddChecklistItem(checklist2);

        var policy = EvidencePolicy.Create(
            VisitType.BM,
            minPhotosRequired: 3,
            readingsRequired: true,
            checklistRequired: true,
            minChecklistCompletionPercent: 80);

        var result = await _service.ValidateAsync(visit, policy);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ShouldReadThresholdsFromSettings()
    {
        var visit = CreateVisit(VisitType.BM);
        visit.AddPhoto(CreatePhoto(visit.Id, "only-one.jpg"));

        _settingsServiceMock
            .Setup(s => s.GetAsync("Evidence:BM:MinPhotos", It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _settingsServiceMock
            .Setup(s => s.GetAsync("Evidence:BM:ChecklistCompletionPercent", It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var policy = EvidencePolicy.Create(VisitType.BM, 3, false, false, 80);

        var result = await _service.ValidateAsync(visit, policy);

        result.IsValid.Should().BeTrue();
    }

    private static Visit CreateVisit(VisitType type)
        => Visit.Create(
            "V-EVID-1",
            Guid.NewGuid(),
            "S-TNT-500",
            "Site 500",
            Guid.NewGuid(),
            "Engineer A",
            DateTime.UtcNow.AddDays(1),
            type);

    private static VisitPhoto CreatePhoto(Guid visitId, string fileName)
        => VisitPhoto.Create(
            visitId,
            PhotoType.Before,
            PhotoCategory.ShelterInside,
            "Shelter",
            fileName,
            $"/photos/{fileName}");
}
