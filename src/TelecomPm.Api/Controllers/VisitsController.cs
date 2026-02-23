namespace TelecomPm.Api.Controllers;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TelecomPM.Api.Authorization;
using TelecomPM.Application.Common.Interfaces;
using TelecomPm.Api.Contracts.Visits;
using TelecomPm.Api.Mappings;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = ApiAuthorizationPolicies.CanViewVisits)]
public sealed class VisitsController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;

    public VisitsController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    [HttpGet("{visitId:guid}")]
    public async Task<IActionResult> GetById(Guid visitId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            visitId.ToQuery(),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("engineers/{engineerId:guid}")]
    public async Task<IActionResult> GetEngineerVisits(
        Guid engineerId,
        [FromQuery] EngineerVisitQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(parameters.ToQuery(engineerId), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("pending-reviews")]
    public async Task<IActionResult> GetPendingReviews(
        [FromQuery] Guid? officeId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            officeId.ToQuery(),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpGet("scheduled")]
    public async Task<IActionResult> GetScheduledVisits(
        [FromQuery] ScheduledVisitsQueryParameters parameters,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            parameters.ToQuery(),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> Create(
        [FromBody] CreateVisitRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(), cancellationToken);

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
            visitId.ToEvidenceStatusQuery(),
            cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/start")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> Start(
        Guid visitId,
        [FromBody] StartVisitRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(visitId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/checkin")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> CheckIn(
        Guid visitId,
        [FromBody] CheckInVisitRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await Mediator.Send(request.ToCommand(visitId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/checkout")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> CheckOut(
        Guid visitId,
        [FromBody] CheckOutVisitRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await Mediator.Send(request.ToCommand(visitId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/complete")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> Complete(
        Guid visitId,
        [FromBody] CompleteVisitRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(visitId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/submit")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> Submit(Guid visitId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(
            visitId.ToSubmitCommand(),
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
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await Mediator.Send(request.ToCommand(visitId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/reject")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanReviewVisits)]
    public async Task<IActionResult> Reject(
        Guid visitId,
        [FromBody] RejectVisitRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await Mediator.Send(request.ToCommand(visitId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/request-correction")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanReviewVisits)]
    public async Task<IActionResult> RequestCorrection(
        Guid visitId,
        [FromBody] RequestCorrectionRequest request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated || _currentUserService.UserId == Guid.Empty)
        {
            return Unauthorized();
        }

        var result = await Mediator.Send(request.ToCommand(visitId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/checklist-items")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> AddChecklistItem(
        Guid visitId,
        [FromBody] AddChecklistItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(visitId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{visitId:guid}/checklist-items/{checklistItemId:guid}")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> UpdateChecklistItem(
        Guid visitId,
        Guid checklistItemId,
        [FromBody] UpdateChecklistItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(visitId, checklistItemId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/issues")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> AddIssue(
        Guid visitId,
        [FromBody] AddVisitIssueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(visitId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/issues/{issueId:guid}/resolve")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> ResolveIssue(
        Guid visitId,
        Guid issueId,
        [FromBody] ResolveVisitIssueRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(visitId, issueId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/readings")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> AddReading(
        Guid visitId,
        [FromBody] AddVisitReadingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(visitId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/photos")]
    [RequestSizeLimit(25 * 1024 * 1024)]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
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

        var result = await Mediator.Send(request.ToCommand(visitId, stream), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/import/panorama")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> ImportPanoramaEvidence(
        Guid visitId,
        [FromForm] ImportVisitEvidenceRequest request,
        CancellationToken cancellationToken)
    {
        var fileBytes = await ReadExcelBytesOrNullAsync(request.File, cancellationToken);
        if (fileBytes is null)
            return BadRequest("Excel file is required.");

        var result = await Mediator.Send(visitId.ToImportPanoramaEvidenceCommand(fileBytes), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/import/alarms")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> ImportAlarmCaptureEvidence(
        Guid visitId,
        [FromForm] ImportVisitEvidenceRequest request,
        CancellationToken cancellationToken)
    {
        var fileBytes = await ReadExcelBytesOrNullAsync(request.File, cancellationToken);
        if (fileBytes is null)
            return BadRequest("Excel file is required.");

        var result = await Mediator.Send(visitId.ToImportAlarmCaptureCommand(fileBytes), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/import/unused-assets")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> ImportUnusedAssets(
        Guid visitId,
        [FromForm] ImportVisitEvidenceRequest request,
        CancellationToken cancellationToken)
    {
        var fileBytes = await ReadExcelBytesOrNullAsync(request.File, cancellationToken);
        if (fileBytes is null)
            return BadRequest("Excel file is required.");

        var result = await Mediator.Send(visitId.ToImportUnusedAssetsCommand(fileBytes), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/cancel")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> Cancel(
        Guid visitId,
        [FromBody] CancelVisitRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(visitId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/reschedule")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> Reschedule(
        Guid visitId,
        [FromBody] RescheduleVisitRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(visitId), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{visitId:guid}/photos/{photoId:guid}")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> RemovePhoto(
        Guid visitId,
        Guid photoId,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(visitId.ToRemovePhotoCommand(photoId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPatch("{visitId:guid}/readings/{readingId:guid}")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> UpdateReading(
        Guid visitId,
        Guid readingId,
        [FromBody] UpdateVisitReadingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(visitId, readingId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{visitId:guid}/signature")]
    [Authorize(Policy = ApiAuthorizationPolicies.CanManageVisits)]
    public async Task<IActionResult> CaptureSignature(
        Guid visitId,
        [FromBody] CaptureVisitSignatureRequest request,
        CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(request.ToCommand(visitId), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{visitId:guid}/signature")]
    public async Task<IActionResult> GetSignature(Guid visitId, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(visitId.ToVisitSignatureQuery(), cancellationToken);
        return HandleResult(result);
    }

    private static async Task<byte[]?> ReadExcelBytesOrNullAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return null;

        await using var stream = file.OpenReadStream();
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms, cancellationToken);
        return ms.ToArray();
    }
}
