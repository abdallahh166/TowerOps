using TelecomPM.Domain.Common;
using TelecomPM.Domain.Enums;
using TelecomPM.Domain.Events.ChecklistTemplateEvents;
using TelecomPM.Domain.Exceptions;

namespace TelecomPM.Domain.Entities.ChecklistTemplates;

public sealed class ChecklistTemplate : AggregateRoot<Guid>
{
    public VisitType VisitType { get; private set; }
    public string Version { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime EffectiveFromUtc { get; private set; }
    public DateTime? EffectiveToUtc { get; private set; }
    public new string CreatedBy { get; private set; } = string.Empty;
    public string? ApprovedBy { get; private set; }
    public DateTime? ApprovedAtUtc { get; private set; }
    public string? ChangeNotes { get; private set; }
    public List<ChecklistTemplateItem> Items { get; private set; } = new();

    private ChecklistTemplate() : base()
    {
    }

    private ChecklistTemplate(
        VisitType visitType,
        string version,
        DateTime effectiveFromUtc,
        string createdBy,
        string? changeNotes) : base(Guid.NewGuid())
    {
        VisitType = visitType;
        Version = version.Trim();
        IsActive = false;
        EffectiveFromUtc = EnsureUtc(effectiveFromUtc);
        CreatedBy = createdBy.Trim();
        ChangeNotes = changeNotes?.Trim();
    }

    public static ChecklistTemplate Create(
        VisitType visitType,
        string version,
        DateTime effectiveFromUtc,
        string createdBy,
        string? changeNotes = null)
    {
        if (string.IsNullOrWhiteSpace(version))
            throw new DomainException("Checklist template version is required");

        if (string.IsNullOrWhiteSpace(createdBy))
            throw new DomainException("CreatedBy is required");

        return new ChecklistTemplate(visitType, version, effectiveFromUtc, createdBy, changeNotes);
    }

    public void AddItem(
        string category,
        string itemName,
        string? description,
        bool isMandatory,
        int orderIndex,
        string? applicableSiteTypes = null,
        string? applicableVisitTypes = null)
    {
        var item = ChecklistTemplateItem.Create(
            Id,
            category,
            itemName,
            description,
            isMandatory,
            orderIndex,
            applicableSiteTypes,
            applicableVisitTypes);

        Items.Add(item);
    }

    public void Activate(string approvedBy)
    {
        if (string.IsNullOrWhiteSpace(approvedBy))
            throw new DomainException("ApprovedBy is required");

        IsActive = true;
        ApprovedBy = approvedBy.Trim();
        ApprovedAtUtc = DateTime.UtcNow;
        EffectiveToUtc = null;

        AddDomainEvent(new ChecklistTemplateActivatedEvent(Id, VisitType, Version));
    }

    public void Supersede(DateTime effectiveTo)
    {
        IsActive = false;
        EffectiveToUtc = EnsureUtc(effectiveTo);
    }

    public static ChecklistTemplate CreateNewVersion(
        ChecklistTemplate previous,
        string changeNotes,
        string createdBy)
    {
        if (previous is null)
            throw new DomainException("Previous template is required");

        if (string.IsNullOrWhiteSpace(changeNotes))
            throw new DomainException("Change notes are required");

        if (string.IsNullOrWhiteSpace(createdBy))
            throw new DomainException("CreatedBy is required");

        var nextVersion = IncrementVersion(previous.Version);
        var next = Create(
            previous.VisitType,
            nextVersion,
            DateTime.UtcNow,
            createdBy,
            changeNotes);

        foreach (var item in previous.Items.OrderBy(i => i.OrderIndex))
        {
            next.AddItem(
                item.Category,
                item.ItemName,
                item.Description,
                item.IsMandatory,
                item.OrderIndex,
                item.ApplicableSiteTypes,
                item.ApplicableVisitTypes);
        }

        return next;
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    private static string IncrementVersion(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return "v1.0";

        var normalized = version.Trim().ToLowerInvariant();
        if (!normalized.StartsWith('v'))
            return "v1.0";

        var numeric = normalized[1..];
        var parts = numeric.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2 ||
            !int.TryParse(parts[0], out var major) ||
            !int.TryParse(parts[1], out var minor))
        {
            return "v1.0";
        }

        minor++;
        return $"v{major}.{minor}";
    }
}
