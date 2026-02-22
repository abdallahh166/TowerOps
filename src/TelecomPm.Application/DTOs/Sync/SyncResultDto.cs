namespace TelecomPM.Application.DTOs.Sync;

public sealed record SyncResultDto
{
    public int Processed { get; init; }
    public int Conflicts { get; init; }
    public int Failed { get; init; }
    public int Skipped { get; init; }
}
