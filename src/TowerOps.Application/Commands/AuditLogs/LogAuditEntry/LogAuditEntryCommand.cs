namespace TowerOps.Application.Commands.AuditLogs.LogAuditEntry;

using TowerOps.Application.Common;

public record LogAuditEntryCommand : ICommand<Guid>
{
    public string EntityType { get; init; } = string.Empty;
    public Guid EntityId { get; init; }
    public string Action { get; init; } = string.Empty;
    public Guid ActorId { get; init; }
    public string ActorRole { get; init; } = string.Empty;
    public string? PreviousState { get; init; }
    public string? NewState { get; init; }
    public string? Notes { get; init; }
}
