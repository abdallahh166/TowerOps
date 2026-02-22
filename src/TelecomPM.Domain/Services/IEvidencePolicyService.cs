using TelecomPM.Domain.Entities.Visits;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.ValueObjects;

namespace TelecomPM.Domain.Services;

public interface IEvidencePolicyService
{
    Task<EvidencePolicy> GetEffectivePolicyAsync(
        VisitType visitType,
        EvidencePolicy fallback,
        CancellationToken cancellationToken = default);

    Task<ValidationResult> ValidateAsync(
        Visit visit,
        EvidencePolicy policy,
        CancellationToken cancellationToken = default);
}
