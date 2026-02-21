namespace TelecomPM.Application.DTOs.Sites;

public sealed class ImportSiteDataResult
{
    public int ImportedCount { get; set; }
    public int SkippedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}
