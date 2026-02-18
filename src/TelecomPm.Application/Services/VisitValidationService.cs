using System.Collections.Generic;
using System.Linq;
using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Models;
using TelecomPM.Domain.Services;

namespace TelecomPM.Application.Services;

public sealed class VisitValidationService : IVisitValidationService
{
    public ValidationResult ValidateVisitCompletion(Visit visit, Site site)
    {
        var result = new ValidationResult();

        // Validate duration
        if (visit.ActualDuration != null && !visit.ActualDuration.IsValid())
        {
            result.AddError("Duration", "Visit duration is invalid (too short or too long)");
        }

        // Validate readings
        var readingValidation = ValidateReadings(visit);
        result.Merge(readingValidation);

        // Validate photos
        var photoValidation = ValidatePhotos(visit, site);
        result.Merge(photoValidation);

        // Validate materials
        var materialValidation = ValidateMaterialUsage(visit);
        result.Merge(materialValidation);

        return result;
    }

    public ValidationResult ValidateReadings(Visit visit)
    {
        var result = new ValidationResult();

        var requiredReadingTypes = new[]
        {
            "Phase1-Neutral Voltage",
            "Phase2-Neutral Voltage",
            "Phase3-Neutral Voltage",
            "Phase1-Phase2 Voltage",
            "Phase2-Phase3 Voltage",
            "Phase1-Phase3 Voltage",
            "Neutral-Earth Voltage",
            "Phase1 Current",
            "Phase2 Current",
            "Phase3 Current",
            "Neutral Current",
            "Rectifier DC Voltage"
        };

        foreach (var requiredType in requiredReadingTypes)
        {
            var hasReading = visit.Readings.Any(r => r.ReadingType == requiredType);
            if (!hasReading)
            {
                result.AddError("Readings", $"Missing required reading: {requiredType}");
            }
        }

        // Validate reading ranges
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

        var beforePhotos = visit.Photos.Where(p => p.Type == PhotoType.Before).ToList();
        var afterPhotos = visit.Photos.Where(p => p.Type == PhotoType.After).ToList();

        // Minimum photo requirements
        if (beforePhotos.Count < 30)
        {
            result.AddError("Photos", $"Insufficient before photos. Required: 30, Found: {beforePhotos.Count}");
        }

        if (afterPhotos.Count < 30)
        {
            result.AddError("Photos", $"Insufficient after photos. Required: 30, Found: {afterPhotos.Count}");
        }

        // Validate photo dimensions
        var invalidPhotos = visit.Photos.Where(p => !p.MeetsRequirements()).ToList();
        if (invalidPhotos.Any())
        {
            result.AddWarning("Photos", $"{invalidPhotos.Count} photos do not meet dimension requirements");
        }

        // Validate mandatory categories
        var mandatoryCategories = new[]
        {
            PhotoCategory.ShelterInside,
            PhotoCategory.ShelterOutside,
            PhotoCategory.Tower,
            PhotoCategory.GEDP,
            PhotoCategory.Rectifier,
            PhotoCategory.Batteries,
            PhotoCategory.AirConditioner,
            PhotoCategory.EarthBar
        };

        foreach (var category in mandatoryCategories)
        {
            var hasBefore = beforePhotos.Any(p => p.Category == category);
            var hasAfter = afterPhotos.Any(p => p.Category == category);

            if (!hasBefore)
                result.AddError("Photos", $"Missing before photo for category: {category}");
            if (!hasAfter)
                result.AddError("Photos", $"Missing after photo for category: {category}");
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
