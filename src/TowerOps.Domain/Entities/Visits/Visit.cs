using TowerOps.Domain.Common;
using TowerOps.Domain.Enums;
using TowerOps.Domain.ValueObjects;
using TowerOps.Domain.Exceptions;
using TowerOps.Domain.Events.VisitEvents;

namespace TowerOps.Domain.Entities.Visits;

// ==================== Visit (Aggregate Root) ====================
public sealed class Visit : AggregateRoot<Guid>
{
    public string VisitNumber { get; private set; } = string.Empty; // V2025001
    public Guid SiteId { get; private set; }
    public string SiteCode { get; private set; } = string.Empty;
    public string SiteName { get; private set; } = string.Empty;
    
    // Team
    public Guid EngineerId { get; private set; }
    public string EngineerName { get; private set; } = string.Empty;
    public string? ContactPersonName { get; private set; }
    public Guid? SupervisorId { get; private set; }
    public string? SupervisorName { get; private set; }
    public List<string> TechnicianNames { get; private set; } = new();
    
    // Schedule
    public DateTime ScheduledDate { get; private set; }
    public int? PlannedOrder { get; private set; }
    public DateTime? ActualStartTime { get; private set; }
    public DateTime? ActualEndTime { get; private set; }
    public DateTime? EngineerReportedCompletionTimeUtc { get; private set; }
    public TimeRange? ActualDuration { get; private set; }
    
    // Status
    public VisitStatus Status { get; private set; }
    public VisitType Type { get; private set; }
    public Guid? ChecklistTemplateId { get; private set; }
    public string? ChecklistTemplateVersion { get; private set; }
    
    // Location Verification
    public Coordinates? CheckInLocation { get; private set; }
    public DateTime? CheckInTime { get; private set; }
    public GeoLocation? CheckInGeoLocation { get; private set; }
    public DateTime? CheckInTimeUtc { get; private set; }
    public decimal? DistanceFromSiteMeters { get; private set; }
    public bool IsWithinSiteRadius { get; private set; }
    public GeoLocation? CheckOutLocation { get; private set; }
    public DateTime? CheckOutTimeUtc { get; private set; }
    public Signature? SiteContactSignature { get; private set; }
    public bool IsSiteContactSigned => SiteContactSignature is not null;
    
    // Collections
    public List<VisitPhoto> Photos { get; private set; } = new();
    public List<VisitReading> Readings { get; private set; } = new();
    public List<VisitChecklist> Checklists { get; private set; } = new();
    public List<VisitMaterialUsage> MaterialsUsed { get; private set; } = new();
    public List<VisitIssue> IssuesFound { get; private set; } = new();
    public List<VisitApproval> ApprovalHistory { get; private set; } = new();
    
    // Completion
    public bool IsReadingsComplete { get; private set; }
    public bool IsPhotosComplete { get; private set; }
    public bool IsChecklistComplete { get; private set; }
    public int CompletionPercentage { get; private set; }
    
    // Notes
    public string? EngineerNotes { get; private set; }
    public string? SupervisorNotes { get; private set; }
    public string? ReviewerNotes { get; private set; }

    private Visit() : base() { } // EF Core

    private Visit(
        string visitNumber,
        Guid siteId,
        string siteCode,
        string siteName,
        Guid engineerId,
        string engineerName,
        DateTime scheduledDate,
        VisitType type) : base(Guid.NewGuid())
    {
        VisitNumber = visitNumber;
        SiteId = siteId;
        SiteCode = siteCode;
        SiteName = siteName;
        EngineerId = engineerId;
        EngineerName = engineerName;
        ScheduledDate = scheduledDate;
        Type = type.ToCanonical();
        Status = VisitStatus.Scheduled;
    }

    public static Visit Create(
        string visitNumber,
        Guid siteId,
        string siteCode,
        string siteName,
        Guid engineerId,
        string engineerName,
        DateTime scheduledDate,
        VisitType type = VisitType.BM)
    {
        var visit = new Visit(
            visitNumber,
            siteId,
            siteCode,
            siteName,
            engineerId,
            engineerName,
            scheduledDate,
            type);

        visit.AddDomainEvent(new VisitCreatedEvent(visit.Id, siteId, engineerId, scheduledDate));

        return visit;
    }

