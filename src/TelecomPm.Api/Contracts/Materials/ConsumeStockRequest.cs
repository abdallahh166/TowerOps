namespace TelecomPm.Api.Contracts.Materials;

using System;
using System.ComponentModel.DataAnnotations;

public record ConsumeStockRequest
{
    [Required]
    public Guid VisitId { get; init; }
}
