using TowerOps.Application.Common.Interfaces;

namespace TowerOps.Infrastructure.Services;

public sealed class UploadedFileValidationService : IUploadedFileValidationService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".pdf"
    };

    public Task<FileValidationResult> ValidateAsync(
        string fileName,
        Stream fileStream,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return Task.FromResult(new FileValidationResult(false, "File name is required."));

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            return Task.FromResult(new FileValidationResult(false, "Unsupported file type. Allowed: .jpg, .jpeg, .png, .pdf."));

        if (fileStream is null || !fileStream.CanRead)
            return Task.FromResult(new FileValidationResult(false, "File stream is not readable."));

        var signatureValid = HasMatchingMagicBytes(extension, fileStream);
        if (!signatureValid)
            return Task.FromResult(new FileValidationResult(false, "File content signature does not match file extension."));

        return Task.FromResult(new FileValidationResult(true, null));
    }

    private static bool HasMatchingMagicBytes(string extension, Stream fileStream)
    {
        var originalPosition = fileStream.CanSeek ? fileStream.Position : 0;
        try
        {
            if (fileStream.CanSeek)
            {
                fileStream.Seek(0, SeekOrigin.Begin);
            }

            Span<byte> header = stackalloc byte[8];
            var bytesRead = fileStream.Read(header);
            if (bytesRead <= 0)
                return false;

            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => bytesRead >= 3 &&
                                     header[0] == 0xFF &&
                                     header[1] == 0xD8 &&
                                     header[2] == 0xFF,
                ".png" => bytesRead >= 8 &&
                          header[0] == 0x89 &&
                          header[1] == 0x50 &&
                          header[2] == 0x4E &&
                          header[3] == 0x47 &&
                          header[4] == 0x0D &&
                          header[5] == 0x0A &&
                          header[6] == 0x1A &&
                          header[7] == 0x0A,
                ".pdf" => bytesRead >= 5 &&
                          header[0] == 0x25 &&
                          header[1] == 0x50 &&
                          header[2] == 0x44 &&
                          header[3] == 0x46 &&
                          header[4] == 0x2D,
                _ => false
            };
        }
        finally
        {
            if (fileStream.CanSeek)
            {
                fileStream.Seek(originalPosition, SeekOrigin.Begin);
            }
        }
    }
}
