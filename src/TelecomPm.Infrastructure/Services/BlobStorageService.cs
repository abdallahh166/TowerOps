namespace TelecomPM.Infrastructure.Services;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using TelecomPM.Application.Common.Interfaces;

public class BlobStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];

        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = configuration.GetConnectionString("AzureBlobStorage");

        if (string.IsNullOrWhiteSpace(connectionString))
            connectionString = configuration.GetConnectionString("AzureStorage");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Azure Blob Storage is not configured. Set AzureBlobStorage:ConnectionString or ConnectionStrings:AzureBlobStorage/AzureStorage.");
        }

        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerName = configuration["AzureBlobStorage:ContainerName"] ?? "telecompm";
    }

    public async Task<string> UploadFileAsync(
        Stream fileStream,
        string fileName,
        string containerName,
        CancellationToken cancellationToken = default)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync(
            PublicAccessType.None,
            cancellationToken: cancellationToken);

        var blobClient = containerClient.GetBlobClient(fileName);

        await blobClient.UploadAsync(
            fileStream,
            new BlobHttpHeaders { ContentType = GetContentType(fileName) },
            cancellationToken: cancellationToken);

        return blobClient.Uri.ToString();
    }

    public async Task<Stream> DownloadFileAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var uri = new Uri(filePath);
        var blobClient = new BlobClient(uri);

        var response = await blobClient.DownloadAsync(cancellationToken);
        return response.Value.Content;
    }

    public async Task DeleteFileAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var uri = new Uri(filePath);
        var blobClient = new BlobClient(uri);

        await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }

    public async Task<string> GetFileUrlAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(filePath);
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".pdf" => "application/pdf",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream"
        };
    }
}
