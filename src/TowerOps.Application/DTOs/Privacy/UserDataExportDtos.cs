namespace TowerOps.Application.DTOs.Privacy;

public sealed class UserDataExportRequestDto
{
    public Guid RequestId { get; init; }
    public DateTime RequestedAtUtc { get; init; }
    public DateTime ExpiresAtUtc { get; init; }
    public string Status { get; init; } = string.Empty;
}
