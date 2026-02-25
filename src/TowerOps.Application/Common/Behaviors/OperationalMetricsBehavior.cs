using System.Diagnostics;
using MediatR;
using TowerOps.Application.Common.Interfaces;

namespace TowerOps.Application.Common.Behaviors;

public sealed class OperationalMetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IOperationalMetrics _metrics;

    public OperationalMetricsBehavior(IOperationalMetrics metrics)
    {
        _metrics = metrics;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            stopwatch.Stop();

            RecordMetrics(requestName, response, "success", stopwatch.Elapsed.TotalMilliseconds);
            return response;
        }
        catch
        {
            stopwatch.Stop();
            RecordExceptionMetrics(requestName, stopwatch.Elapsed.TotalMilliseconds);
            throw;
        }
    }

    private void RecordMetrics(string requestName, TResponse response, string defaultOutcome, double durationMs)
    {
        if (IsImportRequest(requestName))
        {
            var outcome = defaultOutcome;
            var imported = 0;
            var skipped = 0;
            var errors = 0;

            if (TryReadProperty(response, "IsSuccess", out bool isSuccess) && !isSuccess)
                outcome = "failure";

            if (TryReadProperty(response, "Value", out object? value) && value is not null)
            {
                imported = TryReadProperty(value, "ImportedCount", out int importedCount) ? importedCount : 0;
                skipped = TryReadProperty(value, "SkippedCount", out int skippedCount) ? skippedCount : 0;

                if (TryReadProperty(value, "Errors", out object? errorsObject) &&
                    errorsObject is System.Collections.ICollection errorsCollection)
                {
                    errors = errorsCollection.Count;
                }
            }

            _metrics.RecordImport(
                requestName,
                outcome,
                imported,
                skipped,
                errors,
                durationMs);
            return;
        }

        if (IsSyncRequest(requestName))
        {
            var outcome = defaultOutcome;
            var processed = 0;
            var conflicts = 0;
            var failed = 0;
            var skipped = 0;

            if (TryReadProperty(response, "IsSuccess", out bool isSuccess) && !isSuccess)
                outcome = "failure";

            if (TryReadProperty(response, "Value", out object? value) && value is not null)
            {
                processed = TryReadProperty(value, "Processed", out int processedCount) ? processedCount : 0;
                conflicts = TryReadProperty(value, "Conflicts", out int conflictCount) ? conflictCount : 0;
                failed = TryReadProperty(value, "Failed", out int failedCount) ? failedCount : 0;
                skipped = TryReadProperty(value, "Skipped", out int skippedCount) ? skippedCount : 0;
            }

            _metrics.RecordSyncBatch(
                requestName,
                outcome,
                processed,
                conflicts,
                failed,
                skipped,
                durationMs);
        }
    }

    private void RecordExceptionMetrics(string requestName, double durationMs)
    {
        if (IsImportRequest(requestName))
        {
            _metrics.RecordImport(
                requestName,
                "exception",
                0,
                0,
                0,
                durationMs);
            return;
        }

        if (IsSyncRequest(requestName))
        {
            _metrics.RecordSyncBatch(
                requestName,
                "exception",
                0,
                0,
                0,
                0,
                durationMs);
        }
    }

    private static bool IsImportRequest(string requestName)
        => requestName.StartsWith("Import", StringComparison.OrdinalIgnoreCase);

    private static bool IsSyncRequest(string requestName)
        => requestName.Contains("ProcessSyncBatch", StringComparison.OrdinalIgnoreCase);

    private static bool TryReadProperty<TValue>(object? source, string propertyName, out TValue value)
    {
        value = default!;
        if (source is null)
            return false;

        var property = source.GetType().GetProperty(propertyName);
        if (property is null)
            return false;

        var rawValue = property.GetValue(source);
        if (rawValue is TValue typedValue)
        {
            value = typedValue;
            return true;
        }

        return false;
    }
}
