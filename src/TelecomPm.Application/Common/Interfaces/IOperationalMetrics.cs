namespace TelecomPM.Application.Common.Interfaces;

public interface IOperationalMetrics
{
    void RecordImport(
        string operation,
        string outcome,
        int importedCount,
        int skippedCount,
        int errorCount,
        double durationMs);

    void RecordSyncBatch(
        string operation,
        string outcome,
        int processedCount,
        int conflictCount,
        int failedCount,
        int skippedCount,
        double durationMs);

    void RecordNotification(
        string channel,
        string outcome,
        double durationMs);
}