    // ==================== Visit Lifecycle ====================
    
    public void AssignSupervisor(Guid supervisorId, string supervisorName)
    {
        SupervisorId = supervisorId;
        SupervisorName = supervisorName;
    }

    public void AddTechnician(string technicianName)
    {
        if (!TechnicianNames.Contains(technicianName))
        {
            TechnicianNames.Add(technicianName);
        }
    }

    public void SetContactPersonName(string? contactPersonName)
    {
        ContactPersonName = string.IsNullOrWhiteSpace(contactPersonName)
            ? null
            : contactPersonName.Trim();
    }

    public void SetPlannedOrder(int? plannedOrder)
    {
        if (plannedOrder.HasValue && plannedOrder <= 0)
            throw new DomainException("Planned order must be greater than zero.", "Visit.PlannedOrder.Positive");

        PlannedOrder = plannedOrder;
    }

    public void StartVisit(Coordinates location)
    {
        if (Status != VisitStatus.Scheduled)
            throw new DomainException("Visit must be in Scheduled status to start", "Visit.Start.RequiresScheduled");

        CheckInLocation = location;
        CheckInTime = DateTime.UtcNow;
        CheckInTimeUtc = CheckInTime;
        CheckInGeoLocation = GeoLocation.Create((decimal)location.Latitude, (decimal)location.Longitude);
        ActualStartTime = DateTime.UtcNow;
        Status = VisitStatus.InProgress;

        AddDomainEvent(new VisitStartedEvent(Id, SiteId, EngineerId));
    }

    public void RecordCheckIn(GeoLocation location, decimal distanceFromSiteMeters, bool isWithinSiteRadius)
    {
        if (Status == VisitStatus.Approved || Status == VisitStatus.Rejected || Status == VisitStatus.Cancelled)
            throw new DomainException("Cannot check in for this visit status", "Visit.CheckIn.InvalidStatus");

        CheckInGeoLocation = location;
        CheckInTimeUtc = DateTime.UtcNow;
        DistanceFromSiteMeters = distanceFromSiteMeters;
        IsWithinSiteRadius = isWithinSiteRadius;

        // Backward compatibility for existing check-in coordinates/time fields.
        CheckInLocation ??= Coordinates.Create((double)location.Latitude, (double)location.Longitude);
        CheckInTime ??= CheckInTimeUtc;

        AddDomainEvent(new VisitCheckedInEvent(Id, SiteId, EngineerId, distanceFromSiteMeters, isWithinSiteRadius));

        if (!isWithinSiteRadius)
        {
            AddDomainEvent(new SuspiciousCheckInEvent(Id, SiteId, EngineerId, distanceFromSiteMeters));
        }
    }

    public void RecordCheckOut(GeoLocation location)
    {
        if (!CheckInTimeUtc.HasValue && !CheckInTime.HasValue)
            throw new DomainException("Visit must be checked in before checkout", "Visit.CheckOut.RequiresCheckIn");

        CheckOutLocation = location;
        CheckOutTimeUtc = DateTime.UtcNow;

        AddDomainEvent(new VisitCheckedOutEvent(Id, SiteId, EngineerId));
    }

    public void CaptureSiteContactSignature(Signature signature)
    {
        if (SiteContactSignature is not null)
            throw new DomainException("Site contact signature already captured.", "Visit.Signature.SiteContactAlreadyCaptured");

        SiteContactSignature = signature;
    }

    public void CompleteVisit()
    {
        if (Status != VisitStatus.InProgress)
            throw new DomainException("Visit must be in progress to complete", "Visit.Complete.RequiresInProgress");

        if (!ActualStartTime.HasValue)
            throw new DomainException("Visit start time is not recorded", "Visit.Complete.MissingStartTime");

        ActualEndTime = DateTime.UtcNow;
        ActualDuration = TimeRange.Create(ActualStartTime.Value, ActualEndTime.Value);

        if (!ActualDuration.IsValid())
            throw new DomainException("Visit duration is invalid", "Visit.Complete.InvalidDuration");

        ValidateCompletion();

        Status = VisitStatus.Completed;

        AddDomainEvent(new VisitCompletedEvent(Id, SiteId, EngineerId, ActualDuration));
    }

