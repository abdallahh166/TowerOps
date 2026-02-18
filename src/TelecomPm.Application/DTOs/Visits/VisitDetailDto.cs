using System.Collections.Generic;

namespace TelecomPM.Application.DTOs.Visits;

public record VisitDetailDto : VisitDto
{
    public List<VisitPhotoDto> Photos { get; init; } = new();
    public List<VisitReadingDto> Readings { get; init; } = new();
    public List<VisitChecklistDto> Checklists { get; init; } = new();
    public List<VisitMaterialUsageDto> MaterialsUsed { get; init; } = new();
    public List<VisitIssueDto> IssuesFound { get; init; } = new();
    public List<VisitApprovalDto> ApprovalHistory { get; init; } = new();
}