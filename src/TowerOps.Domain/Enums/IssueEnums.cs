namespace TowerOps.Domain.Enums;

// ==================== Issue Related ====================
public enum IssueSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public enum IssueCategory
{
    Electrical = 1,
    Power = 2,
    Cooling = 3,
    Radio = 4,
    Transmission = 5,
    Generator = 6,
    Fire = 7,
    Structure = 8,
    Other = 99
}

public enum IssueStatus
{
    Open = 1,
    InProgress = 2,
    Resolved = 3,
    Closed = 4,
    Escalated = 5
}
