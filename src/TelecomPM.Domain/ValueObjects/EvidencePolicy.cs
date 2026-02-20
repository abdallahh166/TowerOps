using System.Collections.Generic;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.ValueObjects;

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
            throw new DomainException("Minimum photos required must be greater than or equal to zero");

        if (minChecklistCompletionPercent is < 0 or > 100)
            throw new DomainException("Minimum checklist completion percent must be between 0 and 100");

        return new EvidencePolicy(
            visitType,
            minPhotosRequired,
            readingsRequired,
            checklistRequired,
            minChecklistCompletionPercent);
    }

    public static EvidencePolicy DefaultFor(VisitType visitType)
    {
        return visitType switch
        {
            VisitType.PreventiveMaintenance => Create(visitType, 3, true, true, 80),
            VisitType.CorrectiveMaintenance => Create(visitType, 2, true, true, 100),
            _ => Create(visitType, 3, true, true, 80)
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
