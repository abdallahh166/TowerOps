using System.Linq;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Services;

namespace TelecomPM.Infrastructure.Services;

public sealed class VisitValidationService : IVisitValidationService
{
    public ValidationResult ValidateVisitCompletion(Visit visit, Site site)
    {
        var result = new ValidationResult();

        // Baseline quality checks only.
        // Threshold-based evidence rules are enforced by EvidencePolicyService.
        if (visit.ActualDuration != null && !visit.ActualDuration.IsValid())
        {
            result.AddError("Duration", "Visit duration is invalid (too short or too long)");
        }

        var readingsValidation = ValidateReadings(visit);
        result.Merge(readingsValidation);

        var photosValidation = ValidatePhotos(visit, site);
        result.Merge(photosValidation);

        var materialsValidation = ValidateMaterialUsage(visit);
        result.Merge(materialsValidation);

        return result;
    }

    public ValidationResult ValidateReadings(Visit visit)
    {
        var result = new ValidationResult();

        var outOfRangeReadings = visit.Readings.Where(r => !r.IsWithinRange).ToList();
        if (outOfRangeReadings.Any())
        {
            foreach (var reading in outOfRangeReadings)
            {
                result.AddWarning("Readings", $"{reading.ReadingType} is out of acceptable range: {reading.Value} {reading.Unit}");
            }
        }

        return result;
    }

    public ValidationResult ValidatePhotos(Visit visit, Site site)
    {
        var result = new ValidationResult();

        var invalidPhotos = visit.Photos.Where(p => !p.MeetsRequirements()).ToList();
        if (invalidPhotos.Any())
        {
            result.AddWarning("Photos", $"{invalidPhotos.Count} photos do not meet dimension requirements");
        }

        return result;
    }

    public ValidationResult ValidateMaterialUsage(Visit visit)
    {
        var result = new ValidationResult();

        foreach (var material in visit.MaterialsUsed)
        {
            if (!material.HasRequiredPhotos())
            {
                result.AddError("Materials",
                    $"Material '{material.MaterialName}' is missing before/after photos");
            }

            if (string.IsNullOrWhiteSpace(material.Reason))
            {
                result.AddError("Materials",
                    $"Material '{material.MaterialName}' is missing usage reason");
            }
        }

        return result;
    }
}

