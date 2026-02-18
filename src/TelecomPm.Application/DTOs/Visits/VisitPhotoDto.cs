namespace TelecomPM.Application.DTOs.Visits;

using System;
using TelecomPM.Domain.Enums;

public record VisitPhotoDto
{
    public Guid Id { get; init; }
    public PhotoType Type { get; init; }
    public PhotoCategory Category { get; init; }
    public string ItemName { get; init; } = string.Empty;
    public string FileUrl { get; init; } = string.Empty;
    public string? ThumbnailUrl { get; init; }
    public string? Description { get; init; }
    public bool IsMandatory { get; init; }
    public DateTime CapturedAt { get; init; }
}