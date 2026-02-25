using TowerOps.Domain.Entities.Visits;
using TowerOps.Domain.Enums;
using TowerOps.Domain.ValueObjects;

namespace TowerOps.Domain.Services;

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
