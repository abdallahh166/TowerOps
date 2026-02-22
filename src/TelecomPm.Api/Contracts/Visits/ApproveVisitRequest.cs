namespace TelecomPm.Api.Contracts.Visits;

using System.ComponentModel.DataAnnotations;

public record ApproveVisitRequest
{
    [MaxLength(2000)]
    public string? Notes { get; init; }
}

