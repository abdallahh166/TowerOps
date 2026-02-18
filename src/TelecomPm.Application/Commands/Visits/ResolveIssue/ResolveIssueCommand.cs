namespace TelecomPM.Application.Commands.Visits.ResolveIssue;

using System;
using TelecomPM.Application.Common;

public record ResolveIssueCommand : ICommand
{
    public Guid VisitId { get; init; }
    public Guid IssueId { get; init; }
    public string Resolution { get; init; } = string.Empty;
}

