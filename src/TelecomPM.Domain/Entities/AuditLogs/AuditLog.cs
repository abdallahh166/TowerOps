using TelecomPM.Domain.Common;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.AuditLogs;

public sealed class AuditLog : AggregateRoot<Guid>
{
    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public Guid ActorId { get; private set; }
    public string ActorRole { get; private set; } = string.Empty;
    public DateTime OccurredAtUtc { get; private set; }
    public string? PreviousState { get; private set; }
    public string? NewState { get; private set; }
    public string? Notes { get; private set; }

    private AuditLog() : base() { }

    private AuditLog(
        string entityType,
        Guid entityId,
        string action,
        Guid actorId,
        string actorRole,
        string? previousState,
        string? newState,
        string? notes) : base(Guid.NewGuid())
    {
        EntityType = entityType;
        EntityId = entityId;
        Action = action;
        ActorId = actorId;
        ActorRole = actorRole;
        OccurredAtUtc = DateTime.UtcNow;
        PreviousState = previousState;
        NewState = newState;
        Notes = notes;
    }

    public static AuditLog Create(
        string entityType,
        Guid entityId,
        string action,
        Guid actorId,
        string actorRole,
        string? previousState,
        string? newState,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(entityType))
            throw new DomainException("Entity type is required", "AuditLog.EntityType.Required");

        if (entityId == Guid.Empty)
            throw new DomainException("Entity ID is required", "AuditLog.EntityId.Required");

        if (string.IsNullOrWhiteSpace(action))
            throw new DomainException("Action is required", "AuditLog.Action.Required");

        if (actorId == Guid.Empty)
            throw new DomainException("Actor ID is required", "AuditLog.ActorId.Required");

        if (string.IsNullOrWhiteSpace(actorRole))
            throw new DomainException("Actor role is required", "AuditLog.ActorRole.Required");

        return new AuditLog(
            entityType.Trim(),
            entityId,
            action.Trim(),
            actorId,
            actorRole.Trim(),
            previousState,
            newState,
            notes);
    }
}
