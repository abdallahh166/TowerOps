namespace TelecomPM.Application.DTOs.Sites;

public sealed class ImportSiteDataResult
{
    public int ImportedCount { get; init; }
    public int SkippedCount { get; init; }
    public List<string> Errors { get; init; } = new();
}
