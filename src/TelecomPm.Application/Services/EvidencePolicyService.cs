using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Services;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Application.Services;

public sealed class EvidencePolicyService : IEvidencePolicyService
{
    public ValidationResult Validate(Visit visit, EvidencePolicy policy)
    {
        var result = new ValidationResult();

        if (visit.Photos.Count < policy.MinPhotosRequired)
        {
            result.AddError(
                "EvidencePolicy.Photos",
                $"Insufficient photos. Required: {policy.MinPhotosRequired}, Found: {visit.Photos.Count}");
        }

        if (policy.ReadingsRequired && !visit.Readings.Any())
        {
            result.AddError("EvidencePolicy.Readings", "At least one reading is required");
        }

        if (policy.ChecklistRequired)
        {
            var checklistCompletionPercent = CalculateChecklistCompletionPercent(visit);
            if (checklistCompletionPercent < policy.MinChecklistCompletionPercent)
            {
                result.AddError(
                    "EvidencePolicy.Checklist",
                    $"Checklist completion is below required threshold. Required: {policy.MinChecklistCompletionPercent}%, Found: {checklistCompletionPercent}%");
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
}
