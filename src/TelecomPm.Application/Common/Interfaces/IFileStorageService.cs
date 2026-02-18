using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace TelecomPM.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string containerName,
        CancellationToken cancellationToken = default);

    Task<Stream> DownloadFileAsync(
        string filePath,
        CancellationToken cancellationToken = default);

    Task DeleteFileAsync(
        string filePath,
        CancellationToken cancellationToken = default);

    Task<string> GetFileUrlAsync(
        string filePath,
        CancellationToken cancellationToken = default);
}