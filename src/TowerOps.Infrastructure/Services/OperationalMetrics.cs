using System.Diagnostics.Metrics;
using TowerOps.Application.Common.Interfaces;

namespace TowerOps.Infrastructure.Services;

public sealed class OperationalMetrics : IOperationalMetrics
{
    private static readonly Meter Meter = new("TowerOps.Operations", "1.0.0");

    private static readonly Counter<long> ImportRequestsCounter =
        Meter.CreateCounter<long>("telecompm_import_requests_total");

    private static readonly Counter<long> ImportRowsImportedCounter =
        Meter.CreateCounter<long>("telecompm_import_rows_imported_total");

    private static readonly Counter<long> ImportRowsSkippedCounter =
        Meter.CreateCounter<long>("telecompm_import_rows_skipped_total");

    private static readonly Counter<long> ImportRowsErrorsCounter =
        Meter.CreateCounter<long>("telecompm_import_rows_errors_total");

    private static readonly Histogram<double> ImportDurationMs =
        Meter.CreateHistogram<double>("telecompm_import_duration_ms");

    private static readonly Counter<long> SyncBatchesCounter =
        Meter.CreateCounter<long>("telecompm_sync_batches_total");

    private static readonly Counter<long> SyncItemsProcessedCounter =
        Meter.CreateCounter<long>("telecompm_sync_items_processed_total");

    private static readonly Counter<long> SyncItemsConflictsCounter =
        Meter.CreateCounter<long>("telecompm_sync_items_conflicts_total");

    private static readonly Counter<long> SyncItemsFailedCounter =
        Meter.CreateCounter<long>("telecompm_sync_items_failed_total");

    private static readonly Counter<long> SyncItemsSkippedCounter =
        Meter.CreateCounter<long>("telecompm_sync_items_skipped_total");

    private static readonly Histogram<double> SyncDurationMs =
        Meter.CreateHistogram<double>("telecompm_sync_batch_duration_ms");

    private static readonly Counter<long> NotificationRequestsCounter =
        Meter.CreateCounter<long>("telecompm_notifications_total");

    private static readonly Histogram<double> NotificationDurationMs =
        Meter.CreateHistogram<double>("telecompm_notification_duration_ms");

    public void RecordImport(
        string operation,
        string outcome,
        int importedCount,
        int skippedCount,
        int errorCount,
        double durationMs)
    {
        var tags = new[]
        {
            new KeyValuePair<string, object?>("operation", operation),
            new KeyValuePair<string, object?>("outcome", outcome)
        };

        ImportRequestsCounter.Add(1, tags);
        ImportRowsImportedCounter.Add(importedCount, tags);
        ImportRowsSkippedCounter.Add(skippedCount, tags);
        ImportRowsErrorsCounter.Add(errorCount, tags);
        ImportDurationMs.Record(durationMs, tags);
    }

    public void RecordSyncBatch(
        string operation,
        string outcome,
        int processedCount,
        int conflictCount,
        int failedCount,
        int skippedCount,
        double durationMs)
    {
        var tags = new[]
        {
            new KeyValuePair<string, object?>("operation", operation),
            new KeyValuePair<string, object?>("outcome", outcome)
        };

        SyncBatchesCounter.Add(1, tags);
        SyncItemsProcessedCounter.Add(processedCount, tags);
        SyncItemsConflictsCounter.Add(conflictCount, tags);
        SyncItemsFailedCounter.Add(failedCount, tags);
        SyncItemsSkippedCounter.Add(skippedCount, tags);
        SyncDurationMs.Record(durationMs, tags);
    }

    public void RecordNotification(
        string channel,
        string outcome,
        double durationMs)
    {
        var tags = new[]
        {
            new KeyValuePair<string, object?>("channel", channel),
            new KeyValuePair<string, object?>("outcome", outcome)
        };

        NotificationRequestsCounter.Add(1, tags);
        NotificationDurationMs.Record(durationMs, tags);
    }
}
