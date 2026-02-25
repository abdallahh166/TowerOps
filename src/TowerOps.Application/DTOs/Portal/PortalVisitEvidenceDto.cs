using TowerOps.Domain.Enums;

namespace TowerOps.Application.DTOs.Portal;

public sealed class PortalVisitEvidenceDto
{
    public Guid VisitId { get; init; }
    public string VisitNumber { get; init; } = string.Empty;
    public string SiteCode { get; init; } = string.Empty;
    public VisitType VisitType { get; init; }
    public VisitStatus VisitStatus { get; init; }
    public DateTime ScheduledDateUtc { get; init; }
    public IReadOnlyList<PortalVisitPhotoEvidenceDto> Photos { get; init; } = Array.Empty<PortalVisitPhotoEvidenceDto>();
    public IReadOnlyList<PortalVisitReadingEvidenceDto> Readings { get; init; } = Array.Empty<PortalVisitReadingEvidenceDto>();
    public IReadOnlyList<PortalVisitChecklistEvidenceDto> ChecklistItems { get; init; } = Array.Empty<PortalVisitChecklistEvidenceDto>();
}

public sealed class PortalVisitPhotoEvidenceDto
{
    public Guid PhotoId { get; init; }
    public PhotoType Type { get; init; }
    public PhotoCategory Category { get; init; }
    public string ItemName { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public DateTime? CapturedAtUtc { get; init; }
}

public sealed class PortalVisitReadingEvidenceDto
{
    public Guid ReadingId { get; init; }
    public string ReadingType { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Value { get; init; }
    public string Unit { get; init; } = string.Empty;
    public bool IsWithinRange { get; init; }
    public DateTime MeasuredAtUtc { get; init; }
}

public sealed class PortalVisitChecklistEvidenceDto
{
    public Guid ChecklistItemId { get; init; }
    public string Category { get; init; } = string.Empty;
    public string ItemName { get; init; } = string.Empty;
    public CheckStatus Status { get; init; }
    public bool IsMandatory { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
}
