using System;
using System.Threading;
using System.Threading.Tasks;

namespace TelecomPM.Application.Common.Interfaces;

public interface IReportGenerationService
{
    Task<byte[]> GenerateVisitReportAsync(
        Guid visitId,
        ReportFormat format,
        CancellationToken cancellationToken = default);

    Task<byte[]> GenerateMaterialConsumptionReportAsync(
        Guid officeId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);

    Task<byte[]> GenerateEngineerPerformanceReportAsync(
        Guid engineerId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);
}

public enum ReportFormat
{
    PDF,
    Excel,
    Word
}