    public void SetEngineerReportedCompletionTime(DateTime engineerReportedCompletionTimeUtc)
    {
        EngineerReportedCompletionTimeUtc = engineerReportedCompletionTimeUtc.Kind switch
        {
            DateTimeKind.Utc => engineerReportedCompletionTimeUtc,
            DateTimeKind.Local => engineerReportedCompletionTimeUtc.ToUniversalTime(),
            _ => DateTime.SpecifyKind(engineerReportedCompletionTimeUtc, DateTimeKind.Utc)
        };
    }

    public void Submit()
    {
        if (Status != VisitStatus.Completed && Status != VisitStatus.NeedsCorrection)
            throw new DomainException("Visit must be completed or in correction state before submission", "Visit.Submit.RequiresCompletedOrNeedsCorrection");

        if (!IsReadingsComplete || !IsPhotosComplete || !IsChecklistComplete)
            throw new DomainException("All required items must be completed before submission", "Visit.Submit.RequiresEvidenceComplete");

        Status = VisitStatus.Submitted;

        AddDomainEvent(new VisitSubmittedEvent(Id, SiteId, EngineerId));
    }

    public void StartReview()
    {
        if (Status != VisitStatus.Submitted)
            throw new DomainException("Visit must be submitted for review", "Visit.Review.StartRequiresSubmitted");

        Status = VisitStatus.UnderReview;
    }

    public void Approve(Guid reviewerId, string reviewerName, string? notes = null)
    {
        if (Status != VisitStatus.UnderReview)
            throw new DomainException("Visit must be under review to approve", "Visit.Approve.RequiresUnderReview");

        var approval = VisitApproval.Create(
            Id,
            reviewerId,
            reviewerName,
            ApprovalAction.Approved,
            notes);

        ApprovalHistory.Add(approval);
        ReviewerNotes = notes;
        Status = VisitStatus.Approved;

        AddDomainEvent(new VisitApprovedEvent(Id, SiteId, EngineerId, reviewerId));
    }

    public void RequestCorrection(Guid reviewerId, string reviewerName, string correctionNotes)
    {
        if (Status != VisitStatus.UnderReview)
            throw new DomainException("Visit must be under review to request corrections", "Visit.RequestCorrection.RequiresUnderReview");

        if (string.IsNullOrWhiteSpace(correctionNotes))
            throw new DomainException("Correction notes are required", "Visit.RequestCorrection.NotesRequired");

        var approval = VisitApproval.Create(
            Id,
            reviewerId,
            reviewerName,
            ApprovalAction.RequestCorrection,
            correctionNotes);

        ApprovalHistory.Add(approval);
        ReviewerNotes = correctionNotes;
        Status = VisitStatus.NeedsCorrection;

        AddDomainEvent(new VisitCorrectionRequestedEvent(Id, SiteId, EngineerId, reviewerId, correctionNotes));
    }

    public void Reject(Guid reviewerId, string reviewerName, string rejectionReason)
    {
        if (Status != VisitStatus.UnderReview)
            throw new DomainException("Visit must be under review to reject", "Visit.Reject.RequiresUnderReview");

        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new DomainException("Rejection reason is required", "Visit.Reject.ReasonRequired");

        var approval = VisitApproval.Create(
            Id,
            reviewerId,
            reviewerName,
            ApprovalAction.Rejected,
            rejectionReason);

        ApprovalHistory.Add(approval);
        ReviewerNotes = rejectionReason;
        Status = VisitStatus.Rejected;

        AddDomainEvent(new VisitRejectedEvent(Id, SiteId, EngineerId, reviewerId, rejectionReason));
    }

    public void Cancel(string reason)
    {
        if (Status == VisitStatus.Approved || Status == VisitStatus.Rejected)
            throw new DomainException("Cannot cancel an approved or rejected visit", "Visit.Cancel.NotAllowedFinalState");

        Status = VisitStatus.Cancelled;
        EngineerNotes = reason;
    }

