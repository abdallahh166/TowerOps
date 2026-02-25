namespace TowerOps.Api.Contracts.Sites;

using System;
using System.ComponentModel.DataAnnotations;

public record AssignEngineerRequest
{
    [Required]
    public Guid EngineerId { get; init; }
}

