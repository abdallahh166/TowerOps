namespace TelecomPM.Application.DTOs.Materials;

using System;
using TelecomPM.Domain.Enums;

public record MaterialReservationDto
{
    public Guid Id { get; init; }
    public Guid MaterialId { get; init; }
    public Guid VisitId { get; init; }
    public decimal Quantity { get; init; }
    public string Unit { get; init; } = string.Empty;
    public DateTime ReservedAt { get; init; }
    public bool IsConsumed { get; init; }
}

