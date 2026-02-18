namespace TelecomPM.Application.Commands.Visits.AddPhoto;

using System;
using System.IO;
using TelecomPM.Application.Common;
using TelecomPM.Application.DTOs.Visits;
using TelecomPM.Domain.Enums;

public record AddPhotoCommand : ICommand<VisitPhotoDto>
{
    public Guid VisitId { get; init; }
    public PhotoType Type { get; init; }
    public PhotoCategory Category { get; init; }
    public string ItemName { get; init; } = string.Empty;
    public Stream FileStream { get; init; } = null!;
    public string FileName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public double? Latitude { get; init; }
    public double? Longitude { get; init; }
}