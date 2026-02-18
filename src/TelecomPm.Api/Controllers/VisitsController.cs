namespace TelecomPm.Api.Controllers;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPM.Api.Authorization;
using TelecomPm.Api.Contracts.Visits;
using TelecomPM.Application.Commands.Visits.AddChecklistItem;
using TelecomPM.Application.Commands.Visits.AddIssue;
using TelecomPM.Application.Commands.Visits.AddPhoto;
using TelecomPM.Application.Commands.Visits.AddReading;
using TelecomPM.Application.Commands.Visits.ApproveVisit;
using TelecomPM.Application.Commands.Visits.CompleteVisit;
using TelecomPM.Application.Commands.Visits.CreateVisit;
using TelecomPM.Application.Commands.Visits.RejectVisit;
using TelecomPM.Application.Commands.Visits.RequestCorrection;
using TelecomPM.Application.Commands.Visits.ResolveIssue;
using TelecomPM.Application.Commands.Visits.StartVisit;
using TelecomPM.Application.Commands.Visits.SubmitVisit;
using TelecomPM.Application.Commands.Visits.UpdateChecklistItem;
using TelecomPM.Application.Queries.Visits.GetEngineerVisits;
using TelecomPM.Application.Queries.Visits.GetVisitEvidenceStatus;
using TelecomPM.Application.Queries.Visits.GetPendingReviews;
using TelecomPM.Application.Queries.Visits.GetScheduledVisits;
using TelecomPM.Application.Queries.Visits.GetVisitById;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class VisitsController : ApiControllerBase
{
    [HttpGet("{visitId:guid}")]
    public async Task<IActionResult> GetById(Guid visitId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetVisitByIdQuery { VisitId = visitId },
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("engineers/{engineerId:guid}")]
    public async Task<IActionResult> GetEngineerVisits(
        Guid engineerId,
        [FromQuery] EngineerVisitQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var query = new GetEngineerVisitsQuery
        {
            EngineerId = engineerId,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            Status = parameters.Status,
            From = parameters.From,
            To = parameters.To
        };

        var result = await Mediator.Send(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("pending-reviews")]
    public async Task<IActionResult> GetPendingReviews(
        [FromQuery] Guid? officeId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetPendingReviewsQuery { OfficeId = officeId },
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("scheduled")]
    public async Task<IActionResult> GetScheduledVisits(
        [FromQuery] ScheduledVisitsQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetScheduledVisitsQuery
            {
                Date = parameters.Date,
                EngineerId = parameters.EngineerId
            },
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateVisitRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateVisitCommand
        {
            SiteId = request.SiteId,
            EngineerId = request.EngineerId,
            ScheduledDate = request.ScheduledDate,
            Type = request.Type,
            SupervisorId = request.SupervisorId,
            TechnicianNames = request.TechnicianNames ?? new List<string>()
        };

        var result = await Mediator.Send(command, cancellationToken);

        if (result.IsSuccess && result.Value is not null)
        {
            return CreatedAtAction(
                nameof(GetById),
                new { visitId = result.Value.Id },
                result.Value);
        }

        return HandleResult(result);
    }

    [HttpGet("{visitId:guid}/evidence-status")]
    public async Task<IActionResult> GetEvidenceStatus(Guid visitId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new GetVisitEvidenceStatusQuery { VisitId = visitId },
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/start")]
    public async Task<IActionResult> Start(
        Guid visitId,
        [FromBody] StartVisitRequest request,
        CancellationToken cancellationToken)
    {
        var command = new StartVisitCommand
        {
            VisitId = visitId,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/complete")]
    public async Task<IActionResult> Complete(
        Guid visitId,
        [FromBody] CompleteVisitRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CompleteVisitCommand
        {
            VisitId = visitId,
            EngineerNotes = request.EngineerNotes
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/submit")]
    public async Task<IActionResult> Submit(Guid visitId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            new SubmitVisitCommand { VisitId = visitId },
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/approve")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanReviewVisits)]
    public async Task<IActionResult> Approve(
        Guid visitId,
        [FromBody] ApproveVisitRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ApproveVisitCommand
        {
            VisitId = visitId,
            ReviewerId = request.ReviewerId,
            Notes = request.Notes
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/reject")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanReviewVisits)]
    public async Task<IActionResult> Reject(
        Guid visitId,
        [FromBody] RejectVisitRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RejectVisitCommand
        {
            VisitId = visitId,
            ReviewerId = request.ReviewerId,
            RejectionReason = request.RejectionReason
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/request-correction")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanReviewVisits)]
    public async Task<IActionResult> RequestCorrection(
        Guid visitId,
        [FromBody] RequestCorrectionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RequestCorrectionCommand
        {
            VisitId = visitId,
            ReviewerId = request.ReviewerId,
            CorrectionNotes = request.CorrectionNotes
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/checklist-items")]
    public async Task<IActionResult> AddChecklistItem(
        Guid visitId,
        [FromBody] AddChecklistItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddChecklistItemCommand
        {
            VisitId = visitId,
            Category = request.Category,
            ItemName = request.ItemName,
            Description = request.Description,
            IsMandatory = request.IsMandatory
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{visitId:guid}/checklist-items/{checklistItemId:guid}")]
    public async Task<IActionResult> UpdateChecklistItem(
        Guid visitId,
        Guid checklistItemId,
        [FromBody] UpdateChecklistItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateChecklistItemCommand
        {
            VisitId = visitId,
            ChecklistItemId = checklistItemId,
            Status = request.Status,
            Notes = request.Notes
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/issues")]
    public async Task<IActionResult> AddIssue(
        Guid visitId,
        [FromBody] AddVisitIssueRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddIssueCommand
        {
            VisitId = visitId,
            Category = request.Category,
            Severity = request.Severity,
            Title = request.Title,
            Description = request.Description,
            PhotoIds = request.PhotoIds
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/issues/{issueId:guid}/resolve")]
    public async Task<IActionResult> ResolveIssue(
        Guid visitId,
        Guid issueId,
        [FromBody] ResolveVisitIssueRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ResolveIssueCommand
        {
            VisitId = visitId,
            IssueId = issueId,
            Resolution = request.Resolution
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/readings")]
    public async Task<IActionResult> AddReading(
        Guid visitId,
        [FromBody] AddVisitReadingRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddReadingCommand
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

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/photos")]
    [RequestSizeLimit(25 * 1024 * 1024)]
    public async Task<IActionResult> AddPhoto(
        Guid visitId,
        [FromForm] AddVisitPhotoRequest request,
        CancellationToken cancellationToken)
    {
        if (request.File.Length <= 0)
        {
            return BadRequest("Photo file is required");
        }

        await using var stream = request.File.OpenReadStream();

        var command = new AddPhotoCommand
        {
            VisitId = visitId,
            Type = request.Type,
            Category = request.Category,
            ItemName = request.ItemName,
            FileStream = stream,
            FileName = request.File.FileName,
            Description = request.Description,
            Latitude = request.Latitude,
            Longitude = request.Longitude
        };

        var result = await Mediator.Send(command, cancellationToken);
        return HandleResult(result);
    }
}
