namespace TelecomPm.Api.Mappings;

using System;
using System.Collections.Generic;
using System.IO;
using TelecomPm.Api.Contracts.Visits;
using TelecomPM.Application.Commands.Visits.AddChecklistItem;
using TelecomPM.Application.Commands.Visits.AddIssue;
using TelecomPM.Application.Commands.Visits.AddPhoto;
using TelecomPM.Application.Commands.Visits.AddReading;
using TelecomPM.Application.Commands.Visits.ApproveVisit;
using TelecomPM.Application.Commands.Visits.CancelVisit;
using TelecomPM.Application.Commands.Visits.CompleteVisit;
using TelecomPM.Application.Commands.Visits.CreateVisit;
using TelecomPM.Application.Commands.Imports.ImportAlarmCapture;
using TelecomPM.Application.Commands.Imports.ImportPanoramaEvidence;
using TelecomPM.Application.Commands.Visits.RejectVisit;
using TelecomPM.Application.Commands.Visits.RemovePhoto;
using TelecomPM.Application.Commands.Visits.RequestCorrection;
using TelecomPM.Application.Commands.Visits.RescheduleVisit;
using TelecomPM.Application.Commands.Visits.ResolveIssue;
using TelecomPM.Application.Commands.Visits.StartVisit;
using TelecomPM.Application.Commands.Visits.SubmitVisit;
using TelecomPM.Application.Commands.Visits.UpdateChecklistItem;
using TelecomPM.Application.Commands.Visits.UpdateReading;
using TelecomPM.Application.Queries.Visits.GetEngineerVisits;
using TelecomPM.Application.Queries.Visits.GetPendingReviews;
using TelecomPM.Application.Queries.Visits.GetScheduledVisits;
using TelecomPM.Application.Queries.Visits.GetVisitById;
using TelecomPM.Application.Queries.Visits.GetVisitEvidenceStatus;

public static class VisitsContractMapper
{
    public static GetVisitByIdQuery ToQuery(this Guid visitId)
        => new() { VisitId = visitId };

    public static GetEngineerVisitsQuery ToQuery(this EngineerVisitQueryParameters parameters, Guid engineerId)
        => new()
        {
            EngineerId = engineerId,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            Status = parameters.Status,
            From = parameters.From,
            To = parameters.To
        };

    public static GetPendingReviewsQuery ToQuery(this Guid? officeId)
        => new() { OfficeId = officeId };

    public static GetScheduledVisitsQuery ToQuery(this ScheduledVisitsQueryParameters parameters)
        => new()
        {
            Date = parameters.Date,
            EngineerId = parameters.EngineerId
        };

    public static CreateVisitCommand ToCommand(this CreateVisitRequest request)
        => new()
        {
            SiteId = request.SiteId,
            EngineerId = request.EngineerId,
            ScheduledDate = request.ScheduledDate,
            Type = request.Type,
            SupervisorId = request.SupervisorId,
            TechnicianNames = request.TechnicianNames ?? new List<string>()
        };

    public static GetVisitEvidenceStatusQuery ToEvidenceStatusQuery(this Guid visitId)
        => new() { VisitId = visitId };

    public static StartVisitCommand ToCommand(this StartVisitRequest request, Guid visitId)
        => new()
        {
            VisitId = visitId,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

    public static CompleteVisitCommand ToCommand(this CompleteVisitRequest request, Guid visitId)
        => new()
        {
            VisitId = visitId,
            EngineerNotes = request.EngineerNotes
        };

    public static SubmitVisitCommand ToSubmitCommand(this Guid visitId)
        => new() { VisitId = visitId };

    public static ApproveVisitCommand ToCommand(this ApproveVisitRequest request, Guid visitId)
        => new()
        {
            VisitId = visitId,
            ReviewerId = request.ReviewerId,
            Notes = request.Notes
        };

    public static RejectVisitCommand ToCommand(this RejectVisitRequest request, Guid visitId)
        => new()
        {
            VisitId = visitId,
            ReviewerId = request.ReviewerId,
            RejectionReason = request.RejectionReason
        };

    public static RequestCorrectionCommand ToCommand(this RequestCorrectionRequest request, Guid visitId)
        => new()
        {
            VisitId = visitId,
            ReviewerId = request.ReviewerId,
            CorrectionNotes = request.CorrectionNotes
        };

    public static AddChecklistItemCommand ToCommand(this AddChecklistItemRequest request, Guid visitId)
        => new()
        {
            VisitId = visitId,
            Category = request.Category,
            ItemName = request.ItemName,
            Description = request.Description,
            IsMandatory = request.IsMandatory
        };

    public static UpdateChecklistItemCommand ToCommand(this UpdateChecklistItemRequest request, Guid visitId, Guid checklistItemId)
        => new()
        {
            VisitId = visitId,
            ChecklistItemId = checklistItemId,
            Status = request.Status,
            Notes = request.Notes
        };

    public static AddIssueCommand ToCommand(this AddVisitIssueRequest request, Guid visitId)
        => new()
        {
            VisitId = visitId,
            Category = request.Category,
            Severity = request.Severity,
            Title = request.Title,
            Description = request.Description,
            PhotoIds = request.PhotoIds
        };

    public static ResolveIssueCommand ToCommand(this ResolveVisitIssueRequest request, Guid visitId, Guid issueId)
        => new()
        {
            VisitId = visitId,
            IssueId = issueId,
            Resolution = request.Resolution
        };

    public static AddReadingCommand ToCommand(this AddVisitReadingRequest request, Guid visitId)
        => new()
        {
            VisitId = visitId,
            ReadingType = request.ReadingType,
            Category = request.Category,
            Value = request.Value,
            Unit = request.Unit,
            MinAcceptable = request.MinAcceptable,
            MaxAcceptable = request.MaxAcceptable,
            Phase = request.Phase,
            Equipment = request.Equipment,
            Notes = request.Notes
        };

    public static AddPhotoCommand ToCommand(this AddVisitPhotoRequest request, Guid visitId, Stream fileStream)
        => new()
        {
            VisitId = visitId,
            Type = request.Type,
            Category = request.Category,
            ItemName = request.ItemName,
            FileStream = fileStream,
            FileName = request.File.FileName,
            Description = request.Description,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

    public static CancelVisitCommand ToCommand(this CancelVisitRequest request, Guid visitId)
        => new()
        {
            VisitId = visitId,
            Reason = request.Reason
        };

    public static RescheduleVisitCommand ToCommand(this RescheduleVisitRequest request, Guid visitId)
        => new()
        {
            VisitId = visitId,
            NewScheduledDate = request.NewScheduledDate,
            Reason = request.Reason
        };

    public static RemovePhotoCommand ToRemovePhotoCommand(this Guid visitId, Guid photoId)
        => new()
        {
            VisitId = visitId,
            PhotoId = photoId
        };

    public static UpdateReadingCommand ToCommand(this UpdateVisitReadingRequest request, Guid visitId, Guid readingId)
        => new()
        {
            VisitId = visitId,
            ReadingId = readingId,
            Value = request.Value
        };

    public static ImportPanoramaEvidenceCommand ToImportPanoramaEvidenceCommand(this Guid visitId, byte[] fileContent)
        => new()
        {
            VisitId = visitId,
            FileContent = fileContent
        };

    public static ImportAlarmCaptureCommand ToImportAlarmCaptureCommand(this Guid visitId, byte[] fileContent)
        => new()
        {
            VisitId = visitId,
            FileContent = fileContent
        };
}