    public void Reschedule(DateTime newScheduledDate, string? reason = null)
    {
        if (Status != VisitStatus.Scheduled)
            throw new DomainException("Only scheduled visits can be rescheduled", "Visit.Reschedule.RequiresScheduled");

        if (newScheduledDate < DateTime.Today)
            throw new DomainException("New scheduled date must be today or in the future", "Visit.Reschedule.FutureDateRequired");

        var oldDate = ScheduledDate;
        ScheduledDate = newScheduledDate;
        
        if (!string.IsNullOrWhiteSpace(reason))
        {
            EngineerNotes = $"Rescheduled from {oldDate:yyyy-MM-dd} to {newScheduledDate:yyyy-MM-dd}. Reason: {reason}";
        }
        else
        {
            EngineerNotes = $"Rescheduled from {oldDate:yyyy-MM-dd} to {newScheduledDate:yyyy-MM-dd}";
        }

        AddDomainEvent(new VisitScheduledEvent(Id, SiteId, EngineerId, newScheduledDate));
    }

    // ==================== Photos Management ====================
    
    public void AddPhoto(VisitPhoto photo)
    {
        if (Status == VisitStatus.Approved || Status == VisitStatus.Rejected)
            throw new DomainException("Cannot add photos to an approved or rejected visit", "Visit.Photos.AddNotAllowedFinalState");

        Photos.Add(photo);
        CalculateCompletionPercentage();
    }

    public void RemovePhoto(Guid photoId)
    {
        var photo = Photos.FirstOrDefault(p => p.Id == photoId);
        if (photo != null)
        {
            Photos.Remove(photo);
            CalculateCompletionPercentage();
        }
    }

    public List<VisitPhoto> GetPhotosByType(PhotoType type)
    {
        return Photos.Where(p => p.Type == type).ToList();
    }

    public List<VisitPhoto> GetPhotosByCategory(PhotoCategory category)
    {
        return Photos.Where(p => p.Category == category).ToList();
    }

    // ==================== Readings Management ====================
    
    public void AddReading(VisitReading reading)
    {
        if (Status == VisitStatus.Approved || Status == VisitStatus.Rejected)
            throw new DomainException("Cannot add readings to an approved or rejected visit", "Visit.Readings.AddNotAllowedFinalState");

        Readings.Add(reading);
        CalculateCompletionPercentage();
    }

    public void UpdateReading(Guid readingId, decimal value)
    {
        var reading = Readings.FirstOrDefault(r => r.Id == readingId);
        if (reading != null)
        {
            reading.UpdateValue(value);
        }
    }

    // ==================== Checklist Management ====================
    
    public void AddChecklistItem(VisitChecklist item)
    {
        Checklists.Add(item);
        CalculateCompletionPercentage();
    }

    public void ApplyChecklistTemplate(Guid checklistTemplateId, string checklistTemplateVersion)
    {
        ChecklistTemplateId = checklistTemplateId;
        ChecklistTemplateVersion = checklistTemplateVersion;
    }

    public void UpdateChecklistItem(Guid itemId, CheckStatus status, string? notes = null)
    {
        var item = Checklists.FirstOrDefault(c => c.Id == itemId);
        if (item != null)
        {
            item.UpdateStatus(status, notes);
            CalculateCompletionPercentage();
        }
    }

    // ==================== Material Usage Management ====================
    
    public void LogMaterialUsage(VisitMaterialUsage materialUsage)
    {
        if (Status == VisitStatus.Approved || Status == VisitStatus.Rejected)
            throw new DomainException("Cannot add materials to an approved or rejected visit", "Visit.Materials.AddNotAllowedFinalState");

        MaterialsUsed.Add(materialUsage);
    }

    public Money GetTotalMaterialCost()
    {
        var total = 0m;
        foreach (var material in MaterialsUsed)
        {
            total += material.TotalCost.Amount;
        }
        return Money.Create(total, "EGP");
    }

    // ==================== Issues Management ====================
    
    public void ReportIssue(VisitIssue issue)
    {
        IssuesFound.Add(issue);
        
        if (issue.Severity == IssueSeverity.Critical)
        {
            AddDomainEvent(new CriticalIssueReportedEvent(Id, SiteId, issue.Description));
        }
    }

    public void ResolveIssue(Guid issueId, string resolution)
    {
        var issue = IssuesFound.FirstOrDefault(i => i.Id == issueId);
        if (issue != null)
        {
            issue.Resolve(resolution);
        }
    }

    // ==================== Notes Management ====================
    
    public void AddEngineerNotes(string notes)
    {
        EngineerNotes = notes;
    }

