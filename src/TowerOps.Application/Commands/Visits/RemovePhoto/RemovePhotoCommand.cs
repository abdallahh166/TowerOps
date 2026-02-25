namespace TowerOps.Application.Commands.Visits.RemovePhoto;

using System;
using TowerOps.Application.Common;

public record RemovePhotoCommand : ICommand
{
    public Guid VisitId { get; init; }
    public Guid PhotoId { get; init; }
}

