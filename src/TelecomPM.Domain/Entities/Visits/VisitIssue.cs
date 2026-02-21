using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.Visits;

// ==================== Visit Issue ====================
public sealed class VisitIssue : Entity<Guid>
{
    public Guid VisitId { get; private set; }
    public IssueCategory Category { get; private set; }
    public IssueSeverity Severity { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public IssueStatus Status { get; private set; }
    public DateTime ReportedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public string? Resolution { get; private set; }
    public DateTime? TargetDateUtc { get; private set; }
    public List<Guid> PhotoIds { get; private set; } = new();
    public bool RequiresFollowUp { get; private set; }

    private VisitIssue() : base() { }

    private VisitIssue(
        Guid visitId,
        IssueCategory category,
        IssueSeverity severity,
        string title,
        string description) : base(Guid.NewGuid())
    {
        VisitId = visitId;
        Category = category;
        Severity = severity;
        Title = title;
        Description = description;
        Status = IssueStatus.Open;
        ReportedAt = DateTime.UtcNow;
        RequiresFollowUp = severity >= IssueSeverity.High;
    }

    public static VisitIssue Create(
        Guid visitId,
        IssueCategory category,
        IssueSeverity severity,
        string title,
        string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Issue title is required");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Issue description is required");

        return new VisitIssue(visitId, category, severity, title, description);
    }

    public void AttachPhoto(Guid photoId)
    {
        if (!PhotoIds.Contains(photoId))
        {
            PhotoIds.Add(photoId);
        }
    }

    public void Resolve(string resolution)
    {
        if (string.IsNullOrWhiteSpace(resolution))
            throw new DomainException("Resolution description is required");

        Status = IssueStatus.Resolved;
        Resolution = resolution;
        ResolvedAt = DateTime.UtcNow;
    }

    public void Escalate()
    {
        Status = IssueStatus.Escalated;
    }

    public void SetTargetDate(DateTime? targetDateUtc)
    {
        if (!targetDateUtc.HasValue)
        {
            TargetDateUtc = null;
            return;
        }

        TargetDateUtc = targetDateUtc.Value.Kind switch
        {
            DateTimeKind.Utc => targetDateUtc.Value,
            DateTimeKind.Local => targetDateUtc.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(targetDateUtc.Value, DateTimeKind.Utc)
        };
    }

    public void Close()
    {
        if (Status != IssueStatus.Resolved)
            throw new DomainException("Issue must be resolved before closing");

        Status = IssueStatus.Closed;
    }
}
