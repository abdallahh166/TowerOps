using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Services;

public interface IEvidencePolicyService
{
    ValidationResult Validate(Visit visit, EvidencePolicy policy);
}
