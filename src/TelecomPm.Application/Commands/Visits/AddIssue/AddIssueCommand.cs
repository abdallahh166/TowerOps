namespace TelecomPM.Application.Commands.Visits.AddIssue;

using System;
using System.Collections.Generic;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Enums;

public record AddIssueCommand : ICommand<VisitIssueDto>
{
    public Guid VisitId { get; init; }
    public IssueCategory Category { get; init; }
    public IssueSeverity Severity { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<Guid>? PhotoIds { get; init; }
}

