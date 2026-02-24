using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using TelecomPm.Api.Security;
using Xunit;

namespace TelecomPM.Application.Tests.Services;

public class ApiSecurityHardeningTests
{
    [Theory]
    [InlineData("/api/auth/login", ApiRateLimitBucket.Auth)]
    [InlineData("/api/auth/forgot-password", ApiRateLimitBucket.Auth)]
    [InlineData("/api/sync", ApiRateLimitBucket.Sync)]
    [InlineData("/api/sync/status/device-1", ApiRateLimitBucket.Sync)]
    [InlineData("/api/sites/import", ApiRateLimitBucket.Import)]
    [InlineData("/api/sites/import/power-data", ApiRateLimitBucket.Import)]
    [InlineData("/api/visits/00000000-0000-0000-0000-000000000001/import/panorama", ApiRateLimitBucket.Import)]
    [InlineData("/api/checklisttemplates/import", ApiRateLimitBucket.Import)]
    [InlineData("/api/sites/maintenance", ApiRateLimitBucket.None)]
    public void GetBucket_ShouldClassifyExpectedPaths(string path, ApiRateLimitBucket expectedBucket)
    {
        var bucket = ApiSecurityHardening.GetBucket(path);
        bucket.Should().Be(expectedBucket);
    }

    [Fact]
    public async Task CreateGlobalLimiter_ShouldRateLimitAuthRequests()
    {
        var options = new ApiSecurityHardeningOptions(
            Auth: new ApiRateLimitBucketConfig(PermitLimit: 2, WindowSeconds: 60),
            Sync: new ApiRateLimitBucketConfig(PermitLimit: 100, WindowSeconds: 60),
            Import: new ApiRateLimitBucketConfig(PermitLimit: 100, WindowSeconds: 60));

        var limiter = ApiSecurityHardening.CreateGlobalLimiter(options);
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/auth/login";
        context.Connection.RemoteIpAddress = IPAddress.Parse("10.10.10.10");

        using var lease1 = await limiter.AcquireAsync(context, 1);
        using var lease2 = await limiter.AcquireAsync(context, 1);
        using var lease3 = await limiter.AcquireAsync(context, 1);

        lease1.IsAcquired.Should().BeTrue();
        lease2.IsAcquired.Should().BeTrue();
        lease3.IsAcquired.Should().BeFalse();
    }

    [Fact]
    public async Task CreateGlobalLimiter_ShouldNotLimitNonSensitivePaths()
    {
        var options = new ApiSecurityHardeningOptions(
            Auth: new ApiRateLimitBucketConfig(PermitLimit: 1, WindowSeconds: 60),
            Sync: new ApiRateLimitBucketConfig(PermitLimit: 1, WindowSeconds: 60),
            Import: new ApiRateLimitBucketConfig(PermitLimit: 1, WindowSeconds: 60));

        var limiter = ApiSecurityHardening.CreateGlobalLimiter(options);
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/sites/maintenance";

        for (var i = 0; i < 20; i++)
        {
            using var lease = await limiter.AcquireAsync(context, 1);
            lease.IsAcquired.Should().BeTrue();
        }
    }

    [Fact]
    public void ResolveAllowedOrigins_InDevelopment_UsesLocalhostFallback()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var origins = ApiSecurityHardening.ResolveAllowedOrigins(configuration, isProduction: false);
        origins.Should().ContainSingle();
        origins[0].Should().Be("https://localhost:4200");
    }

    [Fact]
    public void ResolveAllowedOrigins_InProductionWithoutOrigins_ShouldThrow()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        var action = () => ApiSecurityHardening.ResolveAllowedOrigins(configuration, isProduction: true);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cors:AllowedOrigins*");
    }

    [Fact]
    public void ResolveAllowedOrigins_InProductionWithWildcard_ShouldThrow()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Cors:AllowedOrigins:0"] = "*"
            })
            .Build();

        var action = () => ApiSecurityHardening.ResolveAllowedOrigins(configuration, isProduction: true);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*wildcard*");
    }

    [Fact]
    public void ResolveAllowedOrigins_InProductionWithLocalhost_ShouldThrow()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Cors:AllowedOrigins:0"] = "https://localhost:4200"
            })
            .Build();

        var action = () => ApiSecurityHardening.ResolveAllowedOrigins(configuration, isProduction: true);
        action.Should().Throw<InvalidOperationException>()
            .WithMessage("*localhost*");
    }

    [Fact]
    public void ResolveAllowedOrigins_InProductionWithExplicitOrigins_ShouldReturnOrigins()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Cors:AllowedOrigins:0"] = "https://portal.client-a.com",
                ["Cors:AllowedOrigins:1"] = "https://ops.client-b.com"
            })
            .Build();

        var origins = ApiSecurityHardening.ResolveAllowedOrigins(configuration, isProduction: true);
        origins.Should().BeEquivalentTo(
            new[] { "https://portal.client-a.com", "https://ops.client-b.com" },
            options => options.WithStrictOrdering());
    }
}
