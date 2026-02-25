namespace TowerOps.Domain.Common;

// ==================== Auditable Entity ====================
public abstract class AuditableEntity<TId> : Entity<TId> where TId : notnull
{
    public int Version { get; protected set; }

    protected AuditableEntity() : base() { }

    protected AuditableEntity(TId id) : base(id) 
    {
        Version = 1;
    }

    protected void IncrementVersion()
    {
        Version++;
    }
}
