namespace TelecomPm.Api.Contracts.Visits;

using System;
using System.ComponentModel.DataAnnotations;

public class ScheduledVisitsQueryParameters
{
    [Required]
    public DateTime Date { get; init; } = DateTime.UtcNow.Date;

    public Guid? EngineerId { get; init; }
}

