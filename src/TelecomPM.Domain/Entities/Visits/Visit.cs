using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.ValueObjects;
using TelecomPM.Domain.Exceptions;
using TelecomPM.Domain.Events.VisitEvents;

namespace TelecomPM.Domain.Entities.Visits;

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
    public Guid? SupervisorId { get; private set; }
    public string? SupervisorName { get; private set; }
    public List<string> TechnicianNames { get; private set; } = new();
    
    // Schedule
    public DateTime ScheduledDate { get; private set; }
    public DateTime? ActualStartTime { get; private set; }
    public DateTime? ActualEndTime { get; private set; }
    public TimeRange? ActualDuration { get; private set; }
    
    // Status
    public VisitStatus Status { get; private set; }
    public VisitType Type { get; private set; }
    public Guid? ChecklistTemplateId { get; private set; }
    public string? ChecklistTemplateVersion { get; private set; }
    
    // Location Verification
    public Coordinates? CheckInLocation { get; private set; }
    public DateTime? CheckInTime { get; private set; }
    
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
        Type = type;
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
        VisitType type = VisitType.PreventiveMaintenance)
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

    public void StartVisit(Coordinates location)
    {
        if (Status != VisitStatus.Scheduled)
            throw new DomainException("Visit must be in Scheduled status to start");

        CheckInLocation = location;
        CheckInTime = DateTime.UtcNow;
        ActualStartTime = DateTime.UtcNow;
        Status = VisitStatus.InProgress;

        AddDomainEvent(new VisitStartedEvent(Id, SiteId, EngineerId));
    }

    public void CompleteVisit()
    {
        if (Status != VisitStatus.InProgress)
            throw new DomainException("Visit must be in progress to complete");

        if (!ActualStartTime.HasValue)
            throw new DomainException("Visit start time is not recorded");

        ActualEndTime = DateTime.UtcNow;
        ActualDuration = TimeRange.Create(ActualStartTime.Value, ActualEndTime.Value);

        if (!ActualDuration.IsValid())
            throw new DomainException("Visit duration is invalid");

        ValidateCompletion();

        Status = VisitStatus.Completed;

        AddDomainEvent(new VisitCompletedEvent(Id, SiteId, EngineerId, ActualDuration));
    }

    public void Submit()
    {
        if (Status != VisitStatus.Completed && Status != VisitStatus.NeedsCorrection)
            throw new DomainException("Visit must be completed or in correction state before submission");

        if (!IsReadingsComplete || !IsPhotosComplete || !IsChecklistComplete)
            throw new DomainException("All required items must be completed before submission");

        Status = VisitStatus.Submitted;

        AddDomainEvent(new VisitSubmittedEvent(Id, SiteId, EngineerId));
    }

    public void StartReview()
    {
        if (Status != VisitStatus.Submitted)
            throw new DomainException("Visit must be submitted for review");

        Status = VisitStatus.UnderReview;
    }

    public void Approve(Guid reviewerId, string reviewerName, string? notes = null)
    {
        if (Status != VisitStatus.UnderReview)
            throw new DomainException("Visit must be under review to approve");

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
            throw new DomainException("Visit must be under review to request corrections");

        if (string.IsNullOrWhiteSpace(correctionNotes))
            throw new DomainException("Correction notes are required");

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
            throw new DomainException("Visit must be under review to reject");

        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new DomainException("Rejection reason is required");

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
            throw new DomainException("Cannot cancel an approved or rejected visit");

        Status = VisitStatus.Cancelled;
        EngineerNotes = reason;
    }

    public void Reschedule(DateTime newScheduledDate, string? reason = null)
    {
        if (Status != VisitStatus.Scheduled)
            throw new DomainException("Only scheduled visits can be rescheduled");

        if (newScheduledDate < DateTime.Today)
            throw new DomainException("New scheduled date must be today or in the future");

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
            throw new DomainException("Cannot add photos to an approved or rejected visit");

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
            throw new DomainException("Cannot add readings to an approved or rejected visit");

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
            throw new DomainException("Cannot add materials to an approved or rejected visit");

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
            throw new DomainException("No supervisor assigned to this visit");

        SupervisorNotes = notes;
    }

    // ==================== Validation ====================
    
    private void ValidateCompletion()
    {
        // Check if all mandatory readings are collected
        var requiredReadingsCount = 15; // Based on site complexity
        IsReadingsComplete = Readings.Count >= requiredReadingsCount;

        // Check if all mandatory photos are taken
        var beforePhotos = Photos.Count(p => p.Type == PhotoType.Before);
        var afterPhotos = Photos.Count(p => p.Type == PhotoType.After);
        IsPhotosComplete = beforePhotos >= 30 && afterPhotos >= 30; // Minimum required

        // Check if all checklist items are completed
        var totalChecklistItems = Checklists.Count;
        var completedItems = Checklists.Count(c => c.Status != CheckStatus.NA);
        IsChecklistComplete = totalChecklistItems > 0 && completedItems == totalChecklistItems;

        CalculateCompletionPercentage();
    }

    private void CalculateCompletionPercentage()
    {
        var achievedWeight = 0;

        // Photos: 40%
        var photosWeight = 40;
        var beforePhotos = Photos.Count(p => p.Type == PhotoType.Before);
        var afterPhotos = Photos.Count(p => p.Type == PhotoType.After);
        var requiredPhotos = 30;
        var photosScore = Math.Min(100, (beforePhotos + afterPhotos) * 100 / (requiredPhotos * 2));
        achievedWeight += (int)(photosWeight * photosScore / 100);

        // Readings: 30%
        var readingsWeight = 30;
        var requiredReadings = 15;
        var readingsScore = Math.Min(100, Readings.Count * 100 / requiredReadings);
        achievedWeight += (int)(readingsWeight * readingsScore / 100);

        // Checklist: 30%
        var checklistWeight = 30;
        if (Checklists.Count > 0)
        {
            var completedChecklist = Checklists.Count(c => c.Status != CheckStatus.NA);
            var checklistScore = completedChecklist * 100 / Checklists.Count;
            achievedWeight += (int)(checklistWeight * checklistScore / 100);
        }

        CompletionPercentage = achievedWeight;
    }

    public bool CanBeSubmitted()
    {
        return Status == VisitStatus.Completed && 
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
