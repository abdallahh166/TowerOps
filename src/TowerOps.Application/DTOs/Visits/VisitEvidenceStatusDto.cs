namespace TowerOps.Application.DTOs.Visits;

public record VisitEvidenceStatusDto
{
    public Guid VisitId { get; init; }
    public int BeforePhotos { get; init; }
    public int AfterPhotos { get; init; }
    public int RequiredBeforePhotos { get; init; }
    public int RequiredAfterPhotos { get; init; }
    public int ReadingsCount { get; init; }
    public int RequiredReadings { get; init; }
    public int ChecklistItems { get; init; }
    public int CompletedChecklistItems { get; init; }
    public int CompletionPercentage { get; init; }
    public bool IsPhotosComplete { get; init; }
    public bool IsReadingsComplete { get; init; }
    public bool IsChecklistComplete { get; init; }
    public bool CanBeSubmitted { get; init; }
}
