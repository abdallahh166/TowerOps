using TowerOps.Domain.Entities.Sites;
using TowerOps.Domain.Entities.Visits;

namespace TowerOps.Domain.Services;

public interface IVisitValidationService
{
    ValidationResult ValidateVisitCompletion(Visit visit, Site site);
    ValidationResult ValidateReadings(Visit visit);
    ValidationResult ValidatePhotos(Visit visit, Site site);
    ValidationResult ValidateMaterialUsage(Visit visit);
}
