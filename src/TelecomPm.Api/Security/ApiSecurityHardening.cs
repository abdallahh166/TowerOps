using System.Net;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace TelecomPm.Api.Security;

public enum ApiRateLimitBucket
{
    None = 0,
    Auth = 1,
    Sync = 2,
    Import = 3
}

public sealed record ApiRateLimitBucketConfig(int PermitLimit, int WindowSeconds)
{
    public int NormalizedPermitLimit => PermitLimit > 0 ? PermitLimit : 1;
    public int NormalizedWindowSeconds => WindowSeconds > 0 ? WindowSeconds : 60;
}

public sealed record ApiSecurityHardeningOptions(
    ApiRateLimitBucketConfig Auth,
    ApiRateLimitBucketConfig Sync,
    ApiRateLimitBucketConfig Import)
{
    public static ApiSecurityHardeningOptions FromConfiguration(IConfiguration configuration)
    {
        return new ApiSecurityHardeningOptions(
            Auth: new ApiRateLimitBucketConfig(
                configuration.GetValue<int?>("RateLimiting:Auth:PermitLimit") ?? 8,
                configuration.GetValue<int?>("RateLimiting:Auth:WindowSeconds") ?? 60),
            Sync: new ApiRateLimitBucketConfig(
                configuration.GetValue<int?>("RateLimiting:Sync:PermitLimit") ?? 30,
                configuration.GetValue<int?>("RateLimiting:Sync:WindowSeconds") ?? 60),
            Import: new ApiRateLimitBucketConfig(
                configuration.GetValue<int?>("RateLimiting:Import:PermitLimit") ?? 5,
                configuration.GetValue<int?>("RateLimiting:Import:WindowSeconds") ?? 300));
    }
}

public static class ApiSecurityHardening
{
    public static string[] ResolveAllowedOrigins(IConfiguration configuration, bool isProduction)
    {
        var allowedOrigins = (configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (!isProduction)
        {
            return allowedOrigins.Length == 0
                ? new[] { "https://localhost:4200" }
                : allowedOrigins;
        }

        if (allowedOrigins.Length == 0)
            throw new InvalidOperationException("Cors:AllowedOrigins must contain at least one explicit production origin.");

        if (allowedOrigins.Any(origin => origin == "*"))
            throw new InvalidOperationException("Cors:AllowedOrigins cannot contain wildcard (*) in Production.");

        if (allowedOrigins.Any(IsLocalhostOrigin))
            throw new InvalidOperationException("Cors:AllowedOrigins cannot contain localhost origins in Production.");

        return allowedOrigins;
    }

    public static ApiRateLimitBucket GetBucket(PathString path)
    {
        if (path.StartsWithSegments("/api/auth", StringComparison.OrdinalIgnoreCase))
            return ApiRateLimitBucket.Auth;

        if (path.StartsWithSegments("/api/sync", StringComparison.OrdinalIgnoreCase))
            return ApiRateLimitBucket.Sync;

        if (path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase) &&
            path.Value is not null &&
            path.Value.Contains("/import", StringComparison.OrdinalIgnoreCase))
        {
            return ApiRateLimitBucket.Import;
        }

        return ApiRateLimitBucket.None;
    }

    public static PartitionedRateLimiter<HttpContext> CreateGlobalLimiter(ApiSecurityHardeningOptions options)
    {
        return PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        {
            var bucket = GetBucket(httpContext.Request.Path);
            if (bucket == ApiRateLimitBucket.None)
                return RateLimitPartition.GetNoLimiter("none");

            var clientKey = ResolveClientKey(httpContext);
            var partitionKey = $"{bucket}:{clientKey}";

            return bucket switch
            {
                ApiRateLimitBucket.Auth => CreateFixedWindowPartition(partitionKey, options.Auth),
                ApiRateLimitBucket.Sync => CreateFixedWindowPartition(partitionKey, options.Sync),
                ApiRateLimitBucket.Import => CreateFixedWindowPartition(partitionKey, options.Import),
                _ => RateLimitPartition.GetNoLimiter("none")
            };
        });
    }

    private static RateLimitPartition<string> CreateFixedWindowPartition(
        string partitionKey,
        ApiRateLimitBucketConfig bucketConfig)
    {
        return RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = bucketConfig.NormalizedPermitLimit,
            Window = TimeSpan.FromSeconds(bucketConfig.NormalizedWindowSeconds),
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            AutoReplenishment = true
        });
    }

    private static string ResolveClientKey(HttpContext httpContext)
    {
        var userKey = httpContext.User?.FindFirst("sub")?.Value;
        if (string.IsNullOrWhiteSpace(userKey))
            userKey = httpContext.User?.Identity?.Name;

        if (!string.IsNullOrWhiteSpace(userKey))
            return userKey;

        var ip = httpContext.Connection.RemoteIpAddress;
        if (ip is not null && ip != IPAddress.None)
            return ip.ToString();

        return "anonymous";
    }

    private static bool IsLocalhostOrigin(string origin)
    {
        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
            return false;

        return uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
            || uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase)
            || uri.Host.Equals("::1", StringComparison.OrdinalIgnoreCase);
    }
}
