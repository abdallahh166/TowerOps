namespace TelecomPM.Application.Commands.Materials.ConsumeMaterial;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Enums;

public record ConsumeMaterialCommand : ICommand
{
    public Guid MaterialId { get; init; }
    public Guid VisitId { get; init; }
    public string PerformedBy { get; init; } = string.Empty;
}

