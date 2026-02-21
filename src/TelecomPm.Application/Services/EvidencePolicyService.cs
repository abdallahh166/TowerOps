using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Services;
using TelecomPM.Domain.ValueObjects;
using TelecomPM.Application.Common.Interfaces;

namespace TelecomPM.Application.Services;

public sealed class EvidencePolicyService : IEvidencePolicyService
{
    private readonly ISystemSettingsService _settingsService;

    public EvidencePolicyService(ISystemSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public ValidationResult Validate(Visit visit, EvidencePolicy policy)
    {
        var effectivePolicy = ResolveEffectivePolicy(visit.Type, policy);
        var result = new ValidationResult();

        if (visit.Photos.Count < effectivePolicy.MinPhotosRequired)
        {
            result.AddError(
                "EvidencePolicy.Photos",
                $"Insufficient photos. Required: {effectivePolicy.MinPhotosRequired}, Found: {visit.Photos.Count}");
        }

        if (effectivePolicy.ReadingsRequired && !visit.Readings.Any())
        {
            result.AddError("EvidencePolicy.Readings", "At least one reading is required");
        }

        if (effectivePolicy.ChecklistRequired)
        {
            var checklistCompletionPercent = CalculateChecklistCompletionPercent(visit);
            if (checklistCompletionPercent < effectivePolicy.MinChecklistCompletionPercent)
            {
                result.AddError(
                    "EvidencePolicy.Checklist",
                    $"Checklist completion is below required threshold. Required: {effectivePolicy.MinChecklistCompletionPercent}%, Found: {checklistCompletionPercent}%");
            }
        }

        return result;
    }

    private static int CalculateChecklistCompletionPercent(Visit visit)
    {
        if (visit.Checklists.Count == 0)
            return 0;

        var completedCount = visit.Checklists.Count(c => c.Status != CheckStatus.NA);
        return completedCount * 100 / visit.Checklists.Count;
    }

    private EvidencePolicy ResolveEffectivePolicy(VisitType visitType, EvidencePolicy fallback)
    {
        var keyPrefix = visitType switch
        {
            VisitType.BM or VisitType.PreventiveMaintenance => "BM",
            VisitType.CM or VisitType.CorrectiveMaintenance => "CM",
            _ => "BM"
        };

        var minPhotos = _settingsService
            .GetAsync($"Evidence:{keyPrefix}:MinPhotos", fallback.MinPhotosRequired)
            .GetAwaiter()
            .GetResult();

        var checklistPercent = _settingsService
            .GetAsync($"Evidence:{keyPrefix}:ChecklistCompletionPercent", fallback.MinChecklistCompletionPercent)
            .GetAwaiter()
            .GetResult();

        return EvidencePolicy.Create(
            visitType,
            minPhotos,
            fallback.ReadingsRequired,
            fallback.ChecklistRequired,
            checklistPercent);
    }
}
