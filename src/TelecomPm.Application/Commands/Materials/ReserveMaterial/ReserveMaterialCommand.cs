namespace TelecomPM.Application.Commands.Materials.ReserveMaterial;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Enums;

public record ReserveMaterialCommand : ICommand
{
    public Guid MaterialId { get; init; }
    public Guid VisitId { get; init; }
    public decimal Quantity { get; init; }
    public MaterialUnit Unit { get; init; }
}

