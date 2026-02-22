using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Services;

public interface IEvidencePolicyService
{
    EvidencePolicy GetEffectivePolicy(VisitType visitType, EvidencePolicy fallback);
    ValidationResult Validate(Visit visit, EvidencePolicy policy);
}
