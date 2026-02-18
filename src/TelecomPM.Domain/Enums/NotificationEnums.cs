namespace TelecomPM.Domain.Enums;

// ==================== Notification Related ====================
public enum NotificationType
{
    VisitScheduled = 1,
    VisitStarted = 2,
    VisitCompleted = 3,
    VisitApproved = 4,
    VisitRejected = 5,
    VisitNeedsCorrection = 6,
    MaterialLowStock = 7,
    MaterialRequested = 8,
    IssueReported = 9,
    VisitOverdue = 10
}

public enum NotificationPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}
