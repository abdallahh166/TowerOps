using FluentAssertions;
using TelecomPM.Application.Services;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.ValueObjects;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class EvidencePolicyServiceTests
{
    private readonly EvidencePolicyService _service = new();

    [Fact]
    public void Validate_ShouldFail_WhenPhotosAreInsufficient()
    {
        var visit = CreateVisit(VisitType.PreventiveMaintenance);
        visit.AddPhoto(CreatePhoto(visit.Id, "before-1.jpg"));

        var policy = EvidencePolicy.Create(
            VisitType.PreventiveMaintenance,
            minPhotosRequired: 3,
            readingsRequired: false,
            checklistRequired: false,
            minChecklistCompletionPercent: 0);

        var result = _service.Validate(visit, policy);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainKey("EvidencePolicy.Photos");
    }

    [Fact]
    public void Validate_ShouldPass_WhenPolicyIsMet()
    {
        var visit = CreateVisit(VisitType.PreventiveMaintenance);

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
            VisitType.PreventiveMaintenance,
            minPhotosRequired: 3,
            readingsRequired: true,
            checklistRequired: true,
            minChecklistCompletionPercent: 80);

        var result = _service.Validate(visit, policy);

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