    public void AddSupervisorNotes(string notes)
    {
        if (!SupervisorId.HasValue)
            throw new DomainException("No supervisor assigned to this visit", "Visit.Supervisor.RequiredForNotes");

        SupervisorNotes = notes;
    }

    // ==================== Validation ====================
    
    private void ValidateCompletion()
    {
        CalculateCompletionPercentage();
    }

    public void ApplyEvidencePolicy(EvidencePolicy policy)
    {
        if (policy is null)
            throw new DomainException("Evidence policy is required.", "Visit.EvidencePolicy.Required");

        IsPhotosComplete = Photos.Count >= policy.MinPhotosRequired;
        IsReadingsComplete = !policy.ReadingsRequired || Readings.Any();

        if (!policy.ChecklistRequired)
        {
            IsChecklistComplete = true;
        }
        else
        {
            var totalChecklistItems = Checklists.Count;
            var completedItems = Checklists.Count(c => c.Status != CheckStatus.NA);
            var completionPercent = totalChecklistItems == 0
                ? 0
                : completedItems * 100 / totalChecklistItems;

            IsChecklistComplete = completionPercent >= policy.MinChecklistCompletionPercent;
        }

        CalculateCompletionPercentageForPolicy(policy);
    }

    private void CalculateCompletionPercentage()
    {
        var beforePhotos = Photos.Count(p => p.Type == PhotoType.Before);
        var afterPhotos = Photos.Count(p => p.Type == PhotoType.After);
        var totalChecklistItems = Checklists.Count;
        var completedChecklistItems = Checklists.Count(c => c.Status != CheckStatus.NA);

        // Baseline completion snapshot without policy thresholds.
        IsPhotosComplete = beforePhotos > 0 && afterPhotos > 0;
        IsReadingsComplete = Readings.Count > 0;
        IsChecklistComplete = totalChecklistItems > 0 && completedChecklistItems == totalChecklistItems;

        var achievedWeight = 0;

        // Photos: 40%
        var photosWeight = 40;
        var completedPhotoParts = (beforePhotos > 0 ? 1 : 0) + (afterPhotos > 0 ? 1 : 0);
        var photosScore = completedPhotoParts * 50;
        achievedWeight += (int)(photosWeight * photosScore / 100);

        // Readings: 30%
        var readingsWeight = 30;
        var readingsScore = Readings.Count > 0 ? 100 : 0;
        achievedWeight += (int)(readingsWeight * readingsScore / 100);

        // Checklist: 30%
        var checklistWeight = 30;
        if (totalChecklistItems > 0)
        {
            var checklistScore = completedChecklistItems * 100 / totalChecklistItems;
            achievedWeight += (int)(checklistWeight * checklistScore / 100);
        }

        CompletionPercentage = achievedWeight;
    }

    private void CalculateCompletionPercentageForPolicy(EvidencePolicy policy)
    {
        var achievedWeight = 0;

        var photosWeight = 40;
        var photosScore = policy.MinPhotosRequired <= 0
            ? 100
            : Math.Min(100, Photos.Count * 100 / policy.MinPhotosRequired);
        achievedWeight += (int)(photosWeight * photosScore / 100);

        var readingsWeight = 30;
        var readingsScore = !policy.ReadingsRequired
            ? 100
            : (Readings.Any() ? 100 : 0);
        achievedWeight += (int)(readingsWeight * readingsScore / 100);

        var checklistWeight = 30;
        if (!policy.ChecklistRequired)
        {
            achievedWeight += checklistWeight;
        }
        else
        {
            var checklistScore = 0;
            if (Checklists.Count > 0)
            {
                var completedChecklist = Checklists.Count(c => c.Status != CheckStatus.NA);
                checklistScore = completedChecklist * 100 / Checklists.Count;
            }

            achievedWeight += (int)(checklistWeight * checklistScore / 100);
        }

        CompletionPercentage = achievedWeight;
    }

    public bool CanBeSubmitted()
    {
        return (Status == VisitStatus.Completed || Status == VisitStatus.NeedsCorrection) && 
               IsReadingsComplete && 
               IsPhotosComplete && 
               IsChecklistComplete;
    }

    public bool CanBeEdited()
    {
        return Status == VisitStatus.Scheduled || 
               Status == VisitStatus.InProgress || 
               Status == VisitStatus.Completed ||
               Status == VisitStatus.NeedsCorrection;
    }
}
