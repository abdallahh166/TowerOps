namespace TelecomPM.Application.Commands.Visits.RemovePhoto;

using System;
using TelecomPM.Application.Common;

public record RemovePhotoCommand : ICommand
{
    public Guid VisitId { get; init; }
    public Guid PhotoId { get; init; }
}

