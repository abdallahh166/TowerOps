namespace TelecomPM.Application.Commands.Materials.RestockMaterial;

using System;
using TelecomPM.Application.Common;
using TelecomPM.Domain.Enums;

public record RestockMaterialCommand : ICommand
{
    public Guid MaterialId { get; init; }
    public decimal Quantity { get; init; }
    public MaterialUnit Unit { get; init; }
    public string RestockedBy { get; init; } = string.Empty;
    public string? Supplier { get; init; }
}

