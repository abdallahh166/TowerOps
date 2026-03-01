using TowerOps.Domain.Events;

namespace TowerOps.Domain.Common;

// ==================== Base Entity ====================
public abstract class Entity<TId> where TId : notnull
{
    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; protected set; }
    public string CreatedBy { get; protected set; } = string.Empty;
    public DateTime? UpdatedAt { get; protected set; }
    public string? UpdatedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public DateTime? DeletedAt { get; protected set; }
    public string? DeletedBy { get; protected set; }
    public bool IsUnderLegalHold { get; protected set; }
    public DateTime? LegalHoldAppliedAtUtc { get; protected set; }
    public string? LegalHoldReason { get; protected set; }

    protected Entity() { }

    protected Entity(TId id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted(string deletedBy)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void ApplyLegalHold(string reason, string updatedBy)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Legal hold reason is required.", nameof(reason));

        IsUnderLegalHold = true;
        LegalHoldAppliedAtUtc = DateTime.UtcNow;
        LegalHoldReason = reason.Trim();
        MarkAsUpdated(updatedBy);
    }

    public void ReleaseLegalHold(string updatedBy)
    {
        IsUnderLegalHold = false;
        LegalHoldAppliedAtUtc = null;
        LegalHoldReason = null;
        MarkAsUpdated(updatedBy);
    }

    protected void MarkAsUpdated(string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        if (Id.Equals(default(TId)) || other.Id.Equals(default(TId)))
            return false;

        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }

    public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
    {
        if (a is null && b is null)
            return true;

        if (a is null || b is null)
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity<TId>? a, Entity<TId>? b)
    {
        return !(a == b);
    }
}
