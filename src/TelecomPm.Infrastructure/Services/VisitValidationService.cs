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

        // Validate readings
        var readingsValidation = ValidateReadings(visit);
        result.Merge(readingsValidation);

        // Validate photos
        var photosValidation = ValidatePhotos(visit, site);
        result.Merge(photosValidation);

        // Validate materials
        var materialsValidation = ValidateMaterialUsage(visit);
        result.Merge(materialsValidation);

        return result;
    }

    public ValidationResult ValidateReadings(Visit visit)
    {
        var result = new ValidationResult();

        // Check minimum readings count
        var requiredReadings = 15; // Based on site complexity
        if (visit.Readings.Count < requiredReadings)
        {
            result.AddError("Readings", $"At least {requiredReadings} readings are required");
        }

        // Check for out-of-range readings
        var outOfRangeReadings = visit.Readings.Where(r => !r.IsWithinRange).ToList();
        if (outOfRangeReadings.Any())
        {
            result.AddWarning("Readings", 
                $"{outOfRangeReadings.Count} reading(s) are out of acceptable range");
        }

        // Validate mandatory reading types
        var mandatoryTypes = new[] { "PhaseVoltage", "Current", "Temperature" };
        foreach (var type in mandatoryTypes)
        {
            if (!visit.Readings.Any(r => r.ReadingType.Contains(type)))
            {
                result.AddError("Readings", $"Missing mandatory reading type: {type}");
            }
        }

        return result;
    }

    public ValidationResult ValidatePhotos(Visit visit, Site site)
    {
        var result = new ValidationResult();

        // Check minimum photos count
        var requiredPhotos = site.RequiredPhotosCount;
        if (visit.Photos.Count < requiredPhotos)
        {
            result.AddError("Photos", 
                $"At least {requiredPhotos} photos are required for this site");
        }

        // Validate Before/After photos
        var beforePhotos = visit.Photos.Count(p => p.Type == Domain.Enums.PhotoType.Before);
        var afterPhotos = visit.Photos.Count(p => p.Type == Domain.Enums.PhotoType.After);

        if (beforePhotos < 30)
        {
            result.AddError("Photos", "At least 30 'Before' photos are required");
        }

        if (afterPhotos < 30)
        {
            result.AddError("Photos", "At least 30 'After' photos are required");
        }

        // Check mandatory categories
        var mandatoryCategories = new[]
        {
            Domain.Enums.PhotoCategory.ShelterInside,
            Domain.Enums.PhotoCategory.ShelterOutside,
            Domain.Enums.PhotoCategory.Tower,
            Domain.Enums.PhotoCategory.GEDP,
            Domain.Enums.PhotoCategory.Rectifier,
            Domain.Enums.PhotoCategory.Batteries
        };

        foreach (var category in mandatoryCategories)
        {
            if (!visit.Photos.Any(p => p.Category == category))
            {
                result.AddWarning("Photos", $"Missing photos for category: {category}");
            }
        }

        return result;
    }

    public ValidationResult ValidateMaterialUsage(Visit visit)
    {
        var result = new ValidationResult();

        // Check if materials have required photos
        foreach (var material in visit.MaterialsUsed)
        {
            if (!material.HasRequiredPhotos())
            {
                result.AddError("Materials", 
                    $"Material '{material.MaterialName}' requires both before and after photos");
            }
        }

        // Check if materials have reasons
        var materialsWithoutReason = visit.MaterialsUsed
            .Where(m => string.IsNullOrWhiteSpace(m.Reason))
            .ToList();

        if (materialsWithoutReason.Any())
        {
            result.AddError("Materials", "All materials must have usage reasons");
        }

        return result;
    }
}

