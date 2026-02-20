namespace TelecomPM.Domain.Enums;

public enum SlaClass
{
    P1 = 1,
    P2 = 2,
    P3 = 3,
    P4 = 4
}

public enum WorkOrderStatus
{
    Created = 1,
    Assigned = 2,
    InProgress = 3,
    PendingInternalReview = 4,
    PendingCustomerAcceptance = 5,
    Closed = 6,
    Rejected = 7,
    Rework = 8,
    Cancelled = 9
}

public enum SlaStatus
{
    OnTime = 1,
    AtRisk = 2,
    Breached = 3
}
