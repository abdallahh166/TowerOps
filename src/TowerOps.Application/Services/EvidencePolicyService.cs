using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Services;
using TowerOps.Domain.ValueObjects;
using TowerOps.Application.Common.Interfaces;

namespace TowerOps.Application.Services;

public sealed class EvidencePolicyService : IEvidencePolicyService
{
    private readonly ISystemSettingsService _settingsService;

    public EvidencePolicyService(ISystemSettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task<ValidationResult> ValidateAsync(
        Visit visit,
        EvidencePolicy policy,
        CancellationToken cancellationToken = default)
    {
        var effectivePolicy = await GetEffectivePolicyAsync(visit.Type, policy, cancellationToken);
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

    public async Task<EvidencePolicy> GetEffectivePolicyAsync(
        VisitType visitType,
        EvidencePolicy fallback,
        CancellationToken cancellationToken = default)
    {
        return await ResolveEffectivePolicyAsync(visitType, fallback, cancellationToken);
    }

    private static int CalculateChecklistCompletionPercent(Visit visit)
    {
        if (visit.Checklists.Count == 0)
            return 0;

        var completedCount = visit.Checklists.Count(c => c.Status != CheckStatus.NA);
        return completedCount * 100 / visit.Checklists.Count;
    }

    private async Task<EvidencePolicy> ResolveEffectivePolicyAsync(
        VisitType visitType,
        EvidencePolicy fallback,
        CancellationToken cancellationToken)
    {
        var canonicalVisitType = visitType.ToCanonical();
        var keyPrefix = canonicalVisitType.IsCm() ? "CM" : "BM";

        var minPhotos = await _settingsService.GetAsync(
            $"Evidence:{keyPrefix}:MinPhotos",
            fallback.MinPhotosRequired,
            cancellationToken);

        var checklistPercent = await _settingsService.GetAsync(
            $"Evidence:{keyPrefix}:ChecklistCompletionPercent",
            fallback.MinChecklistCompletionPercent,
            cancellationToken);

        return EvidencePolicy.Create(
            canonicalVisitType,
            minPhotos,
            fallback.ReadingsRequired,
            fallback.ChecklistRequired,
            checklistPercent);
    }
}
