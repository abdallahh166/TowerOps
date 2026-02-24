using FluentAssertions;
using Microsoft.Extensions.Configuration;
using TelecomPM.Infrastructure.Services;
using Xunit;

namespace TelecomPM.Infrastructure.Tests.Services;

public class BlobStorageServiceTests
{
    [Fact]
    public void Ctor_ShouldUse_AzureBlobStorageSectionConnectionString_WhenProvided()
    {
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["AzureBlobStorage:ConnectionString"] = "UseDevelopmentStorage=true",
            ["AzureBlobStorage:ContainerName"] = "files"
        });

        var act = () => new BlobStorageService(configuration);
        act.Should().NotThrow();
    }

    [Fact]
    public void Ctor_ShouldFallbackTo_ConnectionStringsAzureBlobStorage()
    {
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["ConnectionStrings:AzureBlobStorage"] = "UseDevelopmentStorage=true"
        });

        var act = () => new BlobStorageService(configuration);
        act.Should().NotThrow();
    }

    [Fact]
    public void Ctor_ShouldFallbackTo_ConnectionStringsAzureStorage()
    {
        var configuration = BuildConfiguration(new Dictionary<string, string?>
        {
            ["ConnectionStrings:AzureStorage"] = "UseDevelopmentStorage=true"
        });

        var act = () => new BlobStorageService(configuration);
        act.Should().NotThrow();
    }

    [Fact]
    public void Ctor_ShouldThrow_WhenNoConnectionStringConfigured()
    {
        var configuration = BuildConfiguration(new Dictionary<string, string?>());

        var act = () => new BlobStorageService(configuration);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Azure Blob Storage is not configured*");
    }

    private static IConfiguration BuildConfiguration(Dictionary<string, string?> values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
