namespace TelecomPM.Application.Commands.Materials.AdjustStock;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Enums;

public record AdjustStockCommand : ICommand
{
    public Guid MaterialId { get; init; }
    public decimal NewQuantity { get; init; }
    public MaterialUnit Unit { get; init; }
    public string Reason { get; init; } = string.Empty;
}

