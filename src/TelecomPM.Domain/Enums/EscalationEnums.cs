namespace TelecomPM.Domain.Enums;

public enum EscalationLevel
{
    AreaManager = 1,
    BMManagement = 2,
    ProjectSponsor = 3
}

public enum EscalationStatus
{
    Submitted = 1,
    UnderReview = 2,
    Approved = 3,
    Rejected = 4,
    Closed = 5
}
