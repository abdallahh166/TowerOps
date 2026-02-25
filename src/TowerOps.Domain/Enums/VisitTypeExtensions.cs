namespace TowerOps.Domain.Enums;

public static class VisitTypeExtensions
{
    public static VisitType ToCanonical(this VisitType visitType)
    {
        return visitType switch
        {
            VisitType.PreventiveMaintenance => VisitType.BM,
            VisitType.CorrectiveMaintenance => VisitType.CM,
            _ => visitType
        };
    }

    public static bool IsBm(this VisitType visitType)
    {
        var canonical = visitType.ToCanonical();
        return canonical is VisitType.BM or VisitType.Inspection;
    }

    public static bool IsCm(this VisitType visitType)
    {
        var canonical = visitType.ToCanonical();
        return canonical is VisitType.CM or VisitType.Emergency;
    }
}
