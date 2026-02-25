using System.Collections.Generic;
using TowerOps.Domain.Enums;
using TowerOps.Domain.Exceptions;

namespace TowerOps.Domain.ValueObjects;

public sealed class EvidencePolicy : ValueObject
{
    public VisitType VisitType { get; }
    public int MinPhotosRequired { get; }
    public bool ReadingsRequired { get; }
    public bool ChecklistRequired { get; }
    public int MinChecklistCompletionPercent { get; }

    private EvidencePolicy(
        VisitType visitType,
        int minPhotosRequired,
        bool readingsRequired,
        bool checklistRequired,
        int minChecklistCompletionPercent)
    {
        VisitType = visitType;
        MinPhotosRequired = minPhotosRequired;
        ReadingsRequired = readingsRequired;
        ChecklistRequired = checklistRequired;
        MinChecklistCompletionPercent = minChecklistCompletionPercent;
    }

    public static EvidencePolicy Create(
        VisitType visitType,
        int minPhotosRequired,
        bool readingsRequired,
        bool checklistRequired,
        int minChecklistCompletionPercent)
    {
        if (minPhotosRequired < 0)
            throw new DomainException("Minimum photos required must be greater than or equal to zero", "EvidencePolicy.MinPhotos.NonNegative");

        if (minChecklistCompletionPercent is < 0 or > 100)
            throw new DomainException("Minimum checklist completion percent must be between 0 and 100", "EvidencePolicy.MinChecklistCompletionPercent.Range");

        return new EvidencePolicy(
            visitType,
            minPhotosRequired,
            readingsRequired,
            checklistRequired,
            minChecklistCompletionPercent);
    }

    public static EvidencePolicy DefaultFor(VisitType visitType)
    {
        var canonicalVisitType = visitType.ToCanonical();

        return canonicalVisitType switch
        {
            VisitType.BM => Create(canonicalVisitType, 3, true, true, 80),
            VisitType.CM => Create(canonicalVisitType, 2, true, true, 100),
            _ => Create(canonicalVisitType, 3, true, true, 80)
        };
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return VisitType;
        yield return MinPhotosRequired;
        yield return ReadingsRequired;
        yield return ChecklistRequired;
        yield return MinChecklistCompletionPercent;
    }
}
