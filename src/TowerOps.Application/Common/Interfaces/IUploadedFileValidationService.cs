namespace TowerOps.Application.Common.Interfaces;

public interface IUploadedFileValidationService
{
    Task<FileValidationResult> ValidateAsync(
        string fileName,
        Stream fileStream,
        CancellationToken cancellationToken = default);
}

public interface IFileMalwareScanService
{
    Task<FileMalwareScanResult> ScanAsync(
        string fileName,
        string filePath,
        CancellationToken cancellationToken = default);
}

public readonly record struct FileValidationResult(
    bool IsValid,
    string? Error);

public readonly record struct FileMalwareScanResult(
    bool IsClean,
    string? Details);
