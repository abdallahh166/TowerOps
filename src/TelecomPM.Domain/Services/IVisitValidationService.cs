using TelecomPM.Domain.Entities.Sites;
using TelecomPM.Domain.Entities.Visits;

namespace TelecomPM.Domain.Services;

public interface IVisitValidationService
{
    ValidationResult ValidateVisitCompletion(Visit visit, Site site);
    ValidationResult ValidateReadings(Visit visit);
    ValidationResult ValidatePhotos(Visit visit, Site site);
    ValidationResult ValidateMaterialUsage(Visit visit);
}
